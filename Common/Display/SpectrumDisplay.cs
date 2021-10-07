using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PixelWorld.Formatters;

namespace PixelWorld.Display
{
    public class SpectrumDisplay
    {
        const int AttributeWidth = 32;
        const int AttributeHeight = 24;
        const int PixelWidth = 8 * AttributeWidth;
        const int PixelHeight = 8 * AttributeHeight;
        const int AttributeOffset = PixelWidth * AttributeHeight;

        static readonly UInt16[] LookupY = new UInt16[PixelHeight];

        static readonly Color[] palette =
        {
            Color.Black, Color.DarkBlue, Color.DarkRed, Color.DarkMagenta, Color.Green, Color.DarkCyan, Color.Olive, Color.Gray,
            Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.LimeGreen, Color.Cyan, Color.Yellow, Color.White
        };

        static SpectrumDisplay()
        {
            UInt16 pos = 0;
            for (var third = 0; third < 3; third++)
                for (var line = 0; line < 8; line++)
                    for (var y = 0; y < 63; y += 8)
                    {
                        LookupY[y + line + (third * 64)] = pos;
                        pos += 32;
                    }
        }

        public static bool IsBlank(byte[] buffer, int offset)
        {
            return buffer.Skip(offset).Take(768).All(b => b == 0);
        }

        public static Bitmap GetBitmap(byte[] buffer, int offset)
        {
            var bitmap = new Bitmap(PixelWidth, PixelHeight);

            for (var ay = 0; ay < AttributeHeight; ay++)
                for (var ax = 0; ax < AttributeWidth; ax++)
                {
                    var attribute = buffer[offset + ay * AttributeWidth + AttributeOffset + ax];
                    var bright = (Byte)((attribute & 64) >> 3);
                    var foreColor = palette[(attribute & 7) | bright];
                    var backColor = palette[((attribute & 56) >> 3) | bright];
                    for (var py = 0; py < 8; py++)
                    {
                        var y = ay * 8 + py;
                        var pixels = buffer[offset + LookupY[y] + ax];
                        for (var px = 0; px < 8; px++)
                        {
                            var a = 128 >> px;
                            var x = ax * 8 + px;
                            bitmap.SetPixel(x, y, (pixels & a) != 0 ? foreColor : backColor);
                        }
                    }
                }

            return bitmap;
        }

        public static byte[][] GetCandidates(byte[] buffer, int offset)
        {
            var uniques = new HashSet<byte[]>(new ByteArrayEqualityComparer());

            for (var ay = 0; ay < AttributeHeight; ay++)
                for (var ax = 0; ax < AttributeWidth; ax++)
                {
                    var block = new byte[8];
                    for (var py = 0; py < 8; py++)
                    {
                        var y = ay * 8 + py;
                        block[py] = buffer[offset + LookupY[y] + ax];
                    }

                    uniques.Add(block);
                }

            // Empty and full blocks match too many things and slow things down
            uniques.Remove(EmptyChar); 
            uniques.Remove(FullChar);

            return uniques.ToArray();
        }

        private static readonly byte[] EmptyChar = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] FullChar = { 255, 255, 255, 255, 255, 255, 255, 255 };
    }
}