using Spectre.Console.Cli;
using System.ComponentModel;

namespace CommandLine.Commands
{
    public class AssemblerSettings : TextOutputSettings
    {
        [CommandOption("--base <BASE>")]
        [Description("What base to use for font glyph data.")]
        [TypeConverter(typeof(NumberBaseConverter))]
        [DefaultValue("hex")]
        public NumberBase Base { get; set; }
    }
}
