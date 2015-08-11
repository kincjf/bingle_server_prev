using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data.Body
{
    public class BodySTAT : IBingleProtocol
    {
        public const int SIZE = 4;
        public int ResponseCode { get; private set; }

        public BodySTAT(int responseCode)
        {
            ResponseCode = responseCode;
        }

        /// <summary>
        /// ResponseCode를 byte배열 반환
        /// </summary>
        /// <returns>byte[HEADER_SIZE]</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[SIZE];
            byte[] temp;

            if (BitConverter.IsLittleEndian)
            {
                temp = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ResponseCode));
            }
            else
            {
                temp = BitConverter.GetBytes(ResponseCode);
            }

            Array.Copy(temp, 0, bytes, 0, temp.Length);

            return bytes;
        }

        public int GetSize()
        {
            return SIZE;
        }
    }
}
