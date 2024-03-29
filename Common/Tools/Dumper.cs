﻿using PixelWorld.BinarySource;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace PixelWorld.Tools;

public static class Dumper
{
    public static void Dump(List<string> fileNames, string outputFolder)
    {
        Out.Write($"\nDumping {fileNames.Count} files");

        foreach (var fileName in fileNames)
        {
            Out.Write($"Opening file {fileName}");
            using var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            ProcessStream(fileName, file, (a, b) => WriteDumpToDisk(a, b, outputFolder));
        }
    }

    public static void ProcessStream(string fileName, Stream stream, Func<string, ArraySegment<byte>, bool> processor, bool processUnknown = false)
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
                processor(fileName, SnaBinarySource.Instance.GetMemory(stream));
                break;
            }
            case ".z80":
            {
                processor(fileName, Z80BinarySource.Instance.GetMemory(stream));
                break;
            }

            default:
            {
                if (processUnknown)
                {
                    processor(fileName, stream.ReadAllBytes());
                }
                else
                {
                    Out.Write($"  Skipping file {fileName} as unknown extension {extension}");
                }
                break;
            }
        }
    }

    public static bool WriteDumpToDisk(string fileName, ArraySegment<byte> dump, string outputFolder)
    {
        if (dump.Array is null) throw new ArgumentOutOfRangeException(nameof(dump), "Array is null");

        if (dump.Count < 768)
        {
            Out.Write($"  Skipping {fileName} as too short {dump.Count}");
            return false;
        }

        var newFileName = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "dmp"));

        Out.Write($"  Dumping {fileName} to {newFileName}");

        File.WriteAllBytes(newFileName, dump.Array);
        return true;
    }
}