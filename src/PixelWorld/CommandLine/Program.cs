using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PixelWorld.BinarySource;
using PixelWorld.Display;
using PixelWorld.Finders;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Font = PixelWorld.Fonts.Font;

namespace PixelWorld.CommandLine
{
    class Program
    {
        static string outputFolder;

        static void Main(string[] args)
        {
            Out.Attach(Console.WriteLine);
            Out.Write("PixelWorld font ripper");

            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            string command = args[0];
            string inputs = args[1];
            outputFolder = args[2];

            ProcessMatches(command, inputs);
        }

        static void ProcessMatches(string command, string inputMatch)
        {
            var matcher = new Matcher(StringComparison.CurrentCultureIgnoreCase);
            matcher.AddInclude(inputMatch);
            var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(".")));

            Out.Write($"Matching files {inputMatch}");

            foreach (var match in matches.Files)
            {
                Out.Write($"Opening file {match.Path}");

                switch (command)
                {
                    case "process":
                        ProcessFile(match.Path, ExtractFontFromMemoryBuffer);
                        break;
                    case "dump":
                        ProcessFile(match.Path, WriteDumpToDisk);
                        break;
                    case "hunt":
                        ExtractFontFromDumpFile(match.Path);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command {command}");
                }
            }
        }

        static void ExtractFontFromDumpFile(string fileName)
        {
            using (var source = File.OpenRead(fileName))
                ExtractFontFromMemoryBuffer(fileName, new ArraySegment<byte>(source.ReadAllBytes()));
        }

        private static void ExtractFontFromMemoryBuffer(string fileName, ArraySegment<byte> dump)
        {
            Out.Write($"  Extracting from ${fileName}");

            WriteScreenPreview(dump.Array, 0, MakeFilename(fileName, ".screen.png"));
            if (dump.Count > 49152)
                WriteScreenPreview(dump.Array, 0x14000 - 16384, MakeFilename(fileName, ".screen2.png"));

            using (var memory = new MemoryStream(dump.Array))
            {
                using (var reader = new BinaryReader(memory))
                {
                    var fonts = ByteFontPatternFinder.Read(reader, fileName);
                    int i = 1;
                    foreach (var font in fonts)
                    {
                        var newFileName = MakeFilename(fileName, $".{i++}.chr");
                        Out.Write($"    Creating byte font {newFileName}");
                        //                        Out.Write(font.ToDebug("Testing"));
                        // ByteFontFormatter.Write(font, File.Create(newFileName));
                        WriteFontPreviewPng(font, newFileName);
                    }
                }
            }
        }

        private static void WriteScreenPreview(byte[] buffer, int offset, string newFileName)
        {
            using (var b = SpectrumDisplay.GetBitmap(buffer, offset))
                b.Save(newFileName, ImageFormat.Png);
        }

        private static string MakeFilename(string filename, string extension)
        {
            return Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(filename), extension));
        }

        private static void WriteFontPreviewPng(Font font, string newFileName)
        {
            using (var b = new Bitmap(font.Glyphs.Sum(g => g.Value.Width), font.Height))
            {
                var offset = 0;
                foreach (var glyph in font.Glyphs)
                {
                    for (var y = 0; y < font.Height; y++)
                        for (var x = 0; x < glyph.Value.Width; x++)
                            b.SetPixel(offset + x, y, glyph.Value.Data[x, y] ? Color.Black : Color.White);

                    offset += glyph.Value.Width;
                }

                b.Save(MakeFilename(newFileName, ".png"), ImageFormat.Png);
            }
        }

        static void ProcessFile(string fileName, Action<string, ArraySegment<byte>> processor)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".zip":
                    {
                        using (var zip = ZipFile.Open(fileName, ZipArchiveMode.Read))
                            foreach (var entry in zip.Entries)
                            {
                                var extension = Path.GetExtension(entry.Name).ToLower();
                                if (extension == ".z80")
                                    processor(entry.Name, GetRawBinary(extension, entry.Open()));
                            }
                        break;
                    }
                case ".z80":
                    {
                        processor(fileName, GetRawBinary(Path.GetExtension(fileName), File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)));
                        break;
                    }
            }
        }

        static void WriteDumpToDisk(string name, ArraySegment<byte> dump)
        {
            Out.Write($"    Dumping {name}");
            File.WriteAllBytes(MakeFilename(name, ".dmp"), dump.Array);
        }

        static ArraySegment<byte> GetRawBinary(string extension, Stream source)
        {
            switch (extension.ToLower())
            {
                case ".z80":
                    return new Z80BinarySource().Read(source);
                default:
                    return new RawBinarySource().Read(source);
            }
        }

        static void ShowUsage()
        {
            Out.Write("pw.exe <command> <filename/wildcard/glob> <outputFolder>");
            Out.Write("  dump - produce memory dumps from zip/z80");
            Out.Write("  hunt - hunt dumps for possible fonts");
            Out.Write("  process - both steps in one go");
        }
    }
}
