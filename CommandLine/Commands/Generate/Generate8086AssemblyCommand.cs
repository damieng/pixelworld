using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate Intel 8086 assembly")]
public class Generate8086AssemblyCommand : Command<AssemblySettings>
{
    public override Int32 Execute([NotNull] CommandContext context, [NotNull] AssemblySettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        switch (settings.Base)
        {
            case NumberBase.Binary:
                AssemblyFontFormatter.CreateDefines("x86", "db\t", "0b{0:b8}", files, settings.OutputFolder, settings.Credit);
                break;
            case NumberBase.Hex:
                AssemblyFontFormatter.CreateDefines("x86", "db\t", "0x{0:x2}", files, settings.OutputFolder, settings.Credit);
                break;
            default:
                AssemblyFontFormatter.CreateDefines("x86", "db\t", "{0}", files, settings.OutputFolder,
                    settings.Credit);
                break;
        }

        return 0;
    }
}