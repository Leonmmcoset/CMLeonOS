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
using System.IO;

namespace CMLeonOS.Commands.Utility
{
    public static class HexCommand
    {
        public static void EditHexFile(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Usage: hex <filename>");
                    Console.WriteLine("Example: hex test.bin");
                    Console.WriteLine();
                    Console.WriteLine("Controls:");
                    Console.WriteLine("  Arrow Keys - Move cursor");
                    Console.WriteLine("  Page Up/Down - Scroll view");
                    Console.WriteLine("  0-9, A-F - Edit byte");
                    Console.WriteLine("  S - Save file");
                    Console.WriteLine("  Q - Quit");
                    Console.ResetColor();
                    return;
                }

                string filePath = args.Trim();
                
                if (!File.Exists(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"File not found: {filePath}");
                    Console.ResetColor();
                    return;
                }

                var editor = new HexEditor(filePath);
                editor.Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error opening hex editor: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
