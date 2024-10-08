﻿using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorld.Formatters;

public static class AssemblyFontFormatter
{
    public static void CreateDefines(string language, string defineByteInstruction, string format, List<string> fileNames, string outputFolder, string credit)
    {
        foreach (var fileName in fileNames)
        {
            Out.Write($"Generating {language} assembly file for {fileName}");
            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
            var output = new StringBuilder();
            output.Append($"\t; {Path.GetFileNameWithoutExtension(fileName)} font {credit}\n");
            foreach (var glyph in font.Glyphs)
            {
                output.Append("\t" + defineByteInstruction);
                for (var y = 0; y < font.Height; y++)
                {
                    var b = new Byte();
                    var charWidth = glyph.Value.Width;
                    for (var x = 0; x < charWidth; x++)
                    {
                        if (glyph.Value.Data[x, y])
                            b |= (byte)(1 << charWidth - 1 - x);
                    }
                    if (y > 0)
                        output.Append(',');
                    output.AppendFormat(format, b);
                }

                output.Append($" ; {glyph.Key}\n");
            }

            File.WriteAllText(Utils.MakeFileName(fileName, language + ".asm", outputFolder), output.ToString());
        }
    }

    public static void GenZ80AsmBinary(List<string> fileNames, string outputFolder, string credit)
    {
        foreach (var fileName in fileNames)
        {
            Out.Write($"Generating Z80 assembly file for {fileName}");
            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
            var output = new StringBuilder();
            output.Append($"\t; {Path.GetFileNameWithoutExtension(fileName)} font {credit}\n");
            foreach (var glyph in font.Glyphs)
            {
                output.Append($"\t; {glyph.Key}\n");

                for (var y = 0; y < font.Height; y++)
                {
                    var b = new Byte();
                    var charWidth = glyph.Value.Width;
                    for (var x = 0; x < charWidth; x++)
                    {
                        if (glyph.Value.Data[x, y])
                            b |= (byte)(1 << charWidth - 1 - x);
                    }
                    string binary = "00000000" + Convert.ToString(b, 2);
                    output.Append($"\tdefb %{binary.Substring(binary.Length - 8, 8)}\n");
                }
            }

            File.WriteAllText(Utils.MakeFileName(fileName, "z80.asm", outputFolder), output.ToString());
        }
    }
}