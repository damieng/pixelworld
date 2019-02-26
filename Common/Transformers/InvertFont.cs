using PixelWorld.Fonts;

namespace PixelWorld.Transformers
{
    public class InvertFont
    {
        public Font Invert(Font source)
        {
            var target = new Font(source.Name, source.Height);

            foreach (var glyph in source.Glyphs)
                target.Glyphs.Add(glyph.Key, new Glyph(glyph.Value.Width, glyph.Value.Height, Invert(glyph.Value.Data)));

            return target;
        }

        public bool[,] Invert(bool[,] source)
        {
            var target = new bool[source.Length, source.GetUpperBound(1)];

            for (int x = 0; x < source.Length; x++)
                for (int y = 0; y < source.GetUpperBound(1); y++)
                    target[x, y] = !source[x, y];

            return target;
        }
    }
}
