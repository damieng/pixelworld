using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorld.Formatters;

public static class RustHeaderFontFormatter
{
    public static void CreateFontHeaderConst(List<String> fileNames, String outputFolder, String credit)
    {
        foreach (var fileName in fileNames)
        {
            var fontName = Path.GetFileNameWithoutExtension(fileName);
            Out.Write($"Generating Rust header file for {fileName}");
            var rustFontName = fontName.ToUpperInvariant().Replace(" ", "_").Replace("-", "_");

            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var font = ByteFontFormatter.Create(reader, fontName, 0, Spectrum.UK);
            var output = new StringBuilder();
            output.AppendLine($"// {fontName} font {credit}");

            output.AppendLine($"pub const FONT_{rustFontName}_BITMAP: &[u8] = &[");
            foreach (var glyph in font.Glyphs)
            {
                output.Append('\t');
                for (var y = 0; y < font.Height; y++)
                {
                    var b = new Byte();
                    var charWidth = glyph.Value.Width;
                    for (var x = 0; x < charWidth; x++)
                    {
                        if (glyph.Value.Data[x, y])
                            b |= (Byte)(1 << charWidth - 1 - x);
                    }
                    if (y > 0) output.Append(", ");
                    output.Append($"0x{b:x2}");
                }

                output.Append($", // {glyph.Key}\n");
            }
            output.AppendLine("];");

            File.WriteAllText(Utils.MakeFileName(fileName, ".rs", outputFolder), output.ToString());
        }
    }
}