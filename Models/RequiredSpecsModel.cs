using System.Windows.Media;

namespace SystemCheck.Models
{
    public class RequiredSpecsModel
    {
        public int MinCpuCores { get; set; }
        public double MinCpuClockGhz { get; set; }
        public int MinRamGb { get; set; }
        public int MinStorageGb { get; set; }
        public string RequiredOs { get; set; }
        public int RequiredGpuYear { get; set; }
        public string RequiredDirectory { get; set; }
    }

    public class CheckStatusItem
    {
        public string StatusText { get; set; }
        public Brush StatusColor { get; set; }
    }
}