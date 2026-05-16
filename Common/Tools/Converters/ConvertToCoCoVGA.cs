using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToCoCoVGA
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
    {
        var templateFilename = Path.Combine(templatePath, "cocovga.chr");
        Out.Write($"Using template {templateFilename}");
        var buffer = File.ReadAllBytes(templateFilename);
        if (buffer.Length != 3082) throw new Exception($"Template file {templateFilename} is not 3082 bytes");

        var lower = @"£abcdefghijklmnopqrstuvwxyz{|}~©".ToCharArray();
        var upper = @"@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_".ToCharArray();
        var symbols = " !\"#$%&'()*+,-./0123456789:;<=>?".ToCharArray();

        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, "chr", outputFolder);
            Out.Write($"Converting file {sourceFileName} to {targetFileName}");
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);

            using var target = File.Create(targetFileName);
            var index = 0;

            WriteGlyphs(lower);
            WriteGlyphs(symbols, true);
            WriteGlyphs(upper);
            WriteGlyphs(symbols);

            target.Write(buffer);

            void WriteGlyphs(Char[] chars, bool inverted = false)
            {
                foreach (var c in chars)
                {
                    var glyph = sourceFont.Glyphs[c];
                    var charOffset = 5 + index * 12;

                    for (var y = 0; y < 8; y++)
                    {
                        var rowData = glyph.GetRowByte(y);
                        if (inverted) rowData = (Byte)~rowData;
                        buffer[charOffset + 2 + y] = rowData;
                    }

                    index++;
                }
            }
        }
    }
}
