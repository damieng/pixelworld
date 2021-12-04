using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PixelWorld.Tools
{
    public static class Converter
    {
        public static void ConvertToUfo(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            foreach (var sourceFileName in fileNames)
            {
                var targetFolderName = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(sourceFileName), ".ufo"));
                Out.Write($"Converting file {sourceFileName} to {targetFolderName}");
                using var source = File.OpenRead(sourceFileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);
                UfoFontFormatter.Write(sourceFont, targetFolderName);
            }

        }

        public static void ConvertToAtari8(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string templatePath)
        {
            var templateFull = Path.Combine(templatePath, "atari8.fnt");
            Out.Write("Using template " + templateFull);
            var template = File.ReadAllBytes(templateFull);

            foreach (var sourceFileName in fileNames)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, "fnt", outputFolder);
                Out.Write($"Converting file {sourceFileName} to {targetFileName}");
                using var source = File.OpenRead(sourceFileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);
                using var target = File.Create(targetFileName);
                ByteFontFormatter.Write(sourceFont, target, Atari8.US, 128, i => new ArraySegment<byte>(template, i, 8));
            }
        }

        public static void ConvertToFZX(List<string> fileNames, IReadOnlyDictionary<int, char> charset, bool makeProportional, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating FZX file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, charset);
                using var target = File.Create(Utils.MakeFileName(fileName, "fzx", outputFolder));
                FZXFontFormatter.Write(font, target, Spectrum.UK, makeProportional);
            }
        }

        public static void ConvertToAmstradCPC(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string credit, int startLine)
        {
            foreach (var sourceFileName in fileNames)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, "bas", outputFolder);

                var line = startLine;

                Out.Write($"Converting file {sourceFileName} to {targetFileName}");
                using (var source = File.OpenRead(sourceFileName))
                using (var reader = new BinaryReader(source))
                {
                    var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);

                    var output = new StringBuilder();

                    output.AppendFormat("{0} REM {1} font\r\n", line, Path.GetFileNameWithoutExtension(sourceFileName));
                    if (!String.IsNullOrEmpty(credit)) output.AppendFormat("{0} REM {1}\r\n", line += 10, credit);

                    var spaceIsBlank = sourceFont.Glyphs[' '].IsBlank();
                    output.AppendFormat("{0} SYMBOL AFTER {1}\r\n", line += 10, spaceIsBlank ? 33 : 32);

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
                }

                void WriteSymbolLine(StringBuilder output, int charIdx, Glyph glyph)
                {
                    output.AppendFormat("{0} SYMBOL {1},{2}\r\n", line += 10, charIdx, String.Join(',', MakeList(glyph.Data)));
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

        public static void ConvertToC64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string templatePath)
        {
            var bothCaseTemplate = Path.Combine(templatePath, "c64-both.ch8");
            var upperCaseTemplate = Path.Combine(templatePath, "c64-upper.ch8");
            Out.Write($"Using templates {bothCaseTemplate} and {upperCaseTemplate}");

            var cases = new[] { (
                 template: File.ReadAllBytes(bothCaseTemplate),
                 charset: Commodore64.BothUK,
                 suffix: "both"
                ),
                (
                 template: File.ReadAllBytes(upperCaseTemplate),
                 charset: Commodore64.UpperUK,
                 suffix: "upper"
                )
            };

            foreach (var sourceFileName in fileNames)
            {
                using var source = File.OpenRead(sourceFileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);
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
}
