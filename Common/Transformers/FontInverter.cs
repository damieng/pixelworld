﻿using PixelWorld.Fonts;
using System.Linq;

namespace PixelWorld.Transformers
{
    public class FontInverter
    {
        public Font Invert(Font source)
        {
            var target = source.Copy();
            var allKeys = target.Glyphs.Keys.ToList();
            foreach (var key in allKeys)
                target.Glyphs[key] = GlyphInverter.Invert(target.Glyphs[key]);
            return target;
        }
    }
}