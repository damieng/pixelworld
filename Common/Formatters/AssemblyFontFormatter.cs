using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelWorld.Formatters
{
    public class AssemblyFontFormatter
    {
        public static int GenAsmHex(string language, string defineByteInstruction, string format, List<string> fileNames, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating {language} assembly file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                var output = new StringBuilder();
                output.AppendFormat("\t; {0} font by DamienG https://damieng.com\n", Path.GetFileNameWithoutExtension(fileName));
                foreach (var glyph in font.Glyphs)
                {
                    output.Append("\t" + defineByteInstruction);
                    for (int y = 0; y < font.Height; y++)
                    {
                        var b = new Byte();
                        var charWidth = glyph.Value.Width;
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Value.Data[x, y])
                                b |= (byte)(1 << charWidth - 1 - x);
                        }
                        if (y > 0)
                            output.Append(',');
                        output.AppendFormat(format, b);
                    }
                    output.AppendFormat(" ; {0}\n", glyph.Key);
                }

                File.WriteAllText(Utils.MakeFileName(fileName, language + ".asm", outputFolder), output.ToString());
            }

            return fileNames.Count;
        }

        public static int GenZ80AsmBinary(List<string> fileNames, string outputFolder)
        {
            foreach (var fileName in fileNames)
            {
                Out.Write($"Generating Z80 assembly file for {fileName}");
                using var source = File.OpenRead(fileName);
                using var reader = new BinaryReader(source);
                var font = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Spectrum.UK);
                var output = new StringBuilder();
                output.AppendFormat("\t; {0} font\n", Path.GetFileNameWithoutExtension(fileName));
                foreach (var glyph in font.Glyphs)
                {
                    output.AppendFormat("\t; {0}\n", glyph.Key);

                    for (int y = 0; y < font.Height; y++)
                    {
                        var b = new Byte();
                        var charWidth = glyph.Value.Width;
                        for (int x = 0; x < charWidth; x++)
                        {
                            if (glyph.Value.Data[x, y])
                                b |= (byte)(1 << charWidth - 1 - x);
                        }
                        string binary = "00000000" + System.Convert.ToString(b, 2);
                        output.AppendFormat("\tdefb %{0}\n", binary.Substring(binary.Length - 8, 8));
                    }
                }

                File.WriteAllText(Utils.MakeFileName(fileName, "z80.asm", outputFolder), output.ToString());
            }

            return fileNames.Count;
        }
    }
}
