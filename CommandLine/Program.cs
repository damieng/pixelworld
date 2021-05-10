﻿using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PixelWorld;
using PixelWorld.BinarySource;
using PixelWorld.DumpScanners;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using PixelWorld.Tools;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CommandLine
{
    class Program
    {
        static string outputFolder;

        static void Main(string[] args)
        {
            var log = new StringBuilder();
            Out.Attach(Console.WriteLine);
            Out.Attach(s => log.AppendLine(s));
            Out.Write("PixelWorld command line tool v0.5");

            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            string command = args[0];
            string inputs = args[1];
            outputFolder = args[2];

            ProcessCommand(command, inputs);

            //                File.WriteAllText(Path.Combine(outputFolder, command + ".log"), log.ToString());
        }

        static int ProcessCommand(string command, string inputMatch)
        {
            var globSplitPoint = Utils.GetGlobSplitPoint(inputMatch);
            var glob = inputMatch.Substring(globSplitPoint);
            var directory = globSplitPoint > 0 ? inputMatch.Substring(0, globSplitPoint) : ".";
            Out.Write($"Matching files {glob} in {directory}");

            var matcher = new Matcher(StringComparison.CurrentCultureIgnoreCase);
            matcher.AddInclude(glob);
            var matchResults = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));
            var fileNames = matchResults.Files.Select(f => Path.Combine(directory, f.Path)).ToList();

            return command switch
            {
                "dedupe-title" => DedupePerTitle.Process(fileNames),
                "org-title" => OrganizeByTitle.Process(fileNames),
                "dump" => Dump(fileNames),
                "hunt" => Hunt(fileNames),
                "preview" => Preview(fileNames),
                "z80asmhex" => GenAsmHex("z80", "defb ", "&{0:x2}", fileNames),
                "x86asmhex" => GenAsmHex("x86", "db\t", "0x{0:x2}", fileNames),
                "6502asmhex" => GenAsmHex("6502", ".byte ", "${0:x2}", fileNames),
                "68000asmhex" => GenAsmHex("68000", "DC.B ", "${0:x2}", fileNames),
                "z80asmbinary" => GenZ80AsmBinary(fileNames),
                "zxtofzx" => GenFZX(fileNames, Spectrum.UK, false),
                "zxtofzxp" => GenFZX(fileNames, Spectrum.UK, true),
                "zxtocbm" => ConvertToC64(fileNames, Spectrum.UK),
                "zxtoa8" => ConvertToAtari8(fileNames, Spectrum.UK),
                _ => throw new InvalidOperationException($"Unknown command {command}"),
            };
        }

        private static int GenFZX(List<string> fileNames, IReadOnlyDictionary<int, char> charset, bool makeProportional)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating FZX file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, charset);
                using (var target = File.Create(MakeFileName(fileName, "fzx")))
                    FZXFontFormatter.Write(font, target, Spectrum.UK, makeProportional);
            }

            return fileNames.Count;
        }

        private static int GenAsmHex(string language, string defineByteInstruction, string format, List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating {language} assembly file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                var output = new StringBuilder();
                output.AppendFormat("\t; {0} font by DamienG https://damieng.com\n", Path.GetFileNameWithoutExtension(fileName));
                foreach (var glyph in font.Glyphs)
                {
                    output.Append("\t" + defineByteInstruction);
                    for (int y = 0; y < font.Height; y++)
                    {
                        var b = new Byte();
                        var charWidth = glyph.Value.Width;
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Value.Data[x, y])
                                b |= (byte)(1 << charWidth - 1 - x);
                        }
                        if (y > 0)
                            output.Append(",");
                        output.AppendFormat(format, b);
                    }
                    output.AppendFormat(" ; {0}\n", glyph.Key);
                }

                File.WriteAllText(MakeFileName(fileName, language + ".asm"), output.ToString());
            }

            return fileNames.Count;
        }

        private static int GenZ80AsmBinary(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating Z80 assembly file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                var output = new StringBuilder();
                output.AppendFormat("\t; {0} font\n", Path.GetFileNameWithoutExtension(fileName));
                foreach (var glyph in font.Glyphs)
                {
                    output.AppendFormat("\t; {0}\n", glyph.Key);

                    for (int y = 0; y < font.Height; y++)
                    {
                        var b = new Byte();
                        var charWidth = glyph.Value.Width;
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Value.Data[x, y])
                                b |= (byte)(1 << charWidth - 1 - x);
                        }
                        string binary = "00000000" + System.Convert.ToString(b, 2);
                        output.AppendFormat("\tdefb %{0}\n", binary.Substring(binary.Length - 8, 8));
                    }
                }

                File.WriteAllText(MakeFileName(fileName, "z80.asm"), output.ToString());
            }

            return fileNames.Count;
        }

        private static int Preview(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating preview file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                var bitmap = sourceFont.CreateBitmap();
                bitmap.Save(MakeFileName(fileName, "png"));
            }

            return fileNames.Count;
        }

        private static int ConvertToC64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset)
        {
            int outputCount = 0;
            var cases = new[] { (
                 template: File.ReadAllBytes(@"d:\zxo\_templates\commodore64\both.ch8"),
                 charset: Commodore64.bothUK,
                 suffix: "both"
                ),
                (
                 template: File.ReadAllBytes(@"d:\zxo\_templates\commodore64\upper.ch8"),
                 charset: Commodore64.upperUK,
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
                    var characterRom = File.Create(MakeFileName(fileName, "bin"));

                    foreach (var version in cases)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ByteFontFormatter.Write(sourceFont, memoryStream, version.charset, 128, i => new ArraySegment<byte>(version.template, i, 8));

                            var target64C = File.Create(MakeFileName(fileName, version.suffix + ".64c"));
                            target64C.Write(new byte[] { 0x00, 0x38 }); // 64C header
                            memoryStream.WriteTo(target64C);

                            memoryStream.WriteTo(characterRom);

                            InvertBuffer(memoryStream.GetBuffer());
                            memoryStream.WriteTo(target64C);
                            memoryStream.WriteTo(characterRom);

                            target64C.Close();
                        }
                    }

                    characterRom.Close();
                }

                outputCount++;
            }

            return outputCount;
        }

        private static void InvertBuffer(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)~buffer[i];
        }

        private static int ConvertToAtari8(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset)
        {
            int outputCount = 0;
            var template = File.ReadAllBytes(@"d:\zxo\_templates\atari8\default.fnt");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Converting file {fileName}");
                using (var source = File.OpenRead(fileName))
                using (var reader = new BinaryReader(source))
                {
                    var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);
                    var newFilename = MakeFileName(fileName, "fnt");
                    using (var target = File.Create(newFilename))
                        ByteFontFormatter.Write(sourceFont, target, Atari8.US, 128, i => new ArraySegment<byte>(template, i, 8));
                }
                outputCount++;
            }

            return outputCount;
        }

        private static int Hunt(IEnumerable<string> fileNames)
        {
            int inputsWithOutputsCount = 0;
            int outputCount = 0;

            var inputCount = fileNames.Count();
            Out.Write($"\nHunting {inputCount} files");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Opening file {fileName}");
                var matchOutputs = ExtractFontFromDumpFile(fileName);
                if (matchOutputs > 0)
                {
                    inputsWithOutputsCount++;
                    outputCount += matchOutputs;
                }
            }

            Out.Write($"{inputsWithOutputsCount} files yielded {outputCount} results");
            Out.Write($"{Math.Floor((double)inputsWithOutputsCount / inputCount * 100)}% success rate");

            return 0;
        }

        static int Dump(IEnumerable<string> fileNames)
        {
            Out.Write($"\nDumping {fileNames.Count()} files");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Opening file {fileName}");
                ProcessFile(fileName, WriteDumpToDisk);
            }

            return 0;
        }

        static int ExtractFontFromDumpFile(string fileName)
        {
            using var source = File.OpenRead(fileName);
            return ExtractFontFromMemoryBuffer(fileName, new ArraySegment<byte>(source.ReadAllBytes()));
        }

        private static int ExtractFontFromMemoryBuffer(string fileName, ArraySegment<byte> dump)
        {
            using var memory = new MemoryStream(dump.Array);
            using var reader = new BinaryReader(memory);
            var fontIndex = 0;
            foreach (var font in SpectrumDumpScanner.Read(reader, Path.GetFileNameWithoutExtension(fileName)))
            {
                var newFileName = MakeFileName(font.Name, "ch8");
                fontIndex++;
                Out.Write($"  Creating byte font {newFileName}");
                ByteFontFormatter.Write(font, File.Create(newFileName), Spectrum.UK, 96);
            }

            return fontIndex;
        }

        private static void CreateScreen(string fileName, BinaryReader reader)
        {
            var screenPreviewFileName = Utils.AddSubdirectory(MakeFileName(fileName, ".png"), "Screens");
            if (!File.Exists(screenPreviewFileName))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using var bitmap = SpectrumDumpScanner.GetScreenPreview(reader);
                bitmap?.Save(screenPreviewFileName, ImageFormat.Png);
            }
        }

        private static string MakeFileName(string fileName, string extension)
        {
            return Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), extension));
        }

        static void ProcessFile(string fileName, Func<string, ArraySegment<byte>, int> processor)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".zip":
                    using (var zip = ZipFile.Open(fileName, ZipArchiveMode.Read))
                        foreach (var entry in zip.Entries)
                            ProcessStream(entry.Name, entry.Open(), processor);
                    break;

                default:
                    using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        ProcessStream(fileName, file, processor);
                    break;
            }
        }

        private static void ProcessStream(string fileName, Stream stream, Func<string, ArraySegment<byte>, int> processor)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".z80":
                    {
                        processor(fileName, z80Binary.Read(stream));
                        break;
                    }
            }
        }

        static int WriteDumpToDisk(string name, ArraySegment<byte> dump)
        {
            if (dump.Count < 768)
            {
                Out.Write($"  Skipping {name} as too short {dump.Count}");
                return 0;
            }

            Out.Write($"  Dumping {name}");
            File.WriteAllBytes(MakeFileName(name, ".dmp"), dump.Array);
            return 1;
        }

        static readonly Z80BinarySource z80Binary = new Z80BinarySource();

        static void ShowUsage()
        {
            Out.Write("pw.exe <command> <filename/wildcard/glob> <outputFolder> [options]");
            Out.Write("  dump - produce memory dumps from zip/z80");
            Out.Write("  hunt - hunt dumps for possible fonts");
            Out.Write("  preview - generate a PNG preview for each font");
            Out.Write("  6502asmhex - generate a 6502 assembly def file for each font");
            Out.Write("  68000asmhex - generate a 68000 assembly def file for each font");
            Out.Write("  z80asmhex - generate a Z80 assembly def file for each font");
            Out.Write("  x86asmhex - generate an x86 assembly def file for each font");
            Out.Write("  z80asmbinary - generate a Z80 assembly def file for each font");
            Out.Write("  zxtofzx - generate a FZX file from a ZX file");
            Out.Write("  zxtofzxp - generate a FZX proportional file from a ZX file");
            Out.Write("  dedupe-title - purge duplicate fonts in the same title");
            Out.Write("  org-title - move fonts from the same title into a subfolder");
            Out.Write("  zxtocbm - convert Spectrum RAW to Commodore RAW");
            Out.Write("  zxtoa8 - convert Spectrum RAW to Atari 8-bit");
        }
    }
}
