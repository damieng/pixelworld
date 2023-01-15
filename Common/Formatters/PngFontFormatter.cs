using PixelWorld.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Formatters
{
    public static class PngFontFormatter
    {
        const int CharWidth = 8;
        const int CharHeight = 8;

        public static readonly IImageEncoder DefaultEncoder = new PngEncoder() { ColorType = PngColorType.Rgb, BitDepth = PngBitDepth.Bit8 };

        public static void Read(Fonts.Font font, Stream source, IReadOnlyDictionary<int, char> charset)
        {
            using var image = Image.Load<Rgb24>(source);

            if (image.Width % CharWidth != 0)
                throw new InvalidDataException($"Image width must be multiple of {CharWidth}");
            if (image.Height % CharHeight != 0)
                throw new InvalidDataException($"Image height must be multiple of {CharHeight}");

            Color off = image[0, 0];

            var offColor = image[0, 0];

            int c = 0;
            for (var charY = 0; charY < image.Height; charY += CharHeight)
            {
                for (var charX = 0; charX < image.Width; charX += CharWidth)
                {
                    var data = new bool[CharWidth, CharHeight];
                    for (var y = 0; y < CharHeight; y++)
                        for (var x = 0; x < CharHeight; x++)
                            data[x, y] = image[charX + x, charY + y] != offColor;
                    var glyph = new Glyph(CharWidth, CharHeight, data);
                    if (charset.TryGetValue(c++, out var mappedChar))
                        font.Glyphs.Add(mappedChar, glyph);
                }
            }
        }

        public static void Write(Fonts.Font font, Stream output)
        {
            using var bitmap = font.CreateImage();
            bitmap.Save(output, DefaultEncoder);
        }
    }
}
