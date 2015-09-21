using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingle.Server.CustomProcess
{
    public static class NadirCap
    {
        public static int Run(string imagePath, BingleSession session)
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
            procInfo.FileName = "./Tools/nadircap.sh";
            procInfo.Arguments = imagePath;

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

                    while (!process.HasExited)
                    {
                        if (!session.Connected)
                        {
                            process.Kill();
                            return 1;
                        }

                        //Wait process for 1 sec to end.
                        process.WaitForExit(1000);
                    }

                    if (process.ExitCode == 0)       // success
                    {
                        Console.WriteLine("NadirCap - Success");
                        return 0;
                    }
                    else       // some error
                    {
                        Console.Error.WriteLine("NadirCap - Process Exit Error : {0}", process.ExitCode);
                        session.Logger.Error(process.StandardError.ReadToEnd());
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
