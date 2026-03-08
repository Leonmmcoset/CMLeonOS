// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
