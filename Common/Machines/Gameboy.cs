using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace PixelWorld.Machines;

public static class Gameboy
{
    public static IReadOnlyDictionary<int, char> Studio { get; } = (
        " !\"#$%&'()*+,-./" +
        "0123456789:;<=>?" +
        "@ABCDEFGHIJKLMNO" +
        "PQRSTUVWXYZ[\\]^_" +
        "`abcdefghijklmno" +
        "pqrstuvwxyz{|}~ " +
        "€▒‚ƒ„…†‡ˆ‰Š‹Œ▒Ž▒" +
        "▒‘’“”•–—˜™š›œ▒žŸ" +
        "▒¡¢£¤¥¦§¨©ª«¬▒®¯" +
        "°±²³´µ¶·¸¹º»¼½¾¿" +
        "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏ" +
        "ÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞß" +
        "àáâãäåæçèéêëìíîï" +
        "ðñòóôõö÷øùúûüýþÿ").ToIndexedDictionary();

    public static readonly Color[] Palette =
    {
        Color.ParseHex("#071821"),
        Color.ParseHex("#306850"),
        Color.ParseHex("#86c06c"),
        Color.ParseHex("#e0f8cf"),
    };
}