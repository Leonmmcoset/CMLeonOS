using System;
using System.Collections.Generic;
using CMLeonOS;

namespace CMLeonOS.Commands.FileSystem
{
    public static class RmCommand
    {
        public static void ProcessRm(CMLeonOS.FileSystem fileSystem, string args, bool fixMode, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<file>", 
                        Description = "Delete file",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<file> -norisk", 
                        Description = "Delete file in sys folder without confirmation",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("rm", commandInfos));
                return;
            }
            else
            {
                bool isInSysFolder = (args.Contains(@"\system\") || args.Contains(@"/sys/")) && !fixMode;
                
                if (isInSysFolder)
                {
                    string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    bool hasNorisk = false;
                    string filePath = args;
                    
                    if (parts.Length > 1)
                    {
                        hasNorisk = Array.IndexOf(parts, "-norisk") >= 0;
                        filePath = parts[0];
                    }
                    
                    if (!hasNorisk)
                    {
                        showError("Cannot delete files in sys folder without -norisk parameter");
                        showError(UsageGenerator.GenerateSimpleUsage("rm", "<file> -norisk"));
                    }
                    else
                    {
                        fileSystem.DeleteFile(filePath);
                    }
                }
                else
                {
                    fileSystem.DeleteFile(args);
                }
            }
        }
    }
}
