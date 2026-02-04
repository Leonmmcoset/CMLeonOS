using System;

namespace CMLeonOS.Commands.System
{
    public static class DateCommand
    {
        public static void ProcessDate()
        {
            Console.WriteLine(DateTime.Now.ToShortDateString());
        }
    }
}
