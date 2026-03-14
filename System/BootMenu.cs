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
        private static bool UserDatExists()
        {
            return File.Exists(@"0:\system\user.dat");
        }
        private static void PrintOption(string text, bool selected)
        {
            Console.SetCursorPosition(1, Console.GetCursorPosition().Top);

            Console.BackgroundColor = selected ? ConsoleColor.White : ConsoleColor.Black;
            Console.ForegroundColor = selected ? ConsoleColor.Black : ConsoleColor.White;

            Console.WriteLine(text);
        }

        private static void Render(int selIdx, int remainingTime)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.SetCursorPosition(0, 0);

            uint mem = Cosmos.Core.CPU.GetAmountOfRAM();
            Console.WriteLine($"{Version.DisplayVersion} [{mem} MB memory]");
            Console.WriteLine($"Build Time: {GetBuildTime()}");
            Console.WriteLine();
            Console.WriteLine($"Auto-select in {remainingTime} seconds...");
            Console.WriteLine();
            Console.WriteLine("Select an option:");
            Console.WriteLine();

            bool userDatExists = UserDatExists();
            int optionIndex = 0;

            PrintOption("CMLeonOS (Shell)", selIdx == optionIndex++);
            if (userDatExists)
            {
                PrintOption("CMLeonOS (Desktop)", selIdx == optionIndex++);
            }
            PrintOption("Reboot", selIdx == optionIndex++);
            PrintOption("Shutdown", selIdx == optionIndex++);
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
