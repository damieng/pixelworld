using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders;

public class KnownCharPattern(int charCode, byte[] pattern)
{
    public readonly int CharCode = charCode;
    public readonly byte[] Pattern = pattern;
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

        return [..offsets];
    }
}