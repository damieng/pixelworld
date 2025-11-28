using System;
using System.ComponentModel;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands;

[Description("Identify fonts that use glyphs from a source font")]
public class FindMatchingGlyphsCommand :Command<FindMatchingGlyphsSettings>
{
    public override Int32 Execute(CommandContext context, FindMatchingGlyphsSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        Matcher.Match(files, settings.SourceFontFile, settings.MatchGlyphs);
        return 0;    }
}