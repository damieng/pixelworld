using System;
using PixelWorld.Fonts;

namespace PixelWorld.Transformers;

public static class GlyphShifter
{
    public static Glyph Shift(Glyph source, Int32 horizontal, Int32 vertical, Boolean wrap, Int32? newWidth = null, Int32? newHeight = null)
    {
        var width = newWidth ?? source.Width;
        var height = newHeight ?? source.Height;

        var data = new Boolean[width, height];

        for (var y = 0; y < source.Height; y++)
        {
            var ty = y + vertical;
            if (wrap || ty >= 0 && ty < source.Height)
            {
                ty %= source.Height;
                for (var x = 0; x < source.Width; x++)
                {
                    var tx = x + horizontal;
                    if (wrap || tx < width && tx >= 0)
                    {
                        tx %= width;
                        data[tx, ty] = source.Data[x, y];
                    }
                }
            }
        }

        return new Glyph(width, height, data);
    }
}