namespace PixelWorld.Tests;

public class ByteArrayEqualityComparerTests
{
    [Fact]
    public void Equals_IdenticalArrays_ReturnsTrue()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Assert.True(comparer.Equals(a, b));
    }

    [Fact]
    public void Equals_DifferentArrays_ReturnsFalse()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 9 };
        Assert.False(comparer.Equals(a, b));
    }

    [Fact]
    public void Equals_FirstElementDiffers_ReturnsFalse()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new byte[] { 2, 2, 3, 4, 5, 6, 7, 8 };
        Assert.False(comparer.Equals(a, b));
    }

    [Fact]
    public void Equals_NullBoth_ReturnsTrue()
    {
        var comparer = new ByteArrayEqualityComparer();
        Assert.True(comparer.Equals(null, null));
    }

    [Fact]
    public void Equals_OneNull_ReturnsFalse()
    {
        var comparer = new ByteArrayEqualityComparer();
        Assert.False(comparer.Equals(new byte[8], null));
        Assert.False(comparer.Equals(null, new byte[8]));
    }

    [Fact]
    public void GetHashCode_SameArrays_SameHash()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Assert.Equal(comparer.GetHashCode(a), comparer.GetHashCode(b));
    }

    [Fact]
    public void GetHashCode_DifferentArrays_DifferentHash()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var b = new byte[] { 9, 9, 9, 9, 9, 9, 9, 9 };
        Assert.NotEqual(comparer.GetHashCode(a), comparer.GetHashCode(b));
    }

    [Fact]
    public void GetHashCode_Null_ReturnsZero()
    {
        var comparer = new ByteArrayEqualityComparer();
        Assert.Equal(0, comparer.GetHashCode(null));
    }

    [Fact]
    public void GetHashCode_DiffersInSecondHalf_ProducesDifferentHash()
    {
        var comparer = new ByteArrayEqualityComparer();
        var a = new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 };
        var b = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 };
        Assert.NotEqual(comparer.GetHashCode(a), comparer.GetHashCode(b));
    }
}