using PixelWorld.Fonts;
using PixelWorld.Transformers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Formatters
{
    public static class FZXFontFormatter
    {
        public static void Write(Font font, Stream output, bool makeProportional = false)
        {
            if (makeProportional)
                font = FontSpacer.MakeProportional(font, 0, 0, maxWidth: 16);

            using var writer = new BinaryWriter(output);

            // Header
            writer.Write((byte) font.Height);
            writer.Write((byte) (makeProportional ? 1 : 0));
            writer.Write((byte) 127); // Hard-coded to 7F/end of 7-bit ASCII

            // Figure out how many leading blank rows per char
            var blankRows = new Dictionary<char, Tuple<int, int>>();
            foreach (var glyph in font.Glyphs)
                blankRows[glyph.Key] = CountTopAndBottomBlankRows(glyph.Value);

            // Offset/shift table
            var tableSize = font.Glyphs.Count * 3;
            Console.WriteLine($"Table size is {tableSize:x}");

            var dataOffset = writer.BaseStream.Position + tableSize + 2; // Final word bullshit

            foreach (var glyph in font.Glyphs)
            {
                var relativeOffset = dataOffset - writer.BaseStream.Position;

                var blanks = blankRows[glyph.Key];
                var shift = blanks.Item1 == 8 ? 0 : blanks.Item1; // Spaces are treated weird

                writer.Write((UInt16) relativeOffset);

                dataOffset += glyph.Value.Height - blanks.Item1 - blanks.Item2;
                writer.Write((byte) ((shift << 4) | (glyph.Value.Width - 1)));
            }

            // The "final word" is the relative offset to the end of data
            writer.Write((UInt16) (dataOffset - writer.BaseStream.Position));

            // Data table
            foreach (var glyph in font.Glyphs)
            {
                var blanks = blankRows[glyph.Key];
                var bottom = font.Height - blanks.Item2;
                for (var y = blanks.Item1; y < bottom; y++)
                {
                    var b = new Byte();
                    for (var x = 0; x < glyph.Value.Width; x++)
                    {
                        if (glyph.Value.Data[x, y])
                            b |= (byte) (1 << 8 - x - 1);
                    }

                    writer.Write(b);
                }
            }
        }

        private static Tuple<int, int> CountTopAndBottomBlankRows(Glyph glyph)
        {
            var top = 0;
            var topY = 0;
            while (topY < glyph.Height && glyph.IsRowBlank(topY++))
                top++;

            topY--;
            var bottom = 0;
            var bottomY = glyph.Height - 1;
            while (bottomY > topY && glyph.IsRowBlank(bottomY--))
                bottom++;

            return Tuple.Create(top, bottom);
        }
    }
}