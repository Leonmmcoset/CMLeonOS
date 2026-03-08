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
using CMLeonOS;

namespace CMLeonOS.Commands.Utility
{
    public static class Base64Command
    {
        public static void ProcessBase64Command(string args, Action<string> showError, Action<string> showSuccess)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "encrypt <text>", 
                        Description = "Encode text to Base64",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "decrypt <text>", 
                        Description = "Decode Base64 to text",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateCompactUsage("base64", commandInfos));
                return;
            }
            
            string subcommand = parts[0].ToLower();
            
            if (subcommand != "encrypt" && subcommand != "decrypt")
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "encrypt <text>", 
                        Description = "Encode text to Base64",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "decrypt <text>", 
                        Description = "Decode Base64 to text",
                        IsOptional = false 
                    }
                };

                showError("Error: Invalid subcommand");
                showError(UsageGenerator.GenerateCompactUsage("base64", commandInfos));
                return;
            }
            
            if (parts.Length < 2)
            {
                showError("Error: Please specify text to process");
                showError(UsageGenerator.GenerateSimpleUsage("base64", $"{subcommand} <text>"));
                return;
            }
            
            string text = string.Join(" ", parts, 1, parts.Length - 1);
            
            Console.WriteLine("====================================");
            Console.WriteLine("        Base64");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            try
            {
                if (subcommand == "encrypt")
                {
                    string encoded = Base64Helper.Encode(text);
                    Console.WriteLine($"Original: {text}");
                    Console.WriteLine();
                    Console.WriteLine($"Encoded: {encoded}");
                }
                if (subcommand == "decrypt")
                {
                    string decoded = Base64Helper.Decode(text);
                    Console.WriteLine($"Encoded: {text}");
                    Console.WriteLine();
                    Console.WriteLine($"Decoded: {decoded}");
                }
                
                Console.WriteLine();
                showSuccess("Base64 operation completed");
            }
            catch (Exception ex)
            {
                showError($"Base64 error: {ex.Message}");
            }
        }
    }
}
