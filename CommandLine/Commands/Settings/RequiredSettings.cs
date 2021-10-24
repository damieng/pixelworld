using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings
{
    public class RequiredSettings : CommandSettings
    {
        [CommandArgument(0, "[FileGlob]")]
        public string Glob { get; set; } = "";

        [CommandArgument(1, "[OutputFolder]")]
        public string OutputFolder { get; set; } = ".";
    }
}