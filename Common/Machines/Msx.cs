using System;
using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Msx
{
    public static IReadOnlyDictionary<Int32, Char> International { get; } = (
        " !\"#$%&'()*+,-./0123456789:;<=>?" +
        "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
        "`abcdefghijklmnopqrstuvwxyz{|}~▵" +
        "ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒ" +
        "áíóúñÑªº¿⌐¬½¼¡«»ÃãĨĩÕõŨũĲĳ¾∽◊‰¶§" +
        "▂▚▆🮂▬🮅▎▞▊🮇🮊🮙🮘🭭🭯🭬🭮🮚🮛▘▗▝▖🮖Δ‡ω█▄▌▐▀" +
        "αßΓπΣσµτΦΘΩδ∞⌀∈∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■"
    ).ToIndexedDictionary();

    public static Int32 FontSize => 2048;

    public static String Extension => "fnt";
}