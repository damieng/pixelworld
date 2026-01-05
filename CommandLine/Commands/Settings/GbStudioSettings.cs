using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class GbStudioSettings : RequiredSettings
{
    [CommandOption("--dark")]
    [Description("Generate a dark variant")]
    public Boolean Dark { get; set; }

    [CommandOption("--proportional")]
    [Description("Generate a proportional variant by left aligning and detecting width")]
    public Boolean Proportional { get; set; }
}