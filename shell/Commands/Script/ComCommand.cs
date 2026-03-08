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
