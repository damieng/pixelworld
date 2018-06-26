using System.IO;
using System.Threading.Tasks;

namespace PixelWorld
{
    public static class Utils
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.GetBuffer();
        }
    }
}
