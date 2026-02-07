using System;
using System.Collections.Generic;
using CMLeonOS;

namespace CMLeonOS.Commands.User
{
    public static class HostnameCommand
    {
        public static void ProcessHostnameCommand(string args, CMLeonOS.UserSystem userSystem, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<new_hostname>", 
                        Description = "Set new hostname",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("hostname", commandInfos));
                return;
            }
            
            userSystem.ProcessHostnameCommand(args);
        }
    }
}
