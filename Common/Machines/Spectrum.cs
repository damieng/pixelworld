using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Spectrum
{
    public static IReadOnlyDictionary<int, char> UK { get; } =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_£abcdefghijklmnopqrstuvwxyz{|}~©"
            .ToIndexedDictionary();

    public static int FontSize => 768;
}