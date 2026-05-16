using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToMsx
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
    {
        var templateFilename = Path.Combine(templatePath, "msx.fnt");
        Out.Write($"Using template {templateFilename}");
        var template = File.ReadAllBytes(templateFilename);

        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, Msx.Extension, outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFileName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            using var target = File.Create(targetFileName);
            target.Write(template, 0, 32 * 8); // Low-ASCII
            ByteFontFormatter.Write(sourceFont, target, Msx.International, 224,
                i => new ArraySegment<Byte>(template, i, 8));
        }
    }
}
