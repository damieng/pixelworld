using System.Collections.Generic;
using System.IO;
using PixelWorld.Shared.Fonts;
using PixelWorld.Shared.Formatters;

namespace PixelWorld.Shared.Finders
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

        public static List<Font> Read(BinaryReader reader, string name)
        {
            var inputBuffer = reader.ReadBytes(1024 * 1024);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            const int desiredLength = ByteFont.glyphRange * (ByteFont.charWidth / 8) * ByteFont.charHeight;

            var fontIndex = 0;

            var fonts = new List<Font>();

            var index = 0;
            while (index < inputBuffer.LongLength)
            {
                if (IsCharEmpty(inputBuffer, index))
                {
                    var startMatch = index;

                    if (!IsCharEmpty(inputBuffer, index))
                    {
                        index = startMatch + 1;
                    }
                    else
                    {
                        if (IsCharEmpty(inputBuffer, index + 8))
                        {
                            index = startMatch + 1;
                        }
                        else
                        {
                            if (startMatch + desiredLength > inputBuffer.LongLength)
                                break;

                            int foundChars = 0;
                            for (int c = 34; c < 128; c++)
                            {
                                if (!IsCharEmpty(inputBuffer, index + ((c - 32) * 8)))
                                    foundChars++;
                            }

                            if (foundChars > 26 * 2 + 10)
                            {
                                var font = new Font(name + "-" + ++fontIndex);
                                ByteFont.Read(font, reader);
                                fonts.Add(font);
                                index += desiredLength - (2 * 8);
                            }
                        }
                    }
                }

                index++;
            }

            return fonts;
        }
    }
}
