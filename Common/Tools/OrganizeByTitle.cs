using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld.Tools
{
    public static class OrganizeByTitle
    {
        public static int Process(IEnumerable<string> fileNames)
        {
            var titles = fileNames
                .GroupBy(f => MakeSimplifiedName(f))
                .ToList();

            foreach(var title in titles)
            {
                var targetFolder = title.Key;
                Directory.CreateDirectory(targetFolder);

                foreach (var file in title)
                    File.Move(file, Path.Combine(targetFolder, Path.GetFileName(file)));
            }

            return 0;
        }

        static string MakeSimplifiedName(string fileName)
        {
            return String.Join(")", fileName.Split(')').Take(2)) + ")";
        }
    }
}
