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
    class BingleSession
    {
        public bool session = true;
        public void Logger = (string msg) =>
        {
            Console.WriteLine(msg);
        };
    }

    public class AutopanoTest
    {
        static void Main(string[] args)
        {
            // Mock instance
            BingleSession session = new BingleSession();

            // ServiceProvider.StoreFile에서 ServerContext 경로로 변환해주기 때문에
            // virtualPath만 넣어주면 됨.
            string storeFileName = "201509211855068";
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

            // Autopano Server 설정
            string fullXmlPath = Autopano.SetAPS(storeFolderPath, storeFileName, ServerContext.ImageFileDirectory);

            // Sphere 변환                      
            using (Process process = Autopano.Run(fullXmlPath))
            {
                while (!process.HasExited)
                {
                    Console.WriteLine("Autopano - Wait for Converting");

                    //Wait process for 1 sec to end.
                    process.WaitForExit(1000);
                }


                if (process.ExitCode == 0)       // success
                {
                    Console.WriteLine("Autopano - Success transform image");
                }
                else       // some error
                {
                    Console.Error.WriteLine("Autopano - Error in transform image");
                    return;
                }
            }

            // nadir cap capsulation
            using (Process process = NadirCap.Run(fullImageFilePath))
            {
                while (!process.HasExited)
                {
                    Console.WriteLine("Nadircap - Wait for Converting");

                    //Wait process for 1 sec to end.
                    process.WaitForExit(1000);
                }


                if (process.ExitCode == 0)       // success
                {
                    Console.WriteLine("Nadircap - Success transform ");
                }
                else       // some error
                {
                    Console.Error.WriteLine("Nadircap - Error in transform");
                    return;
                }
            }
        }
    }
}
