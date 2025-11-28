using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SixLabors.ImageSharp;

namespace PixelWorld.Fonts;

[DebuggerDisplay("Print(),nq")]
public class Font(String name, Int32 height = 8) : IEquatable<Font>
{
    public readonly String Name = name;
    public readonly Int32 Height = height;
    public Dictionary<Char, Glyph> Glyphs { get; } = new();

    public Font Copy()
    {
        var copied = new Font(Name, Height);
        foreach (var glyph in Glyphs)
            copied.Glyphs.Add(glyph.Key, glyph.Value);
        return copied;
    }

    public String Print(String input)
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

    public Boolean Equals(Font? other)
    {
        return other is not null
               && (ReferenceEquals(this, other) || string.Equals(Name, other.Name)
                   && Height == other.Height
                   && Glyphs.Count == other.Glyphs.Count
                   && ((IStructuralEquatable)Glyphs.Values.ToArray()).Equals(other.Glyphs.Values.ToArray(),
                       StructuralComparisons.StructuralEqualityComparer));
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is not null
               && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Font)obj));
    }

    public override Int32 GetHashCode()
    {
        return HashCode.Combine(Name, Height, Glyphs);
    }

    public Image<Rgba32> CreateImage(Int32 rows, Boolean transparent)
    {
        var fullWidth = Glyphs.Sum(g => g.Value.Width);
        var previewWidth = fullWidth / rows;
        var glyphsPerRow = Glyphs.Count / rows;

        var image = new Image<Rgba32>(previewWidth, Height * rows);
        DrawImage(image, glyphsPerRow, transparent);
        return image;
    }

    public void DrawImage(Image<Rgba32> image, Int32 glyphsPerRow, Boolean transparent)
    {
        var xOff = 0;
        var yOff = 0;
        var cIdx = 0;
        var foreground = Color.Black;
        var background = transparent ? Color.Transparent : Color.White;

        foreach (var glyph in Glyphs)
        {
            for (var y = 0; y < Height; y++)
            for (var x = 0; x < glyph.Value.Width; x++)
                image[xOff + x, yOff + y] = glyph.Value.Data[x, y] ? foreground : background;

            xOff += glyph.Value.Width;
            cIdx++;
            if (cIdx % glyphsPerRow == 0)
            {
                yOff += Height;
                xOff = 0;
            }
        }
    }

    public void DrawImage(Image<Rgba32> image, Int32 glyphsPerRow, IReadOnlyDictionary<Int32, Char> targetCharset, Color foreground, Color background, Int32? padWidth = null)
    {
        var xOff = 0;
        var yOff = 0;
        var cIdx = 0;

        var spaceWidth = Glyphs[' '].Width;

        foreach (var c in targetCharset.Values)
        {
            if (Glyphs.TryGetValue(c, out var glyph))
            {
                for (var y = 0; y < Height; y++)
                for (var x = 0; x < glyph.Width; x++)
                    image[xOff + x, yOff + y] = glyph.Data[x, y] ? foreground : background;
            }

            xOff += padWidth ?? glyph?.Width ?? spaceWidth;
            cIdx++;
            if (cIdx % glyphsPerRow == 0)
            {
                yOff += Height;
                xOff = 0;
            }
        }
    }
}