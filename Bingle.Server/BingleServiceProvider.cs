using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using SuperSocket.Common;

using Bingle.Server;
using Bingle.Server.Data;
using Bingle.Server.Data.Body;
using Bingle.Server.MetaData;
using System.Net.Sockets;

namespace Bingle.Server
{
    /// <summary>
    /// 파일 저장 방법을 감추기 위해서 abstract class, protected로 선언할 수 있음
    /// </summary>
    public class BingleServiceProvider
    {
        protected BingleServer AppServer { get; private set; }

        /// <summary>
        /// 현재는 session에 저장되어 있는 단일 stream으로 하기 때문에 
        /// 차후 추가 connection을 열어서 하는 방법으로 수정해야함.
        /// </summary>
        /// <param name="serverContext"></param>
        /// <param name="fileContext">session.FileContext</param>
        /// <param name="bingleProtocol"></param>
        /// <returns></returns>
        public bool StoreFile(ServerContext serverContext, string fileName,
            FileContext fileContext, BingleProtocol bingleProtocol)
        {
            int bufLen = 1024 * 4;

            try
            {
                if (fileContext.FileDataStream == null)
                {
                    // (test), serverContext와 각종 데이터와 조합된 이름을 사용해야함
                    //string filePath = fileName;
                    string filePath = GetStoragePath(serverContext, fileName);

                    Console.WriteLine("Store File Path - {0}", filePath);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    fileContext.FileDataStream = new FileStream(
                        filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, bufLen);
                }

                fileContext.FileDataStream.Write(bingleProtocol.Body, 0, bingleProtocol.Header.BodyLength);
                fileContext.TotalReadSize += bingleProtocol.Header.BodyLength;

                if (fileContext.TotalReadSize >= fileContext.FileSize)
                {
                    /// 임시 - 또 다른 요청이 있을 수 있기 때문에 초기화 해야함.
                    /// connection을 여는 방법을 써야 할 것 같다.
                    if (fileContext.FileDataStream != null)
                    {
                        fileContext.FileDataStream.Close();
                        fileContext.FileDataStream.Dispose();
                        fileContext.FileDataStream = null;
                    }
                }


                return true;
            }
            catch (IOException e)
            {
                AppServer.Logger.Error(e);
                Console.WriteLine("BingleServiceProvider - Store file Error");
                return false;
            }
        }

        /// <summary>
        /// 현재는 session에 저장되어 있는 단일 stream으로 하기 때문에 
        /// 차후 추가 connection을 열어서 하는 방법으로 수정해야함.
        /// </summary>
        /// <param name="serverContext">BingleServer.ServerContext</param>
        /// <param name="fileContext">session.FileContext</param>
        /// <param name="bingleSession"></param>
        /// <returns></returns>
        public bool SendFile(ServerContext serverContext, string fileName,
            BingleSession session)
        {
            int bufLen = 1024 * 4;
            byte[] buffer = new byte[bufLen];
            int read = 0;

            byte fragmented = StatusData.FRAGMENTED;
            byte lastMsg = StatusData.NOT_LASTMSG;
            int sequence = 0;

            FileStream fs = null;
            BingleHeader header = null;
            BingleProtocol<BingleHeader, BodySTOR> protocol = null;

            try
            {
                string filePath = GetStoragePath(serverContext, fileName);

                // (test), serverContext와 각종 데이터와 조합된 이름을 사용해야함
                //string filePath = fileContext.SendFilePath;

                Console.WriteLine("Send File Path - {0}", filePath);

                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufLen);

                while ((read = fs.Read(buffer, 0, bufLen)) > 0)
                {
                    if (read < bufLen)
                    {
                        if (sequence > 0)
                        {
                            fragmented = StatusData.FRAGMENTED;
                        }
                        else
                        {
                            fragmented = StatusData.NOT_FRAGMENTED;
                        }

                        lastMsg = StatusData.LASTMSG;
                    }

                    header = new BingleHeader(read, fragmented, lastMsg, sequence);
                    protocol = new BingleProtocol<BingleHeader, BodySTOR>(
                        CommandList.STOR, header, new BodySTOR(buffer.CloneRange(0, read)));

                    session.Send(protocol.GetBytes(), 0, protocol.GetSize());

                    ++sequence;
                }

                return true;
            }
            catch (IOException e)
            {
                AppServer.Logger.Error(e);
                Console.WriteLine("BingleServiceProvider - Failed to Send file");
                return false;
            }
            catch (SocketException e)
            {
                AppServer.Logger.Error(e);
                Console.WriteLine("BingleServiceProvider - Data Connection Error");
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
        }

        public long GetFileSize(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileInfo file = new FileInfo(fileName);
                return file.Length;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// virtualPath는 무조건 상대경로로 인식함.
        /// ServerContext.FileRootPath + virtualPath
        /// example : /image/abc.jpg -> image/abc.jpg
        /// </summary>
        public string GetStoragePath(ServerContext context, string virtualPath)
        {
            string storagePath = GetStoragePathInternal(context, virtualPath);

            // change as Windows path
            if (Path.DirectorySeparatorChar == '\\')
            {
                storagePath = Bingle.Common.StringUtil.ReverseSlash(storagePath, '/');
            }

            return storagePath;
        }

        /// <summary>
        /// virtualPath를 상대경로로 변환 후 기준 경로와 병합함.
        /// </summary>
        protected virtual string GetStoragePathInternal(ServerContext context, string virtualPath)
        {
            virtualPath = virtualPath.TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(context.FileRootPath, virtualPath);
        }

        public string GetRandomName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssFF");
        }
    }
}
