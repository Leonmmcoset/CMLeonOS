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

namespace CMLeonOS.Logger
{
    public class Logger
    {
        private static Logger _instance;
        
        private LogLevel _minLogLevel;
        private bool _enableConsoleOutput;

        private Logger()
        {
            _minLogLevel = LogLevel.Info;
            _enableConsoleOutput = true;
        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        public LogLevel MinLogLevel
        {
            get { return _minLogLevel; }
            set { _minLogLevel = value; }
        }

        public bool EnableConsoleOutput
        {
            get { return _enableConsoleOutput; }
            set { _enableConsoleOutput = value; }
        }

        public void Log(LogLevel level, string source, string message)
        {
            if (level < _minLogLevel)
            {
                return;
            }

            var entry = new LogEntry(level, source, message);

            if (_enableConsoleOutput)
            {
                WriteToConsole(entry);
            }

            CMLeonOS.Logger.Log.Emit(level, source, message);
        }

        public void Debug(string source, string message)
        {
            Log(LogLevel.Debug, source, message);
        }

        public void Info(string source, string message)
        {
            Log(LogLevel.Info, source, message);
        }

        public void Warning(string source, string message)
        {
            Log(LogLevel.Warning, source, message);
        }

        public void Error(string source, string message)
        {
            Log(LogLevel.Error, source, message);
        }

        public void Success(string source, string message)
        {
            Log(LogLevel.Success, source, message);
        }

        private void WriteToConsole(LogEntry entry)
        {
            if (Settings.SettingsManager.LoggerEnabled) {
                ConsoleColor originalColor = Console.ForegroundColor;
                
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

                Console.WriteLine(entry.ToString());
                Console.ForegroundColor = originalColor;
            }
        }
    }
}
