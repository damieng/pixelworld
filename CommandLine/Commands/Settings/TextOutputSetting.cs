using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class TextOutputSettings : RequiredSettings
{
    [CommandOption("--credit")]
    [Description("Credit for the font on generated human-readable versions")]
    public String Credit { get; set; } = "";
}