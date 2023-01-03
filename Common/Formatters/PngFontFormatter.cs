using PixelWorld.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PixelWorld.Formatters
{
    public static class PngFontFormatter
    {
        const int CharWidth = 8;
        const int CharHeight = 8;

        public static readonly ImageCodecInfo DefaultEncoder = ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == ImageFormat.Png.Guid);

        public static readonly EncoderParameters DefaultEncoderParameters = GetEncoderParameters(2);

        public static EncoderParameters GetEncoderParameters(int depth)
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.ColorDepth, depth);
            return encoderParameters;
        }

        public static void Read(Fonts.Font font, Stream source, IReadOnlyDictionary<int, char> charset)
        {
            using Bitmap bitmap = (Bitmap)Image.FromStream(source);
            if (bitmap.Width % CharWidth != 0)
                throw new InvalidDataException($"Image width must be multiple of {CharWidth}");
            if (bitmap.Height % CharHeight != 0)
                throw new InvalidDataException($"Image height must be multiple of {CharHeight}");

            var offColor = bitmap.GetPixel(0, 0);

            int c = 0;
            for (var charY = 0; charY < bitmap.Height; charY += CharHeight)
            {
                for (var charX = 0; charX < bitmap.Width; charX += CharWidth)
                {
                    var data = new bool[CharWidth, CharHeight];
                    for (var y = 0; y < CharHeight; y++)
                        for (var x = 0; x < CharHeight; x++)
                            data[x, y] = bitmap.GetPixel(charX + x, charY + y) != offColor;
                    var glyph = new Glyph(CharWidth, CharHeight, data);
                    if (charset.TryGetValue(c++, out var mappedChar))
                        font.Glyphs.Add(mappedChar, glyph);
                }
            }
        }

        public static void Write(Fonts.Font font, Stream output)
        {
            using var bitmap = font.CreateBitmap();
            bitmap.Save(output, DefaultEncoder, DefaultEncoderParameters);
        }
    }
}
