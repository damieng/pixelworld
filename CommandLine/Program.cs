using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using PixelWorld;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using PixelWorld.Tools;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new StringBuilder();
            Out.Attach(Console.WriteLine);
            Out.Attach(s => log.AppendLine(s));
            Out.Write("PixelWorld command line tool v0.5");

            if (args.Length < 3)
            {
                ShowUsage();
                return;
            }

            ProcessCommand(args[0], args[1], args[2]);
        }

        static int ProcessCommand(string command, string inputMatch, string outputFolder)
        {
            var globSplitPoint = Utils.GetGlobSplitPoint(inputMatch);
            var glob = inputMatch[globSplitPoint..];
            var directory = globSplitPoint > 0 ? inputMatch.Substring(0, globSplitPoint) : ".";
            Out.Write($"Matching files {glob} in {directory}");

            var matcher = new Matcher(StringComparison.CurrentCultureIgnoreCase);
            matcher.AddInclude(glob);
            var matchResults = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directory)));
            var fileNames = matchResults.Files.Select(f => Path.Combine(directory, f.Path)).ToList();

            return command switch
            {
                "dedupe-title" => DedupePerTitle.Process(fileNames),
                "org-title" => OrganizeByTitle.Process(fileNames),
                "dump" => Dumper.Dump(fileNames, outputFolder),
                "hunt" => FontHunter.Hunt(fileNames, outputFolder),
                "preview" => PreviewCreator.Preview(fileNames, outputFolder),
                "z80asmhex" => AssemblyFontFormatter.GenAsmHex("z80", "defb ", "&{0:x2}", fileNames, outputFolder),
                "x86asmhex" => AssemblyFontFormatter.GenAsmHex("x86", "db\t", "0x{0:x2}", fileNames, outputFolder),
                "6502asmhex" => AssemblyFontFormatter.GenAsmHex("6502", ".byte ", "${0:x2}", fileNames, outputFolder),
                "68000asmhex" => AssemblyFontFormatter.GenAsmHex("68000", "DC.B ", "${0:x2}", fileNames, outputFolder),
                "z80asmbinary" => AssemblyFontFormatter.GenZ80AsmBinary(fileNames, outputFolder),
                "zxtofzx" => Converter.ConvertToFZX(fileNames, Spectrum.UK, false, outputFolder),
                "zxtofzxp" => Converter.ConvertToFZX(fileNames, Spectrum.UK, true, outputFolder),
                "zxtocbm" => Converter.ConvertToC64(fileNames, Spectrum.UK, outputFolder),
                "zxtoa8" => Converter.ConvertToAtari8(fileNames, Spectrum.UK, outputFolder),
                "zxtocpc" => Converter.ConvertToAmstradCPC(fileNames, Spectrum.UK, outputFolder),
                _ => throw new InvalidOperationException($"Unknown command {command}"),
            };
        }

        static void ShowUsage()
        {
            Out.Write("pw.exe <command> <filename/wildcard/glob> <outputFolder> [options]");
            Out.Write("  dump - produce memory dumps from zip/z80");
            Out.Write("  hunt - hunt dumps for possible fonts");
            Out.Write("  preview - generate a PNG preview for each font");
            Out.Write("  6502asmhex - generate a 6502 assembly def file for each font");
            Out.Write("  68000asmhex - generate a 68000 assembly def file for each font");
            Out.Write("  z80asmhex - generate a Z80 assembly def file for each font");
            Out.Write("  x86asmhex - generate an x86 assembly def file for each font");
            Out.Write("  z80asmbinary - generate a Z80 assembly def file for each font");
            Out.Write("  zxtofzx - generate a FZX file from a ZX file");
            Out.Write("  zxtofzxp - generate a FZX proportional file from a ZX file");
            Out.Write("  dedupe-title - purge duplicate fonts in the same title");
            Out.Write("  org-title - move fonts from the same title into a subfolder");
            Out.Write("  zxtocbm - convert Spectrum RAW to Commodore RAW");
            Out.Write("  zxtoa8 - convert Spectrum RAW to Atari 8-bit");
            Out.Write("  zxtocpc - convert Spectrum RAW to Amstrad CPC Basic SYMBOL");
        }
    }
}
