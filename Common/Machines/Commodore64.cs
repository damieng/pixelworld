using System;
using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Commodore64
{
    public static IReadOnlyDictionary<Int32, Char> BothUK { get; } =
        "@abcdefghijklmnopqrstuvwxyz[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?\0ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToIndexedDictionary();

    public static IReadOnlyDictionary<Int32, Char> UpperUK { get; } =
        "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?"
            .ToIndexedDictionary();

    public static Int32 Length => 1024;
}