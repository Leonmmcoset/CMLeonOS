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
using CMLeonOS.Logger;

namespace CMLeonOS.Commands
{
    public static class LogsCommand
    {
        public static void ShowLogs()
        {
            try
            {
                var logs = Log.Logs;
                if (logs == null || logs.Count == 0)
                {
                    Console.WriteLine("No logs available.");
                    return;
                }

                Console.WriteLine($"Total logs: {logs.Count}");
                Console.WriteLine();
                int countToShow = Math.Min(logs.Count, 20);
                int startIndex = logs.Count - countToShow;

                for (int i = startIndex; i < logs.Count; i++)
                {
                    LogEntry entry = logs[i];
                    WriteToConsole(entry);
                }

                if (logs.Count > 20)
                {
                    Console.WriteLine();
                    Console.WriteLine($"... and {logs.Count - 20} more logs not shown.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading logs: {ex.Message}");
            }
        }

        private static void WriteToConsole(LogEntry entry)
        {
                ConsoleColor originalBackgroundColor = Console.BackgroundColor;
                ConsoleColor originalForegroundColor = Console.ForegroundColor;
                
                // 日志等级
                Console.Write("[ ");
                switch (entry.Level)
                {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }

                Console.Write(entry.ToStringLevelStr());
                Console.ForegroundColor = originalForegroundColor;
                Console.Write(" ] ");
                
                // 日志来源
                Console.Write($"{entry.Source}: ");

                // 日志内容
                Console.Write(entry.ToStringMessage());

                // 换行
                Console.WriteLine();

                Console.ForegroundColor = originalForegroundColor;
                Console.BackgroundColor = originalBackgroundColor;
        }
    }
}