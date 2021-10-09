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
                using var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                ProcessStream(fileName, file, (a, b) => WriteDumpToDisk(a, b, outputFolder));
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

        public static void ProcessStream(string fileName, Stream stream, Func<string, ArraySegment<byte>, int> processor)
        {

            string extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".zip":
                    {
                        using var zip = ZipFile.Open(fileName, ZipArchiveMode.Read);
                        foreach (var entry in zip.Entries)
                            ProcessStream(entry.Name, entry.Open(), processor);
                        break;
                    }

                case ".sna":
                    {
                        processor(fileName, SNABinarySource.Instance.Read(stream));
                        break;
                    }
                case ".z80":
                    {
                        processor(fileName, ZXSNABinarySource.Instance.Read(stream));
                        break;
                    }

                default:
                    {
                        Out.Write($"  Skipping file {fileName} as unknown extension {extension}");
                        break;
                    }
            }
        }

        public static int WriteDumpToDisk(string fileName, ArraySegment<byte> dump, string outputFolder)
        {
            if (dump.Count < 768)
            {
                Out.Write($"  Skipping {fileName} as too short {dump.Count}");
                return 0;
            }

            var newFileName = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "dmp"));

            Out.Write($"  Dumping {fileName} to {newFileName}");

            File.WriteAllBytes(newFileName, dump.Array);
            return 1;
        }
    }
}
