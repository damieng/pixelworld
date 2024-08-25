using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandLine.Commands.Settings;
using PixelWorld.Formatters;
using PixelWorld.Machines;

namespace CommandLine.Commands;

[Description("Create preview images from ZX font")]
public class PreviewCommand : Command<PreviewSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] PreviewSettings settings)
    {
        foreach (var fileName in Utils.MatchGlobWithFiles(settings.Glob))
        {
            Out.Write($"Generating preview files for {fileName}");
            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);

            if (settings.Webp)
            {
                var targetName = Utils.MakeFileName(fileName, "webp", settings.OutputFolder);
                Out.Write($"  Previewing {fileName} to {targetName}");
                using var output = File.OpenWrite(targetName);
                ImageFontFormatter.Write(sourceFont, output, ImageFontFormatter.WebpEncoder, settings.Transparent);
            }

            if (settings.Png || !settings.Webp)
            {
                var targetName = Utils.MakeFileName(fileName, "png", settings.OutputFolder);
                Out.Write($"  Previewing {fileName} to {targetName}");
                using var output = File.OpenWrite(targetName);
                ImageFontFormatter.Write(sourceFont, output, ImageFontFormatter.PngEncoder, settings.Transparent);
            }
        }

        return 0;
    }
}