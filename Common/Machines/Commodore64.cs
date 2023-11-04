using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Commodore64
{
    public static IReadOnlyDictionary<int, char> BothUK { get; } =
        "@abcdefghijklmnopqrstuvwxyz[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?\0ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToIndexedDictionary();

    public static IReadOnlyDictionary<int, char> UpperUK { get; } =
        "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?"
            .ToIndexedDictionary();

    public static int Length { get; } = 1024;
}