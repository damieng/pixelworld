using PixelWorld;
using PixelWorld.Display;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
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

        static int WriteScreenToDisk(string fileName, ArraySegment<byte> memory, ScreenshotSettings settings)
        {
            var address = settings.Address ?? (memory.Count == 49152 ? 0 : 16384);

            if (settings.Png)
            {
                var newFileName = Path.Combine(settings.OutputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "png"));
                Out.Write($"  Dumping {fileName} @ {address} to {newFileName}");
                using var bitmap = SpectrumDisplay.GetBitmap(memory.ToArray(), address);
                bitmap.Save(newFileName, ImageFormat.Png);
            }

            if (settings.Scr || !settings.Png)
            {
                var newFileName = Path.Combine(settings.OutputFolder, Path.ChangeExtension(Path.GetFileName(fileName), "scr"));
                Out.Write($"  Dumping {fileName} @ {address} to {newFileName}");
                var screenBuffer = memory.Array.AsSpan(address, 6912).ToArray();
                File.WriteAllBytes(newFileName, screenBuffer);
            }

            return 1;
        }
    }
}
