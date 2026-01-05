using System;
using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders;

public class KnownCharPattern(Int32 charCode, Byte[] pattern)
{
    public readonly Int32 CharCode = charCode;
    public readonly Byte[] Pattern = pattern;
}

public static class KnownCharPatternFinder
{
    public static List<Int32> FindOffsets(Byte[] buffer, KnownCharPattern[] knownFont)
    {
        var offsets = new HashSet<Int32>();

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