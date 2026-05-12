using PixelWorld.Fonts;
using PixelWorld.Transformers;

namespace PixelWorld.Tests;

public class FontInverterTests
{
    [Fact]
    public void Invert_FontWithGlyphs_AllInverted()
    {
        var font = new Font("Test", 8);
        var data = new bool[8, 8];
        data[0, 0] = true;
        font.Glyphs['A'] = new Glyph(8, 8, data);

        var inverted = FontInverter.Invert(font);

        Assert.False(inverted.Glyphs['A'].Data[0, 0]);
        Assert.True(inverted.Glyphs['A'].Data[1, 0]);
    }

    [Fact]
    public void Invert_OriginalUnchanged()
    {
        var font = new Font("Test", 8);
        var data = new bool[8, 8];
        data[0, 0] = true;
        font.Glyphs['A'] = new Glyph(8, 8, data);

        var inverted = FontInverter.Invert(font);

        Assert.True(font.Glyphs['A'].Data[0, 0]);
        Assert.False(inverted.Glyphs['A'].Data[0, 0]);
    }

    [Fact]
    public void Invert_MultipleGlyphs_AllAffected()
    {
        var font = new Font("Test", 8);
        font.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);
        font.Glyphs['B'] = new Glyph(8, 8, new bool[8, 8]);
        font.Glyphs['A'].Data[0, 0] = true;
        font.Glyphs['B'].Data[0, 0] = true;

        var inverted = FontInverter.Invert(font);

        Assert.False(inverted.Glyphs['A'].Data[0, 0]);
        Assert.False(inverted.Glyphs['B'].Data[0, 0]);
    }

    [Fact]
    public void Invert_NameAndHeightPreserved()
    {
        var font = new Font("MyFont", 8);
        font.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);

        var inverted = FontInverter.Invert(font);

        Assert.Equal("MyFont", inverted.Name);
        Assert.Equal(8, inverted.Height);
    }
}