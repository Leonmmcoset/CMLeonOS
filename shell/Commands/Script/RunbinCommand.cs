using Cosmos.System.Emulation;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Commands.Script
{
    public static class RunbinCommand
    {
        public static void Execute(string args, CMLeonOS.FileSystem fileSystem, Action<string> showError)
        {
            string[] parts = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                var commandInfos = new List<UsageGenerator.CommandInfo>
                {
                    new UsageGenerator.CommandInfo
                    {
                        Command = "<file>",
                        Description = "Run a MSE executable binary file",
                        IsOptional = false
                    }
                };
                showError(UsageGenerator.GenerateUsage("runbin", commandInfos));
                return;
            }

            string inputPath = parts[0];
            string fullPath = fileSystem.GetFullPath(inputPath);

            if (!File.Exists(fullPath))
            {
                showError($"Error: File not found: {fullPath}");
                return;
            }

            try
            {
                byte[] data = File.ReadAllBytes(fullPath);
                if (data == null || data.Length == 0)
                {
                    showError("Error: Executable file is empty.");
                    return;
                }

                FGMSECInstructionSet set = new FGMSECInstructionSet();
                Executable exec = new Executable(data, set, 3);

                AddDefaultSystemCalls(exec);
                exec.ReadData();

                while (exec.running)
                {
                    exec.NextInstruction();
                }
            }
            catch (Exception ex)
            {
                showError($"runbin error: {ex.Message}");
            }
        }

        private static void AddDefaultSystemCalls(Executable exec)
        {
            // syscall 0: print null-terminated string at address in R3
            exec.AddSystemCall((Executable caller) =>
            {
                int addr = (int)((FGMSECInstructionSet)caller.usingInstructionSet).CPU.GetRegData(3);
                char c = (char)caller.Memory.ReadChar(addr);
                while (c != 0)
                {
                    Console.Write(c);
                    addr++;
                    c = (char)caller.Memory.ReadChar(addr);
                }
            });

            // syscall 1: read line and return address in R3
            exec.AddSystemCall((Executable caller) =>
            {
                string input = Console.ReadLine() ?? string.Empty;
                input += '\0';
                int addr = caller.Memory.Data.Count;
                int caddr = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    char c = input[i];
                    if (!caller.Memory.AddChar(c))
                    {
                        caller.Memory.WriteChar(caddr, c);
                        addr = 0;
                        caddr++;
                    }
                }
                ((FGMSECInstructionSet)caller.usingInstructionSet).CPU.SetRegData(3, (uint)addr);
            });

            // syscall 2: clear console
            exec.AddSystemCall((Executable caller) =>
            {
                Console.Clear();
            });
        }
    }
}
