using System;
using System.IO;
using System.Threading.Tasks;

namespace PixelWorld.Shared.BinarySource
{
    class RawBinarySource : IBinarySource
    {
        public async Task<ReadOnlyMemory<byte>> Read(Stream input)
        {
            return await input.ReadAllBytes();
        }
    }
}
