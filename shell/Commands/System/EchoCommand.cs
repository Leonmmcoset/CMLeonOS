using System;

namespace CMLeonOS.Commands.System
{
    public static class EchoCommand
    {
        public static void ProcessEcho(string args)
        {
            var processedArgs = args.Replace("\\n", "\n");
            Console.WriteLine(processedArgs);
        }
    }
}
