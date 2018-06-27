using System;
using System.IO;

namespace PixelWorld.BinarySource
{
    public interface IBinarySource
    {
        ArraySegment<byte> Read(Stream source);
    }
}
