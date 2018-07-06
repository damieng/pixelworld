using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PixelWorld.Fonts
{
    public class Font : IEquatable<Font>
    {
        public readonly string Name;
        public readonly int Height;
        public Dictionary<char, Glyph> Glyphs { get; } = new Dictionary<char, Glyph>();

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

        public bool Equals(Font other)
        {
            return !(other is null) 
                && (ReferenceEquals(this, other) || string.Equals(Name, other.Name)
                && Height == other.Height
                && Glyphs.Count == other.Glyphs.Count
                && ((IStructuralEquatable) Glyphs.Values.ToArray()).Equals(other.Glyphs.Values.ToArray(), StructuralComparisons.StructuralEqualityComparer));
        }

        public override bool Equals(object obj)
        {
            return !(obj is null)
                   && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Font) obj));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Height;
                hashCode = (hashCode * 397) ^ (Glyphs != null ? Glyphs.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}