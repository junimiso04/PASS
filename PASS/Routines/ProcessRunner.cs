using PASS.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS.Routines
{
    internal static class ProcessRunner
    {
        internal static ResultInformation Run(string path, string arguments, int timeLimit = 1000)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Invalid file path format.");

            if (!File.Exists(path))
                throw new Exception("The file does not exist.");

            ResultInformation result = new ResultInformation();
            result.TimeLimit = timeLimit;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = path;
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            using (Process process = Process.Start(startInfo))
            {

                if (!process.HasExited)
                {
                    process.Refresh();

                    result.PhysicalMemoryUsage = process.WorkingSet64;
                    result.BasePriority = process.BasePriority;
                    result.UserProcessorTime = process.UserProcessorTime;
                    result.PrivilegedProcessorTime = process.PrivilegedProcessorTime;
                    result.TotalProcessorTime = process.TotalProcessorTime;
                    result.PagedSystemMemorySize = process.PagedSystemMemorySize64;
                    result.PagedMemorySize = process.PagedMemorySize64;

                    result.PeakPagedMemorySize = process.PeakPagedMemorySize64;
                    result.PeakVirtualMemorySize = process.PeakVirtualMemorySize64;
                    result.PeakWorkingSet = process.PeakWorkingSet64;

                    System.Threading.Thread.Sleep(timeLimit);
                }
                process.Kill();

                result.ExitCode = process.ExitCode;
                result.OutputResult = process.StandardOutput.ReadToEnd();
                result.ErrorResult = process.StandardError.ReadToEnd();
            }
            return result;
        }
    }
}
