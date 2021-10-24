using PixelWorld.Machines;
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
            var offsets = new HashSet<int>();

            var end = buffer.Length - Spectrum.FontSize;

            for (var i = 0; i < end; i++)
            {
                foreach (var known in knownFont)
                {
                    if (buffer.IsSame(i, known.Pattern))
                    {
                        offsets.Add(i - known.CharCode * 8);
                        break;
                    }
                }
            }

            return new List<int>(offsets);
        }
    }
}
