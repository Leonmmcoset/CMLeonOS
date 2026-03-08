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
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.ShellComponents.Dock
{
    internal class Dock : Process
    {
        internal Dock() : base("Dock", ProcessType.Application)
        {
        }

        internal static Dock CurrentDock
        {
            get
            {
                Dock dock = ProcessManager.GetProcess<Dock>();
                return dock;
            }
        }

        Window window;

        List<BaseDockIcon> Icons = new List<BaseDockIcon>();

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();

        internal static readonly int IconSize = 64;
        internal static readonly int IconImageMaxSize = 48;

        private void Render()
        {
            int newDockWidth = 0;
            foreach (var icon in Icons)
            {
                newDockWidth += (int)icon.Size;
            }

            if (newDockWidth != window.Width)
            {
                window.MoveAndResize((int)(wm.ScreenWidth / 2 - newDockWidth / 2), window.Y, newDockWidth, window.Height);
            }

            window.Clear(Color.FromArgb(130, 202, 255));

            int x = 0;
            foreach (var icon in Icons)
            {
                if (icon.Image != null)
                {
                    Bitmap resizedImage = icon.Image.ResizeWidthKeepRatio((uint)Math.Min(IconImageMaxSize, icon.Size));

                    int imageX = (int)(x + ((icon.Size / 2) - (resizedImage.Width / 2)));
                    window.DrawImageAlpha(resizedImage, imageX, (int)((window.Height / 2) - (resizedImage.Height / 2)));
                }

                x += (int)icon.Size;
            }

            wm.Update(window);
        }

        internal int GetDockHeight()
        {
            return window.Height;
        }

        internal void UpdateWindows()
        {
            // Add new windows and update icons.
            foreach (var window in wm.Windows)
            {
                if (window is not AppWindow appWindow)
                {
                    continue;
                }

                bool found = false;

                foreach (BaseDockIcon icon in Icons)
                {
                    if (icon is AppDockIcon appDockIcon && appDockIcon.AppWindow == appWindow)
                    {
                        icon.Image = appWindow.Icon;

                        found = true;
                    }
                }

                if (!found)
                {
                    Icons.Add(new AppDockIcon(appWindow));
                }
            }

            // Remove deleted windows.
            foreach (BaseDockIcon icon in Icons)
            {
                if (icon is not AppDockIcon appDockIcon)
                {
                    continue;
                }

                bool found = false;

                foreach (Window window in wm.Windows)
                {
                    if (window == appDockIcon.AppWindow)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    icon.Closing = true;
                }
            }

            Render();
        }

        private void DockClick(int x, int y)
        {
            int end = 0;
            foreach (var icon in Icons)
            {
                end += (int)icon.Size;

                if (x < end)
                {
                    icon.Clicked();
                    return;
                }
            }
        }

        public override void Start()
        {
            base.Start();

            window = new Window(this, (int)(wm.ScreenWidth / 2), (int)(wm.ScreenHeight + IconSize), IconSize, IconSize);
            window.OnClick = DockClick;
            wm.AddWindow(window);

            Icons.Add(new StartMenuDockIcon());

            Render();

            MovementAnimation animation = new MovementAnimation(window)
            {
                From = new Rectangle(window.X, window.Y, window.Width, window.Height),
                To = new Rectangle(window.X, (int)(wm.ScreenHeight - IconSize), window.Width, window.Height),
                Duration = 10
            };
            animation.Start();
        }

        public override void Run()
        {
            bool rerenderNeeded = false;

            for (int i = Icons.Count - 1; i >= 0; i--)
            {
                BaseDockIcon icon = Icons[i];

                if (icon.RunAnimation())
                {
                    rerenderNeeded = true;
                }

                if (icon.CloseAnimationComplete)
                {
                    Icons.Remove(icon);
                }
            }

            if (rerenderNeeded)
            {
                Render();
            }
        }
    }
}
