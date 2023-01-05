using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PixelWorld.Tools
{
    public static class ConvertTo
    {
        public static void Ufo(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
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

        public static void Png(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating preview file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);
                using var output = File.OpenWrite(Utils.MakeFileName(fileName, "png", outputFolder));
                PngFontFormatter.Write(sourceFont, output);
            }
        }

        record GBJson(string name) { public Object mapping = new { }; };

        public static void GBStudio(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, bool darkLight)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating GBStudio files for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var fontName = Path.GetFileNameWithoutExtension(fileName);
                var font = ByteFontFormatter.Create(reader, fontName, 0, sourceCharset);

                var outFileName = Path.Combine(outputFolder, fontName);

                GenerateBitmap(font, Path.ChangeExtension(outFileName, "png"), Gameboy.Palette[3], Gameboy.Palette[0]);
                File.WriteAllText(Path.ChangeExtension(outFileName, "json"), JsonSerializer.Serialize(new GBJson(fontName), gbStudioJsonOptions));

                if (darkLight)
                {
                    outFileName += "-dark";
                    GenerateBitmap(font, Path.ChangeExtension(outFileName, "png"), Gameboy.Palette[0], Gameboy.Palette[3]);
                    File.WriteAllText(Path.ChangeExtension(outFileName, "json"), JsonSerializer.Serialize(new GBJson(fontName + " dark"), gbStudioJsonOptions));
                }
            }
        }

        static JsonSerializerOptions gbStudioJsonOptions = new() { WriteIndented = true, IncludeFields = true };

        private static void GenerateBitmap(Fonts.Font sourceFont, string outputFilename, Color background, Color foreground)
        {
            using var output = File.OpenWrite(outputFilename);
            using var bitmap = new Bitmap(128, 112);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(background);
            sourceFont.DrawBitmap(bitmap, 16, Gameboy.Studio, foreground);
            bitmap.Save(output, PngFontFormatter.DefaultEncoder, PngFontFormatter.GetEncoderParameters(8));
        }

        public static void Atari8(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string templatePath)
        {
            var templateFilename = Path.Combine(templatePath, "atari8.fnt");
            Out.Write($"Using template {templateFilename}");
            var template = File.ReadAllBytes(templateFilename);

            foreach (var sourceFileName in fileNames)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, Machines.Atari8.Extension, outputFolder);
                Out.Write($"Converting file {sourceFileName} to {targetFileName}");
                using var source = File.OpenRead(sourceFileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);
                using var target = File.Create(targetFileName);
                ByteFontFormatter.Write(sourceFont, target, Machines.Atari8.US, 128, i => new ArraySegment<byte>(template, i, 8));
            }
        }

        public static void FZX(List<string> fileNames, IReadOnlyDictionary<int, char> charset, bool makeProportional, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating FZX file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, charset);
                using var target = File.Create(Utils.MakeFileName(fileName, "fzx", outputFolder));
                FZXFontFormatter.Write(font, target, makeProportional);
            }
        }

        public static void AmstradCPC(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string credit, int startLine)
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

        public static void MSX(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string templatePath)
        {
            var templateFilename = Path.Combine(templatePath, "msx.fnt");
            Out.Write($"Using template {templateFilename}");
            var template = File.ReadAllBytes(templateFilename);

            foreach (var sourceFileName in fileNames)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, Machines.MSX.Extension, outputFolder);
                Out.Write($"Converting file {sourceFileName} to {targetFileName}");
                using var source = File.OpenRead(sourceFileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(sourceFileName), 0, sourceCharset);
                // TODO: Center font to left-most 5 pixels?
                using var target = File.Create(targetFileName);
                target.Write(template, 0, 32 * 8); // Low-ASCII
                ByteFontFormatter.Write(sourceFont, target, Machines.MSX.International, 224, i => new ArraySegment<byte>(template, i, 8));
            }
        }

        public static void Commodore64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string templatePath)
        {
            var bothCaseTemplate = Path.Combine(templatePath, "c64-both.ch8");
            var upperCaseTemplate = Path.Combine(templatePath, "c64-upper.ch8");
            Out.Write($"Using templates {bothCaseTemplate} and {upperCaseTemplate}");

            var cases = new[] { (
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
