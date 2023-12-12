using Spectre.Console.Cli;

namespace CommandLine.Commands.Settings;

public class FindMatchingGlyphsSettings : CommandSettings
{
    [CommandArgument(0, "[SourceFontFile]")]
    public string SourceFontFile { get; set; } = "";

    [CommandArgument(1, "[FileGlob]")]
    public string Glob { get; set; } = "";

    [CommandArgument(2, "[MatchGlyphs]")]
    public string MatchGlyphs { get; set; } = "";
}