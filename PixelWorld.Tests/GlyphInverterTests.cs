using PixelWorld.Fonts;
using PixelWorld.Transformers;

namespace PixelWorld.Tests;

public class GlyphInverterTests
{
    [Fact]
    public void Invert_BooleanArray_AllBitsFlipped()
    {
        var source = new bool[8, 8];
        source[0, 0] = true;
        source[7, 7] = true;

        var result = GlyphInverter.Invert(source);

        Assert.False(result[0, 0]);
        Assert.False(result[7, 7]);
        Assert.True(result[1, 0]);
    }

    [Fact]
    public void Invert_Glyph_DataInverted()
    {
        var data = new bool[8, 8];
        data[0, 0] = true;
        var glyph = new Glyph(8, 8, data);

        var inverted = GlyphInverter.Invert(glyph);

        Assert.False(inverted.Data[0, 0]);
        Assert.True(inverted.Data[1, 0]);
        Assert.Equal(8, inverted.Width);
        Assert.Equal(8, inverted.Height);
    }

    [Fact]
    public void Invert_RectangularArray_CorrectDimensions()
    {
        var source = new bool[5, 8];

        var result = GlyphInverter.Invert(source);

        Assert.Equal(5, result.GetLength(0));
        Assert.Equal(8, result.GetLength(1));
    }

    [Fact]
    public void Invert_PartiallyFilled_CorrectlyInverts()
    {
        var source = new bool[8, 8];
        source[2, 3] = true;

        var result = GlyphInverter.Invert(source);

        Assert.False(result[2, 3]);
        Assert.True(result[0, 0]);
    }
}