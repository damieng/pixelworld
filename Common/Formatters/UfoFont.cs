using PixelWorld.Fonts;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PixelWorld.Formatters;

public static class UfoFontFormatter
{
    private static readonly Dictionary<char, string> CharToName = new()
    {
        { ' ', "space" },
        { '!', "exclam" },
        { '"', "quotedbl" },
        { '#', "numbersign" },
        { '$', "dollar" },
        { '%', "percent" },
        { '&', "ampersand" },
        { '\'', "quotesingle" },
        { '(', "parenleft" },
        { ')', "parenright" },
        { '*', "asterisk" },
        { '+', "plus" },
        { ',', "comma" },
        { '-', "hyphen" },
        { '.', "period" },
        { '/', "slash" },
        { '0', "zero" },
        { '1', "one" },
        { '2', "two" },
        { '3', "three" },
        { '4', "four" },
        { '5', "five" },
        { '6', "six" },
        { '7', "seven" },
        { '8', "eight" },
        { '9', "nine" },
        { ':', "colon" },
        { ';', "semicolon" },
        { '<', "less" },
        { '=', "equal" },
        { '>', "greater" },
        { '?', "question" },
        { '@', "at" },
        { 'A', "A" },
        { 'B', "B" },
        { 'C', "C" },
        { 'D', "D" },
        { 'E', "E" },
        { 'F', "F" },
        { 'G', "G" },
        { 'H', "H" },
        { 'I', "I" },
        { 'J', "J" },
        { 'K', "K" },
        { 'L', "L" },
        { 'M', "M" },
        { 'N', "N" },
        { 'O', "O" },
        { 'P', "P" },
        { 'Q', "Q" },
        { 'R', "R" },
        { 'S', "S" },
        { 'T', "T" },
        { 'U', "U" },
        { 'V', "V" },
        { 'W', "W" },
        { 'X', "X" },
        { 'Y', "Y" },
        { 'Z', "Z" },
        { '[', "bracketleft" },
        { '\\', "backslash" },
        { ']', "bracketright" },
        { '^', "asciicircum" },
        { '_', "underscore" },
        { '`', "grave" },
        { 'a', "a" },
        { 'b', "b" },
        { 'c', "c" },
        { 'd', "d" },
        { 'e', "e" },
        { 'f', "f" },
        { 'g', "g" },
        { 'h', "h" },
        { 'i', "i" },
        { 'j', "j" },
        { 'k', "k" },
        { 'l', "l" },
        { 'm', "m" },
        { 'n', "n" },
        { 'o', "o" },
        { 'p', "p" },
        { 'q', "q" },
        { 'r', "r" },
        { 's', "s" },
        { 't', "t" },
        { 'u', "u" },
        { 'v', "v" },
        { 'w', "w" },
        { 'x', "x" },
        { 'y', "y" },
        { 'z', "z" },
        { '{', "braceleft" },
        { '|', "bar" },
        { '}', "braceright" },
        { '~', "asciitilde" },
        { '£', "sterling" },
        { '©', "copyright" }
    };

    private static readonly XmlWriterSettings XmlWriterSettings = new() { Indent = true, NewLineChars = "\n" };

    public static void Write(Font font, string path)
    {
        var basePath = Path.Join(path, "glyphs");
        Directory.CreateDirectory(basePath);

        var doc = new XmlDocument();

        var glyphElement = doc.CreateElement("glyph");
        doc.AppendChild(glyphElement);
        var nameAttribute = doc.CreateAttribute("name", "");
        glyphElement.Attributes.Append(nameAttribute);
        glyphElement.Attributes.Append(CreateAttribute("format", "2"));
        var advanceElement = doc.CreateElement("advance");
        var widthAttribute = CreateAttribute("width", "800");
        advanceElement.Attributes.Append(widthAttribute);
        glyphElement.AppendChild(advanceElement);
        var unicodeElement = doc.CreateElement("unicode");
        var hexAttribute = doc.CreateAttribute("hex", "");
        unicodeElement.Attributes.Append(hexAttribute);
        glyphElement.AppendChild(unicodeElement);
        var outlineElement = doc.CreateElement("outline");
        glyphElement.AppendChild(outlineElement);

        foreach (var (key, glyphData) in font.Glyphs)
        {
            var name = CharToName[key];
            nameAttribute.Value = name;
            hexAttribute.Value = ((int)key).ToString("X4");
            outlineElement.RemoveAll();

            for (var y = 0; y < glyphData.Height; y++)
            for (var x = 0; x < glyphData.Width; x++)
                if (glyphData.Data[x, glyphData.Height - y - 1])
                {
                    var pixel = CreatePixelComponent();
                    if (x > 0)
                        pixel.Attributes.Append(CreateAttribute("xOffset", (x * 100).ToString()));
                    pixel.Attributes.Append(CreateAttribute("yOffset", (y * 100).ToString()));
                    outlineElement.AppendChild(pixel);
                }

            WriteDoc(name);
        }

        return;

        void WriteDoc(string glyphName)
        {
            if (glyphName.Length == 1 && char.IsUpper(glyphName[0]))
                glyphName += "_";

            using var writer = XmlWriter.Create(Path.Join(basePath, glyphName + ".glif"), XmlWriterSettings);
            doc.WriteTo(writer);
        }

        XmlElement CreatePixelComponent()
        {
            var pixelComponent = doc.CreateElement("component");
            pixelComponent.Attributes.Append(CreateAttribute("base", "pixel"));
            return pixelComponent;
        }

        XmlAttribute CreateAttribute(string name, string value)
        {
            var attr = doc.CreateAttribute(name);
            attr.Value = value;
            return attr;
        }
    }
}