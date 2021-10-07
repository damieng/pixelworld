﻿using PixelWorld;
using PixelWorld.Formatters;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Generate Z80 assembly def files")]
    public class Z80AsmCommand : Command<AssemblerSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] AssemblerSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            switch (settings.Base)
            {
                case NumberBase.Binary:
                    AssemblyFontFormatter.GenZ80AsmBinary(files, settings.OutputFolder);
                    break;
                case NumberBase.Decimal:
                    AssemblyFontFormatter.GenAsmHex("z80", "defb ", "{0}", files, settings.OutputFolder);
                    break;
                default:
                    AssemblyFontFormatter.GenAsmHex("z80", "defb ", "&{0:x2}", files, settings.OutputFolder);
                    break;
            }
            return 0;
        }
    }
}