using System;
using System.Collections.Generic;

namespace CMLeonOS
{
    public class UsageGenerator
    {
        public class CommandInfo
        {
            public string Command { get; set; }
            public List<SubCommandInfo> SubCommands { get; set; } = new List<SubCommandInfo>();
            public string Description { get; set; }
            public bool IsOptional { get; set; }
        }

        public class SubCommandInfo
        {
            public string Command { get; set; }
            public string Description { get; set; }
            public bool IsOptional { get; set; }
        }

        public static string GenerateUsage(string commandName, List<CommandInfo> commandInfos)
        {
            List<string> lines = new List<string>();
            
            lines.Add("Usage: " + commandName + " [subcommand] [args]");
            lines.Add("");
            
            foreach (var commandInfo in commandInfos)
            {
                string cmdDisplay = commandInfo.IsOptional ? "[" + commandInfo.Command + "]" : commandInfo.Command;
                string commandLine = "  " + commandName + " " + cmdDisplay;
                if (!string.IsNullOrEmpty(commandInfo.Description))
                {
                    int padding = 50 - commandLine.Length;
                    if (padding < 1) padding = 1;
                    commandLine += new string(' ', padding) + "- " + commandInfo.Description;
                }
                lines.Add(commandLine);
                
                if (commandInfo.SubCommands.Count > 0)
                {
                    foreach (var subCommand in commandInfo.SubCommands)
                    {
                        string subCmdDisplay = subCommand.IsOptional ? "[" + subCommand.Command + "]" : subCommand.Command;
                        string subCommandLine = "    " + commandName + " " + commandInfo.Command + " " + subCmdDisplay;
                        if (!string.IsNullOrEmpty(subCommand.Description))
                        {
                            int padding = 50 - subCommandLine.Length;
                            if (padding < 1) padding = 1;
                            subCommandLine += new string(' ', padding) + "- " + subCommand.Description;
                        }
                        lines.Add(subCommandLine);
                    }
                }
            }
            
            return BuildStringList(lines);
        }

        public static string GenerateSimpleUsage(string commandName, string usagePattern)
        {
            return "Usage: " + commandName + " " + usagePattern;
        }

        public static string GenerateMultiLineUsage(string commandName, List<CommandInfo> commandInfos)
        {
            List<string> lines = new List<string>();
            
            lines.Add("Usage: " + commandName + " [subcommand] [args]");
            lines.Add("");
            
            foreach (var commandInfo in commandInfos)
            {
                string cmdDisplay = commandInfo.IsOptional ? "[" + commandInfo.Command + "]" : commandInfo.Command;
                string commandLine = "  " + commandName + " " + cmdDisplay;
                if (!string.IsNullOrEmpty(commandInfo.Description))
                {
                    int padding = 50 - commandLine.Length;
                    if (padding < 1) padding = 1;
                    commandLine += new string(' ', padding) + "- " + commandInfo.Description;
                }
                lines.Add(commandLine);
            }
            
            return BuildStringList(lines);
        }

        public static string GenerateDetailedUsage(string commandName, List<CommandInfo> commandInfos)
        {
            List<string> lines = new List<string>();
            
            lines.Add("Usage: " + commandName + " [subcommand] [args]");
            lines.Add("");
            lines.Add("Available subcommands:");
            lines.Add("");
            
            foreach (var commandInfo in commandInfos)
            {
                string cmdDisplay = commandInfo.IsOptional ? "[" + commandInfo.Command + "]" : commandInfo.Command;
                string commandLine = "  " + cmdDisplay;
                if (!string.IsNullOrEmpty(commandInfo.Description))
                {
                    int padding = 30 - commandLine.Length;
                    if (padding < 1) padding = 1;
                    commandLine += new string(' ', padding) + "- " + commandInfo.Description;
                }
                lines.Add(commandLine);
                
                if (commandInfo.SubCommands.Count > 0)
                {
                    lines.Add("");
                    lines.Add("  " + commandInfo.Command + " options:");
                    lines.Add("");
                    
                    foreach (var subCommand in commandInfo.SubCommands)
                    {
                        string subCmdDisplay = subCommand.IsOptional ? "[" + subCommand.Command + "]" : subCommand.Command;
                        string subCommandLine = "    " + subCmdDisplay;
                        if (!string.IsNullOrEmpty(subCommand.Description))
                        {
                            int padding = 30 - subCommandLine.Length;
                            if (padding < 1) padding = 1;
                            subCommandLine += new string(' ', padding) + "- " + subCommand.Description;
                        }
                        lines.Add(subCommandLine);
                    }
                    lines.Add("");
                }
            }
            
            return BuildStringList(lines);
        }

