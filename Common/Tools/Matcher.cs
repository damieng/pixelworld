using System;
using System.Collections.Generic;
using System.IO;
using PixelWorld.Formatters;
using PixelWorld.Machines;

namespace PixelWorld.Tools;

public static class Matcher
{
    public static void Match(List<string> fileNames, string matchFile, string matchGlyphs)
    {
        Out.Write($"Reading match file {matchFile}");
        using var matchReader = new BinaryReader(File.OpenRead(matchFile));
        var matchFont = ByteFontFormatter.Create(matchReader, Path.GetFileNameWithoutExtension(matchFile), 0, Spectrum.UK);

        var matchedCount = 0;

        Out.Write($"\nMatching against {fileNames.Count} files");

        foreach (var fileName in fileNames)
        {
            using var candidateReader = new BinaryReader(File.OpenRead(fileName));
            var candidateFont = ByteFontFormatter.Create(candidateReader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);

            int candidateMatches = 0;
            foreach (var glyph in matchGlyphs)
            {
                if (matchFont.Glyphs.TryGetValue(glyph, out var matchGlyph) && candidateFont.Glyphs.TryGetValue(glyph, out var targetGlyph) && matchGlyph.Equals(targetGlyph))
                    candidateMatches++;
                else
                    break;
            }

            if (candidateMatches == matchGlyphs.Length)
            {
                Out.Write($"Matched {fileName}");
                matchedCount++;
            }
        }

        Out.Write($"Matched {matchedCount} files with glyphs {matchGlyphs}");
    }

}