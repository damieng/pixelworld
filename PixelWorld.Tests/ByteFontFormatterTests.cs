using System.IO;
using PixelWorld.Fonts;
using PixelWorld.Formatters;

namespace PixelWorld.Tests;

public class ByteFontFormatterTests
{
    [Fact]
    public void Read_ValidFont_CreatesCorrectGlyphs()
    {
        using var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (int i = 0; i < 96; i++)
        {
            for (int y = 0; y < 8; y++)
                writer.Write((byte)(i * (y + 1)));
        }

        stream.Position = 0;
        using var reader = new BinaryReader(stream);
        var font = ByteFontFormatter.Create(reader, "Test", 0, Machines.Spectrum.UK);

        Assert.Equal(96, font.Glyphs.Count);
        Assert.True(font.Glyphs.ContainsKey('A'));
        Assert.Equal(8, font.Glyphs['A'].Width);
        Assert.Equal(8, font.Glyphs['A'].Height);
    }

    [Fact]
    public void Write_RoundTrip_PreservesData()
    {
        var font = new Font("Test", 8);
        var data = new bool[8, 8];
        data[0, 0] = true;
        data[7, 7] = true;
        font.Glyphs['A'] = new Glyph(8, 8, data);

        using var stream = new MemoryStream();
        var charset = new Dictionary<int, char> { { 0, 'A' } };
        ByteFontFormatter.Write(font, stream, charset, 1);

        stream.Position = 0;
        using var reader = new BinaryReader(stream);
        var loaded = ByteFontFormatter.Create(reader, "Test", 0, charset);

        Assert.True(loaded.Glyphs['A'].Data[0, 0]);
        Assert.True(loaded.Glyphs['A'].Data[7, 7]);
    }

    [Fact]
    public void Load_NonExistentFile_Throws()
    {
        Assert.Throws<FileNotFoundException>(() =>
            ByteFontFormatter.Load("nonexistent.file", Machines.Spectrum.UK));
    }
}