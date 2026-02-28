using System;

namespace CMLeonOS.Commands.System
{
    public static class KillCommand
    {
        public static void KillProcess(string args, Action<string> showError, Action<string> showWarning)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                {
                    showError("Usage: kill <process_id>");
                    showError("Example: kill 123");
                    return;
                }
                
                if (!ulong.TryParse(args, out ulong processId))
                {
                    showError($"Invalid process ID: {args}");
                    showError("Process ID must be a number.");
                    return;
                }
                
                Process process = ProcessManager.GetProcessById(processId);
                if (process == null)
                {
                    showError($"Process not found: {processId}");
                    showError("Use 'ps' command to list all processes.");
                    return;
                }
                
                if (process.Name == "Shell")
                {
                    showError("Cannot kill Shell process.");
                    showError("Use 'exit' command instead.");
                    return;
                }
                
                if (!process.IsRunning)
                {
                    showWarning($"Process {process.Name} ({processId}) is already stopped.");
                    return;
                }
                
                CMLeonOS.Logger.Logger.Instance.Info("Kill", $"Killing process: {process.Name} ({processId})");
                process.TryStop();
                
                Console.WriteLine($"Process {process.Name} ({processId}) stopped successfully.");
            }
            catch (Exception ex)
            {
                showError($"Error killing process: {ex.Message}");
            }
        }
    }
}
