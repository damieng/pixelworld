using System.IO;
using System.Threading.Tasks;

namespace PixelWorld.Shared
{
    static class Utils
    {
        public static async Task<byte[]> ReadAllBytes(this Stream stream)
        {
            var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            return memory.GetBuffer();
        }
    }
}
