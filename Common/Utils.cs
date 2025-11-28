using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld;

public static class Utils
{
    public static List<String> MatchGlobWithFiles(String inputPattern)
    {
        var splitPoint = GetGlobSplitPoint(inputPattern);
        var pattern = inputPattern[splitPoint..];
        var directory = splitPoint > 0 ? inputPattern[..splitPoint] : ".";
        Out.Write($"Matching files {pattern} in {directory}");

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude(pattern);

        var matchResults = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));
        return matchResults.Files.Select(f => Path.Combine(directory, f.Path)).ToList();
    }

    public static Byte[] ReadAllBytes(this Stream stream)
    {
        var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.GetBuffer();
    }

    public static String MakeFileName(String fileName, String extension, String folder)
    {
        return Path.Combine(folder, Path.ChangeExtension(Path.GetFileName(fileName), extension));
    }

    public static Int32 GetGlobSplitPoint(String pathGlob)
    {
        var doubleStar = pathGlob.IndexOf("**", StringComparison.Ordinal);
        return doubleStar > -1 ? doubleStar : pathGlob.LastIndexOf('\\') + 1;
    }

    public static Dictionary<Int32, Char> ToIndexedDictionary(this String sequence)
    {
        return sequence
            .Select((c, i) => Tuple.Create(c, i))
            .Where(c => c.Item1 != '\0')
            .ToDictionary(k => k.Item2, v => v.Item1);
    }
}