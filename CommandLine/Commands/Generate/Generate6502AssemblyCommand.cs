using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Generate;

[Description("Generate MOS 6502 assembly")]
public class Generate6502AssemblyCommand : Command<AssemblySettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] AssemblySettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        switch (settings.Base)
        {
            case NumberBase.Decimal:
                AssemblyFontFormatter.CreateDefines("6502", ".byte ", "${0:x2}", files, settings.OutputFolder, settings.Credit);
                break;
            default:
                AssemblyFontFormatter.CreateDefines("6502", ".byte ", "{0}", files, settings.OutputFolder, settings.Credit);
                break;
        }
        return 0;
    }
}