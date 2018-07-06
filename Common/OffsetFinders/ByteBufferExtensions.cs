using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorld.Finders
{
    public static class ByteBufferExtensions
    {
        public static bool IsEmpty(this byte[] buffer, int index, int rows = 8)
        {
            for (int e = 0; e < rows; e++)
                if (buffer[index + e] != 0)
                    return false;

            return true;
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
