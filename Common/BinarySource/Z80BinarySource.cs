using PixelWorld.BinarySource.Decoders;
using System;
using System.IO;

namespace PixelWorld.BinarySource
{
    public class Z80BinarySource : IBinarySource
    {
        enum MemoryModel
        {
            ZX48,
            ZX128,
            ZXPlus3
        };

        public ArraySegment<Byte> Read(Stream source)
        {
            // Use Ziggy's Z80 loader to decode the file
            try
            {
                var z80Snapshot = Z80File.LoadZ80(source);
                var memoryModel = GetMemoryModel(z80Snapshot);

                switch (memoryModel)
                {
                    case MemoryModel.ZX48:
                        return Setup48KMemory(z80Snapshot);

                    case MemoryModel.ZX128:
                        return Setup128KMemory(z80Snapshot);

                    case MemoryModel.ZXPlus3:
                        return SetupPlus3Memory(z80Snapshot);
                }

                throw new NotSupportedException($"Unknown MemoryModel {memoryModel}");
            }
            catch (Exception e)
            {
                Out.Write($"  Unable to process {e.Message}");
                return new ArraySegment<byte>();
            }
        }

        private ArraySegment<byte> SetupPlus3Memory(Z80_SNAPSHOT z80Snapshot)
        {
            if ((z80Snapshot.PORT_1FFD & 1) == 0)
                return Setup128KMemory(z80Snapshot);

            throw new NotSupportedException("Need to implement +3 ROM bank paging");
        }

        private static readonly int[] mappablePages = { 0, 2, 6, 8, 12, 14 };

        private static ArraySegment<byte> Setup128KMemory(Z80_SNAPSHOT z80Snapshot)
        {
            var extraSpace = 16;
            var bankAtC8000 = GetC800RamBank(z80Snapshot.PORT_7FFD);
            if (bankAtC8000 == 10 || bankAtC8000 == 04)
            {
                // This means the top bank is duplicated. Which is stupid but happens.
                extraSpace = 32;
            }

            var ram = new byte[(extraSpace + 128) * 1024]; // Leave first 16KB ROM blank


            CopyBank(z80Snapshot, 10, ram, 0x4000);
            CopyBank(z80Snapshot, 04, ram, 0x8000);
            CopyBank(z80Snapshot, bankAtC8000, ram, 0xC000);

            int nextAddress = 0x10000;
            for (var i = 0; i < mappablePages.Length; i++)
            {
                var page = mappablePages[i];
                if (page != bankAtC8000)
                {
                    CopyBank(z80Snapshot, page, ram, nextAddress);
                    nextAddress += 0x4000;
                }
            }

            return new ArraySegment<byte>(ram);
        }

        private static ArraySegment<byte> Setup48KMemory(Z80_SNAPSHOT z80Snapshot)
        {
            var memory = new byte[(16 + 48) * 1024]; // Leave first 16KB ROM blank
            CopyBank(z80Snapshot, 10, memory, 0x4000);
            CopyBank(z80Snapshot, 04, memory, 0x8000);
            CopyBank(z80Snapshot, 00, memory, 0xC000);
            return new ArraySegment<byte>(memory);
        }

        private static int GetC800RamBank(int out7ffd)
        {
            return (out7ffd & 0x07) * 2;
        }

        private static MemoryModel GetMemoryModel(Z80_SNAPSHOT z80Snapshot)
        {
            if (z80Snapshot.FileVersion == 1) return MemoryModel.ZX48;

            switch (z80Snapshot.Byte34)
            {
                case 0:
                case 1:
                case 2: // treat SAMRAM as 48k, ignore shadow pages
                    return MemoryModel.ZX48;
                case 3:
                    return z80Snapshot.FileVersion == 3 ? MemoryModel.ZX48 : MemoryModel.ZX128;
                case 4:
                case 5:
                case 6:
                case 11:
                    return MemoryModel.ZX128;
                case 7:
                case 8:
                case 13:
                    return MemoryModel.ZXPlus3;
            }

            throw new NotSupportedException($"Unknown memory model for v{z80Snapshot.FileVersion} indicator {z80Snapshot.Byte34}");
        }

        private static void CopyBank(Z80_SNAPSHOT z80Snapshot, int bank, byte[] ram, int address)
        {
            Array.Copy(z80Snapshot.RAM_BANK[bank], 0, ram, address, 8192);
            Array.Copy(z80Snapshot.RAM_BANK[bank + 1], 0, ram, address + 8192, 8192);
        }
    }
}
