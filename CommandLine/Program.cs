using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PixelWorld;
using PixelWorld.BinarySource;
using PixelWorld.DumpScanners;
using PixelWorld.Formatters;
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
            Out.Write("PixelWorld command line tool");

            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            string command = args[0];
            string inputs = args[1];
            outputFolder = args[2];

            ProcessCommand(command, inputs);

            File.WriteAllText(Path.Combine(outputFolder, command + ".log"), log.ToString());
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

            switch (command)
            {
                case "dedupe-title":
                    return DedupePerTitle.Process(fileNames);
                case "org-title":
                    return OrganizeByTitle.Process(fileNames);
                case "dump":
                    return Dump(fileNames);
                case "hunt":
                    return Hunt(fileNames);
                default:
                    throw new InvalidOperationException($"Unknown command {command}");
            }
        }

        static int Hunt(IEnumerable<string> fileNames)
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
                Out.Write($"{inputsWithOutputsCount} files yielded {outputCount} results");
                Out.Write($"{Math.Floor((double)inputsWithOutputsCount / inputCount * 100)}% success rate");
            }

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
            using (var source = File.OpenRead(fileName))
                return ExtractFontFromMemoryBuffer(fileName, new ArraySegment<byte>(source.ReadAllBytes()));
        }

        private static int ExtractFontFromMemoryBuffer(string fileName, ArraySegment<byte> dump)
        {
            using (var memory = new MemoryStream(dump.Array))
            using (var reader = new BinaryReader(memory))
            {
                var fontIndex = 0;
                foreach (var font in SpectrumDumpScanner.Read(reader, Path.GetFileNameWithoutExtension(fileName)))
                {
                    var newFileName = MakeFileName(font.Name, $"ch8");
                    fontIndex++;
                    Out.Write($"  Creating byte font {newFileName}");

                    ByteFontFormatter.Write(font, File.Create(newFileName));
                }

                return fontIndex;
            }
        }

        private static void CreateScreen(string fileName, BinaryReader reader)
        {
            var screenPreviewFileName = Utils.AddSubdirectory(MakeFileName(fileName, ".png"), "Screens");
            if (!File.Exists(screenPreviewFileName))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using (var bitmap = SpectrumDumpScanner.GetScreenPreview(reader))
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
        static readonly RawBinarySource rawBinary = new RawBinarySource();

        static void ShowUsage()
        {
            Out.Write("pw.exe <command> <filename/wildcard/glob> <outputFolder>");
            Out.Write("  dump - produce memory dumps from zip/z80");
            Out.Write("  hunt - hunt dumps for possible fonts");
            Out.Write("  dedupe-title - purge duplicate fonts in the same title");
            Out.Write("  org-title - move fonts from the same title into a subfolder");
        }
    }
}
