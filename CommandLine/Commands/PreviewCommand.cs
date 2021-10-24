using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;

namespace CommandLine.Commands
{
    [Description("Generate PNG previews of fonts")]
    public class PreviewCommand : Command<RequiredSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            PreviewCreator.Preview(files, settings.OutputFolder);
            return 0;
        }
    }
}
