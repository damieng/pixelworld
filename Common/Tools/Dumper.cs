using PixelWorld.BinarySource;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools;

public static class Dumper
{
    public static void Dump(List<String> fileNames, String outputFolder)
    {
        Out.Write($"\nDumping {fileNames.Count} files");

        foreach (var fileName in fileNames)
        {
            Out.Write($"Opening file {fileName}");
            using var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            SnapshotProcessor.ProcessStream(fileName, file, (a, b) => WriteDumpToDisk(a, b, outputFolder));
        }
    }

    public static Boolean WriteDumpToDisk(String fileName, ArraySegment<Byte> dump, String outputFolder)
    {
        if (dump.Array is null) throw new ArgumentOutOfRangeException(nameof(dump), "Array is null");

        if (dump.Count < 768)
        {
            Out.Write($"  Skipping {fileName} as too short {dump.Count}");
            return false;
        }

        var newFileName = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "dmp"));

        Out.Write($"  Dumping {fileName} to {newFileName}");

        File.WriteAllBytes(newFileName, dump.Array.AsSpan(dump.Offset, dump.Count).ToArray());
        return true;
    }
}