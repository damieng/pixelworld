using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class ConvertSettings : RequiredSettings
{
    [CommandOption("--templatePath <PATH>")]
    [Description("What path to use for binary templates containing missing glyphs.")]
    [DefaultValue(".")]
    public string TemplatePath { get; set; } = ".";
}