using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

            int responseCode = BitConverter.ToInt32(requestInfo.Body, 0);
            
            if (BitConverter.IsLittleEndian)
            {
                responseCode = IPAddress.NetworkToHostOrder(responseCode);
            }

            switch(responseCode)
            {
                case StatusData.DataConnectionAccepted_150:
                    /// Sphere 이미지 생성

                    /// ServiceProvider.SendFile에서 ServerContext 경로로 변환해주기 때문에
                    /// virtualPath만 넣어주면 됨.
                    // string imagePath = Path.Combine(
                    //  session.AppServer.ServerContext.ImageFilePath, session.FileContext.FilePath + ".jpg");
                    string imagePath = Path.Combine(
                        session.AppServer.ServerContext.ImageFileDirectory, "test360Image.jpg");
                    
                /// 파일 전송(STOR)
                    session.AppServer.ServiceProvider.SendFile(
                        session.AppServer.ServerContext, imagePath, session);
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
