using System;
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

        private static readonly Regex titleFromPath = new Regex(@"^.*[\/\\]([^.\(\[ ]+).*");

        public static bool TitlesMatch(params string[] fileNames)
        {
            return fileNames
                .Select(f => GetTitle(f).ToLowerInvariant())
                .Distinct()
                .Count() == 1;
        }

        public static string GetTitle(string fileName)
        {
            return titleFromPath.Match(fileName).Captures[0].Value.Trim();
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
    }
}
