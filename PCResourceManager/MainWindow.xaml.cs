using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Diagnostics;
using PCResourceManager.Core;
using PCResourceManager.Models;

namespace PCResourceManager
{
    public partial class MainWindow : Window
    {
        private ProcessInfo _ramApp;
        private ProcessInfo _netApp;
        private long _previousTotalBytes = 0;
        private bool _isLimiting = false;
        private DispatcherTimer _liveTimer;

        // Timer just for the dashboard list (updates slower to save CPU)
        private int _dashboardTickCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            SetupLiveMonitor();
            // Start on Dashboard
            UpdateDashboardList();
        }

        private void SetupLiveMonitor()
        {
            _liveTimer = new DispatcherTimer();
            _liveTimer.Interval = TimeSpan.FromSeconds(1);
            _liveTimer.Tick += UpdateDashboard_Tick;
            _liveTimer.Start();
        }

        private void UpdateDashboard_Tick(object sender, EventArgs e)
        {
            // 1. UPDATE RAM CONTROLS (Always runs in background)
            if (_ramApp != null)
            {
                try
                {
                    Process p = Process.GetProcessById(_ramApp.Pid);
                    p.Refresh();
                    long currentMem = p.WorkingSet64 / (1024 * 1024);
                    ProcessSelectBtn.Content = $"✅ {_ramApp.DisplayName} : {currentMem} MB";
                }
                catch { ProcessSelectBtn.Content = "App Closed"; }
            }

            // 2. UPDATE NETWORK CONTROLS (Always runs in background)
            if (_netApp != null)
            {
                try
                {
                    long currentTotalBytes = SystemMonitor.GetTotalBytes(_netApp.Pid);
                    long bytesDiff = (_previousTotalBytes > 0) ? (currentTotalBytes - _previousTotalBytes) : 0;
                    long speedKB = bytesDiff / 1024;
                    _previousTotalBytes = currentTotalBytes;

                    string speedText = speedKB > 1024
                        ? $"{(speedKB / 1024.0):F1} MB/s"
                        : $"{speedKB} KB/s";

                    NetSelectBtn.Content = $"✅ {_netApp.DisplayName} | Activity: {speedText}";

                    // Limiter Logic
                    if (_isLimiting && long.TryParse(SpeedLimitBox.Text, out long maxSpeedKB))
                    {
                        if (maxSpeedKB <= 0) maxSpeedKB = 1;

                        if (speedKB > maxSpeedKB)
                        {
                            ProcessThrottler.StartThrottling(_netApp.Pid, 90);
                            NetSelectBtn.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            ProcessThrottler.StopThrottling();
                            NetSelectBtn.Foreground = new SolidColorBrush(Colors.Green);
                        }
                    }
                }
                catch
                {
                    NetSelectBtn.Content = "App Closed";
                    _isLimiting = false;
                    ToggleLimitBtn.Content = "Enable Limit";
                    ToggleLimitBtn.Background = new SolidColorBrush(Color.FromRgb(16, 124, 16));
                }
            }

            // 3. UPDATE DASHBOARD LIST (Every 3 seconds)
            _dashboardTickCounter++;
            if (_dashboardTickCounter >= 3)
            {
                _dashboardTickCounter = 0;
                // Only update if dashboard is actually visible
                if (DashboardView.Visibility == Visibility.Visible)
                {
                    UpdateDashboardList();
                }
            }
        }

        // --- DASHBOARD HELPER ---
        private void UpdateDashboardList()
        {
            // Get all apps, take top 5 by RAM
            var topApps = ProcessScanner.GetRunningProcesses()
                                        .OrderByDescending(p => p.MemoryMB)
                                        .Take(5)
                                        .ToList();
            TopAppsList.ItemsSource = topApps;
        }

        // --- NAVIGATION LOGIC ---
        private void Nav_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            // Show Dashboard, Hide others
            DashboardView.Visibility = Visibility.Visible;
            RamView.Visibility = Visibility.Collapsed;
            NetworkView.Visibility = Visibility.Collapsed;

            // Update Buttons (Dark Background = Active)
            NavDashBtn.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            NavRamBtn.Background = Brushes.Transparent;
            NavNetBtn.Background = Brushes.Transparent;

            UpdateDashboardList(); // Refresh list immediately
        }

        private void Nav_Ram_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            RamView.Visibility = Visibility.Visible;
            NetworkView.Visibility = Visibility.Collapsed;

            NavDashBtn.Background = Brushes.Transparent;
            NavRamBtn.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            NavNetBtn.Background = Brushes.Transparent;
        }

        private void Nav_Network_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            RamView.Visibility = Visibility.Collapsed;
            NetworkView.Visibility = Visibility.Visible;

            NavDashBtn.Background = Brushes.Transparent;
            NavRamBtn.Background = Brushes.Transparent;
            NavNetBtn.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow(this);
            settings.Owner = this;
            settings.ShowDialog();
        }

        // --- FEATURE BUTTONS ---
        private void BrowseProcesses_Click(object sender, RoutedEventArgs e)
        {
            var selector = new AppSelector(ProcessScanner.GetRunningProcesses());
            if (selector.ShowDialog() == true)
            {
                _ramApp = selector.SelectedProcess;
                RamInputBox.Text = "1024";
            }
        }

        private void ApplyRamLimit_Click(object sender, RoutedEventArgs e)
        {
            if (_ramApp != null && long.TryParse(RamInputBox.Text, out long limit))
                JobController.LimitProcessMemory(_ramApp.Pid, limit);
        }

        private void NetSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var selector = new AppSelector(ProcessScanner.GetRunningProcesses());
            if (selector.ShowDialog() == true)
            {
                _netApp = selector.SelectedProcess;
                _previousTotalBytes = SystemMonitor.GetTotalBytes(_netApp.Pid);
                NetSelectBtn.Background = new SolidColorBrush(Color.FromRgb(220, 255, 220));
            }
        }

        private void ToggleLimitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_netApp == null) { MessageBox.Show("Select an app first!"); return; }

            _isLimiting = !_isLimiting;

            if (_isLimiting)
            {
                ToggleLimitBtn.Content = "DISABLE LIMIT";
                ToggleLimitBtn.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                ToggleLimitBtn.Content = "Enable Limit";
                ToggleLimitBtn.Background = new SolidColorBrush(Color.FromRgb(16, 124, 16));
                ProcessThrottler.StopThrottling();
            }
        }
    }
}