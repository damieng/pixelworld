using CommandLine.Commands;
using PixelWorld;
using Spectre.Console.Cli;
using System;
using CommandLine.Commands.Convert;
using CommandLine.Commands.Generate;

namespace CommandLine;

internal static class Program
{
    private static void Main(String[] args)
    {
        Out.Attach(Console.WriteLine);

        var app = new CommandApp();
        app.Configure(config =>
        {
            config.SetApplicationName("pw.exe");
            config.SetApplicationVersion(typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "Unknown");
            AddCommands(config);
        });

        app.Run(args);
    }

    private static void AddCommands(IConfigurator config)
    {
        config.AddCommand<DumpCommand>("dump");
        config.AddCommand<HuntCommand>("hunt");
        config.AddCommand<ExtractScreenTilesCommand>("extracttiles");

        config.AddCommand<ScreenshotCommand>("screenshot");
        config.AddCommand<PreviewCommand>("preview");
        config.AddCommand<ConvertFromPngCommand>("pngtozx");
        config.AddCommand<ConvertFromC64Command>("c64tozx");
        config.AddCommand<FindMatchingGlyphsCommand>("findmatches");

        config.AddCommand<GenerateZ80AssemblyCommand>("z80asm");
        config.AddCommand<Generate6502AssemblyCommand>("6502asm");
        config.AddCommand<Generate68000AssemblyCommand>("68000asm");
        config.AddCommand<Generate8086AssemblyCommand>("x86asm");
        config.AddCommand<GenerateCHeaderCommand>("chead");
        config.AddCommand<GenerateRustHeaderCommand>("rusthead");

        config.AddCommand<ConvertToFzxCommand>("zxtofzx");
        config.AddCommand<ConvertToC64Command>("zxtoc64");
        config.AddCommand<ConvertToMsxCommand>("zxtomsx");
        config.AddCommand<ConvertToCpcCommand>("zxtocpc");
        config.AddCommand<ConvertToAtari8BitCommand>("zxtoa8");
        config.AddCommand<ConvertToUfoCommand>("zxtoufo");
        config.AddCommand<ConvertToGbStudioCommand>("zxtogbs");
    }
}