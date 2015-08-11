using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Data.Body
{
    public class BodySTOR : IBingleProtocol
    {
        public byte[] BinaryData { get; private set; }

        public BodySTOR(byte[] data)
        {
            BinaryData = data;
        }

        /// <summary>
        /// BinaryData를 byte배열 반환
        /// </summary>
        /// <returns>byte[HEADER_SIZE]</returns>
        public byte[] GetBytes()
        {
            return BinaryData;
        }

        public int GetSize()
        {
            return BinaryData.Length;
        }
    }
}
