using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings
{
    public class ScreenshotSettings : RequiredSettings
    {
        [CommandOption("--address <MEMORY>")]
        [Description("What memory address to use - otherwise auto detected.")]
        public int? Address { get; set; }

        [CommandOption("--png")]
        [Description("Write a .png version of the screenshot.")]
        public bool Png { get; set; }

        [CommandOption("--scr")]
        [Description("Write a .scr version of the screenshot (default).")]
        public bool Scr { get; set; }

        [CommandOption("--flashed")]
        [Description("Write png with the alternate flashed attribute state.")]
        public bool Flashed { get; set; }
    }
}
