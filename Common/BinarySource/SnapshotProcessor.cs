using PixelWorld.Tools;
using System;
using System.IO;
using System.IO.Compression;

namespace PixelWorld.BinarySource;

public static class SnapshotProcessor
{
    public static void ProcessStream(String fileName, Stream stream, Func<String, ArraySegment<Byte>, Boolean> processor, Boolean processUnknown = false)
    {
        var extension = Path.GetExtension(fileName).ToLower();

        if (extension == ".zip")
        {
            using var zip = ZipFile.Open(fileName, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries)
                ProcessStream(entry.Name, entry.Open(), processor);
            return;
        }

        var source = BinarySourceFactory.Create(extension);
        if (source is not null)
        {
            processor(fileName, source.GetMemory(stream));
            return;
        }

        if (processUnknown)
        {
            processor(fileName, stream.ReadAllBytes());
        }
        else
        {
            Out.Write($"  Skipping file {fileName} as unknown extension {extension}");
        }
    }
}
