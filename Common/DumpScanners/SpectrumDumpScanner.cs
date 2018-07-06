using PixelWorld.Display;
using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Font = PixelWorld.Fonts.Font;

namespace PixelWorld.Finders
{
    public class SpectrumDumpScanner
    {
        public static List<Font> Read(BinaryReader reader, string name)
        {
            var rawBuffer = reader.ReadBytes(1024 * 1024);
            var candidates = SpectrumDisplay.GetCandidates(rawBuffer, 0);

            var scanBuffer = new ArraySegment<byte>(rawBuffer, screenLength, rawBuffer.Length - screenLength);
            var offsets = ByteKnownCharacterFinder.FindOffsets(scanBuffer, Spectrum);
            //offsets.AddRange(ByteCandidatesWindowFinder.FindOffsets(scanBuffer, candidates));
            //            offsets.AddRange(ByteHeuristicFinder.FindOffsets(scanBuffer));

            var fonts = new List<Font>();
            foreach (var offset in offsets.Distinct())
                fonts.Add(ByteFontFormatter.Create(reader, $"{name}-{offset}-", offset));

            return fonts;
        }

        public static Bitmap GetScreenPreview(BinaryReader reader)
        {
            var buffer = reader.ReadBytes(screenLength);
            return SpectrumDisplay.GetBitmap(buffer, 0);
        }

        private static readonly KnownCharPattern[] Spectrum = {
            new KnownCharPattern(3, new byte[] // #
            {
                0b00000000,
                0b00100100,
                0b01111110,
                0b00100100,
                0b00100100,
                0b01111110,
                0b00100100,
                0b00000000,
            }),
            new KnownCharPattern(4, new byte[] // $
            {
                0b00000000,
                0b00001000,
                0b00111110,
                0b00101000,
                0b00111110,
                0b00001010,
                0b00111110,
                0b00001000,
            }),
            new KnownCharPattern(6, new byte[] // &
            {
                0b00000000,
                0b00010000,
                0b00101000,
                0b00010000,
                0b00101010,
                0b01000100,
                0b00111010,
                0b00000000,
            }),
            new KnownCharPattern(32, new byte[] // @
            {
                0b00000000,
                0b00111100,
                0b01001010,
                0b01010110,
                0b01011110,
                0b01000000,
                0b00111100,
                0b00000000,
            }),
            new KnownCharPattern(64, new byte[] // £
            {
                0b00000000,
                0b00011100,
                0b00100010,
                0b01111000,
                0b00100000,
                0b00100000,
                0b01111110,
                0b00000000,
            }),
            new KnownCharPattern(95, new byte[] // ©
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

        private const int screenLength = 6912;
    }
}
