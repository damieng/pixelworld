using Spectre.Console.Cli;

namespace CommandLine.Commands
{
    public class BasicSettings : CommandSettings
    {
        [CommandArgument(0, "[FileGlob]")]
        public string Glob { get; set; }

        [CommandArgument(1, "[OutputFolder]")]
        public string OutputFolder { get; set; }
    }
}