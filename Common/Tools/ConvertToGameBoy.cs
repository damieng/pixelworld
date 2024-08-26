using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using PixelWorld.Transformers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace PixelWorld.Tools;

public static class ConvertToGameBoy
{
    private static readonly PngEncoder gbPngEncoder = new() {ColorType = PngColorType.Palette};

    public static void GbStudio(List<string> fileNames, IReadOnlyDictionary<int, char> sourceCharset, string outputFolder, bool dark, bool proportional)
    {
        foreach (var fileName in fileNames)
        {
            Out.Write($"Generating GBStudio files for {fileName}");
            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var fontName = Path.GetFileNameWithoutExtension(fileName);
            var font = ByteFontFormatter.Create(reader, fontName, 0, sourceCharset);

            var outFileName = Path.Combine(outputFolder, fontName);
            {
                using var image = CreateFilledGbsBitmap(GameBoy.Palette[3]);
                font.DrawImage(image, 16, GameBoy.Studio, GameBoy.Palette[0], GameBoy.Palette[3]);
                image.SaveAsPng(Path.ChangeExtension(outFileName, "png"), gbPngEncoder);
                File.WriteAllText(Path.ChangeExtension(outFileName, "json"), JsonSerializer.Serialize(new GBJson(fontName + " Mono"), gbStudioJsonOptions));
            }

            if (dark)
            {
                var darkFileName = outFileName + "-dark";
                using var image = CreateFilledGbsBitmap(GameBoy.Palette[0]);
                font.DrawImage(image, 16, GameBoy.Studio, GameBoy.Palette[3], GameBoy.Palette[0]);
                image.SaveAsPng(Path.ChangeExtension(darkFileName, "png"), gbPngEncoder);
                File.WriteAllText(Path.ChangeExtension(darkFileName, "json"), JsonSerializer.Serialize(new GBJson(fontName + " Mono Dark"), gbStudioJsonOptions));
            }

            if (proportional)
            {
                var varFileName = outFileName + "-var";
                var varFont = FontSpacer.MakeProportional(font, 0, 1, 8);
                using var image = CreateFilledGbsBitmap(Color.Magenta);
                varFont.DrawImage(image, 16, GameBoy.Studio, GameBoy.Palette[0], GameBoy.Palette[3], 8);
                image.SaveAsPng(Path.ChangeExtension(varFileName, "png"), gbPngEncoder);
                File.WriteAllText(Path.ChangeExtension(varFileName, "json"), JsonSerializer.Serialize(new GBJson(fontName + " Variable Width"), gbStudioJsonOptions));
            }
        }
    }

    record GBJson(string name)
    {
        public Object mapping = new { };
    };

    static readonly JsonSerializerOptions gbStudioJsonOptions = new() {WriteIndented = true, IncludeFields = true};

    private static Image<Rgba32> CreateFilledGbsBitmap(Color fill)
    {
        var image = new Image<Rgba32>(128, 112);
        for (var x = 0; x < 128; x++)
        for (var y = 0; y < 112; y++)
            image[x, y] = fill;
        return image;
    }
}