using System;
using PixelWorld.Fonts;
using System.Linq;

namespace PixelWorld.Transformers;

public static class FontSpacer
{
    public static Font MakeProportional(Font source, Int32 leftPad, Int32 rightPad, Int32 maxWidth)
    {
        var target = source.Copy();
        var allKeys = target.Glyphs.Keys.ToList();

        foreach (var key in allKeys.Where(key => key != ' '))
            target.Glyphs[key] = GlyphSpacer.Proportional(target.Glyphs[key], leftPad, rightPad, maxWidth);

        var spaceWidth = target.Glyphs['{'].Width - leftPad - rightPad;
        target.Glyphs[' '] = new Glyph(spaceWidth, source.Height, new Boolean[spaceWidth, source.Height]);

        return target;
    }
}