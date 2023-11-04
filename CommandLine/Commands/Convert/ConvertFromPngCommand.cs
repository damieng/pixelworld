using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Convert from preview PNG file")]
public class ConvertFromPngCommand : Command<RequiredSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
    {
        foreach (var fileName in Utils.MatchGlobWithFiles(settings.Glob))
            ConvertFromPng(settings, fileName);

        return 0;
    }

    private static void ConvertFromPng(RequiredSettings settings, string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        Out.Write($"Generating ch8 file from {fileName}");

        var sourceFont = new Font(name);
        PngFontFormatter.Read(sourceFont, File.OpenRead(fileName), Spectrum.UK);

        using var output = File.Create(Utils.MakeFileName(fileName, "ch8", settings.OutputFolder));
        ByteFontFormatter.Write(sourceFont, output, Spectrum.UK, 96);
    }
}