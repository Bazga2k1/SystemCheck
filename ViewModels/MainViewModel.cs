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
        private bool _isDarkTheme = true;

        public List<string> AvailableOsList { get; private set; }
        public Dictionary<string, int> AvailableGpuGenerations { get; private set; }

        public string InputCpuCores { get; set; }
        public string InputCpuClockGhz { get; set; }
        public string InputRamGb { get; set; }
        public string InputStorageGb { get; set; }
        public string SelectedOs { get; set; }
        public KeyValuePair<string, int> SelectedGpu { get; set; }
        public string InputDirectory { get; set; }

        public string CurrentCpuCores { get; private set; }
        public string CurrentCpuClock { get; private set; }
        public string CurrentRam { get; private set; }
        public string CurrentStorage { get; private set; }
        public string CurrentGpu { get; private set; }
        public string CurrentOs { get; private set; }

        // --- Theme Colors ---
        public Brush ThemeBackground { get; private set; }
        public Brush ThemeForeground { get; private set; }
        public Brush ThemeControlBackground { get; private set; }
        public Brush ThemeControlForeground { get; private set; }
        public Brush ThemeControlBorder { get; private set; }
        public Brush ThemeSeparator { get; private set; }

        // Dynamic Accent Button Colors
        public Brush AccentButtonBackground { get; private set; }
        public Brush AccentButtonHoverBackground { get; private set; }

        public Brush SuccessColor { get; private set; }
        public Brush ErrorColor { get; private set; }
        public Brush DefaultTextColor { get; private set; }

        // --- Output Status Colors ---
        public Brush CpuCoreColor { get; private set; }
        public Brush CpuClockColor { get; private set; }
        public Brush RamColor { get; private set; }
        public Brush StorageColor { get; private set; }
        public Brush GpuColor { get; private set; }
        public Brush OsColor { get; private set; }
        public Brush DirectoryColor { get; private set; }

        // --- Boolean State Trackers ---
        private bool? _cpuCoresPassed;
        private bool? _cpuClockPassed;
        private bool? _ramPassed;
        private bool? _storagePassed;
        private bool? _osPassed;
        private bool? _gpuPassed;
        private bool? _directoryPassed;

        public ICommand CheckSystemCommand { get; private set; }
        public ICommand ToggleThemeCommand { get; private set; }

        public MainViewModel()
        {
            PopulateDropdowns();
            CheckSystemCommand = new RelayCommand(p => PerformSystemCheck());
            ToggleThemeCommand = new RelayCommand(p => ToggleTheme());

            InputCpuCores = "4";
            InputCpuClockGhz = "2.5";
            InputRamGb = "8";
            InputStorageGb = "50";
            InputDirectory = @"C:\MojaAplikacija";

            ApplyTheme(true); // Start in Dark Mode
        }

        private void ToggleTheme()
        {
            _isDarkTheme = !_isDarkTheme;
            ApplyTheme(_isDarkTheme);
        }

        private void ApplyTheme(bool isDark)
        {
            var bc = new BrushConverter();
            if (isDark)
            {
                ThemeBackground = (Brush)bc.ConvertFrom("#1E1E1E");
                ThemeForeground = (Brush)bc.ConvertFrom("#E0E0E0");
                ThemeControlBackground = (Brush)bc.ConvertFrom("#2D2D2D");
                ThemeControlForeground = (Brush)bc.ConvertFrom("#FFFFFF");
                ThemeControlBorder = (Brush)bc.ConvertFrom("#555555");
                ThemeSeparator = (Brush)bc.ConvertFrom("#444444");

                // Darker blue for Dark Mode
                AccentButtonBackground = (Brush)bc.ConvertFrom("#004080");
                AccentButtonHoverBackground = (Brush)bc.ConvertFrom("#002B52");

                SuccessColor = Brushes.LimeGreen;
                ErrorColor = Brushes.Tomato;
                DefaultTextColor = Brushes.LightGray;
            }
            else
            {
                ThemeBackground = (Brush)bc.ConvertFrom("#F0F0F0");
                ThemeForeground = (Brush)bc.ConvertFrom("#000000");
                ThemeControlBackground = (Brush)bc.ConvertFrom("#FFFFFF");
                ThemeControlForeground = (Brush)bc.ConvertFrom("#000000");
                ThemeControlBorder = (Brush)bc.ConvertFrom("#CCCCCC");
                ThemeSeparator = (Brush)bc.ConvertFrom("#DDDDDD");

                // Original blue for Light Mode
                AccentButtonBackground = (Brush)bc.ConvertFrom("#007ACC");
                AccentButtonHoverBackground = (Brush)bc.ConvertFrom("#005C99");

                SuccessColor = Brushes.Green;
                ErrorColor = Brushes.Red;
                DefaultTextColor = Brushes.Black;
            }

            UpdateStatusColors();
            OnPropertyChanged(""); // Refresh UI
        }

        private void UpdateStatusColors()
        {
            CpuCoreColor = GetColorForState(_cpuCoresPassed);
            CpuClockColor = GetColorForState(_cpuClockPassed);
            RamColor = GetColorForState(_ramPassed);
            StorageColor = GetColorForState(_storagePassed);
            OsColor = GetColorForState(_osPassed);
            GpuColor = GetColorForState(_gpuPassed);
            DirectoryColor = GetColorForState(_directoryPassed);
        }

        private Brush GetColorForState(bool? passed)
        {
            if (passed == null) return DefaultTextColor;
            return passed.Value ? SuccessColor : ErrorColor;
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

            try
            {
                _directoryPassed = !string.IsNullOrWhiteSpace(req.RequiredDirectory) && Directory.Exists(req.RequiredDirectory);
            }
            catch (Exception ex)
            {
                LogError("Directory Check Failed", ex);
                _directoryPassed = false;
            }

            UpdateStatusColors();
            OnPropertyChanged(""); 
        }

        private void CheckHardwareAndOS(RequiredSpecsModel req)
        {
            try
            {
                int actualCores = Environment.ProcessorCount;
                CurrentCpuCores = $"Jezgre: {actualCores}";
                _cpuCoresPassed = actualCores >= req.MinCpuCores;

                using (var searcher = new ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        uint mhz = Convert.ToUInt32(item["MaxClockSpeed"]);
                        double ghz = mhz / 1000.0;
                        CurrentCpuClock = string.Format("Brzina: {0:0.00} GHz", ghz);
                        _cpuClockPassed = ghz >= req.MinCpuClockGhz;
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentCpuClock = "Greška: " + ex.Message;
                _cpuClockPassed = false;
                LogError("CPU Clock Check Failed", ex);
            }

            try
            {
                DriveInfo cDrive = new DriveInfo("C");
                long freeGb = cDrive.AvailableFreeSpace / (1024 * 1024 * 1024);
                CurrentStorage = $"Slobodno: {freeGb} GB";
                _storagePassed = freeGb >= req.MinStorageGb;
            }
            catch (Exception ex)
            {
                CurrentStorage = "Greška diska: " + ex.Message;
                _storagePassed = false;
                LogError("Storage Check Failed", ex);
            }

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        long ramGb = Convert.ToInt64(item["TotalPhysicalMemory"]) / (1024 * 1024 * 1024);
                        CurrentRam = $"RAM: {ramGb} GB";
                        _ramPassed = ramGb >= req.MinRamGb;
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        string osName = item["Caption"].ToString();
                        CurrentOs = osName;
                        _osPassed = osName.IndexOf(req.RequiredOs, StringComparison.OrdinalIgnoreCase) >= 0;
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
                    _gpuPassed = gpuFound;
                }
            }
            catch (Exception ex)
            {
                CurrentRam = "Greška WMI: " + ex.Message;
                CurrentOs = "Greška WMI: " + ex.Message;
                CurrentGpu = "Greška WMI: " + ex.Message;
                
                _ramPassed = false;
                _osPassed = false;
                _gpuPassed = false;

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}