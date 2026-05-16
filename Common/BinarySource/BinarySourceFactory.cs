using System;
using System.IO;

namespace PixelWorld.BinarySource;

public static class BinarySourceFactory
{
    public static IBinarySource? Create(String extension)
    {
        return extension.ToLower() switch
        {
            ".sna" => SnaBinarySource.Instance,
            ".z80" => Z80BinarySource.Instance,
            _ => null
        };
    }
}
