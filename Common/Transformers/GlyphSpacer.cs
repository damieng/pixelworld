using System;
using PixelWorld.Fonts;

namespace PixelWorld.Transformers;

public static class GlyphSpacer
{
    public static Glyph Proportional(Glyph source, Int32 leftPad = 0, Int32 rightPad = 0, Int32 maxWidth = 8)
    {
        var (right, left) = CountLeftAndRightBlankColumns(source);
        var actualWidth = source.Width - right - left;
        var newWidth = actualWidth + leftPad + rightPad;
        if (newWidth > maxWidth) newWidth = maxWidth;

        return GlyphShifter.Shift(source, 0 - right + leftPad, 0, false, newWidth);
    }

    private static (Int32 right, Int32 left) CountLeftAndRightBlankColumns(Glyph glyph)
    {
        var rightSpace = 0;
        var rightIndex = 0;
        while (rightIndex < glyph.Width && glyph.IsColumnBlank(rightIndex++))
            rightSpace++;

        rightIndex--;
        var leftSpace = 0;
        var leftIndex = glyph.Width - 1;
        while (leftIndex > rightIndex && glyph.IsColumnBlank(leftIndex--))
            leftSpace++;

        return (rightSpace, leftSpace);
    }
}