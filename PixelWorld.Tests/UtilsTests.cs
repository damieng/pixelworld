using System.IO;

namespace PixelWorld.Tests;

public class UtilsTests
{
    [Fact]
    public void ReadAllBytes_ReturnsExactBytes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);

        var result = stream.ReadAllBytes();

        Assert.Equal(5, result.Length);
        Assert.Equal(data, result);
    }

    [Fact]
    public void ReadAllBytes_EmptyStream_ReturnsEmpty()
    {
        using var stream = new MemoryStream();

        var result = stream.ReadAllBytes();

        Assert.Empty(result);
    }

    [Fact]
    public void ToIndexedDictionary_BasicString_ReturnsCorrectMapping()
    {
        var str = "ABZ";
        var dict = str.ToIndexedDictionary();

        Assert.Equal(3, dict.Count);
        Assert.Equal('A', dict[0]);
        Assert.Equal('B', dict[1]);
        Assert.Equal('Z', dict[2]);
    }

    [Fact]
    public void ToIndexedDictionary_IgnoresNullChar()
    {
        var str = "A\0B";
        var dict = str.ToIndexedDictionary();

        Assert.Equal(2, dict.Count);
        Assert.Equal('A', dict[0]);
        Assert.Equal('B', dict[2]);
    }

    [Fact]
    public void GetGlobSplitPoint_DoubleStar_ReturnsDoubleStarIndex()
    {
        var path = "C:\\folder\\**\\*.txt";
        var split = Utils.GetGlobSplitPoint(path);
        Assert.Equal(10, split);
    }

    [Fact]
    public void MakeFileName_CreatesCorrectPath()
    {
        var result = Utils.MakeFileName("input.ch8", "bin", "output");
        Assert.Equal(Path.Combine("output", "input.bin"), result);
    }
}