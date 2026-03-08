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

using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class Tasks : Process
    {
        internal Tasks() : base("Tasks", ProcessType.Application) { }

        AppWindow window;

        Table table;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        int lastSecond = DateTime.Now.Second;

        private void PopulateTable()
        {
            table.Cells.Clear();
            foreach (Process process in ProcessManager.Processes)
            {
                table.Cells.Add(new TableCell(process.Name));
            }
            table.Render();
        }

        private void EndTaskClicked(int x, int y)
        {
            if (table.SelectedCellIndex != -1 && table.SelectedCellIndex < ProcessManager.Processes.Count)
            {
                if (UserSystem.CurrentLoggedInUser == null || !UserSystem.CurrentLoggedInUser.Admin)
                {
                    MessageBox messageBox = new MessageBox(this, Name, "You must be an admin to end tasks.");
                    messageBox.Show();

                    return;
                }

                ProcessManager.Processes[table.SelectedCellIndex].TryStop();
                ProcessManager.Sweep();
                table.SelectedCellIndex = -1;
                PopulateTable();
            }
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 384, 256);
            wm.AddWindow(window);
            window.Title = "Tasks";
            window.Icon = AppManager.GetAppMetadata("Tasks").Icon;
            window.Closing = TryStop;

            window.Clear(Color.Gray);

            table = new Table(window, 12, 12, window.Width - 24, window.Height - 24 - 20 - 12);
            PopulateTable();
            wm.AddWindow(table);

            Button endTask = new Button(window, window.Width - 100 - 12, window.Height - 20 - 12, 100, 20);
            endTask.Text = "End Task";
            endTask.OnClick = EndTaskClicked;
            wm.AddWindow(endTask);

            wm.Update(window);
        }

        public override void Run()
        {
            DateTime now = DateTime.Now;
            if (lastSecond != now.Second)
            {
                PopulateTable();
                lastSecond = now.Second;
            }
        }
    }
}
