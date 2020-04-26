using PixelWorld.Fonts;

namespace PixelWorld.Transformers
{
    public static class GlyphShifter
    {
        public static Glyph Shift(Glyph source, int horizontal, int vertical, bool wrap, int? newWidth = null, int? newHeight = null)
        {
            bool[,] data = new bool[source.Width, source.Height];

            for (var y = 0; y < source.Height; y++)
            {
                var ty = y + vertical;
                if (wrap || (ty >= 0 && ty < source.Height))
                {
                    ty = ty % source.Height;
                    for (var x = 0; x < source.Width; x++)
                    {
                        var tx = x + horizontal;
                        if (wrap || (tx < source.Width && tx >= 0))
                        {
                            tx = tx % source.Width;
                            data[tx, ty] = source.Data[x, y]; ;
                        }
                    }
                }
            }

            var target = new Glyph(newWidth ?? source.Width, newHeight ?? source.Height, data);
            return target;
        }
    }
}
