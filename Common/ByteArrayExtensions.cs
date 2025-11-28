using System;

namespace PixelWorld;

public static class ByteArrayExtensions
{
    public static String ToHex(this Byte[] input)
    {
        var output = new Char[input.Length * 2];
        for (var i = 0; i < input.Length; i++)
        {
            var r = i * 2;
            var h = input[i].ToString("x2");
            output[r] = h[0];
            output[r + 1] = h[1];
        }
        return new String(output);
    }

    public static void InvertBuffer(this Byte[] buffer)
    {
        for (var i = 0; i < buffer.Length; i++)
            buffer[i] = (Byte)~buffer[i];
    }

    public static Boolean IsEmpty(this Byte[] buffer, Int32 index, Int32 rows = 8)
    {
        for (var e = 0; e < rows; e++)
            if (buffer[index + e] != 0)
                return false;

        return true;
    }

    public static Boolean IsFull(this Byte[] buffer, Int32 index, Int32 rows = 8)
    {
        for (var e = 0; e < rows; e++)
            if (buffer[index + e] != 255)
                return false;

        return true;
    }

    public static Int32 CountBlankGlyphs(this Byte[] buffer, Int32 index, Int32 length, Int32 height)
    {
        var count = 0;
        for (var i = index; i < index + length; i += height)
        {
            if (IsEmpty(buffer, i, height))
                count++;
        }
        return count;
    }

    public static Boolean IsSame(this Byte[] buffer, Int32 firstIndex, Int32 secondIndex)
    {
        for (var e = 0; e < 8; e++)
            if (buffer[firstIndex + e] != buffer[secondIndex + e])
                return false;

        return true;
    }

    public static Boolean IsSame(this Byte[] buffer, Int32 firstIndex, Byte[] character)
    {
        for (var e = 0; e < 8; e++)
            if (buffer[firstIndex + e] != character[e])
                return false;

        return true;
    }

    public static Int32 PixelCount(this Byte[] buffer, Int32 offset, Char c)
    {
        var count = 0;
        for (var y = 0; y < 8; y++)
        {
            Int32 g = buffer[offset + c - 32 + y];
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