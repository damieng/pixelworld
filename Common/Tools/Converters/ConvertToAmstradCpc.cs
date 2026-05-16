using PixelWorld.Fonts;
using PixelWorld.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PixelWorld.Tools.Converters;

public static class ConvertToAmstradCpc
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String credit, Int32 startLine)
    {
        foreach (var sourceFileName in fileNames)
        {
            var targetFileName = Utils.MakeFileName(sourceFileName, "bas", outputFolder);

            var line = startLine;

            Out.Write($"Converting file {sourceFileName} to {targetFileName}");

            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            var output = new StringBuilder();

            output.Append($"{line} REM {Path.GetFileNameWithoutExtension(sourceFileName)} font\r\n");
            if (!string.IsNullOrEmpty(credit)) output.Append($"{line += 10} REM {credit}\r\n");

            var spaceIsBlank = sourceFont.Glyphs[' '].IsBlank();
            output.Append($"{line += 10} SYMBOL AFTER {(spaceIsBlank ? 33 : 32)}\r\n");

            foreach (var (key, value) in sourceFont.Glyphs.Where(g => !g.Value.IsBlank()).OrderBy(g => g.Key))
            {
                switch (key)
                {
                    case '©':
                        WriteSymbolLine(164, value);
                        break;
                    default:
                        WriteSymbolLine(key, value);
                        break;
                }
            }

            File.WriteAllText(targetFileName, output.ToString());

            void WriteSymbolLine(Int32 charIdx, Glyph glyph)
            {
                output.Append($"{line += 10} SYMBOL {charIdx},{string.Join(',', MakeList(glyph))}\r\n");
            }
        }

        static Int32[] MakeList(Glyph glyph)
        {
            var results = new Int32[8];
            for (var y = 0; y < 8; y++)
                results[y] = glyph.GetRowByte(y);
            return results;
        }
    }
}
