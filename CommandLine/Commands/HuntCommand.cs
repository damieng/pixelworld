﻿using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands;

[Description("Hunt memory dumps for possible ZX fonts")]
public class HuntCommand : Command<RequiredSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        FontHunter.Hunt(files, settings.OutputFolder);
        return 0;
    }
}