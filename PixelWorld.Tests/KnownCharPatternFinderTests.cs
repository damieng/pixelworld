using PixelWorld.OffsetFinders;

namespace PixelWorld.Tests;

public class KnownCharPatternFinderTests
{
    [Fact]
    public void FindOffsets_BufferTooSmall_ReturnsEmpty()
    {
        var buffer = new byte[100];
        var patterns = new[] { new KnownCharPattern(0, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }) };

        var offsets = KnownCharPatternFinder.FindOffsets(buffer, patterns);

        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_NoMatch_ReturnsEmpty()
    {
        var buffer = new byte[800];
        var patterns = new[] { new KnownCharPattern(0, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }) };

        var offsets = KnownCharPatternFinder.FindOffsets(buffer, patterns);

        Assert.Empty(offsets);
    }

    [Fact]
    public void KnownCharPattern_StoresValues()
    {
        var pattern = new KnownCharPattern(65, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        Assert.Equal(65, pattern.CharCode);
        Assert.Equal(1, pattern.Pattern[0]);
        Assert.Equal(8, pattern.Pattern[7]);
    }
}