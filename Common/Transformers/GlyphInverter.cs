using PixelWorld.Fonts;

namespace PixelWorld.Transformers
{
    public static class GlyphInverter
    {
        public static Glyph Invert(Glyph source)
        {
            return new Glyph(source.Width, source.Height, Invert(source.Data));
        }

        public static bool[,] Invert(bool[,] source)
        {
            var target = new bool[source.Length, source.GetUpperBound(1)];

            for (var x = 0; x < source.Length; x++)
                for (var y = 0; y < source.GetUpperBound(1); y++)
                    target[x, y] = !source[x, y];

            return target;
        }
    }
}
