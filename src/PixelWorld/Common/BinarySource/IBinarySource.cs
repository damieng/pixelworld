using System;
using System.IO;
using System.Threading.Tasks;

namespace PixelWorld.BinarySource
{
    public interface IBinarySource
    {
        ArraySegment<byte> Read(Stream source);
    }
}
