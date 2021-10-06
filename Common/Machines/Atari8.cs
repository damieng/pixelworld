using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public class Atari8
    {
        public static IReadOnlyDictionary<int, char> US {get; } = (
            " !\"#$%&'()*+,-./0123456789:;<=>?" +
            "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
            "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0" +
            "\0abcdefghijklmnopqrstuvwxyz"
        ).ToIndexedDictionary();

        public static int FontSize { get; }  = 1024;

        public static string Extension { get; } = "fnt";
    }
}
