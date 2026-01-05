using PixelWorld.BinarySource.Decoders;
using System;
using System.IO;

namespace PixelWorld.BinarySource;

public class ZXSnaBinarySource : SpectrumBinarySource, IBinarySource
{
    public static IBinarySource Instance { get; } = new ZXSnaBinarySource();

    public ArraySegment<Byte> GetMemory(Stream source)
    {
        // Use Ziggy SNA loader to decode the file
        try
        {
            var snapshot = SNAFile.LoadSNA(source);

            return snapshot switch
            {
                SNA_48K sna48K => Setup48KMemory(sna48K),
                SNA_128K sna128K => Setup128KMemory(sna128K.RAM_BANK, sna128K.PORT_7FFD),
                null => throw new NotSupportedException("Could not decode as ZX Spectrum SNA format file"),
                _ => throw new NotSupportedException($"Unknown SNA MemoryModel {snapshot.GetType()}")
            };
        }
        catch (Exception e)
        {
            Out.Write($"  Unable to process {e.Message}");
            return new ArraySegment<Byte>();
        }
    }

    private static ArraySegment<Byte> Setup48KMemory(SNA_48K snapshot)
    {
        Out.Write("  Loading as ZX Spectrum 48K");
        return new ArraySegment<Byte>(snapshot.RAM);
    }
}