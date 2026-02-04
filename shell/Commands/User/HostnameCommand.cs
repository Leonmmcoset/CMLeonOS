using System;

namespace CMLeonOS.Commands.User
{
    public static class HostnameCommand
    {
        public static void ProcessHostnameCommand(string args, CMLeonOS.UserSystem userSystem, Action<string> showError)
        {
            if (string.IsNullOrEmpty(args))
            {
                showError("Usage: hostname <new_hostname>");
                return;
            }
            
            userSystem.ProcessHostnameCommand(args);
        }
    }
}
