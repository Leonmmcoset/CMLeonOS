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
