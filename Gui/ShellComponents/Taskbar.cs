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

using Cosmos.System.Graphics;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using CMLeonOS.UILib.Animations;
using CMLeonOS.Settings;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.ShellComponents
{
    internal class Taskbar : Process
    {
        internal Taskbar() : base("Taskbar", ProcessType.Application)
        {
            Critical = true;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Start.bmp")]
        private static byte[] startBytes;
        private static Bitmap startBitmap = new Bitmap(startBytes);

        Window window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        DateTime lastDate = DateTime.Now;

        TextBlock time;

        ImageBlock start;

        SettingsService settingsService;

        private bool miniCalendarOpen = false;
        private Calendar miniCalendar;

        private int timeUpdateTicks = 0;

        internal void SetLeftHandStartButton(bool left)
        {
            if (left)
            {
                start.X = 0;
            }
            else
            {
                start.X = (int)((window.Width / 2) - (startBitmap.Width / 2));
            }
        }

        internal int GetTaskbarHeight()
        {
            return window.Height;
        }

        internal void UpdateTime()
        {
            string timeText;
            if (SettingsManager.GUI_TwelveHourClock)
            {
                timeText = DateTime.Now.ToString("ddd h:mm tt");
            }
            else
            {
                timeText = DateTime.Now.ToString("ddd HH:mm");
            }
            if (time.Text != timeText)
            {
                time.Text = timeText;
            }
        }

        private void StartClicked(int x, int y)
        {
            StartMenu.CurrentStartMenu.ToggleStartMenu();
        }

        private void TimeClicked(int x, int y)
        {
            miniCalendarOpen = !miniCalendarOpen;
            if (miniCalendarOpen)
            {
                miniCalendar = new Calendar(window, window.Width - 256, window.Height, 256, 256);
                miniCalendar.Background = Color.FromArgb(56, 56, 71);
                miniCalendar.TodayBackground = Color.FromArgb(77, 77, 91);
                miniCalendar.Foreground = Color.White;
                miniCalendar.WeekendForeground = Color.LightPink;
                wm.AddWindow(miniCalendar);
                wm.Update(miniCalendar);
            }
            else
            {
                wm.RemoveWindow(miniCalendar);
            }
        }

        #region Process
        public override void Start()
        {
            base.Start();
            window = new Window(this, 0, -24, (int)wm.ScreenWidth, 24);
            window.Clear(Color.Black);
            wm.AddWindow(window);

            time = new TextBlock(window, window.Width - 136, 0, 128, window.Height);
            time.Background = Color.Black;
            time.Foreground = Color.White;
            time.HorizontalAlignment = Alignment.End;
            time.VerticalAlignment = Alignment.Middle;
            time.OnClick = TimeClicked;
            wm.AddWindow(time);

            start = new ImageBlock(window, (int)((window.Width / 2) - startBitmap.Width / 2), 0, 24, 24);
            start.Image = startBitmap;
            start.OnClick = StartClicked;
            wm.AddWindow(start);

            SetLeftHandStartButton(SettingsManager.GUI_LeftHandStartButton);

            UpdateTime();

            MovementAnimation animation = new MovementAnimation(window)
            {
                From = new Rectangle(window.X, window.Y, window.Width, window.Height),
                To = new Rectangle(window.X, 0, window.Width, window.Height),
                Duration = 10
            };
            animation.Start();

            wm.Update(window);
        }

        public override void Run()
        {
            timeUpdateTicks++;
            if (timeUpdateTicks % 100 == 0)
            {
                UpdateTime();
            }
        }
        #endregion
    }
}
