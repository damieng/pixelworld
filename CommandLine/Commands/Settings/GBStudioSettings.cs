using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings
{
    public class GBStudioSettings : RequiredSettings
    {
        [CommandOption("--darklight")]
        [Description("Generate both dart and light variants")]
        public bool DarkLight { get; set; }
    }
}