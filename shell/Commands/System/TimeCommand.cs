using System;

namespace CMLeonOS.Commands.System
{
    public static class TimeCommand
    {
        public static void ProcessTime()
        {
            Console.WriteLine(DateTime.Now.ToString());
        }
    }
}
