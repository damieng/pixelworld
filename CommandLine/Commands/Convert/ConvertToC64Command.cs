using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create Commodore 64 fonts from ZX font")]
public class ConvertToC64Command : Command<ConvertSettings>
{
    public override Int32 Execute([NotNull] CommandContext context, [NotNull] ConvertSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.Commodore64(files, Spectrum.UK, settings.OutputFolder, settings.TemplatePath);
        return 0;
    }
}