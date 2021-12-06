using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Convert to UFO font file")]
    public class ConvertToUfoCommand : Command<ConvertSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] ConvertSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            ConvertTo.Ufo(files, Spectrum.UK, settings.OutputFolder);
            return 0;
        }
    }
}
