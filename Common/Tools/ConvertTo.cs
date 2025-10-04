using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using PixelWorld.Transformers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace PixelWorld.Tools;

public static class ConvertTo
{
    public static void Ufo(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
    {
        foreach (var sourceFileName in fileNames)
        {
            var targetFolderName = Utils.MakeFileName(sourceFileName, "ufo", outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFolderName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            UfoFontFormatter.Write(sourceFont, targetFolderName);
        }
    }

    public static void Atari8(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder,
        string templatePath)
    {
        var templateFilename = Path.Combine(templatePath, "atari8.fnt");
        Out.Write($"Using template {templateFilename}");
        var template = File.ReadAllBytes(templateFilename);

        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, Machines.Atari8.Extension, outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFileName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            using var target = File.Create(targetFileName);
            ByteFontFormatter.Write(sourceFont, target, Machines.Atari8.US, 128, i => new ArraySegment<byte>(template, i, 8));
        }
    }

    public static void Fzx(List<string> fileNames, IReadOnlyDictionary<int, char> charset, bool makeProportional,
        string outputFolder)
    {
        foreach (var fileName in fileNames)
        {
            Out.Write($"Generating FZX file for {fileName}");
            var font = ByteFontFormatter.Load(fileName, charset);
            using var target = File.Create(Utils.MakeFileName(fileName, "fzx", outputFolder));
            FzxFontFormatter.Write(font, target, makeProportional);
        }
    }

    public static void AmstradCpc(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder,
        string credit, int startLine)
    {
        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, "bas", outputFolder);

            var line = startLine;

            Out.Write($"Converting file {sourceFileName} to {targetFileName}");

            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            var output = new StringBuilder();

            output.Append($"{line} REM {Path.GetFileNameWithoutExtension(sourceFileName)} font\r\n");
            if (!String.IsNullOrEmpty(credit)) output.Append($"{line += 10} REM {credit}\r\n");

            var spaceIsBlank = sourceFont.Glyphs[' '].IsBlank();
            output.Append($"{line += 10} SYMBOL AFTER {(spaceIsBlank ? 33 : 32)}\r\n");

            foreach (var (key, value) in sourceFont.Glyphs.Where(g => !g.Value.IsBlank()).OrderBy(g => g.Key))
            {
                switch (key)
                {
                    case '©':
                        WriteSymbolLine(output, 164, value);
                        break;
                    default:
                        WriteSymbolLine(output, key, value);
                        break;
                }
            }

            File.WriteAllText(targetFileName, output.ToString());

            void WriteSymbolLine(StringBuilder output, int charIdx, Glyph glyph)
            {
                output.Append($"{line += 10} SYMBOL {charIdx},{String.Join(',', MakeList(glyph.Data))}\r\n");
            }
        }

        int[] MakeList(bool[,] data)
        {
            var results = new int[8];
            for (var y = 0; y < 8; y++)
            {
                var b = new Byte();
                for (var x = 0; x < 8; x++)
                {
                    if (data[x, y])
                        b |= (byte)(1 << 8 - 1 - x);
                }

                results[y] = b;
            }

            return results;
        }
    }

    public static void Msx(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder,
        string templatePath)
    {
        var templateFilename = Path.Combine(templatePath, "msx.fnt");
        Out.Write($"Using template {templateFilename}");
        var template = File.ReadAllBytes(templateFilename);

        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, Machines.Msx.Extension, outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFileName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            // TODO: Center font to left-most 5 pixels?
            using var target = File.Create(targetFileName);
            target.Write(template, 0, 32 * 8); // Low-ASCII
            ByteFontFormatter.Write(sourceFont, target, Machines.Msx.International, 224,
                i => new ArraySegment<byte>(template, i, 8));
        }
    }

    public static void Commodore64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder,
        string templatePath)
    {
        var bothCaseTemplate = Path.Combine(templatePath, "c64-both.ch8");
        var upperCaseTemplate = Path.Combine(templatePath, "c64-upper.ch8");
        Out.Write($"Using templates {bothCaseTemplate} and {upperCaseTemplate}");

        var cases = new[]
        {
            (
                template: File.ReadAllBytes(bothCaseTemplate),
                charset: Machines.Commodore64.BothUK,
                suffix: "both"
            ),
            (
                template: File.ReadAllBytes(upperCaseTemplate),
                charset: Machines.Commodore64.UpperUK,
                suffix: "upper"
            )
        };

        foreach (var sourceFileName in fileNames)
        {
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            using var characterRom = File.Create(Utils.MakeFileName(sourceFileName, "bin", outputFolder));

            foreach (var (template, charset, suffix) in cases)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, suffix + ".64c", outputFolder);

                Out.Write($"Converting file {sourceFileName} to {targetFileName}");

                using var memoryStream = new MemoryStream();
                ByteFontFormatter.Write(sourceFont, memoryStream, charset, 128, i => new ArraySegment<byte>(template, i, 8));

                using var targetFile = File.Create(targetFileName);
                targetFile.Write(new byte[] { 0x00, 0x38 }); // 64C header
                memoryStream.WriteTo(targetFile);
                memoryStream.WriteTo(characterRom);

                memoryStream.GetBuffer().InvertBuffer();

                memoryStream.WriteTo(targetFile);
                memoryStream.WriteTo(characterRom);
            }
        }
    }
}