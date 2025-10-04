using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands;

[Description("Dump memory from emulator snapshot")]
public class ExtractScreenTilesCommand : Command<ExtractTilesSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] ExtractTilesSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        TileExtractor.Extract(files, settings.OutputFolder, settings.MinTiles, settings.MaxTiles);
        return 0;
    }
}