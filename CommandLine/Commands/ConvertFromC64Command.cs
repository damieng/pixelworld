using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Convert from Commodore 64 binary font format")]
    public class ConvertFromC64Command : Command<RequiredSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            ConvertFrom.Commodore64(files, Commodore64.BothUK, settings.OutputFolder);
            return 0;
        }
    }
}
