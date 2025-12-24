using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PCResourceManager.Core
{
    public static class SystemMonitor
    {
        // We ask kernel32.dll for the "IO Counters" structure
        [DllImport("kernel32.dll")]
        private static extern bool GetProcessIoCounters(IntPtr ProcessHandle, out IO_COUNTERS IoCounters);

        // The structure Windows returns
        [StructLayout(LayoutKind.Sequential)]
        struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;  // Bytes Downloaded/Read
            public ulong WriteTransferCount; // Bytes Uploaded/Written
            public ulong OtherTransferCount;
        }

        // Returns the TRUE Total Bytes (Read + Write) used by the app
        public static long GetTotalBytes(int pid)
        {
            try
            {
                // We must get a "Handle" to the process to ask about it
                using (var process = Process.GetProcessById(pid))
                {
                    IO_COUNTERS counters;

                    if (GetProcessIoCounters(process.Handle, out counters))
                    {
                        // Combine Downloads (Read) + Uploads (Write)
                        return (long)(counters.ReadTransferCount + counters.WriteTransferCount);
                    }
                }
            }
            catch
            {
                // If the app closed or we don't have permission (System app)
                return 0;
            }
            return 0;
        }
    }
}