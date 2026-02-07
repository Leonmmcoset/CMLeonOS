using System;
using System.Collections.Generic;
using System.IO;
using UniLua;
using CMLeonOS;

namespace CMLeonOS.Commands.Script
{
    public static class LuaCommand
    {
        public static void ExecuteLuaScript(string args, CMLeonOS.FileSystem fileSystem, Shell shell, Action<string> showError, Action<string> showWarning)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length == 0)
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "<file>", 
                        Description = "Execute Lua script file",
                        IsOptional = false 
                    },
                    new UsageGenerator.CommandInfo 
                    { 
                        Command = "--shell", 
                        Description = "Enter Lua interactive shell",
                        IsOptional = false 
                    }
                };

                showError(UsageGenerator.GenerateUsage("lua", commandInfos));
                return;
            }
            
            if (parts.Length == 1 && parts[0] == "--shell")
            {
                EnterLuaShell(showError);
                return;
            }
            
            string filePath = parts[0];
            string originalPath = filePath;
            
            if (!filePath.StartsWith("0:\\") && !filePath.StartsWith("0:/"))
            {
                string currentDir = fileSystem.CurrentDirectory;
                if (currentDir == "/" || currentDir == "\\")
                {
                    filePath = "0:\\" + filePath.TrimStart('/').TrimStart('\\');
                }
                else
                {
                    filePath = Path.Combine(currentDir, filePath);
                }
            }
            
            if (!File.Exists(filePath))
            {
                showError($"Error: File not found: {filePath}");
                return;
            }
            
            try
            {
                string scriptContent = File.ReadAllText(filePath);
                
                if (string.IsNullOrWhiteSpace(scriptContent))
                {
                    showWarning("Script file is empty");
                    return;
                }
                
                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();
                
                UniLua.ThreadStatus loadResult = lua.L_LoadString(scriptContent);
                
                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                    
                    if (callResult == UniLua.ThreadStatus.LUA_OK)
                    {
                    }
                    else
                    {
                        string errorMsg = lua.ToString(-1);
                        if (string.IsNullOrWhiteSpace(errorMsg))
                        {
                            showError($"Script execution error: Unknown error");
                        }
                        else
                        {
                            showError($"Script execution error: {errorMsg}");
                        }
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        showError($"Script load error: Unknown error");
                    }
                    else
                    {
                        showError($"Script load error: {errorMsg}");
                    }
                }
            }
            catch (Exception ex)
            {
                showError($"Lua execution error: {ex.Message}");
            }
        }

        private static void EnterLuaShell(Action<string> showError)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Lua Interactive Shell");
            Console.WriteLine("====================================");
            Console.WriteLine("Type 'exit' or 'quit' to exit");
            Console.WriteLine();
            
            ILuaState lua = LuaAPI.NewState();
            lua.L_OpenLibs();
            
            while (true)
            {
                Console.Write("lua> ");
                string input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }
                
                if (input.ToLower() == "exit" || input.ToLower() == "quit")
                {
                    Console.WriteLine("Exiting Lua shell...");
                    break;
                }
                
                try
                {
                    UniLua.ThreadStatus loadResult = lua.L_LoadString(input);
                    
                    if (loadResult == UniLua.ThreadStatus.LUA_OK)
                    {
                        UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                        
                        if (callResult == UniLua.ThreadStatus.LUA_OK)
                        {
                            int top = lua.GetTop();
                            if (top > 0)
                            {
                                for (int i = 1; i <= top; i++)
                                {
                                    string result = lua.ToString(i);
                                    if (!string.IsNullOrWhiteSpace(result))
                                    {
                                        Console.WriteLine(result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string errorMsg = lua.ToString(-1);
                            if (string.IsNullOrWhiteSpace(errorMsg))
                            {
                                showError($"Execution error: Unknown error");
                            }
                            else
                            {
                                showError($"Execution error: {errorMsg}");
                            }
                        }
                    }
                    else
                    {
                        string errorMsg = lua.ToString(-1);
                        if (string.IsNullOrWhiteSpace(errorMsg))
                        {
                            showError($"Load error: Unknown error");
                        }
                        else
                        {
                            showError($"Load error: {errorMsg}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    showError($"Lua error: {ex.Message}");
                }
            }
        }
    }
}
