using System;
using System.Collections.Generic;
using System.IO;
using PixelWorld.Display;

namespace PixelWorld.Tools;

public class TileExtractor
{
    public static void Extract(List<String> fileNames, String outputFolder, Int32 minTiles, Int32 maxTiles)
    {
        var inputsWithOutputsCount = 0;
        var outputCount = 0;

        var inputCount = fileNames.Count;
        Out.Write($"Extracting screen tiles from {inputCount} files");

        foreach (var fileName in fileNames)
        {
            Out.Write($"Opening file {fileName}");

            using var source = File.OpenRead(fileName);
            using var reader = new BinaryReader(source);
            var buffer = reader.ReadBytes(1024 * 2048);
            var address = buffer.Length == 65536 ? 16384 : 0;
            var tiles = SpectrumDisplay.GetCandidates(buffer, address);

            if (tiles.Length > maxTiles)
                Out.Write($"  Skipping {tiles.Length} tiles as greater than {maxTiles} maxTiles setting");
            else if (tiles.Length < minTiles)
                Out.Write($"  Skipping {tiles.Length} tiles as less than {minTiles} minTiles setting");
            else
            {
                var newFileName = Utils.MakeFileName(Path.GetFileNameWithoutExtension(fileName), "tiles", outputFolder);
                Out.Write($"  Creating {tiles.Length} tiles in {newFileName}");
                using var target = File.Create(newFileName);
                foreach (var tile in tiles)
                foreach (var row in tile)
                    target.WriteByte(row);
                inputsWithOutputsCount++;
                outputCount += tiles.Length;
            }
        }

        Out.Write($"{inputsWithOutputsCount} files yielded {outputCount} tiles");
        Out.Write($"{Math.Floor((Double)inputsWithOutputsCount / inputCount * 100)}% success rate");
    }
}