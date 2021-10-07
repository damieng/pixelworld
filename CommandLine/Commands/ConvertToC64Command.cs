using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Convert to Commodore 64 binary font formats")]
    public class ConvertToC64Command : Command<BasicSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BasicSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            Converter.ConvertToC64(files, Spectrum.UK, settings.OutputFolder);
            return 0;
        }
    }
}
