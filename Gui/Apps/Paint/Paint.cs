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

using Cosmos.System;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Paint
{
    internal class Paint : Process
    {
        internal Paint() : base("Paint", ProcessType.Application)
        {
        }

        AppWindow window;

        Window canvas;

        ToolBox toolBox;

        ColourPicker colourPicker;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private bool down = false;

        internal Color SelectedColor { get; set; } = Color.Black;

        internal bool IsInBounds(int x, int y)
        {
            if (x >= canvas.Width || y >= canvas.Height) return false;
            if (x < 0 || y < 0) return false;

            return true;
        }

        private void CanvasDown(int x, int y)
        {
            down = true;
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 768, 448);
            window.Title = "Paint";
            window.Icon = AppManager.GetAppMetadata("Paint").Icon;
            window.Closing = TryStop;
            window.Clear(Color.FromArgb(73, 73, 73));
            wm.AddWindow(window);

            int canvasWidth = 384;
            int canvasHeight = 256;
            canvas = new Window(this, (window.Width / 2) - (canvasWidth / 2), (window.Height / 2) - (canvasHeight / 2), canvasWidth, canvasHeight);
            canvas.RelativeTo = window;
            canvas.OnDown = CanvasDown;
            canvas.Clear(Color.White);
            wm.AddWindow(canvas);

            toolBox = new ToolBox(this, 0, 0, 128, window.Height);
            toolBox.RelativeTo = window;
            colourPicker = new ColourPicker(this, window.Width - 128, 0, 128, window.Height);
            colourPicker.RelativeTo = window;

            wm.Update(window);
        }

        public override void Run()
        {
            if (down)
            {
                if (MouseManager.MouseState == MouseState.None)
                {
                    down = false;
                }

                toolBox.SelectedTool.Run(
                   this,
                   canvas,
                   MouseManager.MouseState,
                   (int)(MouseManager.X - canvas.ScreenX),
                   (int)(MouseManager.Y - canvas.ScreenY)
                );

                wm.Update(canvas);
            }
        }
    }
}
