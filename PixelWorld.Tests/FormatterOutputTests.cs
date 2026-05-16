using PixelWorld.Fonts;
using PixelWorld.Formatters;
using PixelWorld.Machines;
using System;
using System.IO;
using System.Text;

namespace PixelWorld.Tests;

public class FormatterOutputTests
{
    private static Font CreateTestFont()
    {
        var font = new Font("TestFont", 8);
        var data = new bool[8, 8];
        data[0, 0] = true;
        data[7, 0] = true;
        font.Glyphs['A'] = new Glyph(8, 8, data);
        return font;
    }

    [Fact]
    public void CHeaderFontFormatter_ProducesValidHeader()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "pw_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        try
        {
            var ch8Path = Path.Combine(tempDir, "test.ch8");
            using (var stream = File.Create(ch8Path))
                ByteFontFormatter.Write(CreateTestFont(), stream, Spectrum.UK, 96);

            CHeaderFontFormatter.CreateFontHeaderConst("uint8_t", [ch8Path], tempDir, "Test credit");

            var output = File.ReadAllText(Path.Combine(tempDir, "test.h"));
            Assert.Contains("#ifndef TEST_H_", output);
            Assert.Contains("#define TEST_H_", output);
            Assert.Contains("static const uint8_t FONT_TEST_BITMAP[]", output);
            Assert.Contains("#endif", output);
            Assert.Contains("0x81", output);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void RustHeaderFontFormatter_ProducesValidHeader()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "pw_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        try
        {
            var ch8Path = Path.Combine(tempDir, "test.ch8");
            using (var stream = File.Create(ch8Path))
                ByteFontFormatter.Write(CreateTestFont(), stream, Spectrum.UK, 96);

            RustHeaderFontFormatter.CreateFontHeaderConst([ch8Path], tempDir, "Test credit");

            var output = File.ReadAllText(Path.Combine(tempDir, "test.rs"));
            Assert.Contains("pub const FONT_TEST_BITMAP: &[u8] = &[", output);
            Assert.Contains("];", output);
            Assert.Contains("0x81", output);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void AssemblyFontFormatter_ProducesValidZ80Assembly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "pw_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        try
        {
            var ch8Path = Path.Combine(tempDir, "test.ch8");
            using (var stream = File.Create(ch8Path))
                ByteFontFormatter.Write(CreateTestFont(), stream, Spectrum.UK, 96);

            AssemblyFontFormatter.CreateDefines("Z80", "defb", " {0:X2}h", [ch8Path], tempDir, "Test credit");

            var output = File.ReadAllText(Path.Combine(tempDir, "test.Z80.asm"));
            Assert.Contains("defb", output);
            Assert.Contains("81h", output);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void FzxFontFormatter_WritesValidHeader()
    {
        var font = CreateTestFont();
        using var stream = new MemoryStream();
        FzxFontFormatter.Write(font, stream);

        stream.Position = 0;
        var data = stream.ToArray();
        Assert.True(data.Length > 0);
        Assert.Equal(8, data[0]);       // Height
        Assert.Equal(0, data[1]);       // Proportional flag (0)
        Assert.Equal(127, data[2]);     // Last char (7F)
    }

    [Fact]
    public void FzxFontFormatter_Proportional_SetsFlag()
    {
        var font = CreateTestFont();
        using var stream = new MemoryStream();
        FzxFontFormatter.Write(font, stream, makeProportional: true);

        stream.Position = 0;
        var data = stream.ToArray();
        Assert.Equal(1, data[1]); // Proportional flag = 1
    }

    [Fact]
    public void Font_Print_RendersCorrectly()
    {
        var font = new Font("Test", 8);
        var data = new bool[8, 8];
        data[3, 3] = true;
        font.Glyphs['X'] = new Glyph(8, 8, data);

        var output = font.Print("X");
        Assert.Contains("\u2588", output); // Block char for pixel
    }

    [Fact]
    public void ByteFontFormatter_RoundTrip_PreservesPixelData()
    {
        var font = CreateTestFont();
        using var stream = new MemoryStream();
        ByteFontFormatter.Write(font, stream, Spectrum.UK, 96);
        stream.Position = 0;

        using var reader = new BinaryReader(stream);
        var loaded = ByteFontFormatter.Create(reader, "RoundTrip", 0, Spectrum.UK);

        Assert.True(loaded.Glyphs.ContainsKey('A'));
        Assert.True(loaded.Glyphs['A'].Data[0, 0]);
        Assert.True(loaded.Glyphs['A'].Data[7, 0]);
        Assert.False(loaded.Glyphs['A'].Data[1, 0]);
    }
}
