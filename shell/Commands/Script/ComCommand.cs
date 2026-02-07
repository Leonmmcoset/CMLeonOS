using System;
using System.Collections.Generic;
using System.IO;
using CMLeonOS;

namespace CMLeonOS.Commands.Script
{
    public static class ComCommand
    {
        public static void ExecuteCommandFile(string args, CMLeonOS.FileSystem fileSystem, Shell shell, Action<string> showError, Action<string> showWarning)
        {
            if (string.IsNullOrEmpty(args))
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<filename.cm>", 
                        Description = "Execute command file",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("com", commandInfos));
                return;
            }
            
            string filePath = fileSystem.GetFullPath(args);
            
            if (!File.Exists(filePath))
            {
                showError($"File not found: {args}");
                return;
            }
            
            try
            {
                string[] lines = fileSystem.ReadFile(filePath).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                
                if (lines.Length == 0 || (lines.Length == 1 && string.IsNullOrWhiteSpace(lines[0])))
                {
                    showWarning("Command file is empty");
                    return;
                }
                
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#"))
                    {
                        continue;
                    }
                    
                    shell.ExecuteCommand(line);
                }
            }
            catch (Exception ex)
            {
                showError($"Error executing command file: {ex.Message}");
            }
        }
    }
}
