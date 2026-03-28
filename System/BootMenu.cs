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
using Sys = Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core;
using System.Threading;
using System.IO;

namespace CMLeonOS
{
    public enum BootMenuAction
    {
        NormalBoot,
        GuiBoot,
        Reboot,
        Shutdown
    }

    internal static class BootMenu
    {
        private const int MinPanelWidth = 58;
        private const int MaxPanelWidth = 86;

        private static bool UserDatExists()
        {
            return File.Exists(@"0:\system\user.dat");
        }

        private static string FitText(string text, int width)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty.PadRight(width);
            }

            if (text.Length > width)
            {
                if (width <= 3)
                {
                    return text.Substring(0, width);
                }
                return text.Substring(0, width - 3) + "...";
            }

            return text.PadRight(width);
        }

        private static void WriteAt(int x, int y, string text, ConsoleColor fg, ConsoleColor bg)
        {
            if (y < 0 || y >= Console.WindowHeight)
            {
                return;
            }

            Console.SetCursorPosition(Math.Max(0, x), y);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(text);
        }

        private static void DrawPanelLine(int left, int top, int width, int row, string content, ConsoleColor contentFg, ConsoleColor contentBg)
        {
            int innerWidth = width - 2;
            int y = top + row;

            // Keep borders in a consistent theme color.
            WriteAt(left, y, "|", ConsoleColor.Cyan, ConsoleColor.Black);
            WriteAt(left + 1, y, FitText(content, innerWidth), contentFg, contentBg);
            WriteAt(left + width - 1, y, "|", ConsoleColor.Cyan, ConsoleColor.Black);
        }

        private static void DrawHorizontalBorder(int left, int y, int width, bool topBorder, ConsoleColor fg, ConsoleColor bg)
        {
            string mid = new string('-', width - 2);
            string line = topBorder ? ("+" + mid + "+") : ("+" + mid + "+");
            WriteAt(left, y, line, fg, bg);
        }

        private static void Render(int selIdx, int remainingTime)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            bool userDatExists = UserDatExists();
            string[] options = userDatExists
                ? new[] { "CMLeonOS Shell", "CMLeonOS Desktop", "Reboot", "Shutdown" }
                : new[] { "CMLeonOS Shell", "Reboot", "Shutdown" };

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int panelWidth = Math.Min(MaxPanelWidth, Math.Max(MinPanelWidth, width - 8));
            int panelLeft = Math.Max(0, (width - panelWidth) / 2);
            int contentLines = 9 + options.Length;
            int panelTop = Math.Max(0, (height - contentLines) / 2);

            DrawHorizontalBorder(panelLeft, panelTop, panelWidth, true, ConsoleColor.Cyan, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 1, " CMLeonOS Boot Manager ", ConsoleColor.Black, ConsoleColor.Cyan);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 2, string.Empty, ConsoleColor.White, ConsoleColor.Black);

            uint mem = CPU.GetAmountOfRAM();
            DrawPanelLine(panelLeft, panelTop, panelWidth, 3, $" Version : {Version.DisplayVersion}", ConsoleColor.Gray, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 4, $" Memory  : {mem} MB", ConsoleColor.Gray, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 5, $" Build   : {GetBuildTime()}", ConsoleColor.DarkGray, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 6, string.Empty, ConsoleColor.White, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 7, $" Auto boot in {remainingTime}s", ConsoleColor.Yellow, ConsoleColor.Black);
            DrawPanelLine(panelLeft, panelTop, panelWidth, 8, " Use Up/Down to select, Enter to boot", ConsoleColor.DarkGray, ConsoleColor.Black);

            for (int i = 0; i < options.Length; i++)
            {
                bool selected = i == selIdx;
                string prefix = selected ? " > " : "   ";
                string text = prefix + options[i];
                DrawPanelLine(
                    panelLeft,
                    panelTop,
                    panelWidth,
                    9 + i,
                    text,
                    selected ? ConsoleColor.Black : ConsoleColor.White,
                    selected ? ConsoleColor.White : ConsoleColor.Black
                );
            }

            DrawHorizontalBorder(panelLeft, panelTop + contentLines, panelWidth, false, ConsoleColor.Cyan, ConsoleColor.Black);
            Console.ResetColor();
        }

        private static BootMenuAction Confirm(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.SetCursorPosition(0, 0);

            Console.CursorVisible = true;

            bool userDatExists = UserDatExists();
            int optionIndex = 0;

            if (selIdx == optionIndex++)
            {
                return BootMenuAction.NormalBoot;
            }
            if (userDatExists && selIdx == optionIndex++)
            {
                return BootMenuAction.GuiBoot;
            }
            if (selIdx == optionIndex++)
            {
                Sys.Power.Reboot();
                return BootMenuAction.Reboot;
            }
            if (selIdx == optionIndex++)
            {
                Sys.Power.Shutdown();
                return BootMenuAction.Shutdown;
            }

            return BootMenuAction.NormalBoot;
        }

        public static BootMenuAction Show()
        {
            if (Settings.SettingsManager.SkipToGui && UserDatExists())
            {
                return BootMenuAction.GuiBoot;
            }
            
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.CursorVisible = false;

            int selIdx = 0;
            int remainingTime = 10;
            int counter = 0;

            while (true)
            {
                Render(selIdx, remainingTime);

                if (Sys.KeyboardManager.TryReadKey(out var key))
                {
                    if (key.Key == Sys.ConsoleKeyEx.Enter)
                    {
                        return Confirm(selIdx);
                    }
                    else if (key.Key == Sys.ConsoleKeyEx.DownArrow)
                    {
                        selIdx++;
                        remainingTime = 10;
                        counter = 0;
                    }
                    else if (key.Key == Sys.ConsoleKeyEx.UpArrow)
                    {
                        selIdx--;
                        remainingTime = 10;
                        counter = 0;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                    counter++;

                    if (counter >= 10)
                    {
                        remainingTime--;
                        counter = 0;

                        if (remainingTime <= 0)
                        {
                            return Confirm(selIdx);
                        }
                    }
                }

                int maxOptionIndex = UserDatExists() ? 3 : 2;

                if (selIdx < 0)
                {
                    selIdx = maxOptionIndex;
                }

                if (selIdx > maxOptionIndex)
                {
                    selIdx = 0;
                }
            }
        }

        private static string GetBuildTime()
        {
            try
            {
                if (Kernel.buildTimeFile != null && Kernel.buildTimeFile.Length > 0)
                {
                    return System.Text.Encoding.UTF8.GetString(Kernel.buildTimeFile);
                }
            }
            catch
            {
            }
            return "Unknown";
        }
    }
}
