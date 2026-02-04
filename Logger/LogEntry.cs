using System;

namespace CMLeonOS.Logger
{
    public class LogEntry
    {
        public DateTime Timestamp;
        public LogLevel Level;
        public string Source;
        public string Message;

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
