using Spectre.Console.Cli;
using System.ComponentModel;

namespace CommandLine.Commands
{
    public class TextOutputSettings : BasicSettings
    {
        [CommandOption("--credit")]
        [Description("Credit for the font on generated human-readable versions")]
        public string Credit { get; set; } = "";
    }
}
