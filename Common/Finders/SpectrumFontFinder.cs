using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWorld.Fonts;

namespace PixelWorld.Finders
{
    public class SpectrumFontFinder
    {
        private static KnownCharPattern[] Spectrum = new KnownCharPattern[]
        {
            new KnownCharPattern(96, new byte[] // Copyright
            {
                0b00111100,
                0b01000010,
                0b10011001,
                0b10100001,
                0b10100001,
                0b10011001,
                0b01000010,
                0b00111100
            })
        };

        public static List<Font> Read(BinaryReader reader, string name)
        {
            var offsetsForKnown = ByteKnownOffsetFinder.Read(reader, name, Spectrum);
            var 
        }
    }
}
