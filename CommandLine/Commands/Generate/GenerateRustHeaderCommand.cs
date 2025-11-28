using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate Rust header")]
public class GenerateRustHeaderCommand : Command<TextOutputSettings>
{
    public override Int32 Execute(CommandContext context, TextOutputSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        RustHeaderFontFormatter.CreateFontHeaderConst(files, settings.OutputFolder, settings.Credit);
        return 0;
    }
}