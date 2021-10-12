using System;
using System.IO;
using System.Text;

namespace PixelWorld.BinarySource
{
    public class SNABinarySource : IBinarySource
    {
        public static IBinarySource Instance { get; } = new SNABinarySource();

        public ArraySegment<Byte> GetMemory(Stream source)
        {
            var signatureBuffer = new byte[8];
            source.Read(signatureBuffer, 0, signatureBuffer.Length);

            if (Encoding.ASCII.GetString(signatureBuffer, 0, signatureBuffer.Length) == "MV - SNA")
            {
                Out.Write("  Loading as Amstrad CPC");
                // Amstrad CPC SNA file
                source.Seek(0x100, SeekOrigin.Begin);
                return new ArraySegment<Byte>(source.ReadAllBytes());
            }

            source.Seek(0, SeekOrigin.Begin);
            return ZXSNABinarySource.Instance.GetMemory(source);
        }
    }
}
