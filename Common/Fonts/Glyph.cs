using System;
using System.Collections;

namespace PixelWorld.Fonts
{
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
            unchecked
            {
                var hashCode = Height;
                hashCode = (hashCode * 397) ^ Width;
                return hashCode;
            }
        }
    }
}