using System;

namespace CMLeonOS.Utils
{
    public static class MemoryStatisticsProvider
    {
        public static ulong GetAvailableMemory()
        {
            return Cosmos.Core.CPU.GetAmountOfRAM() * 1024 * 1024;
        }

        public static ulong GetUsedMemory()
        {
            ulong totalMemory = GetAvailableMemory();
            ulong freeMemory = GetFreeMemory();
            return totalMemory - freeMemory;
        }

        public static ulong GetFreeMemory()
        {
            ulong totalMemory = GetAvailableMemory();
            return totalMemory / 4;
        }

        public static ulong GetTotalMemory()
        {
            return Cosmos.Core.CPU.GetAmountOfRAM() * 1024 * 1024;
        }

        public static string FormatBytes(ulong bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        public static string GetMemoryUsagePercentage()
        {
            ulong used = GetUsedMemory();
            ulong total = GetTotalMemory();
            double percentage = (double)used / total * 100;
            return $"{percentage:0.1}%";
        }

        public static MemoryStatistics GetMemoryStatistics()
        {
            return new MemoryStatistics
            {
                TotalMemory = GetTotalMemory(),
                UsedMemory = GetUsedMemory(),
                FreeMemory = GetFreeMemory(),
                UsagePercentage = GetMemoryUsagePercentage()
            };
        }
    }

    public class MemoryStatistics
    {
        public ulong TotalMemory { get; set; }
        public ulong UsedMemory { get; set; }
        public ulong FreeMemory { get; set; }
        public string UsagePercentage { get; set; }
        
        public ulong TotalMB
        {
            get { return TotalMemory / (1024 * 1024); }
        }
        
        public ulong UnavailableMB
        {
            get { return 0; }
        }
        
        public ulong UsedMB
        {
            get { return UsedMemory / (1024 * 1024); }
        }
        
        public ulong FreeMB
        {
            get { return FreeMemory / (1024 * 1024); }
        }
        
        public int PercentUsed
        {
            get
            {
                if (UsagePercentage.EndsWith("%"))
                {
                    string percentage = UsagePercentage.TrimEnd('%');
                    if (int.TryParse(percentage, out int result))
                    {
                        return result;
                    }
                }
                return 0;
            }
        }
    }
}
