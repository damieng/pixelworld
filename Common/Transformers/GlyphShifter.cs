using PixelWorld.Fonts;

namespace PixelWorld.Transformers
{
    public static class GlyphShifter
    {
        public static Glyph Shift(Glyph source, int horizontal, int vertical, bool wrap, int? newWidth = null, int? newHeight = null)
        {
            var width = newWidth ?? source.Width;
            var height = newHeight ?? source.Height;

            bool[,] data = new bool[width, height];

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
}
