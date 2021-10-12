using System;
using System.IO;

namespace PixelWorld.BinarySource
{
    public class RawBinarySource : IBinarySource
    {
        public ArraySegment<byte> GetMemory(Stream input)
        {
            return new ArraySegment<byte>(input.ReadAllBytes());
        }
    }
}
