using System;
using System.Collections.Generic;

namespace PixelWorld
{
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            return x != null && y != null
                             && x[0] == y[0]
                             && x[1] == y[1]
                             && x[2] == y[2]
                             && x[3] == y[3]
                             && x[4] == y[4]
                             && x[5] == y[5]
                             && x[6] == y[6]
                             && x[7] == y[7];
        }

        public int GetHashCode(byte[] b)
        {
            return b[0] | b[1] << 8 | b[2] << 16 | b[3] << 24
                   ^ (b[4] | b[5] | b[6] | b[7]);
        }
    }

    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] input)
        {
            var output = new char[input.Length * 2];
            for (int i = 0; i < input.Length; i++)
            {
                var r = i * 2;
                var h = input[i].ToString("x2");
                output[r] = h[0];
                output[r + 1] = h[1];
            }
            return new String(output);
        }

        public static void InvertBuffer(this byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)~buffer[i];
        }

        public static bool IsEmpty(this byte[] buffer, int index, int rows = 8)
        {
            for (int e = 0; e < rows; e++)
                if (buffer[index + e] != 0)
                    return false;

            return true;
        }

        public static bool IsFull(this byte[] buffer, int index, int rows = 8)
        {
            for (int e = 0; e < rows; e++)
                if (buffer[index + e] != 255)
                    return false;

            return true;
        }

        public static int CountBlankGlyphs(this byte[] buffer, int index, int length, int height)
        {
            int count = 0;
            for (int i = index; i < index + length; i += height)
            {
                if (IsEmpty(buffer, i, height))
                    count++;
            }
            return count;
        }

        public static int CountBlankLines(this byte[] buffer, int index)
        {
            int blankLines = 0;
            for (int e = 0; e < 8; e++)
                if (buffer[index + e] == 0)
                    blankLines++;
            return blankLines;
        }

        public static bool IsSame(this byte[] buffer, int firstIndex, int secondIndex)
        {
            for (int e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != buffer[secondIndex + e])
                    return false;

            return true;
        }


        public static bool IsSame(this byte[] buffer, int firstIndex, byte[] character)
        {
            for (int e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != character[e])
                    return false;

            return true;
        }

        public static int PixelCount(this byte[] buffer, int offset, char c)
        {
            int count = 0;
            for (int y = 0; y < 8; y++)
            {
                int g = buffer[offset + c - 32 + y];
                for (int x = 0; x < 8; x++)
                {
                    int f = 1 << x;
                    if ((g & f) == f)
                        count += 1;
                }
            }

            return count;
        }
    }
}
