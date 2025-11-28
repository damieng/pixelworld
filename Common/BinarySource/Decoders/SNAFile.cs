// Copyright (c) 2009-2015 Arjun Nair. See the LICENSE file for license rights and limitations (MIT).
// Original source: https://github.com/ArjunNair/Zero-Emulator/blob/master/Ziggy/Peripherals/SNAFile.cs

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PixelWorld.BinarySource.Decoders
{
    #nullable disable // Leave 3rd-party code as-is
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SNA_HEADER
    {
        public Byte I;                  //I Register
        public UInt16 HL_, DE_, BC_, AF_;  //Alternate registers
        public UInt16 HL, DE, BC, IY, IX;  //16 bit main registers
        public Byte IFF2;               //Interrupt enabled? (bit 2 on/off)
        public Byte R;                  //R Register
        public UInt16 AF, SP;              //AF and SP register
        public Byte IM;                 //Interupt Mode
        public Byte BORDER;             //Border colour
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SNA_SNAPSHOT
    {
        public Byte TYPE;                      //0 = 48k, 1 = 128;
        public SNA_HEADER HEADER;              //The above header
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SNA_48K : SNA_SNAPSHOT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 49152)]
        public Byte[] RAM;              //Contents of the RAM
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SNA_128K : SNA_SNAPSHOT
    {
        public UInt16 PC;                                  //PC Register
        public Byte PORT_7FFD;                          //Current state of port 7ffd
        public Byte TR_DOS;                             //Is TR DOS ROM paged in?
        public Byte[][] RAM_BANK = new Byte[16][];        //Contents of the 8192*16 ram banks
    }

    public class SNAFile
    {
        //Will return a filled snapshot structure from buffer
        public static SNA_SNAPSHOT LoadSNA(Stream fs)
        {
            SNA_SNAPSHOT snapshot;

            using (BinaryReader r = new(fs))
            {
                var bytesToRead = (Int32)fs.Length;

                var buffer = new Byte[bytesToRead];
                var bytesRead = r.Read(buffer, 0, bytesToRead);

                if (bytesRead == 0)
                    return null; //something bad happened!

                if (bytesRead == 49179)
                {
                    snapshot = new SNA_48K
                    {
                        TYPE = 0
                    };
                }
                else if (bytesRead == 131103 || bytesRead == 147487)
                {
                    snapshot = new SNA_128K
                    {
                        TYPE = 1
                    };
                }
                else
                    return null;

                snapshot.HEADER.I = buffer[0];
                snapshot.HEADER.HL_ = (UInt16)(buffer[1] | (buffer[2] << 8));
                snapshot.HEADER.DE_ = (UInt16)(buffer[3] | (buffer[4] << 8));
                snapshot.HEADER.BC_ = (UInt16)(buffer[5] | (buffer[6] << 8));
                snapshot.HEADER.AF_ = (UInt16)(buffer[7] | (buffer[8] << 8));

                snapshot.HEADER.HL = (UInt16)(buffer[9] | (buffer[10] << 8));
                snapshot.HEADER.DE = (UInt16)(buffer[11] | (buffer[12] << 8));
                snapshot.HEADER.BC = (UInt16)(buffer[13] | (buffer[14] << 8));
                snapshot.HEADER.IY = (UInt16)(buffer[15] | (buffer[16] << 8));
                snapshot.HEADER.IX = (UInt16)(buffer[17] | (buffer[18] << 8));

                snapshot.HEADER.IFF2 = buffer[19];
                snapshot.HEADER.R = buffer[20];
                snapshot.HEADER.AF = (UInt16)(buffer[21] | (buffer[22] << 8));
                snapshot.HEADER.SP = (UInt16)(buffer[23] | (buffer[24] << 8));
                snapshot.HEADER.IM = buffer[25];
                snapshot.HEADER.BORDER = (Byte)(buffer[26] & 0x07);

                //48k snapshot
                if (snapshot.TYPE == 0)
                {
                    ((SNA_48K)snapshot).RAM = new Byte[49152];
                    Array.Copy(buffer, 27, ((SNA_48K)snapshot).RAM, 0, 49152);
                }
                else
                {
                    //128k snapshot
                    for (var f = 0; f < 16; f++)
                    {
                        ((SNA_128K)snapshot).RAM_BANK[f] = new Byte[8192];
                    }

                    //Copy ram bank 5
                    Array.Copy(buffer, 27, ((SNA_128K)snapshot).RAM_BANK[10], 0, 8192);
                    Array.Copy(buffer, 27 + 8192, ((SNA_128K)snapshot).RAM_BANK[11], 0, 8192);

                    //Copy ram bank 2
                    Array.Copy(buffer, 27 + 16384, ((SNA_128K)snapshot).RAM_BANK[4], 0, 8192);
                    Array.Copy(buffer, 27 + 16384 + 8192, ((SNA_128K)snapshot).RAM_BANK[5], 0, 8192);

                    ((SNA_128K)snapshot).PORT_7FFD = buffer[49181]; //we'll load this in earlier 'cos we need it now!

                    var BankInPage4 = ((SNA_128K)snapshot).PORT_7FFD & 0x07;

                    //Copy currently paged in bank (actually we don't care here 'cos we're simply filling in all the b(l)anks)
                    Array.Copy(buffer, 27 + 16384 + 16384, ((SNA_128K)snapshot).RAM_BANK[BankInPage4 * 2], 0, 8192);
                    Array.Copy(buffer, 27 + 16384 + 16384 + 8192, ((SNA_128K)snapshot).RAM_BANK[BankInPage4 * 2 + 1], 0, 8192);

                    ((SNA_128K)snapshot).PC = (UInt16)(buffer[49179] | (buffer[49180] << 8));

                    ((SNA_128K)snapshot).TR_DOS = buffer[49182];

                    var t = 0;
                    for (var f = 0; f < 8; f++)
                    {
                        if (f == 5 || f == 2 || f == BankInPage4)
                            continue;

                        Array.Copy(buffer, 49183 + 16384 * t, ((SNA_128K)snapshot).RAM_BANK[f * 2], 0, 8192);
                        Array.Copy(buffer, 49183 + 16384 * t + 8192, ((SNA_128K)snapshot).RAM_BANK[f * 2 + 1], 0, 8192);
                        t++;
                    }
                }
            }
            return snapshot;
        }
    }
    #nullable restore
}