using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PixelWorld.Fonts
{
    public class Font : IEquatable<Font>
    {
        public readonly string Name;
        public readonly int Height;
        public Dictionary<char, Glyph> Glyphs { get; } = new();

        public Font Copy()
        {
            var copied = new Font(Name, Height);
            foreach (var glyph in Glyphs)
                copied.Glyphs.Add(glyph.Key, glyph.Value);
            return copied;
        }

        public Font(string name, int height = 8)
        {
            Name = name;
            Height = height;
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

        public bool Equals(Font? other)
        {
            return other is not null 
                && (ReferenceEquals(this, other) || string.Equals(Name, other.Name)
                && Height == other.Height
                && Glyphs.Count == other.Glyphs.Count
                && ((IStructuralEquatable) Glyphs.Values.ToArray()).Equals(other.Glyphs.Values.ToArray(), StructuralComparisons.StructuralEqualityComparer));
        }

        public override bool Equals(object? obj)
        {
            return obj is not null
                   && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Font) obj));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Height, Glyphs);
        }

        public Bitmap CreateBitmap()
        {
            const int rows = 3;
            var fullWidth = Glyphs.Sum(g => g.Value.Width);
            var previewWidth = fullWidth / rows;
            var glphysPerRow = Glyphs.Count / rows;

            // HACK: Allow subsetted fonts
            previewWidth = 256;
            glphysPerRow = 32;

            var bitmap = new Bitmap(previewWidth, Height * rows);
            var xOff = 0;
            var yOff = 0;
            int cIdx = 0;

            foreach (var glyph in Glyphs)
            {
                for (var y = 0; y < Height; y++)
                for (var x = 0; x < glyph.Value.Width; x++)
                    bitmap.SetPixel(xOff + x, yOff + y, glyph.Value.Data[x, y] ? Color.Black : Color.Transparent);

                xOff += glyph.Value.Width;
                cIdx++;
                if (cIdx % glphysPerRow == 0)
                {
                    yOff += Height;
                    xOff = 0;
                }
            }

            return bitmap;
        }
    }
}