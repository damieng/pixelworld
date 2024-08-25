using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Display;
using PixelWorld.Tools;
using SixLabors.ImageSharp;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace CommandLine.Commands;

[Description("Create screenshots from memory dumps or emulator snapshots")]
public class ScreenshotCommand : Command<ScreenshotSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ScreenshotSettings settings)
    {
        var fileNames = Utils.MatchGlobWithFiles(settings.Glob);
        foreach (var fileName in fileNames)
        {
            Out.Write($"Opening file {fileName}");
            using var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Dumper.ProcessStream(fileName, file, (a, b) => WriteScreenToDisk(a, b, settings), true);
        }

        return 0;
    }

    const int FlashDelay = 64;

    static bool WriteScreenToDisk(string fileName, ArraySegment<byte> memory, ScreenshotSettings settings)
    {
        var address = settings.Address ?? (memory.Count == 49152 ? 0 : 16384);
        if (settings.Png || settings.Webp)
        {
            using var image = SpectrumDisplay.GetBitmap(memory.ToArray(), address, settings.Flashed);
            if (settings.Png) WritePng(fileName, image, settings, address);
            if (settings.Webp) WriteWebp(fileName, image, settings, address);
        }

        if (settings.Gif)
            WriteGif(fileName, memory, settings, address);

        // Write SCR if specified OR if nothing specified (so it is the default)
        if (settings.Scr | settings is {Png: false, Gif: false, Webp: false})
            WriteScr(fileName, memory, settings, address);

        return true;
    }

    private static void WriteScr(string sourceName, ArraySegment<byte> memory, ScreenshotSettings settings, int address)
    {
        var targetName = Utils.MakeFileName(sourceName, "scr", settings.OutputFolder);
        Out.Write($"  Screenshotting {sourceName} @ {address} to {targetName}");
        var screenBuffer = memory.Array.AsSpan(address, 6912).ToArray();
        File.WriteAllBytes(targetName, screenBuffer);
    }

    private static void WriteGif(string sourceName, ArraySegment<byte> memory, ScreenshotSettings settings, int address)
    {
        var targetName = Utils.MakeFileName(sourceName, "gif", settings.OutputFolder);
        Out.Write($"  Screenshotting {sourceName} @ {address} to {targetName}");
        using var animated = SpectrumDisplay.GetBitmap(memory.ToArray(), address);
        animated.Metadata.GetGifMetadata().RepeatCount = 0;
        animated.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;
        using var flashFrame = SpectrumDisplay.GetBitmap(memory.ToArray(), address, true).Frames.RootFrame;
        flashFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;
        animated.Frames.AddFrame(flashFrame);
        animated.SaveAsGif(targetName);
    }

    private static void WritePng(string sourceName, Image<Rgb24> image, ScreenshotSettings settings, int address)
    {
        var targetName = Utils.MakeFileName(sourceName, "png", settings.OutputFolder);
        Out.Write($"  Screenshotting {sourceName} @ {address} to {targetName}");
        image.SaveAsPng(targetName);
    }

    private static void WriteWebp(string sourceName, Image<Rgb24> image, ScreenshotSettings settings, int address)
    {
        var targetName = Utils.MakeFileName(sourceName, "webp", settings.OutputFolder);
        Out.Write($"  Screenshotting {sourceName} @ {address} to {targetName}");
        image.SaveAsWebp(targetName);
    }
}