using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToFzx
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> charset, Boolean makeProportional, String outputFolder)
    {
        foreach (var fileName in fileNames)
        {
            Out.Write($"Generating FZX file for {fileName}");
            var font = ByteFontFormatter.Load(fileName, charset);
            using var target = File.Create(Utils.MakeFileName(fileName, "fzx", outputFolder));
            FzxFontFormatter.Write(font, target, makeProportional);
        }
    }
}
