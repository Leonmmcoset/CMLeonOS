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

using CMLeonOS;
using CMLeonOS.Logger;
using CMLeonOS.Settings;
using System;

namespace CMLeonOS.Gui
{
    internal static class Gui
    {
        private enum LowMemoryAction
        {
            Continue,
            ReturnBootMenu,
            Cancel
        }

        private static bool guiRunning = false;
        private static WindowManager windowManager;

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

        private static void DrawBorderLine(int left, int y, int width, ConsoleColor fg, ConsoleColor bg)
        {
            string mid = new string('-', width - 2);
            WriteAt(left, y, "+" + mid + "+", fg, bg);
        }

        private static void DrawPanelLine(int left, int y, int width, string content, ConsoleColor contentFg, ConsoleColor contentBg)
        {
            int innerWidth = width - 2;
            WriteAt(left, y, "|", ConsoleColor.Cyan, ConsoleColor.Black);
            WriteAt(left + 1, y, FitText(content, innerWidth), contentFg, contentBg);
            WriteAt(left + width - 1, y, "|", ConsoleColor.Cyan, ConsoleColor.Black);
        }

        private static LowMemoryAction ShowLowMemoryWarning(uint ramMb)
        {
            int selected = 0;
            string[] options = new[] { "Continue Anyway", "Return to Boot Menu", "Cancel" };
            bool needsRender = true;

            while (true)
            {
                if (needsRender)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();

                    int width = Console.WindowWidth;
                    int height = Console.WindowHeight;
                    int panelWidth = Math.Min(86, Math.Max(58, width - 8));
                    int contentLineCount = 7 + options.Length;
                    int panelHeight = contentLineCount + 2;
                    int left = Math.Max(0, (width - panelWidth) / 2);
                    int top = Math.Max(0, (height - panelHeight) / 2);

                    DrawBorderLine(left, top, panelWidth, ConsoleColor.Cyan, ConsoleColor.Black);
                    DrawPanelLine(left, top + 1, panelWidth, " CMLeonOS Boot Warning ", ConsoleColor.Black, ConsoleColor.Cyan);
                    DrawPanelLine(left, top + 2, panelWidth, string.Empty, ConsoleColor.White, ConsoleColor.Black);
                    DrawPanelLine(left, top + 3, panelWidth, " Not enough system memory is available for Desktop GUI.", ConsoleColor.Yellow, ConsoleColor.Black);
                    DrawPanelLine(left, top + 4, panelWidth, $" Detected : {ramMb} MB", ConsoleColor.Gray, ConsoleColor.Black);
                    DrawPanelLine(left, top + 5, panelWidth, " Required : at least 1024 MB", ConsoleColor.Gray, ConsoleColor.Black);
                    DrawPanelLine(left, top + 6, panelWidth, string.Empty, ConsoleColor.White, ConsoleColor.Black);
                    DrawPanelLine(left, top + 7, panelWidth, " Use Up/Down to select, Enter to confirm", ConsoleColor.DarkGray, ConsoleColor.Black);

                    for (int i = 0; i < options.Length; i++)
                    {
                        bool isSelected = i == selected;
                        DrawPanelLine(
                            left,
                            top + 8 + i,
                            panelWidth,
                            (isSelected ? " > " : "   ") + options[i],
                            isSelected ? ConsoleColor.Black : ConsoleColor.White,
                            isSelected ? ConsoleColor.White : ConsoleColor.Black
                        );
                    }

                    DrawBorderLine(left, top + panelHeight - 1, panelWidth, ConsoleColor.Cyan, ConsoleColor.Black);
                    needsRender = false;
                }

                if (Cosmos.System.KeyboardManager.TryReadKey(out var key))
                {
                    if (key.Key == Cosmos.System.ConsoleKeyEx.UpArrow)
                    {
                        selected--;
                        if (selected < 0)
                        {
                            selected = options.Length - 1;
                        }
                        needsRender = true;
                    }
                    else if (key.Key == Cosmos.System.ConsoleKeyEx.DownArrow)
                    {
                        selected++;
                        if (selected >= options.Length)
                        {
                            selected = 0;
                        }
                        needsRender = true;
                    }
                    else if (key.Key == Cosmos.System.ConsoleKeyEx.Enter)
                    {
                        if (selected == 0)
                        {
                            return LowMemoryAction.Continue;
                        }
                        if (selected == 1)
                        {
                            return LowMemoryAction.ReturnBootMenu;
                        }

                        return LowMemoryAction.Cancel;
                    }
                    else if (key.Key == Cosmos.System.ConsoleKeyEx.Escape)
                    {
                        return LowMemoryAction.ReturnBootMenu;
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(16);
                }
            }
        }

        internal static bool StartGui()
        {
            Console.Clear();
            Console.CursorVisible = false;

            windowManager = new WindowManager();

            Logger.Logger.Instance.Info("Gui", "GUI starting.");

            uint ramMb = Cosmos.Core.CPU.GetAmountOfRAM();
            if (ramMb < 1000)
            {
                LowMemoryAction action = ShowLowMemoryWarning(ramMb);
                if (action != LowMemoryAction.Continue)
                {
                    return false;
                }

                Console.Clear();
            }

            Console.WriteLine("Loading apps...");
            AppManager.LoadAllApps();

            SettingsManager.LoadSettings();
            UILib.UITheme.ApplyTheme(SettingsManager.GUI_Theme);

            ProcessManager.AddProcess(windowManager);

            ProcessManager.AddProcess(windowManager, new SettingsService()).Start();

            windowManager.Start();

            ProcessManager.AddProcess(windowManager, new Sound.SoundService()).Start();

            ProcessManager.AddProcess(windowManager, new MemService()).Start();

            Logger.Logger.Instance.Info("Gui", "Memory management service started");

            if (!Kernel.userSystem.HasUsers)
            {
                Console.WriteLine("Starting OOBE...");
                ProcessManager.AddProcess(windowManager, new ShellComponents.Oobe()).Start();
            }
            else
            {
                Console.WriteLine("Starting lock screen...");
                ProcessManager.AddProcess(windowManager, new ShellComponents.Lock()).Start();
            }

            guiRunning = true;

            // 进入 GUI 事件循环
            RunGuiLoop();

            return true;
        }

        private static void RunGuiLoop()
        {
            while (guiRunning)
            {
                ProcessManager.Yield();
            }
        }

        internal static void StopGui()
        {
            guiRunning = false;
            if (windowManager != null)
            {
                windowManager.Stop();
            }
        }
    }
}
