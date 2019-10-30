using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public class Atari8
    {
        public static IReadOnlyDictionary<int, char> US = (
            " !\"#$%&'()*+,-./0123456789:;<=>?" +
            "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
            "abcdefghijklmnopqrstuvwxyz{|}~©"
        ).ToIndexedDictionary();

        public static int FontSize = 1024;

        public static string Extension = "fnt";
    }
}
