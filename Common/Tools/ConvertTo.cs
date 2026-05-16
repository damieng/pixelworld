using PixelWorld.Tools.Converters;
using System;
using System.Collections.Generic;

namespace PixelWorld.Tools;

public static class ConvertTo
{
    public static void Ufo(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder)
        => ConvertToUfo.Convert(fileNames, sourceCharset, outputFolder);

    public static void Atari8(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
        => ConvertToAtari8.Convert(fileNames, sourceCharset, outputFolder, templatePath);

    public static void CoCoVGA(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
        => ConvertToCoCoVGA.Convert(fileNames, sourceCharset, outputFolder, templatePath);

    public static void Fzx(List<String> fileNames, IReadOnlyDictionary<Int32, Char> charset, Boolean makeProportional, String outputFolder)
        => ConvertToFzx.Convert(fileNames, charset, makeProportional, outputFolder);

    public static void AmstradCpc(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String credit, Int32 startLine)
        => ConvertToAmstradCpc.Convert(fileNames, sourceCharset, outputFolder, credit, startLine);

    public static void Msx(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
        => ConvertToMsx.Convert(fileNames, sourceCharset, outputFolder, templatePath);

    public static void Commodore64(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
        => ConvertToCommodore64.Convert(fileNames, sourceCharset, outputFolder, templatePath);
}
