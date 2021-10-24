using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Generate Motorola 68000 assembly def files")]
    public class Generate68000AssemblyCommand : Command<AssemblySettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AssemblySettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            switch (settings.Base)
            {
                case NumberBase.Decimal:
                    AssemblyFontFormatter.CreateAssemblyDefines("68000", "DC.B ", "${0:x2}", files, settings.OutputFolder, settings.Credit);
                    break;
                default:
                    AssemblyFontFormatter.CreateAssemblyDefines("68000", "DC.B ", "{0}", files, settings.OutputFolder, settings.Credit);
                    break;
            }
            return 0;
        }
    }
}
