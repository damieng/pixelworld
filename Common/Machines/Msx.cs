using System.Collections.Generic;

namespace PixelWorld.Machines;

public static class Msx
{
    public static IReadOnlyDictionary<int, char> International { get; } = (
        " !\"#$%&'()*+,-./0123456789:;<=>?" +
        "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
        "`abcdefghijklmnopqrstuvwxyz{|}~▵" +
        "ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒ" +
        "áíóúñÑªº¿⌐¬½¼¡«»ÃãĨĩÕõŨũĲĳ¾∽◊‰¶§" +
        "▂▚▆🮂▬🮅▎▞▊🮇🮊🮙🮘🭭🭯🭬🭮🮚🮛▘▗▝▖🮖Δ‡ω█▄▌▐▀" +
        "αßΓπΣσµτΦΘΩδ∞⌀∈∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■"
    ).ToIndexedDictionary();

    public static int FontSize { get; } = 2048;

    public static string Extension { get; } = "fnt";
}