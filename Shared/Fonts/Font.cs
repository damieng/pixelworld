using System.Collections.Generic;

namespace PixelWorld.Shared.Fonts
{
    public class Font
    {
        public string Name { get; }
        public int Height { get; set; }
        public Dictionary<char, Glyph> Glyphs { get; } = new Dictionary<char, Glyph>();

        public Font(string name)
        {
            Name = name;
        }
    }
}