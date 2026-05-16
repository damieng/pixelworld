using PixelWorld.Fonts;
using PixelWorld.Transformers;

namespace PixelWorld.Tests;

public class FontSpacerTests
{
    [Fact]
    public void MakeProportional_MissingBraceChar_UsesFallbackWidth()
    {
        var font = new Font("Test", 8);
        var data = new bool[8, 8];
        data[3, 3] = true;
        font.Glyphs['A'] = new Glyph(8, 8, data);
        // ' ' is required by MakeProportional
        font.Glyphs[' '] = new Glyph(8, 8, new bool[8, 8]);

        // Should not throw even though '{' is missing
        var result = FontSpacer.MakeProportional(font, 0, 0, 8);

        Assert.True(result.Glyphs.ContainsKey(' '));
        Assert.Equal(8, result.Glyphs[' '].Width);
    }

    [Fact]
    public void MakeProportional_WithBraceChar_UsesBraceWidth()
    {
        var font = new Font("Test", 8);
        // Make a wide 'A' and a full-width '{' so its width stays after proportionalization
        var wideData = new bool[8, 8];
        for (var x = 0; x < 8; x++)
            wideData[x, 3] = true;
        font.Glyphs['A'] = new Glyph(8, 8, wideData);
        // Full-width brace (all columns filled)
        font.Glyphs['{'] = new Glyph(8, 8, wideData);
        font.Glyphs[' '] = new Glyph(8, 8, new bool[8, 8]);

        var result = FontSpacer.MakeProportional(font, 0, 0, 8);

        Assert.True(result.Glyphs.ContainsKey(' '));
        Assert.Equal(8, result.Glyphs[' '].Width);
    }
}
