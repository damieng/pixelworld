using System;
using System.IO;
using System.Threading.Tasks;

namespace PixelWorld.Shared.BinarySource
{
    class Z80BinarySource : IBinarySource
    {
        public async Task<ReadOnlyMemory<byte>> Read(Stream source)
        {
            var raw = await source.ReadAllBytes(); 
            var pc = (UInt16) (raw[6] | (raw[7] << 8));
            return pc == 0 ? ReadV2(raw) : ReadV1(raw);
        }

        private static ReadOnlyMemory<byte> ReadV1(ArraySegment<byte> raw)
        {
            Out("V1 detected");
            const int ramSize = 49152;

            var byte12 = raw[12] == 255 ? (byte)1 : raw[12];
            var isCompressed = (byte12 & 0x20) != 0;

            if (!isCompressed)
                return raw.Slice(30, ramSize);

            var decoded = new byte[ramSize]; // 48K only
            return decoded;
        }

        private static ReadOnlyMemory<byte> ReadV2(ArraySegment<byte> raw) {
            Out("V2/3 detected");

            int snapshotType = GetVersion(raw);

            int offset = BitConverter.ToInt16(raw.Array, 32) + 32;
            while (offset < raw.Count)
            {
                int blockSize = BitConverter.ToInt16(raw.Array, offset);
                offset += 2;
                byte page = raw.Array[offset++];
            }

            return new byte[0];
        }

        private static void Out(string message)
        {
            global::PixelWorld.Shared.Out.Write(typeof(Z80BinarySource).Name + " " + message);
        }

        private static ArraySegment<byte> GetPage(ArraySegment<byte> raw, int startIndex, int uncompressedLength)
        {
            if (uncompressedLength == 0xffff) // Not compressed
            {
                return raw.Slice(startIndex, uncompressedLength);
            }

            var uncompressed = new byte[uncompressedLength];

            int uncompressedIndex = 0;
            while (uncompressedIndex < uncompressedLength)
            {
                if (raw.Array[startIndex] == 0xED && raw.Array[startIndex + 1] == 0xED) {
                    int repeatLength = uncompressed[startIndex + 2];
                    byte repeatByte = uncompressed[startIndex + 3];
                    Array.Fill(uncompressed, repeatByte, uncompressedIndex, repeatLength);
                    uncompressedIndex += repeatLength;
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
            switch (buffer[34])
            {
                case 0:
                case 1:
                    return 0;

                case 3:
                    return buffer[30] == 23 ? 1 : 0;

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
