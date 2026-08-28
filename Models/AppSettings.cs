namespace SystemCheck.Models
{
    public class AppSettings
    {
        public string CpuCores { get; set; }
        public string CpuClock { get; set; }
        public string Ram { get; set; }
        public string Storage { get; set; }
        public string Os { get; set; }
        public int GpuYear { get; set; }
        public string Directory { get; set; }
        public bool IsDarkTheme { get; set; }
    }
}