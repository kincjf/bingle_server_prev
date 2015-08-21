using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Command
{
    /// <summary>
    /// request(TYPE) 전송에 대한 결과를 받음 (Get status code)
    /// </summary>
    public class STAT : CommandBase<BingleSession, BingleProtocol>
    {
        public override void ExecuteCommand(BingleSession session, BingleProtocol requestInfo)
        {
            Console.WriteLine("ExecuteCommand - STAT");
            ConsoleLogger.GetProtocolLog(requestInfo);      /// log

            //if (!session.Logged)
            //    return;

            int responseCode = BitConverter.ToInt32(requestInfo.Body, 0);
            
            if (BitConverter.IsLittleEndian)
            {
                responseCode = IPAddress.NetworkToHostOrder(responseCode);
            }

            switch(responseCode)
            {
                case StatusData.DataConnectionAccepted_150:
                    /// 나중에 (RETR)로 변경해야한다.

                    /// ServiceProvider.SendFile에서 ServerContext 경로로 변환해주기 때문에
                    /// virtualPath만 넣어주면 됨.
                    //string imagePath = Path.Combine(
                    //    session.AppServer.ServerContext.ImageFileDirectory, session.Context.TempFileName + ".jpg");
                    string imagePath = Path.Combine(
                        session.AppServer.ServerContext.ImageFileDirectory, "test360Image.jpg");      // test
                    
                    DataConnection dataConn = session.DataConnection;

                    try
                    {
                        if (session.AppServer.ServiceProvider.SendFile(
                                session.AppServer.ServerContext, imagePath, dataConn.GetStream(session.Context)))
                        {
                            Console.WriteLine("STAT - Success Send Data");
                        }
                        else
                        {
                            Console.WriteLine("STAT - DataConnection cannot open!({0})", StatusData.DataConnectionCannotOpen_420);
                        }
                    }
                    catch (SocketException e)
                    {
                        session.Logger.Error(e);
                        Console.WriteLine("STAT - DataConnection Error({0})", StatusData.DataConnectionError_426);
                    }
                    catch (Exception e)
                    {
                        session.Logger.Error(e);
                        Console.WriteLine("STAT - InputFile Error({0}, {1})", StatusData.InputFileError_551, imagePath);
                    }
                    finally
                    {
                        session.CloseDataConnection();
                    }

                    break;
                case StatusData.TransferCompleted_226:
                    /// 모든 request 완료, 연결 종료
                    session.Close();
                    break;
                default:
                    Console.WriteLine("STAT - Undefined response code.");
                    break;
            }          
        }
    }
}
