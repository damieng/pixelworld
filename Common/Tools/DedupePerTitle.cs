using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace PixelWorld.Tools
{
    public static class DedupePerTitle
    {
        public static void Process(string directory, IEnumerable<FilePatternMatch> files)
        {
            var titles = files
                .Select(f => Path.Combine(directory, f.Path))
                .GroupBy(f => MakeSimplifiedName(f))
                .Where(v => v.Count() > 1)
                .ToDictionary(k => k.Key, v => v.Select(f => Tuple.Create(f, CreateHash(MD5.Create(), f))).ToArray());

            foreach (var title in titles)
            {
                var fontsForTitle = title.Value.GroupBy(k => k.Item2, v => v.Item1).First().Select(v => v).ToArray();
                if (fontsForTitle.Length > 1) {
                    var dupes = fontsForTitle.Skip(1).ToArray();
                    Out.Write($"Removing duplicate fonts for {title.Key}\n\t{String.Join("\n\t", dupes).Trim()}");

                    foreach (var dupe in fontsForTitle)
                        File.Delete(dupe);
                }
            }
        }

        public static string CreateHash(HashAlgorithm algorithm, string fileName)
        {
            return algorithm.ComputeHash(File.ReadAllBytes(fileName)).ToHex();
        }

        public static string MakeSimplifiedName(string fileName)
        {
            return String.Join(")", Path.GetFileNameWithoutExtension(fileName).Split(')').Take(3)) + ")";
        }
    }
}
