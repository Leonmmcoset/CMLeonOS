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
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class MessageBox
    {
        internal MessageBox(Process process, string title, string message)
        {
            this.Process = process;
            Title = title;
            Message = message;
        }

        protected const int Padding = 12;

        internal void Show()
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            int longestLineLength = 0;
            foreach (string line in Message.Split('\n'))
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }

            int width = Math.Max(192, (Padding * 2) + (8 * longestLineLength));
            int height = 128 + ((Message.Split('\n').Length - 1) * 16);

            AppWindow window = new AppWindow(Process, (int)((wm.ScreenWidth / 2) - (height / 2)), (int)((wm.ScreenWidth / 2) - (width / 2)), width, height);
            window.Title = Title;
            wm.AddWindow(window);

            window.Clear(Color.LightGray);
            window.DrawFilledRectangle(0, window.Height - (Padding * 2) - 20, window.Width, (Padding * 2) + 20, Color.Gray);
            window.DrawString(Message, Color.Black, Padding, Padding);

            Button ok = new Button(window, window.Width - 80 - Padding, window.Height - 20 - Padding, 80, 20);
            ok.Text = "OK";
            ok.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);
            };
            wm.AddWindow(ok);

            wm.Update(window);

            ProcessManager.GetProcess<Sound.SoundService>().PlaySystemSound(Sound.SystemSound.Alert);
        }

        internal Process Process { get; private set; }

        internal string Title { get; private set; }

        internal string Message { get; private set; }
    }
}
