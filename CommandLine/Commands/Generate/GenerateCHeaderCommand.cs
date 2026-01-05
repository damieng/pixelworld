using System;
using System.ComponentModel;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate C header")]
public class GenerateCHeaderCommand : Command<COutputSettings>
{
    public override Int32 Execute(CommandContext context, COutputSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        CHeaderFontFormatter.CreateFontHeaderConst(settings.ByteType, files, settings.OutputFolder, settings.Credit);
        return 0;
    }
}