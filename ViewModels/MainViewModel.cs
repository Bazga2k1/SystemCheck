using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Management;
using System.Windows.Input;
using System.Windows.Media;
using SystemCheck.Commands;
using SystemCheck.Models;

namespace SystemCheck.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Dropdown Lists
        public List<string> AvailableOsList { get; private set; }
        public Dictionary<string, int> AvailableGpuGenerations { get; private set; }

        // Input Bindings
        public string InputCpuCores { get; set; }
        public string InputCpuClockGhz { get; set; }
        public string InputRamGb { get; set; }
        public string InputStorageGb { get; set; }
        public string SelectedOs { get; set; }
        public KeyValuePair<string, int> SelectedGpu { get; set; }
        public string InputDirectory { get; set; }

        // Output Status Bindings
        public string CurrentCpuCores { get; private set; }
        public string CurrentCpuClock { get; private set; }
        public string CurrentRam { get; private set; }
        public string CurrentStorage { get; private set; }
        public string CurrentGpu { get; private set; }
        public string CurrentOs { get; private set; }

        public Brush CpuCoreColor { get; private set; } = Brushes.Black;
        public Brush CpuClockColor { get; private set; } = Brushes.Black;
        public Brush RamColor { get; private set; } = Brushes.Black;
        public Brush StorageColor { get; private set; } = Brushes.Black;
        public Brush GpuColor { get; private set; } = Brushes.Black;
        public Brush OsColor { get; private set; } = Brushes.Black;
        public Brush DirectoryColor { get; private set; } = Brushes.Black;

        public ICommand CheckSystemCommand { get; private set; }

        public MainViewModel()
        {
            PopulateDropdowns();
            CheckSystemCommand = new RelayCommand(p => PerformSystemCheck());

            InputCpuCores = "4";
            InputCpuClockGhz = "2.5";
            InputRamGb = "8";
            InputStorageGb = "50";
            InputDirectory = @"C:\MojaAplikacija";
        }

        private void PopulateDropdowns()
        {
            AvailableOsList = new List<string>
            {
                "Windows 8", "Windows 8.1", "Windows 10", "Windows 11",
                "Ubuntu 12.04+", "Ubuntu 22.04 LTS", "Ubuntu 24.04 LTS",
                "Debian 7+", "Debian 12", "Fedora 17+", "Fedora 40",
                "CentOS 7+", "Arch Linux"
            };
            SelectedOs = AvailableOsList[2];

            AvailableGpuGenerations = new Dictionary<string, int>
            {
                { "Bilo koja grafička kartica", 0 },
                { "Generacija 2016 (GTX 1000 / RX 400 serija)", 2016 },
                { "Generacija 2018 (RTX 2000 / RX 5000 serija)", 2018 },
                { "Generacija 2020 (RTX 3000 / RX 6000 serija)", 2020 },
                { "Generacija 2022 (RTX 4000 / RX 7000 / Intel ARC)", 2022 },
                { "Generacija 2025 (RTX 5000 / RX 8000 serija)", 2025 }
            };
        }

        private void PerformSystemCheck()
        {
            RequiredSpecsModel req = new RequiredSpecsModel();
            
            int parsedCpu, parsedRam, parsedStorage;
            req.MinCpuCores = int.TryParse(InputCpuCores, out parsedCpu) ? parsedCpu : 0;
            req.MinRamGb = int.TryParse(InputRamGb, out parsedRam) ? parsedRam : 0;
            req.MinStorageGb = int.TryParse(InputStorageGb, out parsedStorage) ? parsedStorage : 0;

            double parsedClock;
            string safeClockStr = (InputCpuClockGhz ?? "0").Replace(',', '.');
            req.MinCpuClockGhz = double.TryParse(safeClockStr, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedClock) ? parsedClock : 0;

            req.RequiredOs = SelectedOs;
            req.RequiredGpuYear = SelectedGpu.Value; 
            req.RequiredDirectory = InputDirectory;

            CheckHardwareAndOS(req);

            bool dirExists = false;
            try
            {
                dirExists = !string.IsNullOrWhiteSpace(req.RequiredDirectory) && Directory.Exists(req.RequiredDirectory);
            }
            catch (Exception ex)
            {
                LogError("Directory Check Failed", ex);
                dirExists = false;
            }

            DirectoryColor = dirExists ? Brushes.Green : Brushes.Red;
            OnPropertyChanged(""); 
        }

        private void CheckHardwareAndOS(RequiredSpecsModel req)
        {
            // CPU Cores & Clock
            try
            {
                int actualCores = Environment.ProcessorCount;
                CurrentCpuCores = $"Jezgre: {actualCores}";
                CpuCoreColor = actualCores >= req.MinCpuCores ? Brushes.Green : Brushes.Red;

                using (var searcher = new ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        uint mhz = Convert.ToUInt32(item["MaxClockSpeed"]);
                        double ghz = mhz / 1000.0;
                        CurrentCpuClock = string.Format("Brzina: {0:0.00} GHz", ghz);
                        CpuClockColor = ghz >= req.MinCpuClockGhz ? Brushes.Green : Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentCpuClock = "Greška: " + ex.Message;
                CpuClockColor = Brushes.Red;
                LogError("CPU Clock Check Failed", ex);
            }

            // Storage
            try
            {
                DriveInfo cDrive = new DriveInfo("C");
                long freeGb = cDrive.AvailableFreeSpace / (1024 * 1024 * 1024);
                CurrentStorage = $"Slobodno: {freeGb} GB";
                StorageColor = freeGb >= req.MinStorageGb ? Brushes.Green : Brushes.Red;
            }
            catch (Exception ex)
            {
                CurrentStorage = "Greška diska: " + ex.Message;
                StorageColor = Brushes.Red;
                LogError("Storage Check Failed", ex);
            }

            // RAM, OS, GPU
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        long ramGb = Convert.ToInt64(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024);
                        CurrentRam = $"RAM: {ramGb} GB";
                        RamColor = ramGb >= req.MinRamGb ? Brushes.Green : Brushes.Red;
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        string osName = item["Caption"].ToString();
                        CurrentOs = osName;
                        OsColor = osName.IndexOf(req.RequiredOs, StringComparison.OrdinalIgnoreCase) >= 0 ? Brushes.Green : Brushes.Red;
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
                {
                    bool gpuFound = false;
                    foreach (var item in searcher.Get())
                    {
                        string gpuName = item["Name"].ToString();
                        CurrentGpu = gpuName;
                        
                        int detectedYear = EstimateGpuGenerationYear(gpuName);
                        if (detectedYear >= req.RequiredGpuYear)
                        {
                            gpuFound = true;
                        }
                    }
                    GpuColor = gpuFound ? Brushes.Green : Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                CurrentRam = "Greška WMI: " + ex.Message;
                CurrentOs = "Greška WMI: " + ex.Message;
                CurrentGpu = "Greška WMI: " + ex.Message;
                
                RamColor = Brushes.Red;
                OsColor = Brushes.Red;
                GpuColor = Brushes.Red;

                LogError("WMI Checks Failed", ex);
            }
        }

        private int EstimateGpuGenerationYear(string gpuName)
        {
            if (string.IsNullOrEmpty(gpuName)) return 0;
            string name = gpuName.ToUpper();

            if (name.Contains("RTX 50") || name.Contains("RX 80")) return 2025;
            if (name.Contains("RTX 40") || name.Contains("RX 70") || name.Contains("ARC")) return 2022;
            if (name.Contains("RTX 30") || name.Contains("RX 60")) return 2020;
            if (name.Contains("RTX 20") || name.Contains("GTX 16") || name.Contains("RX 5000")) return 2018;
            if (name.Contains("GTX 10") || name.Contains("RX 500") || name.Contains("RX 400")) return 2016;

            return 2015; 
        }

        private void LogError(string context, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemCheck_Errors.log");
                string logMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}\n{2}: {3}\n{4}\n\n", 
                    DateTime.Now, context, ex.GetType().Name, ex.Message, ex.StackTrace);
                File.AppendAllText(logPath, logMessage);
            }
            catch { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}