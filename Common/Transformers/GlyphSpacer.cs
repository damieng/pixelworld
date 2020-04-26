using PixelWorld.Fonts;
using System;

namespace PixelWorld.Transformers
{
    public static class GlyphSpacer
    {
        public static Glyph Proportional(Glyph source, int leftPad = 0, int rightPad = 0)
        {
            var margins = CountLeftAndRightBlankColumns(source);
            var actualWidth = source.Width - margins.Item1 - margins.Item2;
            var newWidth = actualWidth + leftPad + rightPad;

            return GlyphShifter.Shift(source, 0 - margins.Item1 + leftPad, 0, false, newWidth);
        }

        private static Tuple<int, int> CountLeftAndRightBlankColumns(Glyph glyph)
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

            return Tuple.Create(rightSpace, leftSpace);
        }
    }
}
