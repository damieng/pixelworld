using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld
{
    public static class Utils
    {
        public static List<string> MatchGlobWithFiles(string inputPattern)
        {
            var splitPoint = GetGlobSplitPoint(inputPattern);
            var pattern = inputPattern[splitPoint..];
            var directory = splitPoint > 0 ? inputPattern.Substring(0, splitPoint) : ".";
            Out.Write($"Matching files {pattern} in {directory}");

            var matcher = new Matcher(StringComparison.CurrentCultureIgnoreCase);
            matcher.AddInclude(pattern);

            var matchResults = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));
            return matchResults.Files.Select(f => Path.Combine(directory, f.Path)).ToList();
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.GetBuffer();
        }

        public static string MakeFileName(string fileName, string extension, string outputFolder)
        {
            return Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(fileName), extension));
        }

        public static int GetGlobSplitPoint(string pathGlob)
        {
            var doubleStar = pathGlob.IndexOf("**", StringComparison.Ordinal);
            return doubleStar > -1 ? doubleStar : pathGlob.LastIndexOf('\\') + 1;
        }

        public static string AddSubdirectory(string fullPath, string subdirectory)
        {
            var directory = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileName(fullPath);
            return Path.Combine(directory, subdirectory, fileName);
        }

        public static Dictionary<int, char> ToIndexedDictionary(this string sequence)
        {
            return sequence.Select((c, i) => Tuple.Create(c, i)).Where((c, i) => c.Item1 != '\0').ToDictionary(k => k.Item2, v => v.Item1);
        }
    }
}
