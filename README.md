# PixelWorld

Internal ZX Spectrum font conversion, ripping and manipulation tool made public!

This is a tool I have been using for a number of years to convert my own fonts into a variety of formats available from https://damieng.com/fonts/zx-origins/ 

It also contains bulk-ripping "dump" and "hunt" for bitmap fonts that currently supports a variety of strategies.

The code is in need of clean-up in various places, please bear with me.

## Commands

### File management

- `dedupe-title` to remove all duplicate files within a folder
- `org-title` to organize files into folders based on hunt output

### Ripping

- `dump` to RAM-dump the contents of files (currently supports `.z80` snapshots and recurses through `.zip` archives)
- `hunt` to look through RAM dumps for possible bitmap fonts (currently supports only ZX Spectrum fonts)

### Conversions

- `preview` to create a .png file

- `z80asmbinary` to create Zilog Z80 assembler source with `defb` binary
- `z80asmhex` to create Zilog Z80 assembler source with `defb` hex
- `x86asmhex` to create Intel 8086 assembler source with `db` hex
- `6502asmhex` to create MOS 6502 assembler source with `.byte` hex
- `68000asmhex` to create Motorola 68000 assembler source with `DB.B` hex

- `zxtofzx` to create a non-proportional `.fzx`
- `zxtofzxp` to create a proportional `.fzx` by auto left aligning and measuring widths
- `zxtocbm` to create `.c64` and `.bin` binary ROM versions for the Commodore 64
- `zxtoa8` to create a `.fnt` binary version for the Atari 8-bit series
- `zxtocpc` to create a `.bas` BASIC file for use with the Amstrad CPC range

## Hunt strategies

Finding bitmap fonts in a raw memory dump isn't trivial as there's no specific header however there are a few different strategies available in the code that can be combined and tweaked, they are:

### EnvironmentGuidedFinder

This relies on programmers using the ROM routines to print text to the screen and is surprisingly successful. It basically looks for a font at whatever RAM location is specified at memory locations 23606 & 23607.

### KnownCharPatternFinder

This looks for fonts based on well-known glyphs that designers rarely changed such as the copyright symbol and by using their known position in the font works out where the font is located.

### CandidatesInWindowFinder

This provides an array of potential glyphs without any knowledge of what they actually are. It requires you pass a certain number of glyphs and looks to find a minimum amount of them in a sequence in RAM.

This works in conjunction with `SpectrumDisplay.GetCandidates` to basically divide the current screen RAM into 8x8 blocks and gather up as many unique glyphs as it can. The principle here is that the font is probably used on-screen and is aligned to the usual text grid.

### GeneralHeuristicFinder

This finder looks for fonts by expecting certain pixel densities in relative to each other. For example ! should have less pixels than m. c should have less pixels than o etc.

## Setup

The project should compile with VS 2019 or later.

Some additional files are necessary for the conversions and should be placed in a `templates` folder.

- atari8.fnt - A dump of the Atari system font that includes all the symbols characters etc.
- c64-both.c64 - A dump of the Commodore 64 upper+lower case
- c64-upper.c64 - A dump of the Commodore 64 upper case + symbols
