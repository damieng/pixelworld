﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create UFO font from ZX font")]
public class ConvertToUfoCommand : Command<ConvertSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ConvertSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.Ufo(files, Spectrum.UK, settings.OutputFolder);
        return 0;
    }
}