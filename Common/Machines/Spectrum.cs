using System;
using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Spectrum
{
    public static IReadOnlyDictionary<Int32, Char> UK { get; } =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_£abcdefghijklmnopqrstuvwxyz{|}~©\u007f\u0080\u0081"
            .ToIndexedDictionary();

    public static Int32 FontSize => 768;

    public const Int32 ScreenStart = 16384;
    public const Int32 ScreenSize = 6912;
    public const Int32 CharsSysVar = 23606;
    public const Int32 Ram48K = 49152;
    public const Int32 AddressSpace = 65536;
}