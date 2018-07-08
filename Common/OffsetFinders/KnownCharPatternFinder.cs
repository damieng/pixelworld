using PixelWorld.Formatters;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders
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

    public static class KnownCharPatternFinder
    {
        public static List<int> FindOffsets(byte[] buffer, KnownCharPattern[] knownFont)
        {
            var offsets = new List<int>();

            var end = buffer.Length - ByteFontFormatter.ExpectedLength;

            for (var i = 0; i < end; i++)
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
