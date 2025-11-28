using System;
using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Spectrum
{
    public static IReadOnlyDictionary<Int32, Char> UK { get; } =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_£abcdefghijklmnopqrstuvwxyz{|}~©"
            .ToIndexedDictionary();

    public static Int32 FontSize => 768;
}