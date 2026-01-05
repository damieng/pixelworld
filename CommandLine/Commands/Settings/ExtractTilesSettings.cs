using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class ExtractTilesSettings : RequiredSettings
{
    [CommandOption("--min <NUMBER>")]
    [Description("Minimum number of unique tiles for extraction")]
    [DefaultValue("1")]
    public Int32 MinTiles { get; set; } = 1;
    
    [CommandOption("--max <NUMBER>")]
    [Description("Maximum number of unique tiles for extraction")]
    [DefaultValue("768")]
    public Int32 MaxTiles { get; set; } = 768;
}