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
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Commands
{
    public static class AliasCommand
    {
        private static string aliasFilePath = @"0:\system\alias.dat";
        private static Dictionary<string, string> aliases = new Dictionary<string, string>();

        public static void LoadAliases()
        {
            aliases.Clear();
            
            try
            {
                if (File.Exists(aliasFilePath))
                {
                    string[] lines = File.ReadAllLines(aliasFilePath);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                        {
                            int separatorIndex = line.IndexOf('=');
                            if (separatorIndex > 0)
                            {
                                string name = line.Substring(0, separatorIndex).Trim();
                                string value = line.Substring(separatorIndex + 1).Trim();
                                aliases[name] = value;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading aliases: {e.Message}");
            }
        }

        public static void SaveAliases()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(aliasFilePath));
                
                using (StreamWriter writer = new StreamWriter(aliasFilePath))
                {
                    writer.WriteLine("# CMLeonOS Alias Configuration");
                    writer.WriteLine("# Format: alias_name=command");
                    writer.WriteLine();
                    
                    var keys = new List<string>(aliases.Keys);
                    
                    for (int i = 0; i < keys.Count - 1; i++)
                    {
                        for (int j = i + 1; j < keys.Count; j++)
                        {
                            if (string.Compare(keys[i], keys[j]) > 0)
                            {
                                string temp = keys[i];
                                keys[i] = keys[j];
                                keys[j] = temp;
                            }
                        }
                    }
                    
                    foreach (string key in keys)
                    {
                        writer.WriteLine($"{key}={aliases[key]}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving aliases: {e.Message}");
            }
        }

        public static void AddAlias(string name, string command)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine("Error: Alias name and command cannot be empty");
                return;
            }

            aliases[name] = command;
            SaveAliases();
            Console.WriteLine($"Alias '{name}' added successfully");
        }

        public static void RemoveAlias(string name)
        {
            if (aliases.Remove(name))
            {
                SaveAliases();
                Console.WriteLine($"Alias '{name}' removed successfully");
            }
            else
            {
                Console.WriteLine($"Error: Alias '{name}' not found");
            }
        }

        public static void ListAliases()
        {
            if (aliases.Count == 0)
            {
                Console.WriteLine("No aliases defined");
                return;
            }

            Console.WriteLine("Defined aliases:");
            var keys = new List<string>(aliases.Keys);
            
            for (int i = 0; i < keys.Count - 1; i++)
            {
                for (int j = i + 1; j < keys.Count; j++)
                {
                    if (string.Compare(keys[i], keys[j]) > 0)
                    {
                        string temp = keys[i];
                        keys[i] = keys[j];
                        keys[j] = temp;
                    }
                }
            }
            
            foreach (string key in keys)
            {
                Console.WriteLine($"  {key} => {aliases[key]}");
            }
        }

        public static string GetAlias(string name)
        {
            if (aliases.TryGetValue(name, out string value))
            {
                return value;
            }
            return null;
        }

        public static bool AliasExists(string name)
        {
            return aliases.ContainsKey(name);
        }

        public static void ClearAliases()
        {
            aliases.Clear();
            SaveAliases();
            Console.WriteLine("All aliases cleared");
        }
    }
}
