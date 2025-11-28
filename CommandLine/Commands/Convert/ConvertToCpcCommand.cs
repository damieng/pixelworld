using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create Amstrad CPC BASIC font from ZX font")]
public class ConvertToCpcCommand : Command<BasicOutputSettings>
{
    public override Int32 Execute([NotNull] CommandContext context, [NotNull] BasicOutputSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.AmstradCpc(files, Spectrum.UK, settings.OutputFolder, settings.Credit, settings.Line);
        return 0;
    }
}