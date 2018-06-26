using System.Collections.Generic;
using System.IO;
using PixelWorld.Fonts;
using PixelWorld.Formatters;

namespace PixelWorld.Finders
{
    public static class ByteFontPatternFinder
    {
        static bool IsCharEmpty(byte[] buffer, int index)
        {
            for (int e = 0; e < 8; e++)
                if (buffer[index + e] != 0)
                    return false;

            return true;
        }

        static bool IsCharSame(byte[] buffer, int firstIndex, int secondIndex)
        {
            for (int e = 0; e < 8; e++)
                if (buffer[firstIndex + e] != buffer[secondIndex + e])
                    return false;

            return true;
        }

        public static List<Font> Read(BinaryReader reader, string name)
        {
            var inputBuffer = reader.ReadBytes(1024 * 1024);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            const int desiredLength = ByteFontFormatter.glyphRange * (ByteFontFormatter.charWidth / 8) * ByteFontFormatter.charHeight;

            var fontIndex = 0;

            var fonts = new List<Font>();

            var index = 0;
            while (index < inputBuffer.LongLength)
            {
                if (IsCharEmpty(inputBuffer, index) && !IsCharEmpty(inputBuffer, index + 8)) // Start with a space
                {
                    if (index + desiredLength > inputBuffer.LongLength)
                        break;

                    int differentChars = 0;
                    for (int c = 0; c < 95; c++)
                    {
                        bool charIsUnique = true;
                        for (int x = c + 1; x < 95; x++)
                        {
                            if (IsCharSame(inputBuffer, index + c * 8, index + x * 8))
                                charIsUnique = false;
                        }

                        if (charIsUnique)
                            differentChars++;
                    }

                    if (differentChars > 90)
                    {
                        Out.Write($"Believed to have {differentChars} different chars");
                        var font = new Font(name + "-" + ++fontIndex);
                        reader.BaseStream.Seek(index, SeekOrigin.Begin);
                        ByteFontFormatter.Read(font, reader);
                        fonts.Add(font);
                        index += desiredLength;
                    }
                }

                index++;
            }

            return fonts;
        }
    }
}
