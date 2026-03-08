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
    public static class PsCommand
    {
        public static void ShowProcesses(Action<string> showError, Action<string> showWarning)
        {
            try
            {
                Console.WriteLine("====================================");
                Console.WriteLine("        Process List");
                Console.WriteLine("====================================");
                Console.WriteLine();
                
                if (ProcessManager.Processes.Count == 0)
                {
                    Console.WriteLine("No processes running.");
                    Console.WriteLine();
                    return;
                }
                
                Console.Write("ID      ");
                Console.Write("Name                ");
                Console.Write("Type        ");
                Console.Write("Parent    ");
                Console.WriteLine("Status    ");
                Console.WriteLine("----------------------------------------------------------------------");
                
                foreach (var process in ProcessManager.Processes)
                {
                    string parentId = process.Parent != null ? process.Parent.Id.ToString() : "N/A";
                    string status = process.IsRunning ? "Running" : "Stopped";
                    string type = process.Type.ToString();
                    
                    Console.Write(process.Id.ToString().PadRight(8));
                    Console.Write(" ");
                    Console.Write(process.Name.PadRight(20));
                    Console.Write(" ");
                    Console.Write(type.PadRight(12));
                    Console.Write(" ");
                    Console.Write(parentId.PadRight(8));
                    Console.Write(" ");
                    Console.WriteLine(status);
                }
                
                Console.WriteLine();
                Console.WriteLine("Total processes: " + ProcessManager.Processes.Count);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                showError($"Error listing processes: {ex.Message}");
            }
        }
    }
}
