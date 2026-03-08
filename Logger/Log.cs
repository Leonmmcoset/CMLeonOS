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
using System.Collections.Generic;

namespace CMLeonOS.Logger
{
    public static class Log
    {
        private static List<LogEntry> logs = new List<LogEntry>();
        private static List<Action<LogEntry>> logEmittedReceivers = new List<Action<LogEntry>>();

        public static List<LogEntry> Logs
        {
            get { return logs; }
        }

        public static List<Action<LogEntry>> LogEmittedReceivers
        {
            get { return logEmittedReceivers; }
        }

        public static void AddReceiver(Action<LogEntry> receiver)
        {
            logEmittedReceivers.Add(receiver);
        }

        public static void Emit(LogLevel level, string source, string message)
        {
            var entry = new LogEntry(level, source, message);
            logs.Add(entry);

            foreach (var receiver in logEmittedReceivers)
            {
                receiver(entry);
            }
        }
    }
}
