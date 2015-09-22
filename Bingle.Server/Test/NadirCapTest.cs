using Bingle.Server.CustomProcess;
using Bingle.Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.Test
{
    public class NadirCapTest
    {
        static void Main(string[] args)
        {
            // Mock instance
            BingleSession session = new BingleSession();

            // ServiceProvider.StoreFile에서 ServerContext 경로로 변환해주기 때문에
            // virtualPath만 넣어주면 됨.
            string storeFileName = "2015092304200382";
            string imageFileName = storeFileName + ".jpg";

            ServerContext ServerContext = new ServerContext(".", 2021);
            BingleServiceProvider ServiceProvider = new BingleServiceProvider();

            Console.WriteLine("File Save Path - \"{0}\"", ServerContext.RootPath);

            string tempDirectory = ServiceProvider.GetStoragePath(
                ServerContext, ServerContext.TempFileDirectory);
            string imageDirectory = ServiceProvider.GetStoragePath(
                ServerContext, ServerContext.ImageFileDirectory);

            string storeFolderPath = Path.Combine(ServerContext.TempFileDirectory, storeFileName);
            string fullZipFilePath = storeFolderPath + ".zip";
            string fullImageFilePath = Path.Combine(ServerContext.ImageFileDirectory, imageFileName);

            var status = NadirCap.Run(fullImageFilePath);

            if (status == 0)
            {
                Console.WriteLine("NadirCapTest - Trandformation Success!");
            }
            else
            {
                Console.Error.WriteLine("NadirCap Error");
            }
        }
    }
}
