using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CommandLine.Commands.Settings;
using PixelWorld;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using Spectre.Console.Cli;

namespace CommandLine.Commands.Convert;

[Description("Convert from Commodore 64 binary font format")]
public class ConvertFromC64Command : Command<RequiredSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] RequiredSettings settings)
    {
        foreach (var fileName in Utils.MatchGlobWithFiles(settings.Glob))
            ConvertFromC64(settings, fileName);

        return 0;
    }

    private static void ConvertFromC64(RequiredSettings settings, string fileName)
    {
        Out.Write($"Generating ch8 file from {fileName}");

        using var source = File.OpenRead(fileName);

        using var reader = new BinaryReader(source);
        source.Seek(2, SeekOrigin.Current); // Skip header

        var sourceFont = ByteFontFormatter.Create(reader, Path.GetFileNameWithoutExtension(fileName), 0, Commodore64.BothUK);

        using var output = File.Create(Utils.MakeFileName(fileName, "ch8", settings.OutputFolder));
        ByteFontFormatter.Write(sourceFont, output, Commodore64.BothUK, 96);
    }
}