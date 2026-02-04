using System;
using Sys = Cosmos.System;

namespace CMLeonOS.Commands.Power
{
    public static class RestartCommand
    {
        public static void ProcessRestart()
        {
            Console.WriteLine("Restarting system...");
            Sys.Power.Reboot();
        }
    }
}
