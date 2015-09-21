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
    /// 데이터(Binary) 송/수신, 파일 경로 관리 
    /// - 파일 저장 방법을 감추기 위해서 abstract class, protected로 선언할 수 있음
    /// - 현재는 Server Root Path 기준(ServerContext)이지만,
    ///   향후에는 계정별 파일 관리를 위하여 사용자별(BingleContext)로 변경 되어야 한다.
    /// - 속도가 느리기 때문에 파일 송/수신을 직접 구현하지말고, 라이브러리를 가져다 쓰기.
    /// - Connection 연결, 데이터 수신 여부 처리가 어렵다. 가져다 쓰자! 
    /// </summary>
    public class BingleServiceProvider
    {
        protected BingleServer AppServer { get; private set; }

        public virtual bool StoreFile(ServerContext context, string filename, Stream stream, long storeLength)
        {
            return StoreFile(context, filename, stream, new StoreOption(storeLength));
        }

        /// <summary>
        /// 현재는 session에 저장되어 있는 단일 stream으로 하기 때문에 
        /// 차후 추가 connection을 열어서 하는 방법으로 수정해야함.
        /// </summary>
        /// <param name="serverContext"></param>
        /// <param name="fileContext">session.FileContext</param>
        /// <param name="bingleProtocol"></param>
        /// <returns></returns>
        public bool StoreFile(ServerContext context, string fileName, Stream stream, StoreOption option)
        {
            int bufLen = 1024 * 128;
            byte[] buffer = new byte[bufLen];
            int read = 0;
            long totalRead = 0;

            FileStream fs = null;

            try
            {
                string filePath = GetStoragePath(context, fileName);

                Console.WriteLine("BingleServerProvider - Store File Path : {0}", filePath);       // for debug

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                fs = new FileStream(
                    filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, bufLen);

                while (totalRead < option.TotalRead)
                {
                    if((read = stream.Read(buffer, 0, bufLen)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                        totalRead += read;
                        Console.WriteLine("totalRead - {0}", totalRead);
                    }
                }

                Console.WriteLine("BingleServiceProvider - StoreFile Complete : TotalRead : {0}", totalRead);

                return true;
            }
            catch (IOException e)
            {
                AppServer.Logger.Error(e);
                Console.WriteLine("BingleServiceProvider - Store file Error");
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

        /// <summary>
        /// 현재는 session에 저장되어 있는 단일 stream으로 하기 때문에 
        /// 차후 추가 connection을 열어서 하는 방법으로 수정해야함.
        /// </summary>
        /// <param name="serverContext">BingleServer.ServerContext</param>
        /// <param name="fileContext">session.FileContext</param>
        /// <param name="bingleSession"></param>
        /// <returns></returns>
        public virtual bool SendFile(ServerContext serverContext, string fileName,
            Stream stream)
        {
            int bufLen = 1024 * 128;
            byte[] buffer = new byte[bufLen];
            int read = 0;

            FileStream fs = null;

            try
            {
                string filePath = GetStoragePath(serverContext, fileName);

                // (test), serverContext와 각종 데이터와 조합된 이름을 사용해야함
                //string filePath = fileContext.SendFilePath;

                Console.WriteLine("BingleServiceProvider - Send File Path : {0}", filePath);

                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufLen);

                while ((read = fs.Read(buffer, 0, bufLen)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }

                Console.WriteLine("BingleServiceProvider - Send File Complete : {0}", filePath);

                return true;
            }
            catch (Exception e)
            {
                AppServer.Logger.Error(e);
                Console.WriteLine("BingleServiceProvider - Failed to Send file");
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
            return Path.Combine(context.RootPath, virtualPath);
        }

        public string GetRandomName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssFF");
        }
    }
}
