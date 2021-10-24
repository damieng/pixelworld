using System;

namespace PixelWorld
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] input)
        {
            var output = new char[input.Length * 2];
            for (var i = 0; i < input.Length; i++)
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
            for (var e = 0; e < rows; e++)
                if (buffer[index + e] != 0)
                    return false;

            return true;
        }

        public static bool IsFull(this byte[] buffer, int index, int rows = 8)
        {
            for (var e = 0; e < rows; e++)
                if (buffer[index + e] != 255)
                    return false;

            return true;
        }

        public static int CountBlankGlyphs(this byte[] buffer, int index, int length, int height)
        {
            var count = 0;
            for (var i = index; i < index + length; i += height)
            {
                if (IsEmpty(buffer, i, height))
                    count++;
            }
            return count;
        }

        public static int CountBlankLines(this byte[] buffer, int index)
        {
            var blankLines = 0;
            for (var e = 0; e < 8; e++)
                if (buffer[index + e] == 0)
                    blankLines++;
            return blankLines;
        }

        public static bool IsSame(this byte[] buffer, int firstIndex, int secondIndex)
        {
            for (var e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != buffer[secondIndex + e])
                    return false;

            return true;
        }


        public static bool IsSame(this byte[] buffer, int firstIndex, byte[] character)
        {
            for (var e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != character[e])
                    return false;

            return true;
        }

        public static int PixelCount(this byte[] buffer, int offset, char c)
        {
            var count = 0;
            for (var y = 0; y < 8; y++)
            {
                int g = buffer[offset + c - 32 + y];
                for (var x = 0; x < 8; x++)
                {
                    var f = 1 << x;
                    if ((g & f) == f)
                        count += 1;
                }
            }

            return count;
        }
    }
}
