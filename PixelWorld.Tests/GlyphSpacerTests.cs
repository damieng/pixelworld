using PixelWorld.Fonts;
using PixelWorld.Transformers;

namespace PixelWorld.Tests;

public class GlyphSpacerTests
{
    [Fact]
    public void Proportional_TrimsRightBlankColumns()
    {
        var data = new bool[8, 8];
        data[0, 0] = true;
        var glyph = new Glyph(8, 8, data);

        var result = GlyphSpacer.Proportional(glyph);

        Assert.Equal(1, result.Width);
    }

    [Fact]
    public void Proportional_WithMaxWidth_CapsWidth()
    {
        var data = new bool[8, 8];
        for (int x = 0; x < 8; x++) data[x, 3] = true;
        var glyph = new Glyph(8, 8, data);

        var result = GlyphSpacer.Proportional(glyph, maxWidth: 4);

        Assert.Equal(4, result.Width);
    }

    [Fact]
    public void Proportional_WithLeftPad_AddsLeftPadding()
    {
        var data = new bool[8, 8];
        data[7, 3] = true;
        var glyph = new Glyph(8, 8, data);

        var result = GlyphSpacer.Proportional(glyph, leftPad: 2);

        Assert.Equal(3, result.Width);
    }

    [Fact]
    public void Proportional_WithRightPad_AddsRightPadding()
    {
        var data = new bool[8, 8];
        data[0, 3] = true;
        var glyph = new Glyph(8, 8, data);

        var result = GlyphSpacer.Proportional(glyph, rightPad: 2);

        Assert.Equal(3, result.Width);
    }
}