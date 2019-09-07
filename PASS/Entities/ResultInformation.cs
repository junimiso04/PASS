using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS.Entities
{
    internal class ResultInformation
    {
        internal int TimeLimit;

        internal double PhysicalMemoryUsage;
        internal double BasePriority;

        internal TimeSpan UserProcessorTime;
        internal TimeSpan PrivilegedProcessorTime;
        internal TimeSpan TotalProcessorTime;

        internal double PagedSystemMemorySize;
        internal double PagedMemorySize;

        internal double PeakPagedMemorySize;
        internal double PeakVirtualMemorySize;
        internal double PeakWorkingSet;

        internal string OutputResult;
        internal string ErrorResult;

        internal int ExitCode;
    }
}
