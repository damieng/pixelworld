using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class COutputSettings : TextOutputSettings
{
    [CommandOption("--byteType")]
    [Description("What type to use for the byte")]
    [DefaultValue("uint8_t")]
    public String ByteType { get; set; } = "";
}