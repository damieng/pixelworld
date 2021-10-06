using System;
using System.Collections.Generic;
using System.IO;
using PixelWorld.Fonts;

namespace PixelWorld.Formatters
{
    public static class ByteFontFormatter
    {
        public const int charWidth = 8;
        public const int charHeight = 8;
        private static readonly ArraySegment<byte> blankChar = new(new byte[8], 0, 8);
        private static readonly Func<int, ArraySegment<byte>> blankWriter = (i) => blankChar;

        public static Font Create(BinaryReader reader, string name, int offset, IReadOnlyDictionary<int, char> charset)
        {
            var font = new Font(name);
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            Read(font, reader, charset);
            return font;
        }

        public static void Read(Font font, BinaryReader reader, IReadOnlyDictionary<int, char> charset)
        {
            int c = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
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
                if (charset.TryGetValue(c++, out char mappedChar))
                    font.Glyphs.Add(mappedChar, glyph);
            }
        }

        public static void Write(Font font, Stream output, IReadOnlyDictionary<int, char> charset, int length, Func<int, ArraySegment<byte>> fallback = null)
        {
            fallback ??= blankWriter;

           var writer = new BinaryWriter(output);
            for (var i = 0; i < length; i++)
            {
                if (charset.TryGetValue(i, out var charToWrite) && font.Glyphs.TryGetValue(charToWrite, out var glyph)) {
                    for (int y = 0; y < charHeight; y++)
                    {
                        var b = new Byte();
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Data[x, y])
                                b |= (byte)(1 << charWidth - 1 - x);
                        }
                        writer.Write(b);
                    }
                } else
                {
                    var buffer = fallback((int)writer.BaseStream.Position);
                    writer.Write(buffer.Array, buffer.Offset, buffer.Count);
                }
            }
        }
    }
}
