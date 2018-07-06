using System;
using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld.Finders
{
    /// <summary>
    /// Finds fonts by looking for a minimum number of likely candidates
    /// within a sliding window of the buffer.  
    /// Normally used in conjunction with a screen parser to collect the
    /// likely candidates from the title screen in RAM.
    /// </summary>
    public static class ByteCandidatesWindowFinder
    {
        const int minUniqueInWindow = 20;

        class CandidateLocation
        {
            public readonly int Offset;
            public readonly int Candidate;

            public CandidateLocation(int offset, int candidate)
            {
                Offset = offset;
                Candidate = candidate;
            }
        }

        public static List<Font> Read(BinaryReader reader, string name, byte[][] candidates)
        {
            var fonts = new List<Font>();

            if (candidates.Length < minUniqueInWindow) return fonts;

            var buffer = reader.ReadBytes(1024 * 1024);
            const int desiredLength = ByteFontFormatter.glyphRange * (ByteFontFormatter.charWidth / 8) * ByteFontFormatter.charHeight;

            var fontIndex = 0;

            var offset = 6911; // Don't bother checking screen

            var window = new Queue<CandidateLocation>();
            int lastSavedOffset = 0;

            var bestOffset = 0;
            var bestMatches = 0;

            while (offset + desiredLength < buffer.LongLength)
            {
                var skew = offset % 8;
                for (var c = 0; c < candidates.Length; c++)
                {
                    var y = 0;
                    var found = false;
                    while (y < 8 && buffer[offset + y] == candidates[c][y])
                    {
                        if (++y == 8)
                            found = true;
                    }

                    if (found)
                    {
                        window.Enqueue(new CandidateLocation(offset, c));
                        break; // Can only match one as they are unique
                    }
                }

                if (window.Count > minUniqueInWindow)
                {
                    // Which ones are actually aligned?
                    var aligned = window.Where(w => w.Offset % 8 == skew).ToArray();
                    if (aligned.Length > minUniqueInWindow)
                    {
                        // But are they unique?
                        var uniques = aligned.Select(w => w.Candidate).Distinct().Count();
                        var calculatedOffset = aligned[0].Offset;
                        if (uniques > minUniqueInWindow && lastSavedOffset != calculatedOffset && aligned[0].Candidate == 0)
                        {
                            lastSavedOffset = calculatedOffset;
                            if (uniques > bestMatches)
                            {
                                bestOffset = calculatedOffset;
                                bestMatches = uniques;
                            }
                        }
                    }
                }

                offset++;

                while (window.Count > 0 && window.Peek().Offset < offset - desiredLength)
                    window.Dequeue();
            }

            if (bestMatches > minUniqueInWindow)
                fonts.Add(ByteFontFormatter.Create(reader, $"{name + "-scrbest-" + ++fontIndex}", bestOffset));

            return fonts;
        }
    }
}
