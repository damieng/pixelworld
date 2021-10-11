using Spectre.Console.Cli;
using System.ComponentModel;

namespace CommandLine.Commands
{
    public class ScreenshotSettings : BasicSettings
    {
        [CommandOption("--address <MEMORY>")]
        [Description("What memory address to use - otherwise auto detected.")]
        public int? Address { get; set; }

        [CommandOption("--png")]
        [Description("Write a .png version of the screenshot.")]
        public bool Png { get; set; }

        [CommandOption("--scr")]
        [Description("Write a .scr version of the screenshot.")]
        public bool Scr { get; set; }
    }
}
