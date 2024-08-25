using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create FZX font from ZX font")]
public class ConvertToFzxCommand : Command<ProportionalSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ProportionalSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.Fzx(files, Spectrum.UK, settings.Proportional, settings.OutputFolder);
        return 0;
    }
}