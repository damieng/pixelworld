using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace PixelWorld.Fonts
{
    [DebuggerDisplay("{Print(),nq}")]
    public class Glyph : IEquatable<Glyph>
    {
        public int Height { get; }
        public int Width { get; }
        public bool[,] Data { get; }

        public Glyph(int width, int height, bool[,] data)
        {
            Width = width;
            Height = height;
            Data = data;
        }

        public bool Equals(Glyph other)
        {
            return !(other is null)
                && (ReferenceEquals(this, other) || Height == other.Height
                && Width == other.Width
                && ((IStructuralEquatable)Data).Equals(other.Data, StructuralComparisons.StructuralEqualityComparer));
        }

        public override bool Equals(object obj)
        {
            return !(obj is null)
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

        public bool IsRowBlank(int row)
        {
            for (var x = 0; x < Width; x++)
                if (Data[x, row] == true)
                    return false;
            return true;
        }

        public bool IsColumnBlank(int column)
        {
            for (var y = 0; y < Height; y++)
                if (Data[column, y] == true)
                    return false;
            return true;
        }
    }
}