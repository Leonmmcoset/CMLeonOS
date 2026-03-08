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
using CMLeonOS.Utils;
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class MemoryStatistics : Process
    {
        internal MemoryStatistics() : base("MemoryStatistics", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private int lastSecond;

        private static int padding = 12;
        private static int barHeight = 12;
        private static Color barColour = Color.FromArgb(0, 155, 254);

        private void Update()
        {
            window.Clear(Color.LightGray);

            var statistics = MemoryStatisticsProvider.GetMemoryStatistics();

            window.DrawString("Memory Statistics", Color.DarkBlue, padding, padding);

            window.DrawString(string.Format(@"Total: {0} MB
Unavailable: {1} MB
Used: {2:d1} MB
Free: {3} MB
Percentage Used: {4:d1}%",
            statistics.TotalMB,
            statistics.UnavailableMB,
            statistics.UsedMB,
            statistics.FreeMB,
            statistics.PercentUsed), Color.Black, padding, padding + FontData.Height + padding);

            window.DrawFilledRectangle(0, window.Height - barHeight, window.Width, barHeight, Color.Black);
            window.DrawFilledRectangle(0, window.Height - barHeight, (int)(window.Width * ((float)statistics.PercentUsed / 100f)), barHeight, barColour);

            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 192);
            wm.AddWindow(window);
            window.Title = "Memory Statistics";
            window.Icon = AppManager.GetAppMetadata("Memory Statistics").Icon;
            window.Closing = TryStop;

            Update();
        }

        public override void Run()
        {
            int second = DateTime.Now.Second;
            if (lastSecond != second)
            {
                lastSecond = second;
                Update();
            }
        }
    }
}
