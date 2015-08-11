using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using System.Net;

namespace Bingle.Server.Filter
{
    /// <summary>
    /// Big Endian으로 통신
    /// </summary>
    class FileReceiveFilter : FixedHeaderReceiveFilter<BingleProtocol>
    {
        public FileReceiveFilter()
            : base(BingleProtocol.HEADER_SIZE)    // 14
        {
        }

        /// <summary>
        /// 현재 bodyLength가 올바른 값인 상태를 가정하고 작성됨.
        /// - Problems
        /// - 1. header의 bodyLength값이 잘못된 경우
        /// - 2. (0 > bodyLength) OR (bodyLength == 0) OR (bodyLength > 큰 수)
        /// 
        /// bodyLength의 값이 큰 수 였을 경우 다음 Packet들을 받지 못하는 경우가 생김
        /// (bodyLength만큼의 Packet[body]가 올 때 까지 계속 receive를 하는 모양임)
        /// 
        /// FixedHeaderReceiveFilter 내부 로직을 통하여 예방 가능한 버그를 알아내고
        /// 이외의 버그는 BodyLength의 최대 길이를 정하거나, 별도의 try/catch 를 구현해야 함.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="offset">sharedBuffer에서 현재 패킷이 시작하는 지점의 index</param>
        /// <param name="length">length of header</param>
        /// <returns></returns>
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {            
            int bodyLength = BitConverter.ToInt32(header, offset + 4);

            if (BitConverter.IsLittleEndian)
            {
                /// Big Endian을 호스트 바이트 순서(LittleEndian)으로 변환함
                bodyLength = IPAddress.NetworkToHostOrder(bodyLength);
            }

            return bodyLength;
        }

        /// <summary>
        /// 전송된 Key에 대하여 알맞는 protocol을 반환함 
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bodyBuffer"></param>
        /// <param name="offset">0(defaults)</param>
        /// <param name="length">The Length of bodyBuffer</param>
        /// <returns></returns>
        protected override BingleProtocol ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            string key = Encoding.ASCII.GetString(header.Array, header.Offset, 4);
            int sequence = BitConverter.ToInt32(header.Array, header.Offset + 10);

            if (BitConverter.IsLittleEndian)
            {
                /// Big Endian을 호스트 바이트 순서(LittleEndian)으로 변환함
                sequence = IPAddress.NetworkToHostOrder(sequence);
            }

            BingleHeader bingleHeader = new BingleHeader(
                length,
                header.Array[header.Offset + 8], header.Array[header.Offset + 9],
                sequence);

            return new BingleProtocol(key, bingleHeader, bodyBuffer.CloneRange(offset, length));
        }
    }
}