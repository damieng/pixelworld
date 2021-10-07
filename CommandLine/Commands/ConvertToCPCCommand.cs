using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Convert to Amstrad CPC BASIC file")]
    public class ConvertToCPCCommand : Command<BasicSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BasicSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            Converter.ConvertToAmstradCPC(files, Spectrum.UK, settings.OutputFolder, settings.Credit);
            return 0;
        }
    }
}
