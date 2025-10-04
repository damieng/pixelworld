using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders;

public static class EnvironmentGuidedFinder
{
    private const int CharsEnvVar = 23606;

    public static List<int> FindOffsets(byte[] buffer)
    {
        var spectrumSysChars = buffer[CharsEnvVar] + buffer[CharsEnvVar + 1] * 256 + 256;

        var results = new List<int>();
        if (spectrumSysChars <= 16384) return results; // Was not pointing to the ROM
        
        if (spectrumSysChars + Spectrum.FontSize < buffer.Length && buffer.IsEmpty(spectrumSysChars))
            results.Add(spectrumSysChars);
        
        return results;
    }
}