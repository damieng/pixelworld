﻿using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands;

[Description("Produce raw memory dumps from snapshots")]
public class DumpCommand : Command<RequiredSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        Dumper.Dump(files, settings.OutputFolder);
        return 0;
    }
}