using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PCResourceManager.Core
{
    public static class JobController
    {
        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public Int64 PerProcessUserTimeLimit;
            public Int64 PerJobUserTimeLimit;
            public UInt32 LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public UInt32 ActiveProcessLimit;
            public UIntPtr Affinity;
            public UInt32 PriorityClass;
            public UInt32 SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct IO_COUNTERS
        {
            public UInt64 ReadOperationCount;
            public UInt64 WriteOperationCount;
            public UInt64 OtherOperationCount;
            public UInt64 ReadTransferCount;
            public UInt64 WriteTransferCount;
            public UInt64 OtherTransferCount;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll")]
        static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll")]
        static extern bool SetInformationJobObject(IntPtr hJob, int JobObjectInfoClass, ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, uint cbJobObjectInfoLength);

        public static string LimitProcessMemory(int pid, long limitInMB)
        {
            try
            {
                Process targetProcess = Process.GetProcessById(pid);

                IntPtr jobHandle = CreateJobObject(IntPtr.Zero, null);
                if (jobHandle == IntPtr.Zero) return "Error: Could not create Job.";

                var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION();

                // --- FIX IS HERE ---
                // Old Code: 0x1000 (Wrong)
                // New Code: 0x200 (JOB_OBJECT_LIMIT_JOB_MEMORY)
                info.BasicLimitInformation.LimitFlags = 0x200;
                // -------------------

                info.JobMemoryLimit = (UIntPtr)(limitInMB * 1024 * 1024);

                int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                bool success = SetInformationJobObject(jobHandle, 9, ref info, (uint)length);
                if (!success) return "Error: Could not set memory limit.";

                success = AssignProcessToJobObject(jobHandle, targetProcess.Handle);
                if (!success) return "Error: Access Denied. Try running Visual Studio as Admin.";

                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}