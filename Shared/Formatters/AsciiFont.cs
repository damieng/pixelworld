using System.IO;
using PixelWorld.Shared.Fonts;

namespace PixelWorld.Shared.Formatters
{
    public static class AsciiFont
    {
        public static string[] FileExtensions = { "asciifont" };

        public static void Write(Font font, Stream output)
        {
            using (var writer = new StreamWriter(output))
            {
                foreach (var pair in font.Glyphs)
                {
                    writer.WriteLine("{0}", pair.Key);
                    var glyph = pair.Value;
                    for (int y = 0; y < glyph.Height; y++)
                    {
                        string line = "";
                        for (int x = 0; x < glyph.Width; x++)
                        {
                            line += (glyph.Data[x, y]) ? "#" : " ";
                        }
                        writer.WriteLine(line);
                    }
                    writer.WriteLine();
                }
                writer.Flush();
            }
        }
    }
}