using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create Atari 8-bit font from ZX font")]
public class ConvertToAtari8BitCommand : Command<ConvertSettings>
{
    public override Int32 Execute([NotNull] CommandContext context, [NotNull] ConvertSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.Atari8(files, Spectrum.UK, settings.OutputFolder, settings.TemplatePath);
        return 0;
    }
}