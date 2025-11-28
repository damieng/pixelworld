using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PixelWorld.Fonts;

[DebuggerDisplay("{Print(),nq}")]
public class Glyph(Int32 width, Int32 height, Boolean[,] data) : IEquatable<Glyph>
{
    public Int32 Height { get; } = height;
    public Int32 Width { get; } = width;
    public Boolean[,] Data { get; } = data;

    public Boolean Equals(Glyph? other)
    {
        return other is not null
               && (ReferenceEquals(this, other) || Height == other.Height
                   && Width == other.Width
                   && SequenceEquals(Data, other.Data));
    }

    public static Boolean SequenceEquals<T>(T[,] a, T[,] b) => a.Rank == b.Rank
                                                               && Enumerable.Range(0, a.Rank).All(d=> a.GetLength(d) == b.GetLength(d))
                                                               && a.Cast<T>().SequenceEqual(b.Cast<T>());

    public override Boolean Equals(Object? obj)
    {
        return obj is not null
               && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Glyph)obj));
    }

    public override Int32 GetHashCode()
    {
        return HashCode.Combine(Height, Width);
    }

    public String Print()
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

    public Boolean IsBlank()
    {
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
            if (Data[y,x])
                return false;
        return true;
    }

    public Boolean IsRowBlank(Int32 row)
    {
        for (var x = 0; x < Width; x++)
            if (Data[x, row])
                return false;
        return true;
    }

    public Boolean IsColumnBlank(Int32 column)
    {
        for (var y = 0; y < Height; y++)
            if (Data[column, y])
                return false;
        return true;
    }
}