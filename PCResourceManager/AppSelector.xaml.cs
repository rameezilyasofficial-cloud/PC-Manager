using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PCResourceManager.Models;

namespace PCResourceManager
{
    public partial class AppSelector : Window
    {
        public ProcessInfo SelectedProcess { get; private set; }

        public AppSelector(List<ProcessInfo> processes)
        {
            InitializeComponent();
            ProcessList.ItemsSource = processes;
        }

        // This runs when you click the "Select" button on ANY row
        private void ItemSelect_Click(object sender, RoutedEventArgs e)
        {
            // 1. Get the button that was clicked
            Button btn = (Button)sender;

            // 2. Retrieve the data attached to that button
            SelectedProcess = (ProcessInfo)btn.Tag;

            // 3. Close and return success
            DialogResult = true;
            Close();
        }
    }
}
