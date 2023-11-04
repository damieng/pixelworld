﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate 6502 assembly def files")]
public class Generate6502AssemblyCommand : Command<AssemblySettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AssemblySettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        switch (settings.Base)
        {
            case NumberBase.Decimal:
                AssemblyFontFormatter.CreateAssemblyDefines("6502", ".byte ", "${0:x2}", files, settings.OutputFolder, settings.Credit);
                break;
            default:
                AssemblyFontFormatter.CreateAssemblyDefines("6502", ".byte ", "{0}", files, settings.OutputFolder, settings.Credit);
                break;
        }
        return 0;
    }
}