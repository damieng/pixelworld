using System;

namespace PixelWorld.BinarySource;

public abstract class SpectrumBinarySource
{
    private static readonly Int32[] mappablePages = [0, 2, 6, 8, 12, 14];

    protected static ArraySegment<Byte> Setup128KMemory(Byte[][] ramBanks, Byte port7ffd)
    {
        Out.Write("  Loading as ZX Spectrum 128K");

        var extraSpace = 16;
        var bankAtC8000 = GetC800RamBank(port7ffd);
        if (bankAtC8000 is 10 or 04)
        {
            // This means the top bank is duplicated. Which is stupid but happens.
            extraSpace = 32;
        }

        var memory = new Byte[(extraSpace + 128) * 1024]; // Leave first 16KB ROM blank

        CopyBank(ramBanks, 10, memory, 0x4000);
        CopyBank(ramBanks, 04, memory, 0x8000);
        CopyBank(ramBanks, bankAtC8000, memory, 0xC000);

        var nextAddress = 0x10000;
        foreach (var page in mappablePages)
        {
            if (page != bankAtC8000)
            {
                CopyBank(ramBanks, page, memory, nextAddress);
                nextAddress += 0x4000;
            }
        }

        return new ArraySegment<Byte>(memory);
    }

    protected static void CopyBank(Byte[][] ramBanks, Int32 bank, Byte[] ram, Int32 address)
    {
        Array.Copy(ramBanks[bank], 0, ram, address, 8192);
        Array.Copy(ramBanks[bank + 1], 0, ram, address + 8192, 8192);
    }

    protected static Int32 GetC800RamBank(Int32 out7ffd)
    {
        return (out7ffd & 0x07) * 2;
    }
}