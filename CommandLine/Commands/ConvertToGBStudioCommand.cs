using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Convert to GB Studio PNG font")]
    public class ConvertToGBStudioCommand : Command<GBStudioSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] GBStudioSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            ConvertTo.GBStudio(files, Spectrum.UK, settings.OutputFolder, settings.DarkLight);
            return 0;
        }
    }
}