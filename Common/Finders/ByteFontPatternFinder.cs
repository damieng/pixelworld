using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld.Finders
{
    public static class ByteFontPatternFinder
    {
        static bool IsEmpty(byte[] buffer, int index, int rows = 8)
        {
            for (int e = 0; e < rows; e++)
                if (buffer[index + e] != 0)
                    return false;

            return true;
        }

        static int CountBlankLines(byte[] buffer, int index)
        {
            int blankLines = 0;
            for (int e = 0; e < 8; e++)
                if (buffer[index + e] == 0)
                    blankLines++;
            return blankLines;
        }

        static bool IsSame(byte[] buffer, int firstIndex, int secondIndex)
        {
            for (int e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != buffer[secondIndex + e])
                    return false;

            return true;
        }

        static int PixelCount(byte[] buffer, int offset, char c)
        {
            int count = 0;
            for (int y = 0; y < 8; y++)
            {
                int g = buffer[offset + c - 32 + y];
                for (int x = 0; x < 8; x++)
                {
                    int f = 1 << x;
                    if ((g & f) == f)
                        count += 1;
                }
            }

            return count;
        }

        static bool HasLikelyDensities(byte[] buffer, int offset)
        {
            return
                PixelCount(buffer, offset, '.') < PixelCount(buffer, offset, '=') &&
                PixelCount(buffer, offset, ',') < PixelCount(buffer, offset, 'B') &&
                PixelCount(buffer, offset, ',') < PixelCount(buffer, offset, 'B') &&
                PixelCount(buffer, offset, '-') < PixelCount(buffer, offset, '+');
        }

        public static List<Font> Read(BinaryReader reader, string name)
        {
            var buffer = reader.ReadBytes(1024 * 1024);
            const int desiredLength = ByteFontFormatter.glyphRange * (ByteFontFormatter.charWidth / 8) * ByteFontFormatter.charHeight;

            var fontIndex = 0;
            var fonts = new List<Font>();

            var i = 0;
            while (i + desiredLength < buffer.LongLength)
            {
                if (IsEmpty(buffer, i) && !IsEmpty(buffer, i + 8)) // Start with a space
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
                            if (IsSame(buffer, i + c * 8, i + x * 8))
                                charIsUnique = false;
                        }

                        if (IsEmpty(buffer, i + c * 8))
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

                    var underscoreOffset = i + ('_' - 32) * 8;
                    var hasUnderscore = IsEmpty(buffer, underscoreOffset, 6) && !IsEmpty(buffer, underscoreOffset + 6, 2);
                    var minusOffset = i + ('-' - 32) * 8;
                    var hasMinus = IsEmpty(buffer, minusOffset, 2) && !IsEmpty(buffer, minusOffset + 2, 4) && IsEmpty(buffer, minusOffset + 6, 2);

                    var missingAlphas = upperMissing > 0 && lowerMissing > 0;

                    if (uniqueCount > 36 && spacesCount < 60 &&
                        hasUnderscore && hasMinus &&
                        !missingAlphas && digitsMissing == 0 &&
                        zeroCount < 700 && HasLikelyDensities(buffer, i))
                    {
                        var font = new Font(name + "-" + ++fontIndex) { Height = 8 };
                        reader.BaseStream.Seek(i, SeekOrigin.Begin);
                        ByteFontFormatter.Read(font, reader);
                        fonts.Add(font);
                        i++;
                    }
                }

                i++;
            }

            return fonts;
        }
    }
}
