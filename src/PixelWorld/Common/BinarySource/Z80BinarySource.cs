using System;
using System.Collections.Generic;
using System.IO;

namespace PixelWorld.BinarySource
{
    public class Z80BinarySource : IBinarySource
    {
        public ArraySegment<Byte> Read(Stream source)
        {
            var raw = new ArraySegment<byte>(source.ReadAllBytes());
            var pc = BitConverter.ToUInt16(raw.Array, 6);
            return pc == 0 ? ReadV2(raw) : ReadV1(raw);
        }

        private static ArraySegment<byte> ReadV1(ArraySegment<byte> sourceSegment)
        {
            Out("V1 detected");
            const int ram48Ksize = 49152;

            var source = sourceSegment.Array;

            var byte12 = source[12] == 255 ? (byte)1 : source[12];
            var isCompressed = (byte12 & 0x20) != 0;

            if (!isCompressed)
                return new ArraySegment<byte>(source, 30, ram48Ksize);

            var target = new byte[ram48Ksize];
            var sourceIndex = 30;
            var targetIndex = 0;
            while (true)
            {
                var current = source[sourceIndex++];
                if (current == 0 && source[sourceIndex] == 0xED && source[sourceIndex + 1] == 0xED && source[sourceIndex + 2] == 0)
                {
                    break;
                }

                if (current == 0xED && source[sourceIndex] == 0xED)
                {
                    sourceIndex++;
                    int repeatLength = source[sourceIndex++];
                    byte repeatData = source[sourceIndex++];
                    while (repeatLength-- > 0)
                        target[targetIndex++] = repeatData;
                }
                else
                {
                    target[targetIndex++] = current;
                }
            }

            return new ArraySegment<byte>(target);
        }

        private static ArraySegment<byte> ReadV2(ArraySegment<byte> source)
        {
            Out("V2/3 detected");

            int snapshotType = GetVersion(source);

            var pages = new Dictionary<int, ArraySegment<byte>>();

            int offset = BitConverter.ToInt16(source.Array, 30) + 32;
            while (offset < source.Count)
            {
                int blockSize = BitConverter.ToInt16(source.Array, offset);
                offset += 2;
                byte page = source.Array[offset++];
                pages[page] = GetPage(source, offset, blockSize);
                offset += blockSize == 0xffff ? 16384 : blockSize;
            }

            var is48K = snapshotType == 0;
            var ramSize = is48K ? 48 * 1024 : 128 * 1024;
            var ram = new byte[ramSize];

            foreach (var page in pages)
            {
                switch (page.Key)
                {
                }
            }

            return new ArraySegment<byte>(ram);
        }

        private static void Out(string message)
        {
            global::PixelWorld.Out.Write("    Z80 " + message);
        }

        private static ArraySegment<byte> GetPage(ArraySegment<byte> raw, int startIndex, int compressedLength)
        {
            if (compressedLength == 0xffff) // Not compressed
            {
                return new ArraySegment<byte>(raw.Array, startIndex, compressedLength);
            }

            var uncompressed = new byte[16384];

            int uncompressedIndex = 0;
            while (uncompressedIndex < compressedLength)
            {
                if (raw.Array[startIndex] == 0xED && raw.Array[startIndex + 1] == 0xED)
                {
                    int repeatLength = uncompressed[startIndex + 2];
                    byte repeatByte = uncompressed[startIndex + 3];
                    while (repeatLength-- > 0)
                        uncompressed[uncompressedIndex++] = repeatByte;
                    startIndex += 4;
                }
                else
                {
                    uncompressed[uncompressedIndex++] = raw.Array[startIndex++];
                }
            }

            return new ArraySegment<byte>(uncompressed);
        }

        private static int GetVersion(ArraySegment<byte> buffer)
        {
            switch (buffer.Array[34])
            {
                case 0:
                case 1:
                    return 0;

                case 3:
                    return buffer.Array[30] == 23 ? 1 : 0;

                case 4:
                case 5:
                case 6:
                    return 1;

                case 7:
                case 8:
                    return 2;

                case 9:
                    return 3;
            }

            return -1;
        }
    }
}
