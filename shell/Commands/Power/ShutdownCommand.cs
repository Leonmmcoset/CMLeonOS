using System;
using Sys = Cosmos.System;

namespace CMLeonOS.Commands.Power
{
    public static class ShutdownCommand
    {
        public static void ProcessShutdown()
        {
            Console.WriteLine("Shutting down system...");
            Sys.Power.Shutdown();
        }
    }
}
