using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Hunt dumps for possible fonts")]
    public class HuntCommand : Command<BasicSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BasicSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            FontHunter.Hunt(files, settings.OutputFolder);
            return 0;
        }
    }
}
