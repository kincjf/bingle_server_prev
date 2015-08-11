using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;

using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Bingle.Server.Command
{
    /// <summary>
    /// 패킷(파일)을 저장함
    /// 차후 command 패턴은 그대로 유지하면서 따로 data 전송을 위한 connection을 구현해서
    /// 전송 속도/효율을 높여야 될 것 같음.
    /// </summary>
    public class STOR : CommandBase<BingleSession, BingleProtocol>
    {
        //#region Receive File and Send File 
        public override void ExecuteCommand(BingleSession session, BingleProtocol requestInfo)
        {
            Console.WriteLine("ExecuteCommand - STOR");
            ConsoleLogger.GetProtocolLog(requestInfo);      /// log

            if (session.FileContext.FilePath == String.Empty)
            {
                session.FileContext.FilePath = session.AppServer.ServiceProvider.GetRandomName();
            }

            /// ServiceProvider.StoreFile에서 ServerContext 경로로 변환해주기 때문에
            /// virtualPath만 넣어주면 됨.
            string storeFilePath = Path.Combine(
                session.AppServer.ServerContext.TempFileDirectory,
                session.FileContext.FilePath);

            try 
            {
                string zipFilePath = storeFilePath + ".zip";

                if (session.AppServer.ServiceProvider.StoreFile(
                    session.AppServer.ServerContext, zipFilePath, session.FileContext, requestInfo))
                {
                    if (session.FileContext.TotalReadSize >= session.FileContext.FileSize)
                    {
                        /// 압축 풀기
                        string uncompressDir = session.AppServer.ServiceProvider.GetStoragePath(
                            session.AppServer.ServerContext, storeFilePath);
                        DirectoryInfo dir = new DirectoryInfo(uncompressDir);
                        
                        if (dir.Exists == true) {
                            /// 읽기 전용 파일이 있을 경우 Exception이 발생할 수 있음
                            /// 이럴 때, 해당 파일들의 읽기 전용 속성을 없애주면 삭제가 됨
                            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);

                            foreach (FileInfo file in files)
                            {
                                file.Attributes = FileAttributes.Normal;
                            }

                            dir.Delete(true);
                            dir.Create();
                        }
                        else
                        {
                            dir.Create();
                        }

                        ZipFile.ExtractToDirectory(zipFilePath, uncompressDir);

                        BingleHeader header = new BingleHeader(
                            BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                        BingleProtocol<BingleHeader, BodySTAT> protocolSTAT 
                            = new BingleProtocol<BingleHeader, BodySTAT>(
                            CommandList.STAT, header, new BodySTAT(StatusData.RequestActionCompleted_200));

                        /// STAT 전송(responseCode : 200)
                        session.Send(protocolSTAT.GetBytes(), 0, protocolSTAT.GetSize());

                        long fileSize = session.AppServer.ServiceProvider.GetFileSize(storeFilePath);
                        
                        header = new BingleHeader(
                            BodyTYPE.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                        BingleProtocol<BingleHeader, BodyTYPE> protocolTYPE 
                            = new BingleProtocol<BingleHeader, BodyTYPE>(
                            CommandList.TYPE, header, new BodyTYPE(fileSize));

                        /// TYPE 전송
                        session.Send(protocolTYPE.GetBytes(), 0, protocolTYPE.GetSize());
                    }
                }
            }
            catch (SocketException e)
            {
                session.Logger.Error(e);
                Console.WriteLine("STOR - Failed to Send prococol");
            }
        }
    }
}
