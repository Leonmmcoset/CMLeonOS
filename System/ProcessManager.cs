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

using System.Collections.Generic;
using CMLeonOS.Logger;

namespace CMLeonOS
{
    public static class ProcessManager
    {
        public static List<Process> Processes = new List<Process>();

        private static ulong nextProcessId = 0;


        public static Process AddProcess(Process parent, Process process)
        {
            process.Parent = parent;
            AddProcess(process);
            return process;
        }

        public static Process AddProcess(Process process)
        {

            if (Processes.Contains(process))
                return process;

            process.Id = nextProcessId;
            nextProcessId++;

            Processes.Add(process);

            Logger.Logger.Instance.Debug("ProcessManager", $"Added process: {process.Name} (ID: {process.Id})");

            return process;
        }

        public static void Sweep()
        {
            int removedCount = 0;
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!Processes[i].IsRunning)
                {
                    Processes.Remove(Processes[i]);
                    removedCount++;
                }
            }

            if (removedCount > 0)
            {
                Logger.Logger.Instance.Debug("ProcessManager", $"Swept {removedCount} stopped processes");
            }
        }

        public static void Yield()
        {
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                Process process = Processes[i];
                if (process.IsRunning)
                {
                    process.TryRun();
                }
            }
        }

        public static Process GetProcessById(ulong processId)
        {
            foreach (Process process in Processes)
            {
                if (process.Id == processId) return process;
            }
            return null;
        }

        public static Process GetProcessByName(string name)
        {
            foreach (Process process in Processes)
            {
                if (process.Name == name) return process;
            }
            return null;
        }

        public static T GetProcess<T>()
        {
            foreach (Process process in Processes)
            {
                if (process is T processT) return processT;
            }
            return default;
        }

        public static void StopAll()
        {
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                Process process = Processes[i];
                process.TryStop();
            }
        }
    }
}