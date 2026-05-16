using PixelWorld.Fonts;

namespace PixelWorld.Tests;

public class GlyphTests
{
    [Fact]
    public void IsBlank_EmptyGlyph_ReturnsTrue()
    {
        var data = new bool[8, 8];
        var glyph = new Glyph(8, 8, data);
        Assert.True(glyph.IsBlank());
    }

    [Fact]
    public void IsBlank_GlyphWithPixels_ReturnsFalse()
    {
        var data = new bool[8, 8];
        data[0, 0] = true;
        var glyph = new Glyph(8, 8, data);
        Assert.False(glyph.IsBlank());
    }

    [Fact]
    public void IsRowBlank_EmptyRow_ReturnsTrue()
    {
        var data = new bool[8, 8];
        var glyph = new Glyph(8, 8, data);
        Assert.True(glyph.IsRowBlank(0));
        Assert.True(glyph.IsRowBlank(7));
    }

    [Fact]
    public void IsRowBlank_RowWithPixels_ReturnsFalse()
    {
        var data = new bool[8, 8];
        data[0, 2] = true;
        var glyph = new Glyph(8, 8, data);
        Assert.False(glyph.IsRowBlank(2));
    }

    [Fact]
    public void IsColumnBlank_EmptyColumn_ReturnsTrue()
    {
        var data = new bool[8, 8];
        var glyph = new Glyph(8, 8, data);
        Assert.True(glyph.IsColumnBlank(0));
        Assert.True(glyph.IsColumnBlank(7));
    }

    [Fact]
    public void IsColumnBlank_ColumnWithPixels_ReturnsFalse()
    {
        var data = new bool[8, 8];
        data[3, 0] = true;
        var glyph = new Glyph(8, 8, data);
        Assert.False(glyph.IsColumnBlank(3));
    }

    [Fact]
    public void Glyph_Equals_SameData_ReturnsTrue()
    {
        var data1 = new bool[8, 8];
        data1[1, 1] = true;
        var data2 = new bool[8, 8];
        data2[1, 1] = true;
        var glyph1 = new Glyph(8, 8, data1);
        var glyph2 = new Glyph(8, 8, data2);
        Assert.Equal(glyph1, glyph2);
    }

    [Fact]
    public void Glyph_Equals_DifferentData_ReturnsFalse()
    {
        var data1 = new bool[8, 8];
        data1[1, 1] = true;
        var data2 = new bool[8, 8];
        data2[1, 2] = true;
        var glyph1 = new Glyph(8, 8, data1);
        var glyph2 = new Glyph(8, 8, data2);
        Assert.NotEqual(glyph1, glyph2);
    }

    [Fact]
    public void Glyph_WidthHeight_StoredCorrectly()
    {
        var data = new bool[5, 8];
        var glyph = new Glyph(5, 8, data);
        Assert.Equal(5, glyph.Width);
        Assert.Equal(8, glyph.Height);
    }

    [Fact]
    public void GetRowByte_EncodesLeftmostPixelAsBit7()
    {
        var data = new bool[8, 8];
        data[0, 0] = true;
        data[7, 0] = true;
        var glyph = new Glyph(8, 8, data);
        Assert.Equal(0b10000001, glyph.GetRowByte(0));
    }

    [Fact]
    public void GetRowByte_EmptyRow_ReturnsZero()
    {
        var glyph = new Glyph(8, 8, new bool[8, 8]);
        Assert.Equal(0, glyph.GetRowByte(0));
    }

    [Fact]
    public void GetRowByte_NarrowGlyph_OnlySetsLeftBits()
    {
        var data = new bool[5, 8];
        data[0, 0] = true;
        data[4, 0] = true;
        var glyph = new Glyph(5, 8, data);
        Assert.Equal(0b10001000, glyph.GetRowByte(0));
    }
}