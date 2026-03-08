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
    public class HexEditor
    {
        private byte[] data;
        private int cursorPosition;
        private int viewOffset;
        private int bytesPerLine = 16;
        private int bytesPerGroup = 2;
        private string filePath;
        private bool modified;
        private bool running;

        public HexEditor(string path)
        {
            filePath = path;
            if (File.Exists(path))
            {
                data = File.ReadAllBytes(path);
            }
            else
            {
                data = new byte[256];
            }
            cursorPosition = 0;
            viewOffset = 0;
            modified = false;
            running = true;
        }

        public void Run()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       Hex Editor");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("File: " + filePath);
            Console.WriteLine("Size: " + data.Length + " bytes");
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("  Arrow Keys - Move cursor");
            Console.WriteLine("  Page Up/Down - Scroll view");
            Console.WriteLine("  0-9, A-F - Edit byte");
            Console.WriteLine("  S - Save file");
            Console.WriteLine("  Q - Quit");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            
            while (running)
            {
                Display();
                HandleInput();
            }
        }

        private void Display()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("       Hex Editor");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("File: " + filePath);
            Console.WriteLine("Size: " + data.Length + " bytes");
            if (modified)
            {
                Console.Write(" [MODIFIED]");
            }
            Console.WriteLine();
            Console.WriteLine();

            int lines = (int)Math.Ceiling((double)data.Length / bytesPerLine);
            int startLine = viewOffset / bytesPerLine;
            int endLine = Math.Min(startLine + 20, lines);

            for (int line = startLine; line < endLine; line++)
            {
                int offset = line * bytesPerLine;
                if (offset >= data.Length) break;

                Console.Write(offset.ToString("X8") + "  ");

                for (int i = 0; i < bytesPerLine; i++)
                {
                    int byteOffset = offset + i;
                    if (byteOffset >= data.Length)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        byte b = data[byteOffset];
                        bool isCursor = (byteOffset == cursorPosition);
                        
                        if (isCursor)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        
                        Console.Write(b.ToString("X2") + " ");
                        
                        if (isCursor)
                        {
                            Console.ResetColor();
                        }
                        
                        if ((i + 1) % bytesPerGroup == 0)
                        {
                            Console.Write(" ");
                        }
                    }
                }

                Console.Write(" |");
                
                for (int i = 0; i < bytesPerLine; i++)
                {
                    int byteOffset = offset + i;
                    if (byteOffset >= data.Length)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        byte b = data[byteOffset];
                        bool isCursor = (byteOffset == cursorPosition);
                        
                        if (isCursor)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        
                        char c = (b >= 32 && b <= 126) ? (char)b : '.';
                        Console.Write(c);
                        
                        if (isCursor)
                        {
                            Console.ResetColor();
                        }
                    }
                }
                
                Console.WriteLine("|");
            }

            Console.WriteLine();
            Console.WriteLine("Cursor: 0x" + cursorPosition.ToString("X8") + " (" + cursorPosition + ")");
            Console.WriteLine("Value: 0x" + data[cursorPosition].ToString("X2"));
            Console.WriteLine();
            Console.WriteLine("Controls:");
            Console.WriteLine("  Arrow Keys - Move cursor");
            Console.WriteLine("  Page Up/Down - Scroll view");
            Console.WriteLine("  0-9, A-F - Edit byte");
            Console.WriteLine("  S - Save file");
            Console.WriteLine("  Q - Quit");
        }

        private void HandleInput()
        {
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    cursorPosition = Math.Max(0, cursorPosition - bytesPerLine);
                    break;
                case ConsoleKey.DownArrow:
                    cursorPosition = Math.Min(data.Length - 1, cursorPosition + bytesPerLine);
                    break;
                case ConsoleKey.LeftArrow:
                    cursorPosition = Math.Max(0, cursorPosition - 1);
                    break;
                case ConsoleKey.RightArrow:
                    cursorPosition = Math.Min(data.Length - 1, cursorPosition + 1);
                    break;
                case ConsoleKey.PageUp:
                    viewOffset = Math.Max(0, viewOffset - (bytesPerLine * 20));
                    break;
                case ConsoleKey.PageDown:
                    viewOffset = Math.Min(data.Length - 1, viewOffset + (bytesPerLine * 20));
                    break;
                case ConsoleKey.D0:
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.D4:
                case ConsoleKey.D5:
                case ConsoleKey.D6:
                case ConsoleKey.D7:
                case ConsoleKey.D8:
                case ConsoleKey.D9:
                    int digit = key.KeyChar - '0';
                    EditByte(digit);
                    break;
                case ConsoleKey.A:
                    EditByte(10);
                    break;
                case ConsoleKey.B:
                    EditByte(11);
                    break;
                case ConsoleKey.C:
                    EditByte(12);
                    break;
                case ConsoleKey.D:
                    EditByte(13);
                    break;
                case ConsoleKey.E:
                    EditByte(14);
                    break;
                case ConsoleKey.F:
                    EditByte(15);
                    break;
                case ConsoleKey.S:
                    SaveFile();
                    break;
                case ConsoleKey.Q:
                    running = false;
                    break;
            }
        }

        private void EditByte(int value)
        {
            if (cursorPosition >= 0 && cursorPosition < data.Length)
            {
                data[cursorPosition] = (byte)value;
                modified = true;
            }
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllBytes(filePath, data);
                modified = false;
                Console.WriteLine();
                Console.WriteLine("File saved successfully!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error saving file: " + ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }
    }
}
