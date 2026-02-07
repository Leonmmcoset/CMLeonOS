using System;
using System.Collections.Generic;
using System.IO;
using CMLeonOS;

namespace CMLeonOS.Commands.Script
{
    public static class BransweCommand
    {
        public static void ProcessBransweCommand(string args, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<filename>", 
                        Description = "Execute Branswe file",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("branswe", commandInfos));
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
