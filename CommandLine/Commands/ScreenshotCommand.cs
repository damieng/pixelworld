using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Display;
using PixelWorld.Tools;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace CommandLine.Commands
{
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
            {
                var pngFileName = Path.Combine(settings.OutputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "png"));
                Out.Write($"  Dumping {fileName} @ {address} to {pngFileName}");
                using var image = SpectrumDisplay.GetBitmap(memory.ToArray(), address, settings.Flashed);
                image.SaveAsPng(pngFileName);
            }

            if (settings.Gif)
            {
                var gifFileName = Path.Combine(settings.OutputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "gif"));
                Out.Write($"  Dumping {fileName} @ {address} to {gifFileName}");

                using var animated = SpectrumDisplay.GetBitmap(memory.ToArray(), address, false);
                animated.Metadata.GetGifMetadata().RepeatCount = 0;
                animated.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;

                using var flashFrame = SpectrumDisplay.GetBitmap(memory.ToArray(), address, true).Frames.RootFrame;
                flashFrame.Metadata.GetGifMetadata().FrameDelay = FlashDelay;
                animated.Frames.AddFrame(flashFrame);

                animated.SaveAsGif(gifFileName);
            }

            // Write SCR if specified OR if nothing specified (so it is the default)
            if (settings.Scr | (!settings.Png && !settings.Gif))
            {
                var scrFileName = Path.Combine(settings.OutputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "scr"));
                Out.Write($"  Dumping {fileName} @ {address} to {scrFileName}");
                var screenBuffer = memory.Array.AsSpan(address, 6912).ToArray();
                File.WriteAllBytes(scrFileName, screenBuffer);
            }

            return true;
        }
    }
}
