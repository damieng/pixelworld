using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class ScreenshotSettings : RequiredSettings
{
    [CommandOption("--address <MEMORY>")]
    [Description("What memory address to use - otherwise auto detected.")]
    public Int32? Address { get; set; }

    [CommandOption("--png")]
    [Description("Write a .png version of the screenshot.")]
    public Boolean Png { get; set; }

    [CommandOption("--webp")]
    [Description("Write a .webp version of the screenshot.")]
    public Boolean Webp { get; set; }

    [CommandOption("--scr")]
    [Description("Write a .scr version of the screenshot (default).")]
    public Boolean Scr { get; set; }

    [CommandOption("--gif")]
    [Description("Write an animated .gif of the screenshot.")]
    public Boolean Gif { get; set; }

    [CommandOption("--flashed")]
    [Description("Write png with the alternate flashed attribute state.")]
    public Boolean Flashed { get; set; }
}