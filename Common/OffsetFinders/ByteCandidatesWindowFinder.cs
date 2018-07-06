using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWorld.Finders
{
    /// <summary>
    /// Find offsets that contain a minimum number of candidates within a sliding window of the buffer.
    /// Use in conjunction with a screen parser to collect candidates from the title screen in RAM.
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

        public static List<int> FindOffsets(ArraySegment<Byte> buffer, byte[][] candidates)
        {
            var offsets = new HashSet<int>();

            if (candidates.Length < minUniqueInWindow) return offsets.ToList();

            var window = new Queue<CandidateLocation>();

            var end = buffer.Count - ByteFontFormatter.ExpectedLength;

            for (var i = 0; i < end; i++)
            {
                var skew = i % 8;
                for (var c = 0; c < candidates.Length; c++)
                {
                    var y = 0;
                    var found = false;
                    while (y < 8 && buffer.Array[i + y] == candidates[c][y])
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
                    // Which ones are actually aligned?
                    var aligned = window.Where(w => w.Offset % 8 == skew).ToArray();
                    if (aligned.Length > minUniqueInWindow)
                    {
                        // But are they unique?
                        var uniques = aligned.Select(w => w.Candidate).Distinct().Count();
                        var calculatedOffset = aligned[0].Offset;
                        if (uniques > minUniqueInWindow)
                        {
                            offsets.Add(calculatedOffset);
                        }
                    }
                }

                while (window.Count > 0 && window.Peek().Offset < i - ByteFontFormatter.ExpectedLength)
                    window.Dequeue();
            }

            return offsets.ToList();
        }
    }
}
