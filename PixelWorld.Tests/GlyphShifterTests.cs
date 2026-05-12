using PixelWorld.Fonts;
using PixelWorld.Transformers;

namespace PixelWorld.Tests;

public class GlyphShifterTests
{
    [Fact]
    public void Shift_NoChange_ReturnsSameData()
    {
        var data = new bool[8, 8];
        data[3, 3] = true;
        var glyph = new Glyph(8, 8, data);

        var shifted = GlyphShifter.Shift(glyph, 0, 0, false, 8, 8);

        Assert.True(shifted.Data[3, 3]);
    }

    [Fact]
    public void Shift_Horizontal_MovesCorrectly()
    {
        var data = new bool[8, 8];
        data[2, 3] = true;
        var glyph = new Glyph(8, 8, data);

        var shifted = GlyphShifter.Shift(glyph, -2, 0, false);

        Assert.True(shifted.Data[0, 3]);
    }

    [Fact]
    public void Shift_Vertical_MovesCorrectly()
    {
        var data = new bool[8, 8];
        data[3, 2] = true;
        var glyph = new Glyph(8, 8, data);

        var shifted = GlyphShifter.Shift(glyph, 0, -2, false);

        Assert.True(shifted.Data[3, 0]);
    }

    [Fact]
    public void Shift_WithWrap_WrapsAround()
    {
        var data = new bool[8, 8];
        data[0, 0] = true;
        var glyph = new Glyph(8, 8, data);

        var shifted = GlyphShifter.Shift(glyph, 0, 0, true, 8, 8);

        Assert.True(shifted.Data[0, 0]);
    }

    [Fact]
    public void Shift_NewWidth_ExpandsArray()
    {
        var data = new bool[5, 8];
        data[4, 3] = true;
        var glyph = new Glyph(5, 8, data);

        var shifted = GlyphShifter.Shift(glyph, 0, 0, false, 8, 8);

        Assert.Equal(8, shifted.Width);
        Assert.Equal(8, shifted.Height);
    }
}