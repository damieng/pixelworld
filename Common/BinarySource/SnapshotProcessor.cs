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
}
