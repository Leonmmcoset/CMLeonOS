using System;
using System.Linq;

namespace CMLeonOS.Commands.Environment
{
    public static class EnvCommand
    {
        public static void ProcessEnvCommand(string args, EnvironmentVariableManager envManager, Action<string> showError)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                envManager.ListVariables();
                return;
            }
            
            string command = parts[0].ToLower();
            
            switch (command)
            {
                case "list":
                    envManager.ListVariables();
                    break;
                case "see":
                    if (parts.Length >= 2)
                    {
                        string varName = parts[1];
                        string varValue = envManager.GetVariable(varName);
                        if (varValue != null)
                        {
                            Console.WriteLine($"  {varName}={varValue}");
                        }
                        else
                        {
                            showError($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        showError("Error: Please specify variable name");
                        showError("Usage: env see <varname>");
                    }
                    break;
                case "add":
                    if (parts.Length >= 3)
                    {
                        string varName = parts[1];
                        string varValue = parts.Length > 2 ? string.Join(" ", parts.Skip(2).ToArray()) : "";
                        
                        if (envManager.SetVariable(varName, varValue))
                        {
                            Console.WriteLine($"Environment variable '{varName}' added");
                        }
                        else
                        {
                            showError($"Error: Failed to add environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        showError("Error: Please specify variable name and value");
                        showError("Usage: env add <varname> <value>");
                    }
                    break;
                case "change":
                    if (parts.Length >= 3)
                    {
                        string varName = parts[1];
                        string varValue = parts.Length > 2 ? string.Join(" ", parts.Skip(2).ToArray()) : "";
                        
                        if (envManager.SetVariable(varName, varValue))
                        {
                            Console.WriteLine($"Environment variable '{varName}' set to '{varValue}'");
                        }
                        else
                        {
                            showError($"Error: Failed to set environment variable '{varName}'");
                        }
                    }
                    else
                    {
                        showError("Error: Please specify variable name and value");
                        showError("Usage: env change <varname> <value>");
                    }
                    break;
                case "delete":
                    if (parts.Length >= 2)
                    {
                        string varName = parts[1];
                        
                        if (envManager.DeleteVariable(varName))
                        {
                            Console.WriteLine($"Environment variable '{varName}' deleted");
                        }
                        else
                        {
                            showError($"Error: Environment variable '{varName}' not found");
                        }
                    }
                    else
                    {
                        showError("Error: Please specify variable name");
                        showError("Usage: env delete <varname>");
                    }
                    break;
                default:
                    showError("Error: Invalid env command");
                    showError("Usage: env [list] | env see <varname> | env add <varname> <value> | env change <varname> <value> | env delete <varname>");
                    break;
            }
        }
    }
}
