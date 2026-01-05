using System;
using System.ComponentModel;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create CoCoVGA fonts from ZX font")]
public class ConvertToCoVgaCommand : Command<ConvertSettings>
{
    public override Int32 Execute(CommandContext context, ConvertSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.CoCoVGA(files, Spectrum.UK, settings.OutputFolder, settings.TemplatePath);
        return 0;
    }
}