﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Convert to Commodore 64 binary font formats")]
public class ConvertToC64Command : Command<ConvertSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ConvertSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertTo.Commodore64(files, Spectrum.UK, settings.OutputFolder, settings.TemplatePath);
        return 0;
    }
}