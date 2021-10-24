using PixelWorld.Fonts;
using System.Linq;

namespace PixelWorld.Transformers
{
    public static class FontInverter
    {
        public static Font Invert(Font source)
        {
            var target = source.Copy();
            var allKeys = target.Glyphs.Keys.ToList();
            foreach (var key in allKeys)
                target.Glyphs[key] = GlyphInverter.Invert(target.Glyphs[key]);
            return target;
        }
    }
}
