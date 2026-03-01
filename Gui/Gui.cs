using CMLeonOS;
using CMLeonOS.Logger;
using System;
using System.Threading;

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
            Thread.Sleep(1000);
            AppManager.LoadAllApps();

            ProcessManager.AddProcess(windowManager);

            ProcessManager.AddProcess(windowManager, new SettingsService()).Start();

            windowManager.Start();

            ProcessManager.AddProcess(windowManager, new Sound.SoundService()).Start();

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
                System.Threading.Thread.Sleep(1);
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
