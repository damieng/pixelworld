using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Generate Motorola 68000 assembly def files")]
    public class M68000AsmCommand : Command<AssemblerSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AssemblerSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            switch (settings.Base)
            {
                case NumberBase.Decimal:
                    AssemblyFontFormatter.GenAsmHex("68000", "DC.B ", "${0:x2}", files, settings.OutputFolder);
                    break;
                default:
                    AssemblyFontFormatter.GenAsmHex("68000", "DC.B ", "{0}", files, settings.OutputFolder);
                    break;
            }
            return 0;
        }
    }
}
