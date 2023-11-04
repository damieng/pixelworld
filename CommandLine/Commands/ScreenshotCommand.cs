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

namespace CommandLine.Commands;

[Description("Create screenshots from memory or snapshots")]
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
        if (settings.Png)
            WritePng(fileName, memory, settings, address);

        if (settings.Gif)
            WriteGif(fileName, memory, settings, address);

        // Write SCR if specified OR if nothing specified (so it is the default)
        if (settings.Scr | (!settings.Png && !settings.Gif))
            WriteScr(fileName, memory, settings, address);

        return true;
    }

    private static void WriteScr(string fileName, ArraySegment<byte> memory, ScreenshotSettings settings, int address)
    {
        var filename = Utils.MakeFileName(fileName, "scr", settings.OutputFolder);
        Out.Write($"  Dumping {fileName} @ {address} to {filename}");
        var screenBuffer = memory.Array.AsSpan(address, 6912).ToArray();
        File.WriteAllBytes(filename, screenBuffer);
    }

    private static void WriteGif(string fileName, ArraySegment<byte> memory, ScreenshotSettings settings, int address)
    {
        var filename = Utils.MakeFileName(fileName, "gif", settings.OutputFolder);
        Out.Write($"  Dumping {fileName} @ {address} to {filename}");
        using var animated = SpectrumDisplay.GetBitmap(memory.ToArray(), address);
        animated.Metadata.GetGifMetadata().RepeatCount = 0;
        animated.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;
        using var flashFrame = SpectrumDisplay.GetBitmap(memory.ToArray(), address, true).Frames.RootFrame;
        flashFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;
        animated.Frames.AddFrame(flashFrame);
        animated.SaveAsGif(filename);
    }

    private static void WritePng(string fileName, ArraySegment<byte> memory, ScreenshotSettings settings, int address)
    {
        var filename = Utils.MakeFileName(fileName, "png", settings.OutputFolder);
        Out.Write($"  Dumping {fileName} @ {address} to {filename}");
        using var image = SpectrumDisplay.GetBitmap(memory.ToArray(), address, settings.Flashed);
        image.SaveAsPng(filename);
    }
}