using PixelWorld.Display;

namespace PixelWorld.Tests;

public class SpectrumDisplayTests
{
    [Fact]
    public void IsBlank_AllZeros_ReturnsTrue()
    {
        var buffer = new byte[2048];
        Assert.True(SpectrumDisplay.IsBlank(buffer, 0));
    }

    [Fact]
    public void IsBlank_WithNonZero_ReturnsFalse()
    {
        var buffer = new byte[2048];
        buffer[100] = 1;
        Assert.False(SpectrumDisplay.IsBlank(buffer, 0));
    }

    [Fact]
    public void IsBlank_Offset_ChecksCorrectRegion()
    {
        var buffer = new byte[4096];
        buffer[768] = 255;
        Assert.True(SpectrumDisplay.IsBlank(buffer, 0));
        Assert.False(SpectrumDisplay.IsBlank(buffer, 768));
    }

    [Fact]
    public void GetCandidates_LargeEnoughBuffer_ReturnsCandidates()
    {
        var buffer = new byte[6144];
        for (int i = 0; i < 2048; i++) buffer[i] = (byte)(i % 256);

        var candidates = SpectrumDisplay.GetCandidates(buffer, 0);

        Assert.NotEmpty(candidates);
        foreach (var candidate in candidates)
            Assert.Equal(8, candidate.Length);
    }

    [Fact]
    public void GetCandidates_ExcludesEmptyAndFull()
    {
        var buffer = new byte[6144];
        for (int i = 0; i < 2048; i++) buffer[i] = (byte)(i % 256);

        var candidates = SpectrumDisplay.GetCandidates(buffer, 0);

        foreach (var candidate in candidates)
        {
            var isEmpty = candidate.All(b => b == 0);
            var isFull = candidate.All(b => b == 255);
            Assert.False(isEmpty || isFull);
        }
    }
}