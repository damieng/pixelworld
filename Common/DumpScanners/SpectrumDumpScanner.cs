using PixelWorld.Display;
using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Font = PixelWorld.Fonts.Font;

namespace PixelWorld.Finders
{
    public class SpectrumDumpScanner
    {
        public static List<Font> Read(BinaryReader reader, string name)
        {
            var buffer = reader.ReadBytes(1024 * 1024);
            var candidates = SpectrumDisplay.GetCandidates(buffer, 0);

            var start = screenLength;

            var offsets = new List<int>();
            offsets.AddRange(KnownGlyphFinder.FindOffsets(buffer, start, RarelyChangedRomChars));
            offsets.AddRange(CandidatesInWindowFinder.FindOffsets(buffer, start, candidates));
            offsets.AddRange(GeneralHeuristicFinder.FindOffsets(buffer, start));

            var fonts = new List<Font>();
            foreach (var offset in offsets.Distinct())
                if (!IsRomFont(buffer, offset) && buffer.IsEmpty(offset) && !IsMissingTooManyGlyphs(buffer, offset))
                    fonts.Add(ByteFontFormatter.Create(reader, name, offset));

            return fonts;
        }

        public static bool IsMissingTooManyGlyphs(byte[] buffer, int offset)
        {
            return buffer.CountBlankGlyphs(offset, ByteFontFormatter.ExpectedLength, 8) > 36;
        }

        public static bool IsRomFont(byte[] buffer, int offset)
        {
            var sha1 = SHA384.Create().ComputeHash(buffer, offset, ByteFontFormatter.ExpectedLength);
            return sha1.ToHex() == "7fa0e307a6e78cf198c3a480a18437dcbecae485c22634cea69cdea3240e7079fe6bedc3c35a76047fb244b4fa15aa35";
        }

        public static Bitmap GetScreenPreview(BinaryReader reader)
        {
            var buffer = reader.ReadBytes(screenLength);
            return buffer.IsEmpty(0, buffer.Length) ? null : SpectrumDisplay.GetBitmap(buffer, 0);
        }

        private static readonly KnownCharPattern[] RarelyChangedRomChars = {
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
