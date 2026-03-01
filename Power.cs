using Sys = Cosmos.System;

namespace CMLeonOS
{
    public static class Power
    {
        public static void Reboot()
        {
            Sys.Power.Reboot();
        }

        public static void Shutdown(bool reboot = false)
        {
            if (reboot)
            {
                Sys.Power.Reboot();
            }
            else
            {
                Sys.Power.Shutdown();
            }
        }
    }
}
