using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Convert to GB Studio PNG font")]
public class ConvertToGbStudioCommand : Command<GbStudioSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] GbStudioSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.GbStudio(files, Spectrum.UK, settings.OutputFolder, settings.Dark, settings.Proportional);
        return 0;
    }
}