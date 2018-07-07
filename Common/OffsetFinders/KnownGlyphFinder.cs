using PixelWorld.Formatters;
using System;
using System.Collections.Generic;

namespace PixelWorld.Finders
{
    public class KnownCharPattern
    {
        public readonly int CharCode;
        public readonly byte[] Pattern;

        public KnownCharPattern(int charCode, byte[] pattern)
        {
            CharCode = charCode;
            Pattern = pattern;
        }
    }

    public static class KnownGlyphFinder
    {
        public static List<int> FindOffsets(byte[] buffer, int offset, KnownCharPattern[] knownFont)
        {
            var offsets = new List<int>();

            var end = buffer.Length - ByteFontFormatter.ExpectedLength;

            for (var i = offset; i < end; i++)
            {
                for (var c = 0; c < knownFont.Length; c++)
                {
                    var known = knownFont[c];
                    if (buffer.IsSame(i, known.Pattern))
                    {
                        offsets.Add(i - known.CharCode * 8);
                        break;
                    }
                }
            }

            return offsets;
        }
    }
}
