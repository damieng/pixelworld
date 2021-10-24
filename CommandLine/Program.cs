using CommandLine.Commands;
using PixelWorld;
using Spectre.Console.Cli;
using System;

namespace CommandLine
{
    static class Program
    {
        static void Main(string[] args)
        {
            Out.Attach(Console.WriteLine);

            var app = new CommandApp();
            app.Configure(config =>
            {
                config.SetApplicationName("pw.exe");
                config.SetApplicationVersion("0.7");

                config.AddCommand<DumpCommand>("dump");
                config.AddCommand<HuntCommand>("hunt");

                config.AddCommand<ScreenshotCommand>("screenshot");
                config.AddCommand<PreviewCommand>("preview");

                config.AddCommand<Z80AssemblyCommand>("z80asm");
                config.AddCommand<Generate6502AssemblyCommand>("6502asm");
                config.AddCommand<Generate68000AssemblyCommand>("68000asm");
                config.AddCommand<Generate8086AssemblyCommand>("x86asm");

                config.AddCommand<ConvertToFZXCommand>("zxtofzx");
                config.AddCommand<ConvertToC64Command>("zxtoc64");
                config.AddCommand<ConvertToCPCCommand>("zxtocpc");
                config.AddCommand<ConvertToAtari8BitCommand>("zxtoa8");
            });

            app.Run(args);
        }
    }
}
