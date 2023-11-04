using System.Collections.Generic;

namespace PixelWorld;

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