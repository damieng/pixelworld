using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public class Commodore64
    {
        public static IReadOnlyDictionary<int, char> UK =
            "@abcdefghijklmnopqrstuvwxyz[£]^  !\"#$%&'()*+,-./0123456789:;<=>? ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToIndexedDictionary();
        // TODO: Flesh out with box-drawing characters?
    }
}
