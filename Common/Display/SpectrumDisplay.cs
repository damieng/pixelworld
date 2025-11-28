using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;

namespace PixelWorld.Display;

public class SpectrumDisplay
{
    private const Int32 AttributeWidth = 32;
    private const Int32 AttributeHeight = 24;
    private const Int32 PixelWidth = 8 * AttributeWidth;
    private const Int32 PixelHeight = 8 * AttributeHeight;
    private const Int32 AttributeOffset = PixelWidth * AttributeHeight;

    private static readonly UInt16[] lookupY = new UInt16[PixelHeight];

    private static readonly Color[] palette =
    [
        Color.Black, Color.DarkBlue, Color.DarkRed, Color.DarkMagenta, Color.Green, Color.DarkCyan, Color.Olive,
        Color.Gray,
        Color.Black, Color.Blue, Color.Red, Color.Magenta, Color.LimeGreen, Color.Cyan, Color.Yellow, Color.White
    ];

    static SpectrumDisplay()
    {
        UInt16 pos = 0;
        for (var third = 0; third < 3; third++)
        for (var line = 0; line < 8; line++)
        for (var y = 0; y < 63; y += 8)
        {
            lookupY[y + line + third * 64] = pos;
            pos += 32;
        }
    }

    public static Boolean IsBlank(Byte[] buffer, Int32 offset)
    {
        return buffer.Skip(offset).Take(768).All(b => b == 0);
    }

    public static Image<Rgb24> GetBitmap(Byte[] buffer, Int32 offset, Boolean altFlashFrame = false)
    {
        var image = new Image<Rgb24>(PixelWidth, PixelHeight);

        for (var ay = 0; ay < AttributeHeight; ay++)
        for (var ax = 0; ax < AttributeWidth; ax++)
        {
            var attribute = buffer[offset + ay * AttributeWidth + AttributeOffset + ax];
            var bright = (Byte)((attribute & 64) >> 3);
            var foreColor = palette[(attribute & 7) | bright];
            var backColor = palette[((attribute & 56) >> 3) | bright];
            if (altFlashFrame && (attribute & 128) == 128)
                (backColor, foreColor) = (foreColor, backColor);
            for (var py = 0; py < 8; py++)
            {
                var y = ay * 8 + py;
                var pixels = buffer[offset + lookupY[y] + ax];
                for (var px = 0; px < 8; px++)
                {
                    var a = 128 >> px;
                    var x = ax * 8 + px;
                    image[x, y] = (pixels & a) != 0 ? foreColor : backColor;
                }
            }
        }

        return image;
    }

    public static Byte[][] GetCandidates(Byte[] buffer, Int32 offset)
    {
        var uniques = new HashSet<Byte[]>(new ByteArrayEqualityComparer());

        for (var ay = 0; ay < AttributeHeight; ay++)
        for (var ax = 0; ax < AttributeWidth; ax++)
        {
            var block = new Byte[8];
            for (var py = 0; py < 8; py++)
            {
                var y = ay * 8 + py;
                block[py] = buffer[offset + lookupY[y] + ax];
            }

            uniques.Add(block);
        }

        // Empty and full blocks match too many things and slow things down
        uniques.Remove(EmptyChar);
        uniques.Remove(FullChar);

        return uniques.ToArray();
    }

    private static readonly Byte[] EmptyChar = [0, 0, 0, 0, 0, 0, 0, 0];
    private static readonly Byte[] FullChar = [255, 255, 255, 255, 255, 255, 255, 255];
}