using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings
{
    public class FZXSettings : RequiredSettings
    {
        [CommandOption("--proportional")]
        [Description("Make the font proportional by left aligning and detecting width")]
        public bool Proportional { get; set; }
    }
}