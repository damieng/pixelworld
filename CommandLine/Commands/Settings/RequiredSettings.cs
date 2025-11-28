using System;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class RequiredSettings : CommandSettings
{
    [CommandArgument(0, "[FileGlob]")]
    public String Glob { get; set; } = "";

    [CommandArgument(1, "[OutputFolder]")]
    public String OutputFolder { get; set; } = ".";
}