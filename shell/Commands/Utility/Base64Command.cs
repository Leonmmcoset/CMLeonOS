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
