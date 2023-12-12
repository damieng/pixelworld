using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Tools;
using Spectre.Console.Cli;

namespace CommandLine.Commands;

[Description("Find glyphs from a source font against a many possible target fonts")]
public class FindMatchingGlyphsCommand :Command<FindMatchingGlyphsSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] FindMatchingGlyphsSettings settings)
    {
        var files = Utils.MatchGlobWithFiles(settings.Glob);
        Matcher.Match(files, settings.SourceFontFile, settings.MatchGlyphs);
        return 0;    }
}