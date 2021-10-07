using PixelWorld.BinarySource;
using PixelWorld.DumpScanners;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PixelWorld.Tools
{
    public static class Dumper
    {
        public static int Dump(IEnumerable<string> fileNames, string outputFolder)
        {
            Out.Write($"\nDumping {fileNames.Count()} files");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Opening file {fileName}");
                ProcessFile(fileName, (a, b) => WriteDumpToDisk(a, b, outputFolder));
            }

            return 0;
        }

        public static void CreateScreen(string fileName, BinaryReader reader, string outputFolder)
        {
            var screenPreviewFileName = Utils.AddSubdirectory(Utils.MakeFileName(fileName, ".png", outputFolder), "Screens");
            if (!File.Exists(screenPreviewFileName))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                using var bitmap = SpectrumDumpScanner.GetScreenPreview(reader);
                bitmap?.Save(screenPreviewFileName, ImageFormat.Png);
            }
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

        static readonly Z80BinarySource z80Binary = new();

        public static void ProcessStream(string fileName, Stream stream, Func<string, ArraySegment<byte>, int> processor)
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

        public static int WriteDumpToDisk(string name, ArraySegment<byte> dump, string outputFolder)
        {
            if (dump.Count < 768)
            {
                Out.Write($"  Skipping {name} as too short {dump.Count}");
                return 0;
            }

            Out.Write($"  Dumping {name}");

            var newFileName = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(name), "*.dmp"));

            File.WriteAllBytes(newFileName, dump.Array);
            return 1;
        }
    }
}
