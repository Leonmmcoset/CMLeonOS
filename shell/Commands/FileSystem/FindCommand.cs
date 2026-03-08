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
    public static class FindCommand
    {
        public static void FindFile(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name to search");
                showError("find <filename>");
                return;
            }
            
            try
            {
                var files = fileSystem.GetFileList(".");
                bool found = false;
                
                foreach (var file in files)
                {
                    if (file.ToLower().Contains(args.ToLower()))
                    {
                        Console.WriteLine($"Found: {file}");
                        found = true;
                    }
                }
                
                if (!found)
                {
                    Console.WriteLine($"No files found matching '{args}'");
                }
            }
            catch (Exception ex)
            {
                showError($"Error finding file: {ex.Message}");
            }
        }
    }
}
