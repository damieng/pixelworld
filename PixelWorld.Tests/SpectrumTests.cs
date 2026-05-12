using PixelWorld.Machines;

namespace PixelWorld.Tests;

public class SpectrumTests
{
    [Fact]
    public void UK_Charset_IsNotEmpty()
    {
        Assert.True(Spectrum.UK.Count > 0);
    }

    [Fact]
    public void UK_Charset_ContainsSpace()
    {
        Assert.Contains(' ', Spectrum.UK.Values);
    }

    [Fact]
    public void UK_Charset_ContainsUppercaseA()
    {
        Assert.Contains('A', Spectrum.UK.Values);
    }

    [Fact]
    public void UK_Charset_ContainsLowercaseA()
    {
        Assert.Contains('a', Spectrum.UK.Values);
    }

    [Fact]
    public void FontSize_Is768()
    {
        Assert.Equal(768, Spectrum.FontSize);
    }
}