using System;
using System.Collections.Generic;
using CMLeonOS.Commands;
using CMLeonOS.shell;

namespace CMLeonOS
{
    public class ShellProcess : Process
    {
        private Shell shell;

        public ShellProcess(UserSystem userSystem) : base("Shell", ProcessType.Application)
        {
            shell = new Shell(userSystem);
        }

        public override void Run()
        {
            CMLeonOS.Logger.Logger.Instance.Info("Shell", "Shell process started");
            
            try
            {
                shell.Run();
            }
            catch (Exception e)
            {
                CMLeonOS.Logger.Logger.Instance.Error("Shell", $"Shell crashed: {e.ToString()}");
            }
            
            CMLeonOS.Logger.Logger.Instance.Info("Shell", "Shell process stopped");
        }
    }

    public class CommandProcess : Process
    {
        private string command;
        private string args;
        private Shell shell;

        public CommandProcess(string command, string args, Shell shell) : base($"Command_{command}", ProcessType.Application)
        {
            this.command = command;
            this.args = args;
            this.shell = shell;
        }

        public override void Run()
        {
            CMLeonOS.Logger.Logger.Instance.Info($"Command_{command}", $"Executing command: {command} {args}");
            
            try
            {
                CommandList.ProcessCommand(shell, command, args);
            }
            catch (Exception e)
            {
                CMLeonOS.Logger.Logger.Instance.Error($"Command_{command}", $"Command failed: {e.ToString()}");
            }
            
            CMLeonOS.Logger.Logger.Instance.Info($"Command_{command}", $"Command completed: {command}");
        }
    }
}
