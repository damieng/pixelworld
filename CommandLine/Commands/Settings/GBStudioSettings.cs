using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings
{
    public class GBStudioSettings : RequiredSettings
    {
        [CommandOption("--dark")]
        [Description("Generate a dark variant")]
        public bool Dark { get; set; }

        [CommandOption("--proportional")]
        [Description("Generate a proportional variant by left aligning and detecting width")]
        public bool Proportional { get; set; }
    }
}