        public static string GenerateCompactUsage(string commandName, List<CommandInfo> commandInfos)
        {
            List<string> lines = new List<string>();
            
            string commandPattern = BuildCommandPattern(commandInfos);
            lines.Add("Usage: " + commandName + " <" + commandPattern + "> [args]");
            lines.Add("");
            
            int maxCommandLength = FindMaxCommandLength(commandInfos);
            
            foreach (var commandInfo in commandInfos)
            {
                string cmdDisplay = commandInfo.IsOptional ? "[" + commandInfo.Command + "]" : commandInfo.Command;
                string commandLine = "  " + cmdDisplay;
                if (!string.IsNullOrEmpty(commandInfo.Description))
                {
                    int padding = maxCommandLength + 4 - commandLine.Length;
                    if (padding < 1) padding = 1;
                    commandLine += new string(' ', padding) + "- " + commandInfo.Description;
                }
                lines.Add(commandLine);
            }
            
            return BuildStringList(lines);
        }

        public static string GenerateVerticalUsage(string commandName, List<CommandInfo> commandInfos)
        {
            List<string> lines = new List<string>();
            
            lines.Add("Usage: " + commandName + " [subcommand] [args]");
            lines.Add("");
            lines.Add("Available subcommands:");
            lines.Add("");
            
            foreach (var commandInfo in commandInfos)
            {
                string cmdDisplay = commandInfo.IsOptional ? "[" + commandInfo.Command + "]" : commandInfo.Command;
                lines.Add("  " + cmdDisplay);
                if (!string.IsNullOrEmpty(commandInfo.Description))
                {
                    lines.Add("    " + commandInfo.Description);
                }
                
                if (commandInfo.SubCommands.Count > 0)
                {
                    lines.Add("    Options:");
                    foreach (var subCommand in commandInfo.SubCommands)
                    {
                        string subCmdDisplay = subCommand.IsOptional ? "[" + subCommand.Command + "]" : subCommand.Command;
                        lines.Add("      " + subCmdDisplay);
                        if (!string.IsNullOrEmpty(subCommand.Description))
                        {
                            lines.Add("        " + subCommand.Description);
                        }
                    }
                }
                lines.Add("");
            }
            
            return BuildStringList(lines);
        }

        private static string BuildCommandPattern(List<CommandInfo> commandInfos)
        {
            string result = "";
            for (int i = 0; i < commandInfos.Count; i++)
            {
                if (i > 0)
                {
                    result += " | ";
                }
                string cmdDisplay = commandInfos[i].IsOptional ? "[" + commandInfos[i].Command + "]" : commandInfos[i].Command;
                result += cmdDisplay;
            }
            return result;
        }

        private static int FindMaxCommandLength(List<CommandInfo> commandInfos)
        {
            int maxLen = 0;
            foreach (var commandInfo in commandInfos)
            {
                int len = commandInfo.Command.Length;
                if (len > maxLen)
                {
                    maxLen = len;
                }
            }
            return maxLen;
        }

        private static string BuildStringList(List<string> lines)
        {
            string result = "";
            for (int i = 0; i < lines.Count; i++)
            {
                if (i > 0)
                {
                    result += "\n";
                }
                result += lines[i];
            }
            return result;
        }
    }
}