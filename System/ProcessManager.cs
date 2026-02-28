using System.Collections.Generic;

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

            return process;
        }

        public static void Sweep()
        {
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!Processes[i].IsRunning)
                {
                    Processes.Remove(Processes[i]);
                }
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