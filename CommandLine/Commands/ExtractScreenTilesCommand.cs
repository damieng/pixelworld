using System;
using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands;

[Description("Dump memory from emulator snapshot")]
public class ExtractScreenTilesCommand : Command<ExtractTilesSettings>
{
    public override Int32 Execute(CommandContext context, ExtractTilesSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        TileExtractor.Extract(files, settings.OutputFolder, settings.MinTiles, settings.MaxTiles);
        return 0;
    }
}