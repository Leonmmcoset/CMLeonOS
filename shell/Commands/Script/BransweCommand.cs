using System;
using System.IO;

namespace CMLeonOS.Commands.Script
{
    public static class BransweCommand
    {
        public static void ProcessBransweCommand(string args, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Error: Please specify file name");
                showError("Usage: branswe <filename>");
                return;
            }
            
            string filePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(filePath))
            {
                showError($"Error: File not found: {args}");
                return;
            }
            
            try
            {
                string fileContent = File.ReadAllText(filePath);
                Branswe.Run(fileContent);
            }
            catch (Exception ex)
            {
                showError($"Error executing Branswe: {ex.Message}");
            }
        }
    }
}
