using Glob;
using PixelWorld.BinarySource;
using System;
using System.IO;
using System.IO.Compression;
using PixelWorld.Finders;
using PixelWorld.Formatters;

namespace PixelWorld.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            Out.Attach(Console.WriteLine);
            Out.Write("PixelWorld font ripper");

            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            string command = args[0];
            string filenameOrGlob = args[1];

            if (File.Exists(filenameOrGlob))
                ProcessFile(command, filenameOrGlob);
            else
                ProcessMatches(command, filenameOrGlob);
        }

        static void ProcessMatches(string command, string glob)
        {
            var matches = new DirectoryInfo(".").GlobFileSystemInfos(glob);
            Out.Write($"Matching files {glob}");

            foreach (var match in matches)
                ProcessFile(command, match.FullName);
        }

        static void ProcessFile(string command, string fileName)
        {
            Out.Write($"Opening file {fileName}");

            switch (command)
            {
                case "extract":
                    ExtractFile(fileName);
                    break;
                case "hunt":
                    HuntFile(fileName);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown command {command}");
            }
        }

        static void HuntFile(string fileName)
        {
            using (var source = File.OpenRead(fileName))
            using (var reader = new BinaryReader(source))
            {
                var fonts = ByteFontPatternFinder.Read(reader, fileName);
                Out.Write($"Hunting found {fonts.Count} possible fonts");
                int i = 1;
                foreach (var font in fonts)
                {
                    var newFileName = Path.ChangeExtension(fileName, $".{i++}.bin");
                    Out.Write($"  Creating byte font {newFileName}");
                    Out.Write(font.Glyphs['z'].ToDebug());
                    var output = File.Create(newFileName);
                    ByteFontFormatter.Write(font, output);
                }
            }

        }

        static void ExtractFile(string fileName)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".zip":
                    {
                        using (var zip = ZipFile.Open(fileName, ZipArchiveMode.Read))
                        {
                            Out.Write($"  Processing {zip.Entries.Count} zip entries");

                            var targetPath = Path.GetDirectoryName(fileName);
                            foreach (var entry in zip.Entries)
                            {
                                var targetName = Path.Combine(targetPath, entry.FullName);
                                ExtractStream(targetName, entry.Open());
                            }
                        }

                        break;
                    }
                case ".z80":
                    {
                        ExtractStream(fileName, File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                        break;
                    }
            }
        }

        static void ExtractStream(string fileName, Stream stream)
        {
            var binary = GetRawBinary(fileName, stream);
            var newFileName = Path.ChangeExtension(fileName, ".ramdump");
            Out.Write($"    Extracting {newFileName}");
            File.WriteAllBytes(newFileName, binary.Array);
        }

        static ArraySegment<byte> GetRawBinary(string fileName, Stream source)
        {
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".z80":
                    return new Z80BinarySource().Read(source);
                default:
                    return new RawBinarySource().Read(source);
            }
        }

        static void ShowUsage()
        {
            Out.Write($"pw.exe <command> <filename/wildcard/glob>");
            Out.Write($"  extract - extract memory dumps from files");
            Out.Write($"  hunt - hunt dumps for possible fonts");
        }
    }
}
