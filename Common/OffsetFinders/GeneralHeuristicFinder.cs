using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWorld.OffsetFinders
{
    public static class GeneralHeuristicFinder
    {
        static bool HasLikelySymbolDensities(byte[] buffer, int offset)
        {
            return
                buffer.PixelCount(offset, '.') < buffer.PixelCount(offset, ':') &&
                buffer.PixelCount(offset, ',') < buffer.PixelCount(offset, ';') &&
                buffer.PixelCount(offset, '-') < buffer.PixelCount(offset, '+');
        }

        static bool HasLikelyUpperDensities(byte[] buffer, int offset)
        {
            return
                buffer.PixelCount(offset, 'D') < buffer.PixelCount(offset, 'B') &&
                buffer.PixelCount(offset, 'C') < buffer.PixelCount(offset, 'O');
        }

        static bool HasLikelyLowerDensities(byte[] buffer, int offset)
        {
            return
                buffer.PixelCount(offset, 'o') < buffer.PixelCount(offset, 'g') &&
                buffer.PixelCount(offset, 'l') < buffer.PixelCount(offset, 't') &&
                buffer.PixelCount(offset, 'j') < buffer.PixelCount(offset, 'u');
        }

        static bool HasLikelyNumericDensities(byte[] buffer, int offset)
        {
            return
                buffer.PixelCount(offset, '1') < buffer.PixelCount(offset, '2') &&
                buffer.PixelCount(offset, '3') < buffer.PixelCount(offset, '8');
        }

        public static List<int> FindOffsets(byte[] buffer)
        {
            var offsets = new List<int>();

            var end = buffer.Length - Spectrum.FontSize;

            for (var i = 0; i < end; i++)
            {
                if (IsLikelyFont(buffer, i))
                    offsets.Add(i);
            }

            return offsets;
        }

        private static bool IsUnderscore(byte[] buffer, int i)
        {
            return buffer.IsEmpty(i, 5) && !buffer.IsEmpty(i + 5, 3);
        }

        private static bool IsMinus(byte[] buffer, int i)
        {
            return buffer.IsEmpty(i, 2) && !buffer.IsEmpty(i + 2, 4) && buffer.IsEmpty(i + 6, 2);
        }

        private static bool IsLikelyFont(byte[] buffer, int i)
        {
            if (buffer.IsEmpty(i)) // Start with a space
            {
                var longestNoEmptyRun = 0;
                var currentNoEmptyRun = 0;
                var totalNotEmpty = 0;
                var missingNumbers = 0;
                var missingLowercase = 0;
                var missingUppercase = 0;
                var duplicates = 0;

                for (var c = 0; c < 95; c++)
                {
                    if (buffer.IsEmpty(i + c * 8) || buffer.IsFull(i + c * 8))
                    {
                        currentNoEmptyRun = 0;
                        var asc = c + 32;
                        if (asc >= '0' && asc <= '9')
                            missingNumbers++;
                        else if (asc >= 'a' && asc <= 'z')
                            missingLowercase++;
                        else if (asc >= 'A' && asc <= 'Z')
                            missingUppercase++;
                    }
                    else
                    {
                        for (var z = c + 1; z < 95; z++)
                            if (buffer.IsSame(i + c * 8, i + z * 8))
                                duplicates++;

                        currentNoEmptyRun++;
                        totalNotEmpty++;
                        if (currentNoEmptyRun > longestNoEmptyRun)
                            longestNoEmptyRun = currentNoEmptyRun;
                    }
                }

                var likely = new Func<byte[], int, bool>[]
                {
                    HasLikelyLowerDensities, HasLikelyUpperDensities, HasLikelyNumericDensities,
                    HasLikelySymbolDensities
                };

                var likelyDensityCount = likely.Count(l => l(buffer, i));

                return duplicates < 5
                       && longestNoEmptyRun >= 26
                       && totalNotEmpty >= 36
                       && IsMinus(buffer, i + 13 * 8)
                       && IsUnderscore(buffer, i + 63 * 8)
                       && (missingUppercase == 0 || missingLowercase == 0)
                       && missingNumbers == 0
                       && SkewChecks(buffer, i) >= 2
                       && likelyDensityCount > 1;
            }

            return false;
        }

        private static int SkewChecks(byte[] buffer, int offset)
        {
            var looksUnskewed = 0;
            if (buffer[offset + (94 * 8) + 7] == 0)
                looksUnskewed++;
            if (buffer[offset + (95 * 8) + 7] != 0)
                looksUnskewed++;
            return looksUnskewed;
        }
    }
}