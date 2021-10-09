using PixelWorld.BinarySource.Decoders;
using System;
using System.IO;

namespace PixelWorld.BinarySource
{
    public class ZXSNABinarySource : ZXSpectrumBinarySource, IBinarySource
    {
        public static IBinarySource Instance { get; } = new ZXSNABinarySource();

        public ArraySegment<Byte> Read(Stream source)
        {
            // Use Ziggy SNA loader to decode the file
            try
            {
                var snapshot = SNAFile.LoadSNA(source);

                return snapshot switch
                {
                    SNA_48K sna48K => Setup48KMemory(sna48K),
                    SNA_128K sna128K => Setup128KMemory(sna128K.RAM_BANK, sna128K.PORT_7FFD),
                    _ => throw new NotSupportedException($"Unknown MemoryModel {snapshot.GetType()}"),
                };
            }
            catch (Exception e)
            {
                Out.Write($"  Unable to process {e.Message}");
                return new ArraySegment<byte>();
            }
        }

        private static ArraySegment<byte> Setup48KMemory(SNA_48K snapshot)
        {
            Out.Write("  Loading as ZX Spectrum 48K");
            return new ArraySegment<byte>(snapshot.RAM);
        }
    }
}
