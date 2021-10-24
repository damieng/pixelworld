using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Generate Z80 assembly def files")]
    public class Z80AsmCommand : Command<AssemblerSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AssemblerSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            switch (settings.Base)
            {
                case NumberBase.Binary:
                    AssemblyFontFormatter.GenZ80AsmBinary(files, settings.OutputFolder, settings.Credit);
                    break;
                case NumberBase.Decimal:
                    AssemblyFontFormatter.CreateAssemblyDefines("z80", "defb ", "{0}", files, settings.OutputFolder, settings.Credit);
                    break;
                default:
                    AssemblyFontFormatter.CreateAssemblyDefines("z80", "defb ", "&{0:x2}", files, settings.OutputFolder, settings.Credit);
                    break;
            }
            return 0;
        }
    }
}
