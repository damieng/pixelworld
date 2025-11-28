using PixelWorld.Fonts;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Formatters;

public static class ByteFontFormatter
{
    private const Int32 CharWidth = 8;
    private const Int32 CharHeight = 8;
    private static readonly ArraySegment<Byte> BlankChar = new(new Byte[8], 0, 8);
    private static readonly Func<Int32, ArraySegment<Byte>> BlankWriter = _ => BlankChar;

    public static Font Load(String fileName, IReadOnlyDictionary<Int32, Char> charset)
    {
        using var source = File.OpenRead(fileName);
        using var reader = new BinaryReader(source);
        return Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, charset);
    }
    
    public static Font Create(BinaryReader reader, String name, Int32 offset, IReadOnlyDictionary<Int32, Char> charset)
    {
        var font = new Font(name);
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        Read(font, reader, charset);
        return font;
    }

    public static void Read(Font font, BinaryReader reader, IReadOnlyDictionary<Int32, Char> charset)
    {
        var c = 0;
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var data = new Boolean[CharWidth, CharHeight];
            for (var y = 0; y < CharHeight; y++)
            {
                if (reader.BaseStream.Position == reader.BaseStream.Length) return;
                var b = reader.ReadByte();
                for (var x = 0; x < CharWidth; x++)
                {
                    var m = 1 << x;
                    data[CharWidth - x - 1, y] = (b & m) == m;
                }
            }
            var glyph = new Glyph(CharWidth, CharHeight, data);
            if (charset.TryGetValue(c++, out var mappedChar))
                font.Glyphs.Add(mappedChar, glyph);
        }
    }

    public static void Write(Font font, Stream output, IReadOnlyDictionary<Int32, Char> charset, Int32 length, Func<Int32, ArraySegment<Byte>>? fallback = null)
    {
        fallback ??= BlankWriter;

        var writer = new BinaryWriter(output); // Do not dispose as it will close underlying stream
        for (var i = 0; i < length; i++)
        {
            if (charset.TryGetValue(i, out var charToWrite) && font.Glyphs.TryGetValue(charToWrite, out var glyph))
            {
                for (var y = 0; y < CharHeight; y++)
                {
                    var b = new Byte();
                    for (var x = 0; x < CharWidth; x++)
                    {
                        if (glyph.Data[x, y])
                            b |= (Byte)(1 << CharWidth - 1 - x);
                    }
                    writer.Write(b);
                }
            }
            else
            {
                var buffer = fallback((Int32)writer.BaseStream.Position);
                if (buffer.Array is not null)
                    writer.Write(buffer.Array, buffer.Offset, buffer.Count);
            }
        }
    }
}