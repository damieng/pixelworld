﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PixelWorld.Fonts;

[DebuggerDisplay("{Print(),nq}")]
public class Glyph(int width, int height, bool[,] data) : IEquatable<Glyph>
{
    public int Height { get; } = height;
    public int Width { get; } = width;
    public bool[,] Data { get; } = data;

    public bool Equals(Glyph? other)
    {
        return other is not null
               && (ReferenceEquals(this, other) || Height == other.Height
                   && Width == other.Width
                   && SequenceEquals(Data, other.Data));
    }

    public static bool SequenceEquals<T>(T[,] a, T[,] b) => a.Rank == b.Rank
         && Enumerable.Range(0, a.Rank).All(d=> a.GetLength(d) == b.GetLength(d))
         && a.Cast<T>().SequenceEqual(b.Cast<T>());

    public override bool Equals(object? obj)
    {
        return obj is not null
               && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Glyph)obj));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Height, Width);
    }

    public string Print()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
                sb.Append(Data[x, y] ? "*" : ".");
            sb.Append(" \n");
        }

        return sb.ToString();
    }

    public bool IsBlank()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
            if (Data[y,x])
                return false;
        return true;
    }

    public bool IsRowBlank(int row)
    {
        for (var x = 0; x < Width; x++)
            if (Data[x, row])
                return false;
        return true;
    }

    public bool IsColumnBlank(int column)
    {
        for (var y = 0; y < Height; y++)
            if (Data[column, y])
                return false;
        return true;
    }
}