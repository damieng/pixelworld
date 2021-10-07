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
        public static int ConvertToAtari8(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            int outputCount = 0;
            var template = File.ReadAllBytes(@"templates\atari8.fnt");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Converting file {fileName}");
                using (var source = File.OpenRead(fileName))
                using (var reader = new BinaryReader(source))
                {
                    var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);
                    var newFilename = Utils.MakeFileName(fileName, "fnt", outputFolder);
                    using var target = File.Create(newFilename);
                    ByteFontFormatter.Write(sourceFont, target, Atari8.US, 128, i => new ArraySegment<byte>(template, i, 8));
                }
                outputCount++;
            }

            return outputCount;
        }

        public static int ConvertToFZX(List<string> fileNames, IReadOnlyDictionary<int, char> charset, bool makeProportional, string outputFolder)
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

            return fileNames.Count;
        }

        public static int ConvertToAmstradCPC(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, string credit)
        {
            int outputCount = 0;
            var line = 9000;

            foreach (var fileName in fileNames)
            {
                line = 9000;

                Out.Write($"Converting file {fileName}");
                using (var source = File.OpenRead(fileName))
                using (var reader = new BinaryReader(source))
                {
                    var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);
                    var newFilename = Utils.MakeFileName(fileName, "bas", outputFolder);

                    var output = new StringBuilder();

                    output.AppendFormat("{0} REM {1} font\r\n", line, Path.GetFileNameWithoutExtension(fileName));
                    if (!String.IsNullOrEmpty(credit))
                    {
                        output.AppendFormat("{0} REM {1}\r\n", line += 10, credit);
                    }

                    var spaceIsBlank = sourceFont.Glyphs[' '].IsBlank();
                    output.AppendFormat("{0} SYMBOL AFTER {1}\r\n", line += 10, spaceIsBlank ? 33 : 32);

                    foreach (var g in sourceFont.Glyphs.Where(g => !g.Value.IsBlank()).OrderBy(g => g.Key))
                    {
                        switch (g.Key)
                        {
                            case '©':
                                WriteSymbolLine(output, 164, g.Value);
                                break;
                            default:
                                WriteSymbolLine(output, g.Key, g.Value);
                                break;
                        }
                    }

                    File.WriteAllText(newFilename, output.ToString());
                }
                outputCount++;
            }

            void WriteSymbolLine(StringBuilder output, int charIdx, Glyph glyph)
            {
                output.AppendFormat("{0} SYMBOL {1},{2}\r\n", line += 10, charIdx, String.Join(',', MakeList(glyph.Data)));
            }

            int[] MakeList(bool[,] data)
            {
                var results = new int[8];
                for (int y = 0; y < 8; y++)
                {
                    var b = new Byte();
                    for (int x = 0; x < 8; x++)
                    {
                        if (data[x, y])
                            b |= (byte)(1 << 8 - 1 - x);
                    }
                    results[y] = b;
                }
                return results;
            }

            return outputCount;
        }

        public static int ConvertToC64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            int outputCount = 0;
            var cases = new[] { (
                 template: File.ReadAllBytes(@"templates\c64-both.ch8"),
                 charset: Commodore64.BothUK,
                 suffix: "both"
                ),
                (
                 template: File.ReadAllBytes(@"templates\c64-upper.ch8"),
                 charset: Commodore64.UpperUK,
                 suffix: "upper"
                )
            };

            foreach (var fileName in fileNames)
            {
                Out.Write($"Converting file {fileName}");

                using (var source = File.OpenRead(fileName))
                using (var reader = new BinaryReader(source))
                {
                    var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);
                    var characterRom = File.Create(Utils.MakeFileName(fileName, "bin", outputFolder));

                    foreach (var (template, charset, suffix) in cases)
                    {
                        using var memoryStream = new MemoryStream();
                        ByteFontFormatter.Write(sourceFont, memoryStream, charset, 128, i => new ArraySegment<byte>(template, i, 8));

                        var target64C = File.Create(Utils.MakeFileName(fileName, suffix + ".64c", outputFolder));
                        target64C.Write(new byte[] { 0x00, 0x38 }); // 64C header
                        memoryStream.WriteTo(target64C);
                        memoryStream.WriteTo(characterRom);

                        memoryStream.GetBuffer().InvertBuffer();

                        memoryStream.WriteTo(target64C);
                        memoryStream.WriteTo(characterRom);

                        target64C.Close();
                    }

                    characterRom.Close();
                }

                outputCount++;
            }

            return outputCount;
        }
    }
}
