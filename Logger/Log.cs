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
