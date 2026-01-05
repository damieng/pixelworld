using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class BasicOutputSettings : TextOutputSettings
{
    [CommandOption("--line <NUMBER>")]
    [Description("What line number to start on. Defaults to 9000.")]
    [DefaultValue(9000)]
    public Int32 Line { get; set; } = 9000;
}