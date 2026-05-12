using PixelWorld.Fonts;

namespace PixelWorld.Tests;

public class FontTests
{
    [Fact]
    public void Font_Creation_HasCorrectProperties()
    {
        var font = new Font("TestFont", 8);

        Assert.Equal("TestFont", font.Name);
        Assert.Equal(8, font.Height);
        Assert.Empty(font.Glyphs);
    }

    [Fact]
    public void Copy_CreatesIndependentFont()
    {
        var font = new Font("Test", 8);
        font.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);

        var copy = font.Copy();

        Assert.Equal(font.Name, copy.Name);
        Assert.Equal(font.Height, copy.Height);
        Assert.Equal(font.Glyphs.Count, copy.Glyphs.Count);
        copy.Glyphs['B'] = new Glyph(8, 8, new bool[8, 8]);
        Assert.False(font.Glyphs.ContainsKey('B'));
    }

    [Fact]
    public void Font_Equals_SameGlyphs_ReturnsTrue()
    {
        var font1 = new Font("Test", 8);
        font1.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);

        var font2 = new Font("Test", 8);
        font2.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);

        Assert.Equal(font1, font2);
    }

    [Fact]
    public void Font_Equals_DifferentGlyphs_ReturnsFalse()
    {
        var font1 = new Font("Test", 8);
        font1.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);

        var font2 = new Font("Test", 8);
        font2.Glyphs['A'] = new Glyph(8, 8, new bool[8, 8]);
        font2.Glyphs['A'].Data[0, 0] = true;

        Assert.NotEqual(font1, font2);
    }
}