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
using System;

namespace CMLeonOS.Gui
{
    internal static class Gui
    {
        private static bool guiRunning = false;
        private static WindowManager windowManager;

        internal static bool StartGui()
        {
            Console.Clear();
            Console.CursorVisible = false;

            windowManager = new WindowManager();

            Logger.Logger.Instance.Info("Gui", "GUI starting.");

            if (Cosmos.Core.CPU.GetAmountOfRAM() < 1000)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Not enough system memory is available to run the GUI.");
                Console.WriteLine("At least 1 GB should be allocated.");
                Console.ResetColor();
                Console.Write("Continue anyway? [y/N]");

                if (Console.ReadKey(true).Key != ConsoleKey.Y)
                {
                    return false;
                }
            }

            Console.WriteLine("Loading apps...");
            AppManager.LoadAllApps();

            ProcessManager.AddProcess(windowManager);

            ProcessManager.AddProcess(windowManager, new SettingsService()).Start();

            windowManager.Start();

            ProcessManager.AddProcess(windowManager, new Sound.SoundService()).Start();

            ProcessManager.AddProcess(windowManager, new MemService()).Start();

            Logger.Logger.Instance.Info("Gui", "Memory management service started");

            Console.WriteLine("Starting lock screen...");
            ProcessManager.AddProcess(windowManager, new ShellComponents.Lock()).Start();

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
