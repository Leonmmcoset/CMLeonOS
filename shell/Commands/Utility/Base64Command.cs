using System;

namespace CMLeonOS.Commands.Utility
{
    public static class Base64Command
    {
        public static void ProcessBase64Command(string args, Action<string> showError, Action<string> showSuccess)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                showError("Error: Please specify subcommand");
                showError("Usage: base64 encrypt <text> | base64 decrypt <text>");
                return;
            }
            
            string subcommand = parts[0].ToLower();
            
            if (subcommand != "encrypt" && subcommand != "decrypt")
            {
                showError("Error: Invalid subcommand");
                showError("Usage: base64 encrypt <text> | base64 decrypt <text>");
                return;
            }
            
            if (parts.Length < 2)
            {
                showError("Error: Please specify text to process");
                showError($"Usage: base64 {subcommand} <text>");
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
