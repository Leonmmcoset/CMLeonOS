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
