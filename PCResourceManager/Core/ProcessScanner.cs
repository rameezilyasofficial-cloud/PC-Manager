using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices; // Needed for the Fix
using System.Globalization;
using PCResourceManager.Models;
using PCResourceManager.Utils;

namespace PCResourceManager.Core
{
    public static class ProcessScanner
    {
        // --- 1. THE FIX: DEFINE THE WINDOWS KERNEL TOOLS ---
        [DllImport("kernel32.dll")]
        static extern bool GetProcessIoCounters(IntPtr ProcessHandle, out IO_COUNTERS IoCounters);

        [StructLayout(LayoutKind.Sequential)]
        struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;  // This is what we need
            public ulong WriteTransferCount; // This is what we need
            public ulong OtherTransferCount;
        }
        // ----------------------------------------------------

        public static List<ProcessInfo> GetRunningProcesses()
        {
            var list = new List<ProcessInfo>();

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    // Basic Memory Check
                    long mem = process.WorkingSet64 / (1024 * 1024);
                    if (mem < 5) continue; // Filter junk

                    string path = "";
                    string officialName = "";

                    // Try to get Path and Name
                    try
                    {
                        path = process.MainModule?.FileName;
                        if (!string.IsNullOrEmpty(path))
                        {
                            var versionInfo = FileVersionInfo.GetVersionInfo(path);
                            officialName = versionInfo.FileDescription;
                        }
                    }
                    catch { }

                    if (string.IsNullOrEmpty(officialName))
                        officialName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(process.ProcessName);

                    string status = !string.IsNullOrEmpty(process.MainWindowTitle) ? process.MainWindowTitle : "Background Service";

                    // --- 2. THE FIX: GET DATA USAGE SAFELY ---
                    string dataText = "0 MB";
                    try
                    {
                        IO_COUNTERS counters;
                        // Ask Windows Kernel for the data stats
                        if (GetProcessIoCounters(process.Handle, out counters))
                        {
                            // Sum of all bytes downloaded/uploaded/read/written
                            ulong totalBytes = counters.ReadTransferCount + counters.WriteTransferCount;

                            if (totalBytes > 1024 * 1024 * 1024)
                                dataText = $"{(totalBytes / (1024.0 * 1024 * 1024)):F1} GB Data";
                            else
                                dataText = $"{(totalBytes / (1024.0 * 1024)):F0} MB Data";
                        }
                    }
                    catch
                    {
                        // If access denied (System Process), just ignore
                        dataText = "N/A";
                    }
                    // -----------------------------------------

                    list.Add(new ProcessInfo
                    {
                        Name = process.ProcessName,
                        DisplayName = officialName,
                        Description = status,
                        Pid = process.Id,
                        MemoryMB = mem,
                        Path = path,
                        Icon = IconHelper.GetIcon(path),
                        DataUsageText = dataText
                    });
                }
                catch { }
            }

            // Sort: Apps with Titles -> Memory
            return list
                .OrderByDescending(p => !string.IsNullOrEmpty(p.Description) && p.Description != "Background Service")
                .ThenByDescending(p => p.MemoryMB)
                .ToList();
        }
    }
}