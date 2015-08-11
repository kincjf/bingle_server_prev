using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data
{
    /// <summary>
    /// Header Size : 14 (단위 : byte)
    /// Key : 4 / BodyLength : 4 / Fragmented : 1 / LastMsg : 1 / Sequence : 4
    /// </summary>
    public class BingleHeader : IBingleProtocol
    {
        public const int SIZE = 10;      // KEY는 제외함

        public int BodyLength { get; private set; }
        public byte Fragmented { get; private set; }
        public byte LastMsg { get; private set; }
        public int Sequence { get; private set; }
        
        public BingleHeader(int bodyLength, byte fragmented, byte lastMsg, int sequence)
        {
            BodyLength = bodyLength;
            Fragmented = fragmented;
            LastMsg = lastMsg;
            Sequence = sequence;
        }

        /// <summary>
        /// BingleHeader의 byte배열 반환
        /// </summary>
        /// <returns>byte[HEADER_SIZE]</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[SIZE];
            byte[] temp;

            if (BitConverter.IsLittleEndian)
            {
                BodyLength = IPAddress.HostToNetworkOrder(BodyLength);
                Sequence = IPAddress.HostToNetworkOrder(Sequence);
            }

            temp = BitConverter.GetBytes(BodyLength);
            Array.Copy(temp, 0, bytes, 0, temp.Length);

            temp = BitConverter.GetBytes(Fragmented);
            Array.Copy(temp, 0, bytes, 4, temp.Length);

            temp = BitConverter.GetBytes(LastMsg);
            Array.Copy(temp, 0, bytes, 5, temp.Length);

            temp = BitConverter.GetBytes(Sequence);
            Array.Copy(temp, 0, bytes, 6, temp.Length);

            return bytes;
        }

        public int GetSize()
        {
            return SIZE;
        }
    }
}
