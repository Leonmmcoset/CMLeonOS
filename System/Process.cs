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
using Sys = Cosmos.System;

namespace CMLeonOS
{
    public enum ProcessType
    {
        Service,
        Application,
        Background
    }

    public abstract class Process
    {
        private protected Process(string name, ProcessType type)
        {
            Name = name;
            Type = type;
        }

        private protected Process(string name, ProcessType type, Process parent)
        {
            Name = name;
            Type = type;
            Parent = parent;
        }

        internal ulong Id { get; set; }

        internal List<string> Args { get; private set; }

        internal string Name { get; set; }

        internal ProcessType Type { get; private set; }

        internal DateTime Created { get; private set; } = DateTime.Now;

        internal bool IsRunning { get; private set; } = false;

        internal bool Swept { get; set; } = false;

        internal bool Critical { get; set; } = false;

        internal Process Parent { get; set; }

        public virtual void Start()
        {
            if (Type == ProcessType.Service)
            {
                CMLeonOS.Logger.Logger.Instance.Info("Process", $"Service starting: {Name}");
            }

            IsRunning = true;
        }

        public abstract void Run();

        public virtual void Stop()
        {
            if (Type == ProcessType.Service)
            {
                CMLeonOS.Logger.Logger.Instance.Info("Process", $"Service stopping: {Name}");
            }

            IsRunning = false;
            foreach (Process process in ProcessManager.Processes)
            {
                if (process.Parent == this && process.IsRunning)
                {
                    process.TryStop();
                }
            }
        }

        public void TryRun()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                if (Critical)
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("       CRITICAL PROCESS CRASHED");
                    Console.WriteLine("========================================");
                    Console.WriteLine();
                    Console.WriteLine($"Process: {Name}");
                    Console.WriteLine($"ID: {Id}");
                    Console.WriteLine();
                    Console.WriteLine("Error:");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine();
                    Console.WriteLine("Press any key to reboot...");
                    Console.ReadKey();
                    Sys.Power.Reboot();
                }
                else
                {
                    CMLeonOS.Logger.Logger.Instance.Error("Process", $"Process \"{Name}\" ({Id}) crashed: {e.ToString()}");
                    TryStop();
                }
            }
        }

        public void TryStart()
        {
            try
            {
                Start();
            }
            catch (Exception e)
            {
                if (Critical)
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("       CRITICAL PROCESS CRASHED");
                    Console.WriteLine("========================================");
                    Console.WriteLine();
                    Console.WriteLine($"Process: {Name}");
                    Console.WriteLine($"ID: {Id}");
                    Console.WriteLine();
                    Console.WriteLine("Error:");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine();
                    Console.WriteLine("Press any key to reboot...");
                    Console.ReadKey();
                    Sys.Power.Reboot();
                }
                else
                {
                    CMLeonOS.Logger.Logger.Instance.Error("Process", $"Process \"{Name}\" ({Id}) crashed while starting: {e.ToString()}");
                    TryStop();
                }
            }
        }

        public void TryStop()
        {
            try
            {
                Stop();
            }
            catch (Exception e)
            {
                IsRunning = false;
                if (Critical)
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("       CRITICAL PROCESS CRASHED");
                    Console.WriteLine("========================================");
                    Console.WriteLine();
                    Console.WriteLine($"Process: {Name}");
                    Console.WriteLine($"ID: {Id}");
                    Console.WriteLine();
                    Console.WriteLine("Error:");
                    Console.WriteLine(e.ToString());
                    Console.WriteLine();
                    Console.WriteLine("Press any key to reboot...");
                    Console.ReadKey();
                    Sys.Power.Reboot();
                }
                else
                {
                    CMLeonOS.Logger.Logger.Instance.Error("Process", $"Process \"{Name}\" ({Id}) crashed while stopping: {e.ToString()}");
                }
            }
        }
    }
}