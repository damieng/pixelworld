using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Convert to Amstrad CPC BASIC file")]
public class ConvertToCpcCommand : Command<BasicOutputSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] BasicOutputSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.AmstradCPC(files, Spectrum.UK, settings.OutputFolder, settings.Credit, settings.Line);
        return 0;
    }
}