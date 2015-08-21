using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Command
{
    /// <summary>
    /// 수신을 위한 정보를 받음(fileSize등)
    /// (차후 "PORT"(FTP Command)로 변경되어야 함.)
    /// </summary>
    public class TYPE : CommandBase<BingleSession, BingleProtocol>
    {
        /// <param name="session"></param>
        /// <param name="requestInfo">Body는 호스트 바이트 순서(Little Endian)로 변환해야 함.</param>
        public override void ExecuteCommand(BingleSession session, BingleProtocol requestInfo)
        {
            Console.WriteLine("ExecuteCommand - TYPE");
            ConsoleLogger.GetProtocolLog(requestInfo);

            //if (!session.Logged)
            //    return;


            session.Context.TempFileSize = BitConverter.ToInt64(requestInfo.Body, 0);

            if (BitConverter.IsLittleEndian)
            {
                session.Context.TempFileSize = IPAddress.NetworkToHostOrder(
                    session.Context.TempFileSize);
            }

            BingleHeader header = new BingleHeader(
                BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
            BingleProtocol<BingleHeader, BodySTAT> protocol;
            
            DataConnection dataConnection;

            if (DataConnection.TryOpenDataConnection(session, session.AppServer.ServerContext.DataPort, out dataConnection))
            {
                session.DataConnection = dataConnection;
                protocol = new BingleProtocol<BingleHeader, BodySTAT>(
                    CommandList.STAT, header, new BodySTAT(StatusData.DataConnectionAccepted_150));
            }
            else
            {
                protocol = new BingleProtocol<BingleHeader, BodySTAT>(
                    CommandList.STAT, header, new BodySTAT(StatusData.PortInvalid_552));
            }

            session.Send(protocol.GetBytes(), 0, protocol.GetSize());
        }
    }
}
