using System;

namespace PixelWorld.BinarySource
{
    public abstract class ZXSpectrumBinarySource
    {
        private static readonly int[] mappablePages = { 0, 2, 6, 8, 12, 14 };

        protected static ArraySegment<byte> Setup128KMemory(byte[][] ramBanks, byte port7ffd)
        {
            var extraSpace = 16;
            var bankAtC8000 = GetC800RamBank(port7ffd);
            if (bankAtC8000 == 10 || bankAtC8000 == 04)
            {
                // This means the top bank is duplicated. Which is stupid but happens.
                extraSpace = 32;
            }

            var memory = new byte[(extraSpace + 128) * 1024]; // Leave first 16KB ROM blank

            CopyBank(ramBanks, 10, memory, 0x4000);
            CopyBank(ramBanks, 04, memory, 0x8000);
            CopyBank(ramBanks, bankAtC8000, memory, 0xC000);

            int nextAddress = 0x10000;
            for (var i = 0; i < mappablePages.Length; i++)
            {
                var page = mappablePages[i];
                if (page != bankAtC8000)
                {
                    CopyBank(ramBanks, page, memory, nextAddress);
                    nextAddress += 0x4000;
                }
            }

            return new ArraySegment<byte>(memory);
        }

        protected static void CopyBank(byte[][] ramBanks, int bank, byte[] ram, int address)
        {
            Array.Copy(ramBanks[bank], 0, ram, address, 8192);
            Array.Copy(ramBanks[bank + 1], 0, ram, address + 8192, 8192);
        }

        protected static int GetC800RamBank(int out7ffd)
        {
            return (out7ffd & 0x07) * 2;
        }
    }
}
