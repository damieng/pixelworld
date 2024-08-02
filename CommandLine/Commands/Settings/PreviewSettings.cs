using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class PreviewSettings : RequiredSettings
{
    [CommandOption("--png")]
    [Description("Write a .png version of the screenshot.")]
    public bool Png { get; set; }

    [CommandOption("--webp")]
    [Description("Write a .webp version of the screenshot.")]
    public bool Webp { get; set; }
}