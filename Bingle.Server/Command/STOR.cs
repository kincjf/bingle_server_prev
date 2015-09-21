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
using Bingle.Server.Data.APSConfig;
using Bingle.Common;
using System.Diagnostics;
using Bingle.Server.CustomProcess;

namespace Bingle.Server.Command
{
    /// <summary>
    /// 패킷(파일)을 저장하고 Spherical image로 변환함.
    /// - Sphere 변환 도중 연결이 끊겼을 경우에 대한 처리가 필요함.
    /// - 동작에 대한 Error 처리는 해당 Class에서 마무리하자.
    /// - Error처리에 신경쓰다보면 너무 if-else가 많아지고 불 필요한 Exception이 생긴다.
    /// </summary>
    public class STOR : CommandBase<BingleSession, BingleProtocol>
    {
        //#region Receive File and Send File 
        public override void ExecuteCommand(BingleSession session, BingleProtocol requestInfo)
        {
            Console.WriteLine("ExecuteCommand - STOR");
            ConsoleLogger.GetProtocolLog(requestInfo);      /// log

            //if (!session.Logged)
            //    return;

            // ServiceProvider.StoreFile에서 ServerContext 경로로 변환해주기 때문에
            // virtualPath만 넣어주면 됨.
            string storeFileName = session.AppServer.ServiceProvider.GetRandomName();

            string imageFileName = storeFileName + ".jpg";

            string storeFolderPath = Path.Combine(session.AppServer.ServerContext.TempFileDirectory, storeFileName);
            string fullZipFilePath = storeFolderPath + ".zip";
            string fullImageFilePath = Path.Combine(
                session.AppServer.ServerContext.ImageFileDirectory, imageFileName);

            BingleHeader header = null;
            BingleProtocol<BingleHeader, BodySTAT> protocolSTAT = null;
            BingleProtocol<BingleHeader, BodyTYPE> protocolTYPE = null;

            DataConnection dataConn = session.DataConnection;

            if (dataConn.RunDataConnection().Wait(60 * 1000))
            {
                Stream stream = dataConn.GetStream(session.Context);

                try
                {
                    if (session.AppServer.ServiceProvider.StoreFile(
                    session.AppServer.ServerContext, fullZipFilePath, stream, session.Context.TempFileSize))
                    {
                        // 파일 경로를 미리 저장해놔야 나중에 사용이 가능함.
                        session.Context.TempFileName = storeFileName;
                        session.Context.TempFileSize = 0;       // for reuse

                        // 압축 풀기
                        string uncompressDir = session.AppServer.ServiceProvider.GetStoragePath(
                            session.AppServer.ServerContext, storeFolderPath);
                        DirectoryInfo dir = new DirectoryInfo(uncompressDir);

                        if (dir.Exists == true)
                        {
                            // 읽기 전용 파일이 있을 경우 Exception이 발생할 수 있음
                            // 이럴 때, 해당 파일들의 읽기 전용 속성을 없애주면 삭제가 됨
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

                        ZipFile.ExtractToDirectory(fullZipFilePath, uncompressDir);

                        header = new BingleHeader(
                            BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                        protocolSTAT = new BingleProtocol<BingleHeader, BodySTAT>(
                            CommandList.STAT, header, new BodySTAT(StatusData.RequestActionCompleted_200));

                        // STAT 전송(responseCode : 200)
                        session.Send(protocolSTAT.GetBytes(), 0, protocolSTAT.GetSize());

                        // Autopano Server 설정
                        string fullXmlPath = Autopano.SetAPS(storeFolderPath, storeFileName, session.AppServer.ServerContext.ImageFileDirectory);

                        // session connect 여부와 변환 결과 여부를 동시에 확인하기 위해서는
                        // interprocess-communication을 활용해야 할 것 같다.
                        // @link http://stackoverflow.com/questions/4123923/synchronizing-2-processes-using-interprocess-synchronizations-objects-mutex-or
                        // Sphere 변환                      
                        var status = Autopano.Run(fullXmlPath, session);
                        
                        // nadir cap capsulation
                        if (status == 0)
                        {
                            status = NadirCap.Run(fullImageFilePath, session);
                            
                            if (status == 0)
                            {
                                Console.WriteLine("STOR - Trandformation Success!");
                            }
                            else
                            {
                                Console.Error.WriteLine("NadirCap Error");
                                throw new Exception("NadirCap Error");
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine("STOR - Fail to transformation image");
                            throw new Exception("Autopano Error");
                        }

                        long fileSize = session.AppServer.ServiceProvider.GetFileSize(fullImageFilePath);
                        //string testFullImageFilePath = Path.Combine(
                        //  session.AppServer.ServerContext.ImageFileDirectory, "test360Image.jpg");     // for test
                        //long fileSize = session.AppServer.ServiceProvider.GetFileSize(testFullImageFilePath);       // for test

                        header = new BingleHeader(
                            BodyTYPE.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                        protocolTYPE = new BingleProtocol<BingleHeader, BodyTYPE>(
                            CommandList.TYPE, header, new BodyTYPE(fileSize));

                        // TYPE 전송
                        session.Send(protocolTYPE.GetBytes(), 0, protocolTYPE.GetSize());
                    }
                }
                catch (SocketException e)
                {
                    session.Logger.Error(e);

                    header = new BingleHeader(BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                    protocolSTAT = new BingleProtocol<BingleHeader, BodySTAT>(
                        CommandList.STAT, header, new BodySTAT(StatusData.DataConnectionError_426));

                    session.Send(protocolSTAT.GetBytes(), 0, protocolSTAT.GetSize());
                    session.Close();

                    Console.Error.WriteLine("STOR - Data Connection Error");
                }
                catch (Exception e)
                {
                    session.Logger.Error(e);

                    header = new BingleHeader(BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                    protocolSTAT = new BingleProtocol<BingleHeader, BodySTAT>(
                        CommandList.STAT, header, new BodySTAT(StatusData.OutputFileError_551));

                    session.Send(protocolSTAT.GetBytes(), 0, protocolSTAT.GetSize());
                    session.Close();

                    Console.Error.WriteLine("STOR - Output File Error");
                }
            }
            else
            {
                header = new BingleHeader(BodySTAT.SIZE, StatusData.NOT_FRAGMENTED, StatusData.LASTMSG, 0);
                protocolSTAT = new BingleProtocol<BingleHeader, BodySTAT>(
                    CommandList.STAT, header, new BodySTAT(StatusData.DataConnectionCannotOpen_420));

                session.Send(protocolSTAT.GetBytes(), 0, protocolSTAT.GetSize());
                session.Close();

                Console.Error.WriteLine("STOR - DataConnection cannot open");
            }
        }
    }
}
