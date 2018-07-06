using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Finders
{
    public class KnownCharPattern
    {
        public readonly int CharCode;
        public readonly byte[] Pattern;

        public KnownCharPattern(int charCode, byte[] pattern)
        {
            CharCode = charCode;
            Pattern = pattern;
        }
    }

    public static class ByteKnownOffsetFinder
    {
        public static List<Font> Read(BinaryReader reader, string name, KnownCharPattern[] knownFont)
        {
            var buffer = reader.ReadBytes(1024 * 1024);
            const int desiredLength = ByteFontFormatter.glyphRange * (ByteFontFormatter.charWidth / 8) * ByteFontFormatter.charHeight;

            var fontIndex = 0;
            var fonts = new List<Font>();

            var offset = 6911; // Don't bother checking screen
            while (offset + desiredLength < buffer.LongLength)
            {
                for (var c = 0; c < knownFont.Length; c++)
                {
                    var known = knownFont[c];
                    if (buffer.IsSame(offset, known.Pattern))
                    {
                        fonts.Add(ByteFontFormatter.Create(reader, $"{name + "-" + known.CharCode + "-" + ++fontIndex}", offset - known.CharCode * 8));
                        break;
                    }
                }

                offset++;
            }

            return fonts;
        }
    }
}
