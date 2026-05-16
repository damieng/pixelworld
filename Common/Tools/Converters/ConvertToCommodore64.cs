using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.Tools.Converters;

public static class ConvertToCommodore64
{
    public static void Convert(List<String> fileNames, IReadOnlyDictionary<Int32, Char> sourceCharset, String outputFolder, String templatePath)
    {
        var bothCaseTemplate = Path.Combine(templatePath, "c64-both.ch8");
        var upperCaseTemplate = Path.Combine(templatePath, "c64-upper.ch8");
        Out.Write($"Using templates {bothCaseTemplate} and {upperCaseTemplate}");

        var cases = new[]
        {
            (
                template: File.ReadAllBytes(bothCaseTemplate),
                charset: Commodore64.BothUK,
                suffix: "both"
            ),
            (
                template: File.ReadAllBytes(upperCaseTemplate),
                charset: Commodore64.UpperUK,
                suffix: "upper"
            )
        };

        foreach (var sourceFileName in fileNames)
        {
            var sourceFont = ByteFontFormatter.Load(sourceFileName, sourceCharset);
            using var characterRom = File.Create(Utils.MakeFileName(sourceFileName, "bin", outputFolder));

            foreach (var (template, charset, suffix) in cases)
            {
                var targetFileName = Utils.MakeFileName(sourceFileName, suffix + ".64c", outputFolder);

                Out.Write($"Converting file {sourceFileName} to {targetFileName}");

                using var memoryStream = new MemoryStream();
                ByteFontFormatter.Write(sourceFont, memoryStream, charset, 128, i => new ArraySegment<Byte>(template, i, 8));

                using var targetFile = File.Create(targetFileName);
                targetFile.Write("\08"u8); // 64C header
                memoryStream.WriteTo(targetFile);
                memoryStream.WriteTo(characterRom);

                memoryStream.GetBuffer().InvertBuffer();

                memoryStream.WriteTo(targetFile);
                memoryStream.WriteTo(characterRom);
            }
        }
    }
}
