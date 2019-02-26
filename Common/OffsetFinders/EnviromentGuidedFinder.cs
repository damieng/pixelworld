using PixelWorld.Machines;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders
{
    public class EnviromentGuidedFinder
    {
        private const int charsEnvVar = 23606;

        public static List<int> FindOffsets(byte[] buffer)
        {
            var charPointerOnSpeccy = (buffer[charsEnvVar] + buffer[charsEnvVar + 1] * 256) + 256;

            var results = new List<int>();
            if (charPointerOnSpeccy > 0) // Was not pointing to the ROM 
                if (charPointerOnSpeccy + Spectrum.FontSize < buffer.Length && buffer.IsEmpty(charPointerOnSpeccy))
                    results.Add(charPointerOnSpeccy);
            return results;
        }
    }
}
