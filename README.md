# PixelWorld

ZX Spectrum font conversion, ripping and manipulation tool for 8x8 bitmap fonts.

This is a tool I have been using for a number of years to convert my own fonts into a variety of formats available from https://damieng.com/fonts/zx-origins/ 

It also contains bulk-ripping `dump` and `hunt` commands for obtaining ZX Spectrum RAW 768 byte/.ch8 files from memory dumps and snapshots.

## Commands

### Ripping

- `dump` to RAM-dump snapshot files (currently supports `.z80` and `.sna` and recurses through `.zip` archives)
- `hunt` to look through RAM dumps for possible bitmap fonts (currently supports only ZX Spectrum fonts)
- `screenshot` to create a screenshots from RAM-dumps or snapshot files (in PNG, SCR or animated GIF format)
- `extracttiles` to create a files with unique 8x8 character tiles found on the screen in RAM-dumps or snapshot files

### Conversions

The following commands work with ZX Spectrum RAW 768 byte/.ch8 files:

- `preview` to create a `.png` preview file
- `pngtozx` to convert a `.png` back to a ZX Spectrum `.ch8`
- `c64tozx` to create a ZX Spectrum `.ch8` file from a C64 binary file

Any of the following assembly generating commands can use the `--base hex|decimal` flags. The z80 one can also use `--base binary`.

- `z80asm` to create Zilog Z80 assembler source with `defb` hex
- `x86asm` to create Intel 8086 assembler source with `db` hex
- `6502asm` to create MOS 6502 assembler source with `.byte` hex
- `68000asm` to create Motorola 68000 assembler source with `DB.B` hex

And the header-generating commands:

- `chead` to generate C-compatible header files
- `rusthead` to generate Rust-compatible header files

You can also generate directly-usable files (all binary except the CPC)

- `zxtofzx` to create a fixed-width `.fzx` 
- `zxtofzx --proportional` to create a proportional `.fzx` by auto left aligning and measuring widths
- `zxtocbm` to create `.c64` and `.bin` binary ROM versions for the Commodore 64
- `zxtoa8` to create a `.fnt` binary version for the Atari 8-bit series
- `zxtocpc` to create a `.bas` BASIC file for use with the Amstrad CPC range

### Comparisons

- `findmatches` to find glyphs from a source font in as many possible target .ch8 files

Any command can be further detailed using the `--help` option which will detail the additional flags and options available.

## Hunt strategies

Finding bitmap fonts in a raw memory dump isn't trivial as there's no specific header however there are a few different strategies available in the code that can be combined and tweaked, they are:

### [EnvironmentGuidedFinder](https://github.com/damieng/pixelworld/blob/main/Common/OffsetFinders/EnviromentGuidedFinder.cs)

This relies on programmers using the ROM routines to print text to the screen and is surprisingly successful. It basically looks for a font at whatever RAM location is specified at memory locations 23606 & 23607. It can only find one font however so games like Millionaire that use several get missed.

### [KnownCharPatternFinder](https://github.com/damieng/pixelworld/blob/main/Common/OffsetFinders/KnownCharPatternFinder.cs)

This looks for fonts based on well-known glyphs that designers rarely changed such as the copyright symbol and by using their known position in the font works out where the font is located.

### [CandidatesInWindowFinder](https://github.com/damieng/pixelworld/blob/main/Common/OffsetFinders/CandidatesInWindowFinder.cs)

This provides an array of potential glyphs without any knowledge of what they actually are. It requires you pass a certain number of glyphs and looks to find a minimum amount of them in a sequence in RAM.

This works in conjunction with [SpectrumDisplay.GetCandidates](https://github.com/damieng/pixelworld/blob/main/Common/Display/SpectrumDisplay.cs#L69) to divide the current screen memory into as many unique 8x8 character blocks as it can. The principle here is that the font is probably used on-screen and is aligned to the usual (32x24) text grid.

### [GeneralHeuristicFinder](https://github.com/damieng/pixelworld/blob/main/Common/OffsetFinders/GeneralHeuristicFinder.cs)

This finder looks for fonts by expecting certain pixel densities in relative to each other. For example ! should have less pixels than m. c should have less pixels than o etc.

## Setup

The project should compile with VS 2019 or later.

Some [template files are necessary for the conversions](https://github.com/damieng/pixelworld/discussions/14) and should have their location specified by the `--templatePath` parameter.

- `atari8.fnt` - A dump of the Atari system font that includes all the symbols characters etc.
- `c64-both.c64` - A dump of the Commodore 64 upper+lower case
- `c64-upper.c64` - A dump of the Commodore 64 upper case + symbols

## Example usage

Various different usages of this command can be fount in my [ZX Origins font-publishing script](https://gist.github.com/damieng/d2519cda1c674b4ede74f154f05f2431) - every reference there to `pw.exe` is this.
