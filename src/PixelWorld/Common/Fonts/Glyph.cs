using System;
using System.Diagnostics;

namespace PixelWorld.Fonts
{
    [DebuggerDisplay("{" + nameof(ToDebug) + "(), nq}")]
    public class Glyph
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

        public string ToDebug()
        {
            var length = Height * (Width + 1);
            var output = new char[length];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Height; x++)
                {
                    output[y * (Height + 1) + x] = Data[x, y] ? '#' : ' ';
                }

                output[y * (Height + 1) + Width] = '\n';
            }

            return new String(output);
        }
    }
}