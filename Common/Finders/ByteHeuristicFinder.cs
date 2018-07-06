using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld.Finders
{
    public static class ByteHeuristicFinder
    {

        static bool HasLikelyDensities(byte[] buffer, int offset)
        {
            return
                buffer.PixelCount(offset, '.') < buffer.PixelCount(offset, '=') &&
                buffer.PixelCount(offset, ',') < buffer.PixelCount(offset, 'B') &&
                buffer.PixelCount(offset, ',') < buffer.PixelCount(offset, 'B') &&
                buffer.PixelCount(offset, '-') < buffer.PixelCount(offset, '+');
        }

        public static List<Font> Read(BinaryReader reader, string name)
        {
            var buffer = reader.ReadBytes(1024 * 1024);
            const int desiredLength = ByteFontFormatter.glyphRange * (ByteFontFormatter.charWidth / 8) * ByteFontFormatter.charHeight;

            var fontIndex = 0;
            var fonts = new List<Font>();

            var offset = 6911; // Don't bother checking screen
            while (offset + desiredLength < buffer.LongLength)
            {
                if (Deduce(buffer, offset))
                    fonts.Add(ByteFontFormatter.Create(reader, $"{name + "-" + ++fontIndex}", offset));

                offset++;
            }

            return fonts;
        }

        private static bool IsUnderscore(byte[] buffer, int i)
        {
            return buffer.IsEmpty(i, 5) && !buffer.IsEmpty(i + 5, 3);
        }

        private static bool IsMinus(byte[] buffer, int i)
        {
            return buffer.IsEmpty(i, 2) && !buffer.IsEmpty(i + 2, 4) && buffer.IsEmpty(i + 6, 2);
        }

        private static bool Deduce(byte[] buffer, int i)
        {
            if (buffer.IsEmpty(i)) // Start with a space
            {
                int spacesCount = 1;
                int uniqueCount = 0;
                int upperMissing = 0;
                int lowerMissing = 0;
                int digitsMissing = 0;
                int zeroCount = buffer.Take(ByteFontFormatter.glyphRange * 8).Count(b => b == 0);

                for (int c = 0; c < 95; c++)
                {
                    bool charIsUnique = true;
                    for (int x = c + 1; x < ByteFontFormatter.glyphRange; x++)
                    {
                        if (buffer.IsSame(i + c * 8, i + x * 8))
                            charIsUnique = false;
                    }

                    if (buffer.IsEmpty(i + c * 8))
                    {
                        char cg = (char)(c + 32);
                        if (char.IsUpper(cg))
                            upperMissing++;
                        if (char.IsLower(cg))
                            lowerMissing++;
                        if (char.IsNumber(cg))
                            digitsMissing++;

                        spacesCount++;
                    }

                    if (charIsUnique)
                        uniqueCount++;
                }

                var missingAlphas = upperMissing > 0 && lowerMissing > 0;

                return uniqueCount >= 36 && spacesCount < 60
                                      && !missingAlphas && digitsMissing == 0
                                      && zeroCount < 700 && HasLikelyDensities(buffer, i);
            }

            return false;
        }
    }
}
