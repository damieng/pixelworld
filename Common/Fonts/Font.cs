﻿using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SixLabors.ImageSharp;

namespace PixelWorld.Fonts;

[DebuggerDisplay("Print(),nq")]
public class Font(string name, int height = 8) : IEquatable<Font>
{
    public readonly string Name = name;
    public readonly int Height = height;
    public Dictionary<char, Glyph> Glyphs { get; } = new();

    public Font Copy()
    {
        var copied = new Font(Name, Height);
        foreach (var glyph in Glyphs)
            copied.Glyphs.Add(glyph.Key, glyph.Value);
        return copied;
    }

    public string Print(string input)
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
                   && ((IStructuralEquatable)Glyphs.Values.ToArray()).Equals(other.Glyphs.Values.ToArray(),
                       StructuralComparisons.StructuralEqualityComparer));
    }

    public override bool Equals(object? obj)
    {
        return obj is not null
               && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Font)obj));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Height, Glyphs);
    }

    public Image<Rgba32> CreateImage(int rows, bool transparent)
    {
        var fullWidth = Glyphs.Sum(g => g.Value.Width);
        var previewWidth = fullWidth / rows;
        var glyphsPerRow = Glyphs.Count / rows;

        var image = new Image<Rgba32>(previewWidth, Height * rows);
        DrawImage(image, glyphsPerRow, transparent);
        return image;
    }

    public void DrawImage(Image<Rgba32> image, int glyphsPerRow, bool transparent)
    {
        var xOff = 0;
        var yOff = 0;
        var cIdx = 0;
        Color foreground = Color.Black;
        Color background = transparent ? Color.Transparent : Color.White;

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

    public void DrawImage(Image<Rgba32> image, int glyphsPerRow, IReadOnlyDictionary<int, char> targetCharset, Color foreground, Color background, int? padWidth = null)
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