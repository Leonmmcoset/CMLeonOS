using System;
using Sys = Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core;
using System.Threading;

namespace CMLeonOS
{
    public enum BootMenuAction
    {
        NormalBoot,
        Reboot,
        Shutdown
    }

    internal static class BootMenu
    {
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
            // 这里老显示 unknown，谁知道为啥
            // Console.WriteLine($"Git Commit: {Version.GitCommit}");
            Console.WriteLine($"Build Time: {GetBuildTime()}");
            Console.WriteLine();
            Console.WriteLine($"Auto-select in {remainingTime} seconds...");
            Console.WriteLine();
            Console.WriteLine("Select an option:");
            Console.WriteLine();

            PrintOption("Normal Boot", selIdx == 0);
            PrintOption("Reboot", selIdx == 1);
            PrintOption("Shutdown", selIdx == 2);
        }

        private static BootMenuAction Confirm(int selIdx)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            Console.SetCursorPosition(0, 0);

            Console.CursorVisible = true;

            switch (selIdx)
            {
                case 0:
                    return BootMenuAction.NormalBoot;
                case 1:
                    Sys.Power.Reboot();
                    return BootMenuAction.Reboot;
                case 2:
                    Sys.Power.Shutdown();
                    return BootMenuAction.Shutdown;
                default:
                    return BootMenuAction.NormalBoot;
            }
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

                if (selIdx < 0)
                {
                    selIdx = 2;
                }

                if (selIdx > 2)
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
