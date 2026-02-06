using System;

namespace CMLeonOS.Commands.User
{
    public static class UserCommand
    {
        private static CMLeonOS.UserSystem userSystem;

        public static void SetUserSystem(CMLeonOS.UserSystem system)
        {
            userSystem = system;
        }

        public static void ProcessUserCommand(string args, CMLeonOS.UserSystem userSystem, Action<string> showError)
        {
            if (userSystem == null || userSystem.CurrentLoggedInUser == null || !userSystem.CurrentLoggedInUser.IsAdmin)
            {
                showError("Error: Only administrators can use the user command.");
                return;
            }

            if (string.IsNullOrEmpty(args))
            {
                showError("Error: Please specify a user command");
                showError("Please specify a user command");
                showError("user <add|delete> [args]");
                showError("  user add admin <username> <password>  - Add admin user");
                showError("  user add user <username> <password>      - Add regular user");
                showError("  user delete <username>                    - Delete user");
                showError("  user list                                    - List all users");
                return;
            }
            
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                showError("Error: Please specify a user command");
                showError("Usage: user <add|delete> [args]");
                return;
            }
            
            string subCommand = parts[0].ToLower();
            
            if (subCommand == "add")
            {
                if (parts.Length < 4)
                {
                    showError("Error: Please specify user type and username and password");
                    showError("Usage: user add admin <username> <password>");
                    showError("Usage: user add user <username> <password>");
                    return;
                }
                
                string userType = parts[1].ToLower();
                string username = parts[2];
                string password = parts[3];
                bool isAdmin = userType == "admin";
                
                userSystem.AddUser($"{username} {password}", isAdmin);
            }
            else if (subCommand == "delete")
            {
                if (parts.Length < 2)
                {
                    showError("Error: Please specify username");
                    showError("Usage: user delete <username>");
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
