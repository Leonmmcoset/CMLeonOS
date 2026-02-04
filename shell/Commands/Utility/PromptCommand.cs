using System;

namespace CMLeonOS.Commands.Utility
{
    public static class PromptCommand
    {
        public static void ChangePrompt(string args, ref string prompt)
        {
            if (!string.IsNullOrEmpty(args))
            {
                prompt = args;
            }
            else
            {
                prompt = "/";
            }
        }
    }
}
