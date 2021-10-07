﻿using PixelWorld.DumpScanners;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PixelWorld.Tools
{
    public static class FontHunter
    {
        public static int Hunt(IEnumerable<string> fileNames, string outputFolder)
        {
            int inputsWithOutputsCount = 0;
            int outputCount = 0;

            var inputCount = fileNames.Count();
            Out.Write($"\nHunting {inputCount} files");

            foreach (var fileName in fileNames)
            {
                Out.Write($"Opening file {fileName}");
                var matchOutputs = ExtractFontFromDumpFile(fileName, outputFolder);
                if (matchOutputs > 0)
                {
                    inputsWithOutputsCount++;
                    outputCount += matchOutputs;
                }
            }

            Out.Write($"{inputsWithOutputsCount} files yielded {outputCount} results");
            Out.Write($"{Math.Floor((double)inputsWithOutputsCount / inputCount * 100)}% success rate");

            return 0;
        }

        static int ExtractFontFromDumpFile(string fileName, string outputFolder)
        {
            using var source = File.OpenRead(fileName);
            return ExtractFontFromMemoryBuffer(fileName, new ArraySegment<byte>(source.ReadAllBytes()), outputFolder);
        }

        private static int ExtractFontFromMemoryBuffer(string fileName, ArraySegment<byte> dump, string outputFolder)
        {
            using var memory = new MemoryStream(dump.Array);
            using var reader = new BinaryReader(memory);
            var fontIndex = 0;
            foreach (var font in SpectrumDumpScanner.Read(reader, Path.GetFileNameWithoutExtension(fileName)))
            {
                var newFileName = Utils.MakeFileName(font.Name, "ch8", outputFolder);
                fontIndex++;
                Out.Write($"  Creating byte font {newFileName}");
                ByteFontFormatter.Write(font, File.Create(newFileName), Spectrum.UK, 96);
            }

            return fontIndex;
        }
    }
}
