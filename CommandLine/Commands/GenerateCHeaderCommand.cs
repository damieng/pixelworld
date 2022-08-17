using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Generate C header def files")]
    public class GenerateCHeaderCommand : Command<COutputSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] COutputSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            CHeaderFontFormatter.CreateFontHeaderConst(settings.ByteType, files, settings.OutputFolder, settings.Credit);
            return 0;
        }
    }
}
