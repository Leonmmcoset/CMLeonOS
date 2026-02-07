using System;
using System.Collections.Generic;
using System.Linq;
using CMLeonOS;

namespace CMLeonOS.Commands.Environment
{
    public static class EnvCommand
    {
        public static void ProcessEnvCommand(string args, EnvironmentVariableManager envManager, Action<string> showError)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "list", 
                        Description = "List all environment variables",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "see <varname>", 
                        Description = "View environment variable value",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "add <varname> <value>", 
                        Description = "Add environment variable",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "change <varname> <value>", 
                        Description = "Change environment variable",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "delete <varname>", 
                        Description = "Delete environment variable",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("env", commandInfos));
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
                        showError(UsageGenerator.GenerateSimpleUsage("env", "see <varname>"));
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
                        showError(UsageGenerator.GenerateSimpleUsage("env", "add <varname> <value>"));
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
                        showError(UsageGenerator.GenerateSimpleUsage("env", "change <varname> <value>"));
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
                        showError(UsageGenerator.GenerateSimpleUsage("env", "delete <varname>"));
                    }
                    break;
                default:
                    var commandInfos = new List<UsageGenerator.CommandInfo>
                    {
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "list", 
                            Description = "List all environment variables",
                            IsOptional = false 
                        },
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "see <varname>", 
                            Description = "View environment variable value",
                            IsOptional = false 
                        },
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "add <varname> <value>", 
                            Description = "Add environment variable",
                            IsOptional = false 
                        },
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "change <varname> <value>", 
                            Description = "Change environment variable",
                            IsOptional = false 
                        },
                        new UsageGenerator.CommandInfo 
                        { 
                            Command = "delete <varname>", 
                            Description = "Delete environment variable",
                            IsOptional = false 
                        }
                    };

                    showError("Error: Invalid env command");
                    showError(UsageGenerator.GenerateUsage("env", commandInfos));
                    break;
            }
        }
    }
}
