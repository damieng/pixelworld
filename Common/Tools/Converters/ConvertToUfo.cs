using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToUfo
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder)
    {
        foreach (var sourceFileName in fileNames)
        {
            var targetFolderName = Utils.MakeFileName(sourceFileName, "ufo", outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFolderName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            UfoFontFormatter.Write(sourceFont, targetFolderName);
        }
    }
}
