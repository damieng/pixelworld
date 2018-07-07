using PixelWorld.Formatters;
using System.Collections.Generic;

namespace PixelWorld.OffsetFinders
{
    public class EnviromentGuidedFinder
    {
        private const int charVarOnSpeccy = 23606;

        public static List<int> FindOffsets(byte[] buffer)
        {
            var charVarInBuffer = charVarOnSpeccy - 16384;
            var charPointerOnSpeccy = buffer[charVarInBuffer] + buffer[charVarInBuffer + 1] * 256;
            var charPointerInBuffer = charPointerOnSpeccy + 256 - 16384;

            var results = new List<int>();
            if (charPointerInBuffer > 0) // Was not pointing to the ROM
                if (charPointerInBuffer + ByteFontFormatter.ExpectedLength < buffer.Length && buffer.IsEmpty(charPointerInBuffer))
                    results.Add(charPointerInBuffer);
            return results;
        }
    }
}
