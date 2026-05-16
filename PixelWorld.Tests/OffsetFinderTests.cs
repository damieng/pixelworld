using PixelWorld.Machines;
using PixelWorld.OffsetFinders;

namespace PixelWorld.Tests;

public class GeneralHeuristicFinderTests
{
    [Fact]
    public void FindOffsets_EmptyBuffer_ReturnsEmpty()
    {
        var buffer = new byte[800];
        var offsets = GeneralHeuristicFinder.FindOffsets(buffer);
        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_BufferTooSmall_ReturnsEmpty()
    {
        var buffer = new byte[100];
        var offsets = GeneralHeuristicFinder.FindOffsets(buffer);
        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_NoLikelyFonts_ReturnsEmpty()
    {
        var buffer = new byte[2000];
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = 0xFF;
        var offsets = GeneralHeuristicFinder.FindOffsets(buffer);
        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_AllZeros_ReturnsEmpty()
    {
        var buffer = new byte[2000];
        var offsets = GeneralHeuristicFinder.FindOffsets(buffer);
        Assert.Empty(offsets);
    }
}

public class CandidatesInWindowFinderTests
{
    [Fact]
    public void FindOffsets_InsufficientCandidates_ReturnsEmpty()
    {
        var buffer = new byte[2000];
        var candidates = new byte[10][];
        for (int i = 0; i < 10; i++)
            candidates[i] = new byte[8];

        var offsets = CandidatesInWindowFinder.FindOffsets(buffer, candidates);

        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_EmptyCandidates_ReturnsEmpty()
    {
        var buffer = new byte[2000];
        var offsets = CandidatesInWindowFinder.FindOffsets(buffer, Array.Empty<byte[]>());
        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_BufferTooSmall_ReturnsEmpty()
    {
        var buffer = new byte[100];
        var candidates = new byte[40][];
        for (int i = 0; i < 40; i++)
        {
            candidates[i] = new byte[8];
            for (int j = 0; j < 8; j++)
                candidates[i][j] = (byte)(i + j);
        }

        var offsets = CandidatesInWindowFinder.FindOffsets(buffer, candidates);

        Assert.Empty(offsets);
    }
}

public class EnvironmentGuidedFinderTests
{
    [Fact]
    public void FindOffsets_PointerBelow16384_ReturnsEmpty()
    {
        var buffer = new byte[50000];
        buffer[Spectrum.CharsSysVar] = 0;
        buffer[Spectrum.CharsSysVar + 1] = 0;

        var offsets = EnvironmentGuidedFinder.FindOffsets(buffer);

        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_PointerExceedsBuffer_ReturnsEmpty()
    {
        var buffer = new byte[30000];
        buffer[Spectrum.CharsSysVar] = 0xFF;
        buffer[Spectrum.CharsSysVar + 1] = 0xFF;

        var offsets = EnvironmentGuidedFinder.FindOffsets(buffer);

        Assert.Empty(offsets);
    }

    [Fact]
    public void FindOffsets_ValidPointerWithEmptyRegion_ReturnsOffset()
    {
        var buffer = new byte[50000];
        buffer[Spectrum.CharsSysVar] = 0x00;
        buffer[Spectrum.CharsSysVar + 1] = 0x40;
        for (int i = 16640; i < 17408; i++)
            buffer[i] = 0;

        var offsets = EnvironmentGuidedFinder.FindOffsets(buffer);

        Assert.Contains(16640, offsets);
    }

    [Fact]
    public void FindOffsets_PointerToNonEmptyRegion_ReturnsEmpty()
    {
        var buffer = new byte[50000];
        buffer[Spectrum.CharsSysVar] = 0x00;
        buffer[Spectrum.CharsSysVar + 1] = 0x40;
        buffer[16640] = 255;

        var offsets = EnvironmentGuidedFinder.FindOffsets(buffer);

        Assert.Empty(offsets);
    }
}