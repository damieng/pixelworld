using System;
using PixelWorld.Fonts;

namespace PixelWorld.Transformers;

public static class GlyphInverter
{
    public static Glyph Invert(Glyph source)
    {
        return new Glyph(source.Width, source.Height, Invert(source.Data));
    }

    public static Boolean[,] Invert(Boolean[,] source)
    {
        var target = new Boolean[source.GetLength(0), source.GetLength(1)];

        for (var x = 0; x < source.GetLength(0); x++)
        for (var y = 0; y < source.GetLength(1); y++)
            target[x, y] = !source[x, y];

        return target;
    }
}