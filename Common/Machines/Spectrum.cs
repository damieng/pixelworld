using System.Collections.Generic;

namespace PixelWorld.Machines
{
    public class Spectrum
    {
        public static IReadOnlyDictionary<int, char> UK = 
            " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_£abcdefghijklmnopqrstuvwxyz{|}~©"
            .ToIndexedDictionary();

        public static int FontSize = 768;
    }
}
