using System;
using System.IO;
using System.Threading.Tasks;

namespace PixelWorld.Shared.BinarySource
{
    interface IBinarySource
    {
       Task<ReadOnlyMemory<byte>> Read(Stream source);
    }
}
