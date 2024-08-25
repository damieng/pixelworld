using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate C header")]
public class GenerateCHeaderCommand : Command<COutputSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] COutputSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        CHeaderFontFormatter.CreateFontHeaderConst(settings.ByteType, files, settings.OutputFolder, settings.Credit);
        return 0;
    }
}