using System;
using Sys = Cosmos.System;

namespace CMLeonOS.Commands.FileSystem
{
    public static class DiskInfoCommand
    {
        public static void GetDiskInfo(Action<string> showError)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Disk Information");
            Console.WriteLine("====================================");
            
            try
            {
                var disks = Sys.FileSystem.VFS.VFSManager.GetDisks();
                
                if (disks == null || disks.Count == 0)
                {
                    Console.WriteLine("No disks found.");
                    return;
                }
                
                Console.WriteLine($"Total Disks: {disks.Count}");
            }
            catch (Exception ex)
            {
                showError($"Error getting disk info: {ex.Message}");
            }
        }
    }
}
