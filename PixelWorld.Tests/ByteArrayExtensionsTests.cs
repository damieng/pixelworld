namespace PixelWorld.Tests;

public class ByteArrayExtensionsTests
{
    [Fact]
    public void IsEmpty_AllZeros_ReturnsTrue()
    {
        var buffer = new byte[16];
        Assert.True(buffer.IsEmpty(0, 8));
    }

    [Fact]
    public void IsEmpty_WithNonZero_ReturnsFalse()
    {
        var buffer = new byte[16];
        buffer[3] = 1;
        Assert.False(buffer.IsEmpty(0, 8));
    }

    [Fact]
    public void IsFull_All255_ReturnsTrue()
    {
        var buffer = new byte[16];
        for (int i = 0; i < 16; i++) buffer[i] = 255;
        Assert.True(buffer.IsFull(0, 8));
    }

    [Fact]
    public void IsFull_WithNon255_ReturnsFalse()
    {
        var buffer = new byte[16];
        for (int i = 0; i < 16; i++) buffer[i] = 255;
        buffer[3] = 0;
        Assert.False(buffer.IsFull(0, 8));
    }

    [Fact]
    public void IsSame_IdenticalBytes_ReturnsTrue()
    {
        var buffer = new byte[16];
        buffer[0] = 1; buffer[1] = 2; buffer[2] = 3;
        buffer[8] = 1; buffer[9] = 2; buffer[10] = 3;
        Assert.True(buffer.IsSame(0, 8));
    }

    [Fact]
    public void IsSame_DifferentBytes_ReturnsFalse()
    {
        var buffer = new byte[16];
        buffer[0] = 1; buffer[1] = 2; buffer[2] = 3;
        buffer[8] = 1; buffer[9] = 2; buffer[10] = 4;
        Assert.False(buffer.IsSame(0, 8));
    }

    [Fact]
    public void InvertBuffer_AllBitsFlipped()
    {
        var buffer = new byte[] { 0x00, 0xFF, 0xAA, 0x55 };
        buffer.InvertBuffer();
        Assert.Equal(0xFF, buffer[0]);
        Assert.Equal(0x00, buffer[1]);
        Assert.Equal(0x55, buffer[2]);
        Assert.Equal(0xAA, buffer[3]);
    }

    [Fact]
    public void ToHex_ConvertsCorrectly()
    {
        var buffer = new byte[] { 0xAB, 0xCD };
        var hex = buffer.ToHex();
        Assert.Equal("abcd", hex);
    }

    [Fact]
    public void PixelCount_CountsPixelsAtCorrectGlyphOffset()
    {
        var buffer = new byte[300];
        buffer[8] = 0b00001111; // '!' is char 33, glyph starts at (33-32)*8 = 8
        var count = buffer.PixelCount(0, '!');
        Assert.Equal(4, count);
    }
}