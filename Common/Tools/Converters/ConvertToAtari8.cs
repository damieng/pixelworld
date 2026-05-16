using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToAtari8
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
    {
        var templateFilename = Path.Combine(templatePath, "atari8.fnt");
        Out.Write($"Using template {templateFilename}");
        var template = File.ReadAllBytes(templateFilename);

        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, Atari8.Extension, outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFileName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            using var target = File.Create(targetFileName);
            ByteFontFormatter.Write(sourceFont, target, Atari8.US, 128, i => new ArraySegment<Byte>(template, i, 8));
        }
    }
}
