using System;
using System.IO;
using PixelWorld.Fonts;

namespace PixelWorld.Formatters
{
    public static class ByteFontFormatter
    {
        public const int startChar = 32;
        public const int endChar = 127;
        public const int glyphRange = endChar - startChar + 1;
        public const int charWidth = 8;
        public const int charHeight = 8;

        public static int ExpectedLength = glyphRange * (charWidth / 8) * charHeight;

        public static Font Create(BinaryReader reader, string name, int offset)
        {
            var font = new Font(name);
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            Read(font, reader);
            return font;
        }

        public static void Read(Font font, BinaryReader reader)
        {
            for (int c = startChar; c <= endChar; c++)
            {
                var data = new bool[charWidth, charHeight];
                for (int y = 0; y < charHeight; y++)
                {
                    if (reader.BaseStream.Position == reader.BaseStream.Length) return;
                    var b = reader.ReadByte();
                    for (int x = 0; x < charWidth; x++)
                    {
                        var m = 1 << x;
                        data[charWidth - x - 1, y] = (b & m) == m;
                    }
                }
                var glyph = new Glyph(charWidth, charHeight, data);
                font.Glyphs.Add((Char)c, glyph);
            }
        }

        public static void Write(Font font, Stream output)
        {
            using (var writer = new BinaryWriter(output))
            {
                foreach (var glyph in font.Glyphs)
                {
                    for (int y = 0; y < charHeight; y++)
                    {
                        var b = new Byte();
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Value.Data[x, y])
                                b |= (byte)(1 << charWidth -1 - x);
                        }
                        writer.Write(b);
                    }
                }
            }
        }
    }
}