using System;
using System.Collections.Generic;

namespace CMLeonOS.Commands.Utility
{
    public static class HistoryCommand
    {
        public static void ShowHistory(List<string> commandHistory)
        {
            for (int i = 0; i < commandHistory.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {commandHistory[i]}");
            }
        }
    }
}
