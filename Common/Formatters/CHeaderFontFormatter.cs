using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorld.Formatters;

public static class CHeaderFontFormatter
{
    public static void CreateFontHeaderConst(String byteType, List<String> fileNames, String outputFolder, String credit)
    {
        foreach (var fileName in fileNames)
        {
            var fontName = Path.GetFileNameWithoutExtension(fileName);
            Out.Write($"Generating C header file for {fileName}");
            var cFontName = fontName.ToUpperInvariant().Replace(" ", "_").Replace("-", "_");

            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var font = ByteFontFormatter.Create(reader, fontName, 0, Spectrum.UK);
            var output = new StringBuilder();
            output.AppendLine($"// {fontName} font {credit}");

            output.AppendLine($"#ifndef {cFontName}_H_");
            output.AppendLine($"#define {cFontName}_H_");

            output.AppendLine();
            output.AppendLine("#include <stdint.h>");
            output.AppendLine();
            output.AppendLine($"static const {byteType} FONT_{cFontName}_BITMAP[] = {{");
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

                output.Append($", // {(glyph.Key == '\\' ? @"\ (backslash)" : glyph.Key)} \n");
            }
            output.AppendLine("};");
            output.AppendLine();
            output.AppendLine("#endif");

            File.WriteAllText(Utils.MakeFileName(fileName, ".h", outputFolder), output.ToString());
        }
    }
}