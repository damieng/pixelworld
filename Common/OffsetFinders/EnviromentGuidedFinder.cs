﻿using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders
{
    public class EnviromentGuidedFinder
    {
        private const int CharsEnvVar = 23606;

        public static List<int> FindOffsets(byte[] buffer)
        {
            var spectrumSysChars = buffer[CharsEnvVar] + buffer[CharsEnvVar + 1] * 256 + 256;

            var results = new List<int>();
            if (spectrumSysChars > 0) // Was not pointing to the ROM 
                if (spectrumSysChars + Spectrum.FontSize < buffer.Length && buffer.IsEmpty(spectrumSysChars))
                    results.Add(spectrumSysChars);
            return results;
        }
    }
}
