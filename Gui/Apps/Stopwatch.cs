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
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class Stopwatch : Process
    {
        internal Stopwatch() : base("Stopwatch", ProcessType.Application) { }

        AppWindow window;

        Button startStopButton;

        Button lapResetButton;

        Table lapTable;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private const string format = @"hh\:mm\:ss";

        private readonly Color background = Color.FromArgb(56, 56, 71);

        private bool stopwatchRunning = false;

        DateTime startTime;
        DateTime lastLap;
        TimeSpan elapsed = TimeSpan.Zero;
        int lastSecond;

        private void UpdateWindow()
        {
            window.Clear(background);

            string text;
            text = elapsed.ToString(format);
            window.DrawString(text, System.Drawing.Color.White, (window.Width / 2) - ((text.Length * FontData.Width) / 2), 8);

            wm.Update(window);
        }

        private void StartStopwatch()
        {
            if (stopwatchRunning) return;

            stopwatchRunning = true;
            startTime = DateTime.Now;
            lastLap = startTime;
            lastSecond = startTime.Second;

            startStopButton.Text = "Stop";
            lapResetButton.Text = "Lap";
        }

        private void StopStopwatch()
        {
            if (!stopwatchRunning) return;

            stopwatchRunning = false;
            UpdateWindow();

            startStopButton.Text = "Start";
            lapResetButton.Text = "Reset";
        }

        private void Lap()
        {
            DateTime now = DateTime.Now;

            TableCell cell = new TableCell((now - lastLap).ToString(format));
            lapTable.Cells.Add(cell);
            lapTable.ScrollToBottom();
            lapTable.Render();
            wm.Update(lapTable);

            lastLap = DateTime.Now;
        }

        private void Reset()
        {
            elapsed = TimeSpan.Zero;

            lapTable.Cells.Clear();
            lapTable.Render();
            wm.Update(lapTable);

            UpdateWindow();
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 160, 160);
            wm.AddWindow(window);
            window.Title = "Stopwatch";
            window.Icon = AppManager.GetAppMetadata("Stopwatch").Icon;
            window.Closing = TryStop;

            startStopButton = new Button(window, window.Width / 2, window.Height - 24, window.Width / 2, 24);
            startStopButton.Text = "Start";
            startStopButton.OnClick = (int x, int y) =>
            {
                if (stopwatchRunning)
                {
                    StopStopwatch();
                }
                else
                {
                    StartStopwatch();
                }
            };
            wm.AddWindow(startStopButton);

            lapResetButton = new Button(window, 0, window.Height - 24, window.Width / 2, 24);
            lapResetButton.Text = "Reset";
            lapResetButton.OnClick = (int x, int y) =>
            {
                if (stopwatchRunning)
                {
                    Lap();
                }
                else
                {
                    Reset();
                }
            };
            wm.AddWindow(lapResetButton);

            int lapTableY = 8 + FontData.Height + 8;
            lapTable = new Table(window, 0, lapTableY, window.Width, window.Height - lapTableY - 24);
            lapTable.Background = Color.FromArgb(80, 80, 102);
            lapTable.Border = background;
            lapTable.Foreground = Color.FromArgb(185, 185, 234);
            lapTable.TextAlignment = Alignment.Middle;
            lapTable.AllowSelection = false;
            lapTable.CellHeight = 20;
            lapTable.ScrollbarThickness = 15;
            lapTable.Render();
            wm.AddWindow(lapTable);

            UpdateWindow();
        }

        public override void Run()
        {
            int second = DateTime.Now.Second;
            if (lastSecond != second)
            {
                lastSecond = second;
                if (stopwatchRunning)
                {
                    elapsed = DateTime.Now - startTime;
                }
                UpdateWindow();
            }
        }
    }
}
