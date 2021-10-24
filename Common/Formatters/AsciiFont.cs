﻿using System.IO;
using PixelWorld.Fonts;

namespace PixelWorld.Formatters
{
    public static class AsciiFontFormatter
    {
        public static string[] FileExtensions { get; } = { "asciifont" };

        public static void Write(Font font, Stream output)
        {
            using var writer = new StreamWriter(output);
            foreach (var pair in font.Glyphs)
            {
                writer.WriteLine("{0}", pair.Key);
                var glyph = pair.Value;
                for (var y = 0; y < glyph.Height; y++)
                {
                    string line = "";
                    for (var x = 0; x < glyph.Width; x++)
                    {
                        line += glyph.Data[x, y] ? "#" : " ";
                    }
                    writer.WriteLine(line);
                }
                writer.WriteLine();
            }
            writer.Flush();
        }
    }
}