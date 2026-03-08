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
    public static class WordCountCommand
    {
        public static void WordCount(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Please specify a file name");
                return;
            }
            
            try
            {
                string content = fileSystem.ReadFile(args);
                if (string.IsNullOrEmpty(content))
                {
                    Console.WriteLine("File is empty");
                    return;
                }
                
                string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int lineCount = lines.Length;
                int wordCount = 0;
                int charCount = content.Length;
                
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        wordCount += words.Length;
                    }
                }
                
                Console.WriteLine($"Word count for {args}:");
                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Lines: {lineCount}");
                Console.WriteLine($"Words: {wordCount}");
                Console.WriteLine($"Characters: {charCount}");
            }
            catch (Exception ex)
            {
                showError($"Error reading file: {ex.Message}");
            }
        }
    }
}
