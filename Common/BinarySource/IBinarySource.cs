﻿using System;
using System.IO;

namespace PixelWorld.BinarySource;

public interface IBinarySource
{
    ArraySegment<byte> GetMemory(Stream source);
}