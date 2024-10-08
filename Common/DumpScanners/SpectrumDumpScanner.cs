﻿using PixelWorld.Display;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using PixelWorld.OffsetFinders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Font = PixelWorld.Fonts.Font;

namespace PixelWorld.DumpScanners;

public static class SpectrumDumpScanner
{
    public static List<Font> Read(BinaryReader reader, string name)
    {
        var buffer = reader.ReadBytes(1024 * 2048); // 2MB is enough for any Speccy

        var offsets = GetOffsets(buffer);

        var fonts = new List<Font>();
        foreach (var offset in offsets)
            if (!IsRomFont(buffer, offset) && buffer.IsEmpty(offset) && !IsMissingTooManyGlyphs(buffer, offset))
                fonts.Add(ByteFontFormatter.Create(reader, $"{name}#{offset}", offset, Spectrum.UK));

        return fonts;
    }

    private static IEnumerable<int> GetOffsets(byte[] buffer)
    {
        var address = buffer.Length == 65536 ? 16384 : 0;
        var candidates = SpectrumDisplay.GetCandidates(buffer, address);

        var rst = EnvironmentGuidedFinder.FindOffsets(buffer);
        var rom = KnownCharPatternFinder.FindOffsets(buffer, rarelyChangedRomChars);
        var scr = CandidatesInWindowFinder.FindOffsets(buffer, candidates);
        var heu = GeneralHeuristicFinder.FindOffsets(buffer);

        var offsets = new List<int>();
        offsets.AddRange(rst);
        offsets.AddRange(rom);
        offsets.AddRange(scr);
        offsets.AddRange(heu);

        var dupes = new HashSet<int>(offsets
            .GroupBy(o => o)
            .Where(o => o.Count() > 1)
            .Select(g => g.Key));

        OutFinderDetail(rst, "RST 16/CHARS", dupes);
        OutFinderDetail(rom, "ROM Glyphs", dupes);
        OutFinderDetail(scr, "SCREEN$ Tiles", dupes);
        OutFinderDetail(heu, "Heuristics", dupes);

        return new HashSet<int>(offsets);
    }

    public static void OutFinderDetail(List<int> offsets, string method, HashSet<int> dupes)
    {
        if (offsets.Count <= 0) return;

        var uniques = offsets.Count(o => !dupes.Contains(o));
        Out.Write($"  {method} found offsets ({uniques} uniques) {string.Join(", ", offsets)}");
    }

    public static bool IsMissingTooManyGlyphs(byte[] buffer, int offset)
    {
        return buffer.CountBlankGlyphs(offset, Spectrum.FontSize, 8) > 26;
    }

    public static bool IsRomFont(byte[] buffer, int offset)
    {
        var sha1 = SHA384.Create().ComputeHash(buffer, offset, Spectrum.FontSize);
        return sha1.ToHex() ==
               "7fa0e307a6e78cf198c3a480a18437dcbecae485c22634cea69cdea3240e7079fe6bedc3c35a76047fb244b4fa15aa35";
    }

    private static readonly KnownCharPattern[] rarelyChangedRomChars =
    [
        new(3, [
            0b00000000,
            0b00100100,
            0b01111110,
            0b00100100,
            0b00100100,
            0b01111110,
            0b00100100,
            0b00000000
        ]),
        new(4, [
            0b00000000,
            0b00001000,
            0b00111110,
            0b00101000,
            0b00111110,
            0b00001010,
            0b00111110,
            0b00001000
        ]),
        new(6, [
            0b00000000,
            0b00010000,
            0b00101000,
            0b00010000,
            0b00101010,
            0b01000100,
            0b00111010,
            0b00000000
        ]),
        new(32, [
            0b00000000,
            0b00111100,
            0b01001010,
            0b01010110,
            0b01011110,
            0b01000000,
            0b00111100,
            0b00000000
        ]),
        new(64, [
            0b00000000,
            0b00011100,
            0b00100010,
            0b01111000,
            0b00100000,
            0b00100000,
            0b01111110,
            0b00000000
        ]),
        new(95, [
            0b00111100,
            0b01000010,
            0b10011001,
            0b10100001,
            0b10100001,
            0b10011001,
            0b01000010,
            0b00111100
        ])
    ];
}