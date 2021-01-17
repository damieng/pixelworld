using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PixelWorld
{
    public static class Utils
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.GetBuffer();
        }

        private static readonly Regex titleFromPath = new Regex(@"^.*[\/\\]([^.\(\[]+).*");

        public static bool TitlesMatch(params string[] fileNames)
        {
            return fileNames
                .Select(f => GetTitle(f).ToLowerInvariant())
                .Distinct()
                .Count() == 1;
        }

        public static string GetTitle(string fileName)
        {
            return titleFromPath.Match(fileName).Groups[1].Value.Trim();
        }

        public static bool IsAbsolutePath(string path)
        {
            return path.StartsWith("/") || path.StartsWith("\\") || path.Contains(":\\");
        }

        public static int GetGlobSplitPoint(string pathGlob)
        {
            var doubleStar = pathGlob.IndexOf("**", StringComparison.Ordinal);
            return doubleStar > -1 ? doubleStar : pathGlob.LastIndexOf('\\') + 1;
        }

        public static void DumpCandidates(byte[][] candidates)
        {
            foreach (var candidate in candidates)
            {
                foreach (var row in candidate)
                {
                    Console.WriteLine(
                        Convert.ToString(row, 2).PadLeft(8, '0').Replace('0', ' ').Replace('1', '#'));
                }
            }
        }

        public static string ToHex(this byte[] input)
        {
            var output = new char[input.Length * 2];
            for (int i = 0; i < input.Length; i++)
            {
                var r = i * 2;
                var h = input[i].ToString("x2");
                output[r] = h[0];
                output[r + 1] = h[1];
            }
            return new String(output);
        }

        public static string PascalCaseToTitle(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;

            int spacesToAdd = 0;
            for (int i = 1; i < input.Length; i++)
                if (Char.IsUpper(input[i]))
                    spacesToAdd++;
            var output = new char[input.Length + spacesToAdd];
            output[0] = input[0];
            int o = 1;
            for (int i = 1; i < input.Length; i++)
            {
                if (Char.IsUpper(input[i]))
                    output[o++] = ' ';
                output[o] = input[i];
            }

            return new String(output);
        }

        public static void AddRange<T>(this List<Tuple<int, T>> list, IEnumerable<int> source, T label)
        {
            list.AddRange(source.Select(s => Tuple.Create(s, label)));
        }

        public static string AddSubdirectory(string fullPath, string subdirectory)
        {
            var directory = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileName(fullPath);
            return Path.Combine(directory, subdirectory, fileName);
        }

        public static Dictionary<int, char> ToIndexedDictionary (this string sequence)
        {
            return sequence.Select((c, i) => Tuple.Create(c, i)).Where((c, i) => c.Item1 != '\0').ToDictionary(k => k.Item2, v => v.Item1);
        }
    }
}
