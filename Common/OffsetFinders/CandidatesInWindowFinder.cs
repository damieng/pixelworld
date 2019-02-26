using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWorld.OffsetFinders
{
    /// <summary>
    /// Find offsets that contain a minimum number of candidates within a sliding window of the buffer.
    /// Use in conjunction with a screen parser to collect candidates from the title screen in RAM.
    /// </summary>
    public static class CandidatesInWindowFinder
    {
        const int minUniqueInWindow = 36;

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

        public static List<int> FindOffsets(byte[] buffer, byte[][] candidates)
        {
            // We don't have enough candidates to even look
            if (candidates.Length < minUniqueInWindow) return new List<int>();

            var end = buffer.Length - Spectrum.FontSize;

            var ranges = new Dictionary<int, int>();

            // This is our rolling window of candidates - we have one for each 'skew' as a matched glyph is
            // only valid on the 8 byte boundry
            var windows = Enumerable.Range(0, 8).Select(e => new Queue<CandidateLocation>()).ToArray();

            for (var i = 0; i < end; i++)
            {
                var window = windows[i % 8]; // Decide which window to use based on skewing

                for (var c = 0; c < candidates.Length; c++)
                {
                    // Look to see if the current byte is any of our candidates
                    var y = 0;
                    var found = false;
                    while (y < 8 && buffer[i + y] == candidates[c][y])
                    {
                        if (++y == 8)
                            found = true;
                    }

                    if (found)
                    {
                        window.Enqueue(new CandidateLocation(i, c));
                        break; // Can only match one as they are unique
                    }
                }

                if (window.Count > minUniqueInWindow)
                {
                    // See if we have enough unique candidates
                    var uniqueCount = window.Select(w => w.Candidate).Distinct().Count();
                    if (uniqueCount > minUniqueInWindow)
                    {
                        // Look back to the previous space
                        var backOffset = window.Peek().Offset;
                        var foundSpace = false;
                        for (var b = backOffset; b > backOffset - Spectrum.FontSize && b > 0; b -= 8)
                        {
                            if (buffer.IsEmpty(b))
                            {
                                backOffset = b;
                                foundSpace = true;
                                break;
                            }
                        }

                        if (foundSpace)
                        {
                            ranges.TryGetValue(backOffset, out var currentUniqueCount);
                            ranges[backOffset] = Math.Max(uniqueCount, currentUniqueCount);
                        }
                    }
                }

                while (window.Count > 0 && window.Peek().Offset < i - Spectrum.FontSize)
                    window.Dequeue();
            }

            // Okay, we now have a list of ranges and a count of the uniques they contain
            var offsets = new List<int>();
            if (ranges.Count < 25) // Fuck you Bubble Bobble
                offsets.AddRange(ranges.Keys);
            return offsets.ToList();
        }
    }
}
