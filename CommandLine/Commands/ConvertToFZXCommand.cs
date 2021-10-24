using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    public class FZXSettings : RequiredSettings
    {
        [CommandOption("--proportional")]
        [Description("Make the font proportional by left aligning and detecting width")]
        public bool Proportional { get; set; }
    }

    [Description("Convert to FZX-format font")]
    public class ConvertToFZXCommand : Command<FZXSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] FZXSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            Converter.ConvertToFZX(files, Spectrum.UK, settings.Proportional, settings.OutputFolder);
            return 0;
        }
    }
}
