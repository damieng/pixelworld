using System;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class FindMatchingGlyphsSettings : CommandSettings
{
    [CommandArgument(0, "[SourceFontFile]")]
    public String SourceFontFile { get; set; } = "";

    [CommandArgument(1, "[FileGlob]")]
    public String Glob { get; set; } = "";

    [CommandArgument(2, "[MatchGlyphs]")]
    public String MatchGlyphs { get; set; } = "";
}