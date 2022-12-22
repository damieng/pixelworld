using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools
{
    public static class ConvertFrom
    {
        public static void Png(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                Out.Write($"Generating ch8 file from {fileName}");

                var sourceFont = new Font(name);
                PngFontFormatter.Read(sourceFont, File.OpenRead(fileName), sourceCharset);

                using var output = File.Create(Utils.MakeFileName(fileName, "ch8", outputFolder));
                ByteFontFormatter.Write(sourceFont, output, sourceCharset, 96);
            }
        }

        public static void Commodore64(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                var name = Path.GetFileNameWithoutExtension(fileName);
                Out.Write($"Generating ch8 file from {fileName}");

                using var source = File.OpenRead(fileName);

                using var reader = new BinaryReader(source);
                source.Seek(2, SeekOrigin.Current); // Skip header

                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, sourceCharset);

                using var output = File.Create(Utils.MakeFileName(fileName, "ch8", outputFolder));
                ByteFontFormatter.Write(sourceFont, output, sourceCharset, 96);
            }
        }
    }
}
