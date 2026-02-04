using System;

namespace CMLeonOS.Commands.System
{
    public static class UptimeCommand
    {
        public static void ShowUptime(Action<string> showError, Action<string> showWarning)
        {
            try
            {
                Console.WriteLine("====================================");
                Console.WriteLine("        System Uptime");
                Console.WriteLine("====================================");
                Console.WriteLine();
                
                if (Kernel.SystemStartTime != DateTime.MinValue)
                {
                    TimeSpan uptime = DateTime.Now - Kernel.SystemStartTime;
                    
                    Console.WriteLine("System started: " + Kernel.SystemStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    Console.WriteLine("Current time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    Console.WriteLine();
                    
                    int days = uptime.Days;
                    int hours = uptime.Hours;
                    int minutes = uptime.Minutes;
                    int seconds = uptime.Seconds;
                    
                    Console.WriteLine($"System uptime: {days} days, {hours} hours, {minutes} minutes, {seconds} seconds");
                    Console.WriteLine($"Total uptime: {uptime.TotalHours:F2} hours");
                }
                else
                {
                    showWarning("System start time not available.");
                    showWarning("System may have been started before uptime tracking was implemented.");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                showError($"Error showing uptime: {ex.Message}");
            }
        }
    }
}
