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
