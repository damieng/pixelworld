using PixelWorld.DumpScanners;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools;

public static class FontHunter
{
    public static void Hunt(List<String> fileNames, String outputFolder)
    {
        var inputsWithOutputsCount = 0;
        var outputCount = 0;

        var inputCount = fileNames.Count;
        Out.Write($"\nHunting {inputCount} files");

        foreach (var fileName in fileNames)
        {
            Out.Write($"Opening file {fileName}");
            var matchOutputs = ExtractFontFromDumpFile(fileName, outputFolder);
            if (matchOutputs > 0)
            {
                inputsWithOutputsCount++;
                outputCount += matchOutputs;
            }
        }

        Out.Write($"{inputsWithOutputsCount} files yielded {outputCount} results");
        Out.Write($"{Math.Floor((Double)inputsWithOutputsCount / inputCount * 100)}% success rate");
    }

    private static Int32 ExtractFontFromDumpFile(String fileName, String outputFolder)
    {
        using var source = File.OpenRead(fileName);
        return ExtractFontFromMemoryBuffer(fileName, new ArraySegment<Byte>(source.ReadAllBytes()), outputFolder);
    }

    private static Int32 ExtractFontFromMemoryBuffer(String fileName, ArraySegment<Byte> dump, String outputFolder)
    {
        if (dump.Array is null) throw new ArgumentOutOfRangeException(nameof(dump), "Array is null");

        using var memory = new MemoryStream(dump.Array);
        using var reader = new BinaryReader(memory);
        var fontIndex = 0;
        foreach (var font in SpectrumDumpScanner.Read(reader, Path.GetFileNameWithoutExtension(fileName)))
        {
            var newFileName = Utils.MakeFileName(font.Name, "ch8", outputFolder);
            fontIndex++;
            Out.Write($"  Creating byte font {newFileName}");
            ByteFontFormatter.Write(font, File.Create(newFileName), Spectrum.UK, 96);
        }

        return fontIndex;
    }
}