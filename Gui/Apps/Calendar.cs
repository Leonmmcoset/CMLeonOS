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

namespace CMLeonOS.Gui.Apps
{
    internal class Calendar : Process
    {
        internal Calendar() : base("Calendar", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        UILib.Calendar cal;

        Button nextButton;

        Button prevButton;

        private void PrevClicked(int x, int y)
        {
            if (cal.Month == 1)
            {
                cal.SetCalendar(cal.Year - 1, 12);
            }
            else
            {
                cal.SetCalendar(cal.Year, cal.Month - 1);
            }
        }

        private void NextClicked(int x, int y)
        {
            if (cal.Month == 12)
            {

                cal.SetCalendar(cal.Year + 1, 1);
            }
            else
            {
                cal.SetCalendar(cal.Year, cal.Month + 1);
            }
        }

        private void WindowResized()
        {
            cal.Resize(window.Width, window.Height);

            cal.Render();
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 320, 256, 384, 288);
            wm.AddWindow(window);
            window.Title = "Calendar";
            window.Icon = AppManager.GetAppMetadata("Calendar").Icon;
            window.CanResize = true;
            window.UserResized = WindowResized;
            window.Closing = TryStop;

            cal = new UILib.Calendar(window, 0, 0, window.Width, window.Height);
            wm.AddWindow(cal);

            DateTime now = DateTime.Now;
            cal.Year = now.Year;
            cal.Month = now.Month;

            prevButton = new Button(window, 8, 8, 24, 24);
            prevButton.Text = "<";
            prevButton.OnClick = PrevClicked;
            wm.AddWindow(prevButton);

            nextButton = new Button(window, 40, 8, 24, 24);
            nextButton.Text = ">";
            nextButton.OnClick = NextClicked;
            wm.AddWindow(nextButton);

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
