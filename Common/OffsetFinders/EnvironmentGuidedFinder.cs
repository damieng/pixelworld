using System;
using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders;

public static class EnvironmentGuidedFinder
{
    public static List<Int32> FindOffsets(Byte[] buffer)
    {
        var spectrumSysChars = buffer[Spectrum.CharsSysVar] + buffer[Spectrum.CharsSysVar + 1] * 256 + 256;

        var results = new List<Int32>();
        if (spectrumSysChars <= Spectrum.ScreenStart) return results; // Was not pointing to the ROM
        
        if (spectrumSysChars + Spectrum.FontSize < buffer.Length && buffer.IsEmpty(spectrumSysChars))
            results.Add(spectrumSysChars);
        
        return results;
    }
}