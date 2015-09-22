using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.CustomProcess
{
    /// <summary>
    /// - 가정 : Session은 무조건 연결되어있음
    /// </summary>
    public static class NadirCap
    {
        public static int Run(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                return 1;
            }

            ProcessStartInfo procInfo = new ProcessStartInfo();

            procInfo.WindowStyle = ProcessWindowStyle.Normal;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardInput = false;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            procInfo.FileName = "sh";
            procInfo.Arguments = "./Tools/nadircap.sh" + " " + imagePath;

            Console.WriteLine("ImagePath - {0}", imagePath);        // for test
            
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

                    //Wait process for 1 sec to end.
                    process.WaitForExit();

                    if (process.ExitCode == 0)       // success
                    {
                        Console.WriteLine("NadirCap - Success");
                        return 0;
                    }
                    else       // some error
                    {
                        Console.Error.WriteLine("NadirCap - Process Exit Error : {0}", process.ExitCode);
                        Console.Error.WriteLine(process.StandardError.ReadToEnd());
                        // fail to transform, but not disconnected, not fatal error.

                        return 1;
                    }
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
