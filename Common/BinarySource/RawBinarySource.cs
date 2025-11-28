using System;
using System.IO;

namespace PixelWorld.BinarySource;

public class RawBinarySource : IBinarySource
{
    public ArraySegment<Byte> GetMemory(Stream input)
    {
        return new ArraySegment<Byte>(input.ReadAllBytes());
    }
}