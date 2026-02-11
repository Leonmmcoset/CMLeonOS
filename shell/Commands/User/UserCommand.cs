using System;
using System.Collections.Generic;
using CMLeonOS;

namespace CMLeonOS.Commands.User
{
    public static class UserCommand
    {
        private static CMLeonOS.UserSystem userSystem;

        private static bool ContainsInvalidChars(string input)
        {
            char[] invalidChars = { '<', '>', ':', '"', '|', '?', '*', '/', '\\' };
            foreach (char c in invalidChars)
            {
                if (input.Contains(c.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetUserSystem(CMLeonOS.UserSystem system)
        {
            userSystem = system;
        }

        public static void ProcessUserCommand(string args, CMLeonOS.UserSystem userSystem, Action<string> showError)
        {
            if (userSystem == null || userSystem.CurrentLoggedInUser == null || !userSystem.CurrentLoggedInUser.IsAdmin)
            {
                showError("Error: Only administrators can use this command.");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "add admin <username> <password>", 
                        Description = "Add admin user",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "add user <username> <password>", 
                        Description = "Add regular user",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "delete <username>", 
                        Description = "Delete user",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "list", 
                        Description = "List all users",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("user", commandInfos));
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                showError("Error: Please specify a user command");
                showError(UsageGenerator.GenerateSimpleUsage("user", "<add|delete> [args]"));
                return;
            }
            
            string subCommand = parts[0].ToLower();
            
            if (subCommand == "add")
            {
                if (parts.Length < 4)
                {
                    showError("Error: Please specify user type and username and password");
                    showError(UsageGenerator.GenerateSimpleUsage("user", "add admin <username> <password>"));
                    showError(UsageGenerator.GenerateSimpleUsage("user", "add user <username> <password>"));
                    return;
                }
                
                string userType = parts[1].ToLower();
                string username = parts[2];
                string password = parts[3];
                bool isAdmin = userType == "admin";
                
                if (ContainsInvalidChars(username))
                {
                    showError("Error: Username contains invalid characters: < > : \" | ? / \\");
                    return;
                }
                
                userSystem.AddUser($"{username} {password}", isAdmin);
            }
            else if (subCommand == "delete")
            {
                if (parts.Length < 2)
                {
                    showError("Error: Please specify username");
                    showError(UsageGenerator.GenerateSimpleUsage("user", "delete <username>"));
                    return;
                }
                
                string username = parts[1];
                userSystem.DeleteUser(username);
            }
            else if (subCommand == "list")
            {
                userSystem.ListUsers();
            }
            else
            {
                showError($"Error: Unknown user command '{subCommand}'");
                showError("Available commands: add, delete, list");
            }
        }
    }
}
