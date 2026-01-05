using System;
using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Atari8
{
    public static IReadOnlyDictionary<Int32, Char> US {get; } = (
        " !\"#$%&'()*+,-./0123456789:;<=>?" +
        "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
        "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0" +
        "\0abcdefghijklmnopqrstuvwxyz"
    ).ToIndexedDictionary();

    public static Int32 FontSize => 1024;

    public static String Extension => "fnt";
}