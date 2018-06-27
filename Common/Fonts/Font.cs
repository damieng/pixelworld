using System.Collections.Generic;

namespace PixelWorld.Fonts
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

        public string ToDebug(string input)
        {
            var s = "";

            foreach (var c in input)
            {
                for (var y = 0; y < Height; y++)
                {
                    var g = Glyphs[c];
                    for (var x = 0; x < g.Width; x++)
                    {
                        s += g.Data[x, y] ? '█' : ' ';
                    }

                    s += ' ';
                }

                s += '\n';
            }

            return s;
        }
    }
}