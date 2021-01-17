using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public class Commodore64
    {
        public static IReadOnlyDictionary<int, char> bothUK =
            "@abcdefghijklmnopqrstuvwxyz[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?\0ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToIndexedDictionary();

        public static IReadOnlyDictionary<int, char> upperUK =
            "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[£]\0\0 !\"#$%&'()*+,-./0123456789:;<=>?"
            .ToIndexedDictionary();

        public static int Length = 1024;
    }
}
