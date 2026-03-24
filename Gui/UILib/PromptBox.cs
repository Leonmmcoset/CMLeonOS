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
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class PromptBox : MessageBox
    {
        internal PromptBox(Process process, string title, string message, string placeholder, Action<string> submitted) : base(process, title, message)
        {
            Placeholder = placeholder;
            Submitted = submitted;
        }

        internal void Show()
        {
            WindowManager wm = ProcessManager.GetProcess<WindowManager>();

            string[] lines = Message.Split('\n');
            int longestLineLength = 0;
            foreach (string line in lines)
            {
                longestLineLength = Math.Max(longestLineLength, line.Length);
            }

            int width = Math.Max(256, (Padding * 2) + (8 * longestLineLength));
            int textHeight = lines.Length * 16;
            int height = Math.Max(128, Padding + textHeight + 28 + 20 + Padding * 2);

            AppWindow window = new AppWindow(Process, (int)((wm.ScreenWidth / 2) - (width / 2)), (int)((wm.ScreenHeight / 2) - (height / 2)), width, height);
            window.Title = Title;
            wm.AddWindow(window);

            window.Clear(UITheme.Surface);
            window.DrawRectangle(0, 0, window.Width, window.Height, UITheme.SurfaceBorder);
            window.DrawFilledRectangle(0, window.Height - (Padding * 2) - 20, window.Width, (Padding * 2) + 20, UITheme.SurfaceMuted);

            int textY = Padding;
            foreach (string line in lines)
            {
                window.DrawString(line, UITheme.TextPrimary, Padding, textY);
                textY += 16;
            }

            TextBox textBox = new TextBox(window, Padding, textY + 8, 192, 20);
            textBox.PlaceholderText = Placeholder;
            wm.AddWindow(textBox);

            Button ok = new Button(window, window.Width - 80 - Padding, window.Height - 20 - Padding, 80, 20);
            ok.Text = "OK";
            ok.Background = UITheme.Accent;
            ok.Border = UITheme.AccentDark;
            ok.OnClick = (int x, int y) =>
            {
                wm.RemoveWindow(window);

                Submitted.Invoke(textBox.Text);
            };
            wm.AddWindow(ok);

            wm.Update(window);
            ok.Render();

            ProcessManager.GetProcess<Sound.SoundService>().PlaySystemSound(Sound.SystemSound.Alert);
        }

        internal Action<string> Submitted { get; private set; }

        internal string Placeholder { get; private set; }
    }
}
