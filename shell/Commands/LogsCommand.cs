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
                    string level = entry.Level switch
                    {
                        LogLevel.Debug => "DEBUG",
                        LogLevel.Info => "INFO ",
                        LogLevel.Warning => "WARN ",
                        LogLevel.Error => "ERROR",
                        LogLevel.Success => "SUCCESS",
                        _ => "UNKNOWN"
                    };

                    string time = entry.Date.ToString("HH:mm:ss");
                    string message = entry.Message.Length > 50 ? entry.Message.Substring(0, 50) + "..." : entry.Message;

                    Console.ForegroundColor = GetLevelColor(entry.Level);
                    Console.WriteLine($"[{time}] [{level}] [{entry.Source}] {message}");
                    Console.ResetColor();
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

        private static ConsoleColor GetLevelColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.Cyan,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Success => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }
    }
}