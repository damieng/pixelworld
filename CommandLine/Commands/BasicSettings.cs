using Spectre.Console.Cli;
using System.ComponentModel;

namespace CommandLine.Commands
{
    public class BasicSettings : CommandSettings
    {
        [CommandArgument(0, "[FileGlob]")]
        public string Glob { get; set; }

        [CommandArgument(1, "[OutputFolder]")]
        public string OutputFolder { get; set; }

        [CommandOption("--credit CREDIT")]
        [Description("Credit for the font on generated human-readable versions")]
        public string Credit { get; set; }
    }
}