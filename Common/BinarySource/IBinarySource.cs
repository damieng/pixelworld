using System;
using System.IO;

namespace PixelWorld.BinarySource;

public interface IBinarySource
{
    ArraySegment<Byte> GetMemory(Stream source);
}