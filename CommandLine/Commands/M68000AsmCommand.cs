using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Generate Intel 8086 assembly def files")]
    public class X86AsmCommand : Command<AssemblerSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AssemblerSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            switch (settings.Base)
            {
                case NumberBase.Decimal:
                    AssemblyFontFormatter.CreateAssemblyDefines("x86", "db\t", "0x{0:x2}", files, settings.OutputFolder, settings.Credit);
                    break;
                default:
                    AssemblyFontFormatter.CreateAssemblyDefines("x86", "db\t", "{0}", files, settings.OutputFolder, settings.Credit);
                    break;
            }
            return 0;
        }
    }
}
