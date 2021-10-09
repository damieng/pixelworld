using Spectre.Console.Cli;
using System.ComponentModel;

namespace CommandLine.Commands
{
    public class ConvertSettings : BasicSettings
    {
        [CommandOption("--templatePath <PATH>")]
        [Description("What path to use for binary templates containing missing glyphs.")]
        [DefaultValue(".")]
        public string TemplatePath { get; set; }
    }
}
