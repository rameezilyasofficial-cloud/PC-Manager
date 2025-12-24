using Microsoft.Win32;
using System;
using System.Windows;

namespace PCResourceManager
{
    public partial class SettingsWindow : Window
    {
        // Reference to the main window so we can change its properties
        private MainWindow _main;

        public SettingsWindow(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            // 1. Load current state of "Always on Top"
            TopMostCheck.IsChecked = _main.Topmost;

            // 2. Load current state of "Startup" from Registry
            StartupCheck.IsChecked = CheckStartup();
        }

        // --- ALWAYS ON TOP LOGIC ---
        private void TopMost_Changed(object sender, RoutedEventArgs e)
        {
            _main.Topmost = TopMostCheck.IsChecked == true;
        }

        // --- STARTUP LOGIC ---
        private void Startup_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyName = "PCResourceManager";
                string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (StartupCheck.IsChecked == true)
                    key.SetValue(keyName, exePath); // Add to startup
                else
                    key.DeleteValue(keyName, false); // Remove from startup
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update startup settings: " + ex.Message);
            }
        }

        private bool CheckStartup()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                return key.GetValue("PCResourceManager") != null;
            }
            catch { return false; }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}