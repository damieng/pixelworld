using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Create GB Studio PNG font from ZX font")]
public class ConvertToGbStudioCommand : Command<GbStudioSettings>
{
    public override Int32 Execute(CommandContext context, GbStudioSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        ConvertToGameBoy.GbStudio(files, Spectrum.UK, settings.OutputFolder, settings.Dark, settings.Proportional);
        return 0;
    }
}