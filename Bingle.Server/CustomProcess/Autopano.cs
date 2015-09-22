using Bingle.Common;
using Bingle.Server.Data.APSConfig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.CustomProcess
{
    public static class Autopano
    {
        /// <summary>
        /// Run Autopano Server
        /// - 가정 : Session은 무조건 연결되어있음
        /// - Error 발생시 처리는 Command로 넘기기(너무 복잡해지고 필요없는 일을 한다)
        /// </summary>
        /// <param name="fullXmlPath"></param>
        /// <returns>0 - success, 1 - fail</returns>
        public static int Run(string fullXmlPath)
        {            
            if (!File.Exists(fullXmlPath)) {
                return 1;
            }

            ProcessStartInfo procInfo = new ProcessStartInfo();

            procInfo.WindowStyle = ProcessWindowStyle.Normal;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardInput = false;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            procInfo.FileName = "./Tools/AutopanoServer/AutopanoServer";
            procInfo.Arguments = "xml=" + fullXmlPath;

            // change as Windows path
            if (Path.DirectorySeparatorChar == '\\')
            {
                procInfo.FileName = Bingle.Common.StringUtil.ReverseSlash(procInfo.FileName, '/');
                procInfo.Arguments = Bingle.Common.StringUtil.ReverseSlash(procInfo.Arguments, '/');
            }

            using (Process process = new Process())
            {
                process.StartInfo = procInfo;
                process.EnableRaisingEvents = true;
                process.OutputDataReceived
                        += (sender, arguments) => Console.WriteLine(arguments.Data);
                var started = process.Start();

                if (started)
                {
                    process.BeginOutputReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0)       // success
                    {
                        Console.WriteLine("Autopano - Success transform image");

                        return 0;
                    }
                    else       // some error
                    {
                        Console.Error.WriteLine("Autopano - Process Exit Error : {0}", process.ExitCode);
                        Console.Error.WriteLine(process.StandardError.ReadToEnd());
                        return 1;
                    }
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Autopano Server Config
        /// </summary>
        /// <param name="storeFolderPath"></param>
        /// <param name="storeFileName"></param>
        /// <param name="outputFolderPath"></param>
        /// <returns>xml path of Autopano Config file</returns>
        public static string SetAPS(string storeFolderPath, string storeFileName, string outputFolderPath)
        {
            // Autopano Server config xml파일 생성
            APSConfig config = new APSConfig(storeFolderPath, outputFolderPath);
            config.application.log = 2;     // for Debug

            string fullXmlPath = Path.Combine(storeFolderPath, storeFileName + ".xml");
            XmlSerializerUtil.Serialize(fullXmlPath, config);

            return fullXmlPath;
        }
    }
}
