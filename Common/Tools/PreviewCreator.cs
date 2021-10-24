using PixelWorld.Formatters;
using PixelWorld.Machines;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools
{
    public static class PreviewCreator
    {
        public static void Preview(List<string> fileNames, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating preview file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                using var bitmap = sourceFont.CreateBitmap();
                bitmap.Save(Utils.MakeFileName(fileName, "png", outputFolder));
            }
        }
    }
}
