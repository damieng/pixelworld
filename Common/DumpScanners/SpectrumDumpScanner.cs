using PixelWorld.Display;
using PixelWorld.Formatters;
using PixelWorld.OffsetFinders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Font = PixelWorld.Fonts.Font;

namespace PixelWorld.DumpScanners
{
    public class SpectrumDumpScanner
    {
        public static List<Font> Read(BinaryReader reader, string name)
        {
            var buffer = reader.ReadBytes(1024 * 1024); // 1MB is enough for any Speccy

            var offsets = GetOffsets(buffer);

            var fonts = new List<Font>();
            foreach (var offset in offsets)
                if (!IsRomFont(buffer, offset) && buffer.IsEmpty(offset) && !IsMissingTooManyGlyphs(buffer, offset))
                    fonts.Add(ByteFontFormatter.Create(reader, $"{name}#{offset}", offset));

            return fonts;
        }

        private static IEnumerable<int> GetOffsets(byte[] buffer)
        {
            var candidates = SpectrumDisplay.GetCandidates(buffer, 16384);

            var rst = EnviromentGuidedFinder.FindOffsets(buffer);
            //var rom = KnownCharPatternFinder.FindOffsets(buffer, RarelyChangedRomChars);
            //var scr = CandidatesInWindowFinder.FindOffsets(buffer, candidates);
            //var heu = GeneralHeuristicFinder.FindOffsets(buffer);

            var offsets = new List<int>();
            offsets.AddRange(rst);
            //offsets.AddRange(rom);
            //offsets.AddRange(scr);
            //offsets.AddRange(heu);

            var dupes = new HashSet<int>(offsets
                .GroupBy(o => o)
                .Where(o => o.Count() > 1)
                .Select(g => g.Key));

            OutFinderDetail(rst, "RST 16/CHARS", dupes);
            //OutFinderDetail(rom, "ROM Glyphs", dupes);
            //OutFinderDetail(scr, "SCREEN$ Tiles", dupes);
            //OutFinderDetail(heu, "Heuristics", dupes);

            return new HashSet<int>(offsets);
        }

        public static void OutFinderDetail(List<int> offsets, string method, HashSet<int> dupes)
        {
            if (offsets.Count > 0)
            {
                var uniques = offsets.Count(o => !dupes.Contains(o));
                Out.Write($"  {method} found offsets ({uniques} uniques) {String.Join(", ", offsets)}");
            }
        }

        public static bool IsMissingTooManyGlyphs(byte[] buffer, int offset)
        {
            return buffer.CountBlankGlyphs(offset, ByteFontFormatter.ExpectedLength, 8) > 26;
        }

        public static bool IsRomFont(byte[] buffer, int offset)
        {
            var sha1 = SHA384.Create().ComputeHash(buffer, offset, ByteFontFormatter.ExpectedLength);
            return sha1.ToHex() == "7fa0e307a6e78cf198c3a480a18437dcbecae485c22634cea69cdea3240e7079fe6bedc3c35a76047fb244b4fa15aa35";
        }

        public static Bitmap GetScreenPreview(BinaryReader reader)
        {
            reader.BaseStream.Seek(16384, SeekOrigin.Begin);
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
