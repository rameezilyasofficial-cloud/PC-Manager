using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PCResourceManager.Core
{
    public static class ProcessThrottler
    {
        [DllImport("ntdll.dll", PreserveSig = false)]
        public static extern void NtSuspendProcess(IntPtr processHandle);

        [DllImport("ntdll.dll", PreserveSig = false)]
        public static extern void NtResumeProcess(IntPtr processHandle);

        private static bool _isRunning = false;
        private static int _targetPid = 0;
        private static int _throttlePercentage = 0; // 0 to 100

        // This runs a background loop to Pause/Resume the app
        public static async void StartThrottling(int pid, int percentage)
        {
            // If already targeting this app, just update percentage
            if (_isRunning && _targetPid == pid)
            {
                _throttlePercentage = percentage;
                return;
            }

            // Stop any old throttle
            StopThrottling();

            _targetPid = pid;
            _throttlePercentage = percentage;
            _isRunning = true;

            await Task.Run(() =>
            {
                try
                {
                    Process p = Process.GetProcessById(pid);
                    IntPtr handle = p.Handle;

                    while (_isRunning)
                    {
                        if (_throttlePercentage > 0)
                        {
                            // CALCULATION: A 1000ms cycle
                            // If 20% throttle: Sleep 200ms (Frozen), Run 800ms (Active)
                            int freezeTime = _throttlePercentage * 10;
                            int activeTime = 1000 - freezeTime;

                            // 1. FREEZE
                            NtSuspendProcess(handle);
                            Thread.Sleep(freezeTime);

                            // 2. UNFREEZE
                            NtResumeProcess(handle);
                            Thread.Sleep(activeTime);
                        }
                        else
                        {
                            Thread.Sleep(500); // Do nothing if 0%
                        }
                    }

                    // Always make sure it's running when we quit
                    NtResumeProcess(handle);
                }
                catch
                {
                    _isRunning = false;
                }
            });
        }

        public static void StopThrottling()
        {
            _isRunning = false;
            // Give it a moment to wake up
            Thread.Sleep(100);
        }
    }
}