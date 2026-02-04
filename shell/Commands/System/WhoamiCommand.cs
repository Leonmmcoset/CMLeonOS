using System;

namespace CMLeonOS.Commands.System
{
    public static class WhoamiCommand
    {
        public static void ShowCurrentUsername(CMLeonOS.UserSystem userSystem)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Current User");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine($"Username: {userSystem.CurrentUsername}");
            Console.WriteLine();
        }
    }
}
