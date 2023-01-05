﻿using System.Collections.Generic;
using System.Drawing;

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
            ColorTranslator.FromHtml("#071821"),
            ColorTranslator.FromHtml("#306850"),
            ColorTranslator.FromHtml("#86c06c"),
            ColorTranslator.FromHtml("#e0f8cf"),
        };
    }
}
