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
    public class LogEntry
    {
        public DateTime Timestamp;
        public LogLevel Level;
        public string Source;
        public string Message;
        
        public LogLevel Priority
        {
            get { return Level; }
        }
        
        public DateTime Date
        {
            get { return Timestamp; }
        }

        public LogEntry(LogLevel level, string source, string message)
        {
            Timestamp = DateTime.Now;
            Level = level;
            Source = source;
            Message = message;
        }

        public override string ToString()
        {
            string levelStr = GetLevelString(Level);
            return "[" + levelStr + "] [" + Source + "] " + Message;
        }

        private string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Info:
                    return "INFO";
                case LogLevel.Warning:
                    return "WARN";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Success:
                    return "SUCCESS";
                default:
                    return "UNKNOWN";
            }
        }
    }
}
