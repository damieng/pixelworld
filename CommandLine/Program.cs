﻿using CommandLine.Commands;
using Spectre.Console.Cli;

namespace CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.SetApplicationName("PixelWorld");
                config.SetApplicationVersion("0.7");

                config.AddCommand<DumpCommand>("dump");
                config.AddCommand<HuntCommand>("hunt");

                config.AddCommand<PreviewCommand>("preview");

                config.AddCommand<Z80AsmCommand>("z80asm");
                config.AddCommand<M6502AsmCommand>("6502asm");
                config.AddCommand<M68000AsmCommand>("68000asm");
                config.AddCommand<X86AsmCommand>("x86asm");

                config.AddCommand<ConvertToFZXCommand>("zxtofzx");
                config.AddCommand<ConvertToC64Command>("zxtoc64");
                config.AddCommand<ConvertToCPCCommand>("zxtocpc");
                config.AddCommand<ConvertToCPCCommand>("zxtoa8");
            });

            app.Run(args);
        }
    }
}
