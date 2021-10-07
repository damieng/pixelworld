﻿using PixelWorld;
using PixelWorld.Machines;
using PixelWorld.Tools;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CommandLine.Commands
{
    [Description("Convert to Atari 8-bit binary font file")]
    public class ConvertToAtari8BitCommand : Command<BasicSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BasicSettings settings)
        {
            var files = Utils.MatchGlobWithFiles(settings.Glob);
            Converter.ConvertToAtari8(files, Spectrum.UK, settings.OutputFolder);
            return 0;
        }
    }
}