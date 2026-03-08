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

namespace CMLeonOS.Commands.FileSystem
{
    public static class RenameCommand
    {
        public static void RenameFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError, Action<string> showSuccess)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify source and new name");
                showError("rename <source> <newname>");
                return;
            }
            
            try
            {
                string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    showError("Please specify both source and new name");
                    showError("rename <source> <newname>");
                    return;
                }
                
                string sourceFile = parts[0];
                string newName = parts[1];
                
                string sourcePath = fileSystem.GetFullPath(sourceFile);
                string destPath = fileSystem.GetFullPath(newName);
                
                if (!global::System.IO.File.Exists(sourcePath))
                {
                    showError($"Source file '{sourceFile}' does not exist");
                    return;
                }
                
                if (global::System.IO.File.Exists(destPath))
                {
                    showError($"Destination '{newName}' already exists");
                    return;
                }
                
                string content = fileSystem.ReadFile(sourcePath);
                global::System.IO.File.WriteAllText(destPath, content);
                fileSystem.DeleteFile(sourcePath);
                
                showSuccess($"File renamed successfully from '{sourceFile}' to '{newName}'");
            }
            catch (Exception ex)
            {
                showError($"Error renaming file: {ex.Message}");
            }
        }
    }
}
