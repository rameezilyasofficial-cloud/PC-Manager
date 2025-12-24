using System.Windows.Media;

namespace PCResourceManager.Models
{
    public class ProcessInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Pid { get; set; }
        public long MemoryMB { get; set; }
        public string Path { get; set; }
        public ImageSource Icon { get; set; }

        // NEW: Stores Total Data (Network + Disk)
        public string DataUsageText { get; set; }
    }
}