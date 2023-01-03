using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public static class Gameboy
    {
        public static IReadOnlyDictionary<int, char> Studio { get; } = (
            " !\"#$%&'()*+,-./" +
            "0123456789:;<=>?" +
            "@ABCDEFGHIJKLMNO" +
            "PQRSTUVWXYZ[\\]^_" +
            "`abcdefghijklmno" +
            "pqrstuvwxyz{|}~ " +
            "€               " +
            "                " +
            "   £     ©      ").ToIndexedDictionary();
    }
}
