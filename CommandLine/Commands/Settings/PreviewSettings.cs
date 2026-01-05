using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class PreviewSettings : RequiredSettings
{
    [CommandOption("--png")]
    [Description("Write a .png version of the screenshot.")]
    public Boolean Png { get; set; }

    [CommandOption("--webp")]
    [Description("Write a .webp version of the screenshot.")]
    public Boolean Webp { get; set; }

    [CommandOption("--transparent")]
    [DefaultValue(true)]
    [Description("Write images with transparent backgrounds.")]
    public Boolean Transparent { get; set; }
}