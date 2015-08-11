using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace Bingle.Server.Data
{
    /// <summary>
    /// request protocol : key(4 byte) + header(10 byte) + body.Length
    /// Generic Type을 이용해서 request와 response protocol을 하나로 합쳐보려고 했으나..
    /// 형변환 오류가 자꾸 나서 실패..
    /// </summary>
    public class BingleProtocol : RequestInfo<BingleHeader, byte[]>, IBingleProtocol
    {
        public const int HEADER_SIZE = 14;
        public const int KEY_SIZE = 4;
        
        public BingleProtocol(string key, BingleHeader header, byte[] body)
            : base(key, header, body)
        {
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[GetSize()];

            // Each letter is already it's ASCII character value.
            byte[] temp = Encoding.ASCII.GetBytes(Key);
            Array.Copy(temp, bytes, temp.Length);

            Array.Copy(Header.GetBytes(), 0, bytes, KEY_SIZE, Header.GetSize());
            Array.Copy(Body, 0, bytes, KEY_SIZE + Header.GetSize(), Body.Length);
            
            return bytes;
        }

        public int GetSize()
        {
            return KEY_SIZE + Header.GetSize() + Body.Length;
        }
    }

    /// <summary>
    /// request protocol : key(4 byte) + header(10 byte) + body.Length
    /// </summary>
    /// <typeparam name="THeader"></typeparam>
    /// <typeparam name="TBody"></typeparam>
    public class BingleProtocol<THeader, TBody> : RequestInfo<THeader, TBody>, IBingleProtocol
        where THeader : IBingleProtocol
        where TBody : IBingleProtocol
    {
        public const int KEY_SIZE = 4;

        public BingleProtocol(string key, THeader header, TBody body)
            : base(key, header, body)
        {
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[GetSize()];

            // Each letter is already it's ASCII character value.
            byte[] temp = Encoding.ASCII.GetBytes(Key);
            Array.Copy(temp, bytes, temp.Length);

            Array.Copy(Header.GetBytes(), 0, bytes, KEY_SIZE, Header.GetSize());
            Array.Copy(Body.GetBytes(), 0, bytes, KEY_SIZE + Header.GetSize(), Body.GetSize());

            return bytes;
        }

        public int GetSize()
        {
            return KEY_SIZE + Header.GetSize() + Body.GetSize();
        }
    }
}
