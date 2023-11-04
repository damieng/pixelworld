using PixelWorld.BinarySource.Decoders;
using System;
using System.IO;

namespace PixelWorld.BinarySource
{
    public class Z80BinarySource : SpectrumBinarySource, IBinarySource
    {
        public static IBinarySource Instance { get; } = new Z80BinarySource();

        enum MemoryModel
        {
            ZX48,
            ZX128,
            ZXPlus3
        }

        public ArraySegment<Byte> GetMemory(Stream source)
        {
            // Use Ziggy's Z80 loader to decode the file
            try
            {
                var snapshot = Z80File.LoadZ80(source);
                if (snapshot is null)
                    throw new NotSupportedException("Could not decode as Z80 format file");

                var memoryModel = GetMemoryModel(snapshot);

                return memoryModel switch
                {
                    MemoryModel.ZX48 => Setup48KMemory(snapshot),
                    MemoryModel.ZX128 => Setup128KMemory(snapshot.RAM_BANK, snapshot.PORT_7FFD),
                    MemoryModel.ZXPlus3 => SetupPlus3Memory(snapshot),
                    _ => throw new NotSupportedException($"Unknown MemoryModel {memoryModel}")
                };
            }
            catch (Exception e)
            {
                Out.Write($"  Unable to process {e.Message}");
                return new ArraySegment<byte>();
            }
        }

        private static ArraySegment<byte> SetupPlus3Memory(Z80_SNAPSHOT snapshot)
        {
            Out.Write("  Loading as ZX Spectrum 128K +3");

            if ((snapshot.PORT_1FFD & 1) == 0)
                return Setup128KMemory(snapshot.RAM_BANK, snapshot.PORT_7FFD);

            throw new NotSupportedException("Need to implement +3 ROM bank paging");
        }

        private static ArraySegment<byte> Setup48KMemory(Z80_SNAPSHOT snapshot)
        {
            Out.Write("  Loading as ZX Spectrum 48K");
            var memory = new byte[(16 + 48) * 1024]; // Leave first 16KB ROM blank
            CopyBank(snapshot.RAM_BANK, 10, memory, 0x4000);
            CopyBank(snapshot.RAM_BANK, 04, memory, 0x8000);
            CopyBank(snapshot.RAM_BANK, 00, memory, 0xC000);
            return new ArraySegment<byte>(memory);
        }

        private static MemoryModel GetMemoryModel(Z80_SNAPSHOT snapshot)
        {
            if (snapshot.FileVersion == 1) return MemoryModel.ZX48;

            return snapshot.Byte34 switch
            {
                0 or 1 or 2 => MemoryModel.ZX48,
                3 => snapshot.FileVersion == 3 ? MemoryModel.ZX48 : MemoryModel.ZX128,
                4 or 5 or 6 or 11 => MemoryModel.ZX128,
                7 or 8 or 13 => MemoryModel.ZXPlus3,
                _ => throw new NotSupportedException(
                    $"Unknown memory model for v{snapshot.FileVersion} indicator {snapshot.Byte34}")
            };
        }
    }
}