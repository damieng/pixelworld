using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Convert to Amstrad CPC BASIC file")]
    public class ConvertToCPCCommand : Command<BASICOutputSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BASICOutputSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            ConvertTo.AmstradCPC(files, Spectrum.UK, settings.OutputFolder, settings.Credit, settings.Line);
            return 0;
        }
    }
}
