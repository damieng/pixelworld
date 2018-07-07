using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PixelWorld;
using PixelWorld.BinarySource;
using PixelWorld.Display;
using PixelWorld.Finders;
using Font = PixelWorld.Fonts.Font;

namespace CommandLine
{
    class Program
    {
        static string outputFolder;

        static void Main(string[] args)
        {
            var log = new StringBuilder();
            Out.Attach(Console.WriteLine);
            Out.Attach((s) => log.AppendLine(s));
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

            File.WriteAllText(Path.Combine(outputFolder, command + ".log"), log.ToString());
        }

        static void ProcessMatches(string command, string inputMatch)
        {
            var globSplitPoint = Utils.GetGlobSplitPoint(inputMatch);
            var glob = inputMatch.Substring(globSplitPoint);
            var directory = globSplitPoint > 0 ? inputMatch.Substring(0, globSplitPoint) : ".";
            Out.Write($"Matching files {glob} in {directory}");

            var matcher = new Matcher(StringComparison.CurrentCultureIgnoreCase);
            matcher.AddInclude(glob);
            var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));
            int successes = 0;
            int rips = 0;

            foreach (var match in matches.Files)
            {
                Out.Write($"Opening file {match.Path}");
                var fullPath = Path.Combine(directory, match.Path);

                switch (command)
                {
                    case "process":
                        ProcessFile(fullPath, ExtractFontFromMemoryBuffer);
                        break;
                    case "dump":
                        ProcessFile(fullPath, WriteDumpToDisk);
                        break;
                    case "hunt":
                        var ripped = ExtractFontFromDumpFile(fullPath);
                        if (ripped > 0)
                        {
                            successes++;
                            rips += ripped;
                        }

                        break;
                    default:
                        throw new InvalidOperationException($"Unknown command {command}");
                }
            }

            var fileCount = matches.Files.Count();
            Out.Write($"\n{command}ing {fileCount} files\n{successes} files yielded {fileCount} results\n{Math.Floor((double)successes / fileCount * 100)}% success rate");
        }

        static int ExtractFontFromDumpFile(string fileName)
        {
            using (var source = File.OpenRead(fileName))
                return ExtractFontFromMemoryBuffer(fileName, new ArraySegment<byte>(source.ReadAllBytes()));
        }

        private static int ExtractFontFromMemoryBuffer(string fileName, ArraySegment<byte> dump)
        {
            using (var memory = new MemoryStream(dump.Array))
            {
                using (var reader = new BinaryReader(memory))
                {
                    var fontIndex = 0;
                    foreach (var font in SpectrumDumpScanner.Read(reader, Utils.GetTitle(fileName)))
                    {
                        var newFileName = MakeFileName(font.Name, $".{++fontIndex}.chr");
                        Out.Write($"  Creating byte font {newFileName}");
                        // ByteFontFormatter.Write(font, File.Create(newFileName));
                        WriteFontPreviewPng(font, newFileName);
                    }

                    if (fontIndex > 0)
                    {
                        var screenPreviewFileName = MakeFileName(fileName, ".screen.png");
                        if (!File.Exists(screenPreviewFileName))
                        {
                            reader.BaseStream.Seek(0, SeekOrigin.Begin);
                            using (var bitmap = SpectrumDumpScanner.GetScreenPreview(reader))
                                bitmap?.Save(screenPreviewFileName, ImageFormat.Png);
                        }
                    }

                    return fontIndex;
                }
            }
        }

        private static string MakeFileName(string fileName, string extension)
        {
            return Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), extension));
        }

        private static void WriteFontPreviewPng(Font font, string newFileName)
        {
            const int rows = 3;
            var fullWidth = font.Glyphs.Sum(g => g.Value.Width);
            var previewWidth = fullWidth / rows;
            var glphysPerRow = font.Glyphs.Count / rows;

            using (var b = new Bitmap(previewWidth, font.Height * rows))
            {
                var xOff = 0;
                var yOff = 0;
                int cIdx = 0;
                foreach (var glyph in font.Glyphs)
                {
                    for (var y = 0; y < font.Height; y++)
                        for (var x = 0; x < glyph.Value.Width; x++)
                            b.SetPixel(xOff + x, yOff + y, glyph.Value.Data[x, y] ? Color.Black : Color.White);

                    xOff += glyph.Value.Width;
                    cIdx++;
                    if (cIdx % glphysPerRow == 0)
                    {
                        yOff += font.Height;
                        xOff = 0;
                    }
                }

                b.Save(MakeFileName(newFileName, ".png"), ImageFormat.Png);
            }
        }

        static void ProcessFile(string fileName, Func<string, ArraySegment<byte>, int> processor)
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
                                {
                                    Out.Write($" Extracting from {entry.Name}");
                                    processor(entry.Name, GetRawBinary(extension, entry.Open()));
                                }
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

        static int WriteDumpToDisk(string name, ArraySegment<byte> dump)
        {
            Out.Write($" Dumping {name}");
            File.WriteAllBytes(MakeFileName(name, ".dmp"), dump.Array);
            return 1;
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
            Out.Write("  process - produce memory dumps and hunt");
        }
    }
}
