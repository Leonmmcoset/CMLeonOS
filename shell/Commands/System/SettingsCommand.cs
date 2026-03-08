// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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

            if (userSystem == null || UserSystem.CurrentLoggedInUser == null || !UserSystem.CurrentLoggedInUser.IsAdmin)
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
                else if (key.ToLower() == "maxloginattempts")
                {
                    if (int.TryParse(value, out int attempts) && attempts > 0)
                    {
                        SettingsManager.MaxLoginAttempts = attempts;
                        Console.WriteLine($"MaxLoginAttempts set to {attempts}");
                    }
                    else
                    {
                        Console.WriteLine("Error: MaxLoginAttempts must be a positive integer");
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
