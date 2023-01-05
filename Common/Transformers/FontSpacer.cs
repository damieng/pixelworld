using PixelWorld.Fonts;
using System.Linq;

namespace PixelWorld.Transformers
{
    public static class FontSpacer
    {
        public static Font MakeProportional(Font source, int leftPad = 0, int rightPad = 0)
        {
            var target = source.Copy();
            var allKeys = target.Glyphs.Keys.ToList();

            foreach (var key in allKeys.Where(key => key != ' '))
                target.Glyphs[key] = GlyphSpacer.Proportional(target.Glyphs[key], leftPad, rightPad);

            var spaceWidth = target.Glyphs['{'].Width - leftPad - rightPad;
            target.Glyphs[' '] = new Glyph(spaceWidth, source.Height, new bool[spaceWidth, source.Height]);

            return target;
        }
    }
}
