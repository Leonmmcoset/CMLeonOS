using System;
using CMLeonOS.Settings;

namespace CMLeonOS.Commands
{
    public static class SettingsCommand
    {
        private static UserSystem userSystem;

        public static void SetUserSystem(UserSystem system)
        {
            userSystem = system;
        }

        public static void ProcessSettings(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                SettingsManager.ListSettings();
                return;
            }

            if (userSystem == null || userSystem.CurrentLoggedInUser == null || !userSystem.CurrentLoggedInUser.IsAdmin)
            {
                Console.WriteLine("Error: Only administrators can change settings.");
                return;
            }

            string[] parts = args.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 1)
            {
                string key = parts[0];
                string value = SettingsManager.GetSetting(key);
                if (value != null)
                {
                    Console.WriteLine($"{key} = {value}");
                }
                else
                {
                    Console.WriteLine($"Error: Setting '{key}' not found");
                }
            }
            else if (parts.Length == 2)
            {
                string key = parts[0];
                string value = parts[1];
                
                if (key.ToLower() == "loggerenabled")
                {
                    if (value.ToLower() == "true" || value.ToLower() == "false")
                    {
                        SettingsManager.LoggerEnabled = value.ToLower() == "true";
                        Console.WriteLine($"LoggerEnabled set to {value.ToLower()}");
                    }
                    else
                    {
                        Console.WriteLine("Error: LoggerEnabled must be 'true' or 'false'");
                    }
                }
                else
                {
                    SettingsManager.SetSetting(key, value);
                    Console.WriteLine($"{key} set to {value}");
                }
            }
        }
    }
}
