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

namespace CMLeonOS.Gui.Apps.Demos
{
    internal class Mandelbrot : Process
    {
        internal Mandelbrot() : base("Mandelbrot", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private Color GetColor(double v)
        {
            int red = Math.Clamp((int)(255 * v), 0, 255);
            int green = 0;
            int blue = Math.Clamp((int)(255 * (1 - v)), 0, 255);

            return Color.FromArgb(red, green, blue);
        }

        private void RenderMandelbrot()
        {
            window.Clear(Color.Black);
            wm.Update(window);

            int width = window.Width;
            int height = window.Height;

            const int max = 20;
            const double bail = 2.0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double zx = 0;
                    double zy = 0;
                    double cx = (x - width / 2.0) / (width / 4.0);
                    double cy = (y - height / 2.0) / (height / 4.0);

                    int iteration = 0;

                    while (zx * zx + zy * zy < bail && iteration < max)
                    {
                        double zxNew = zx * zx - zy * zy + cx;
                        zy = 2 * zx * zy + cy;
                        zx = zxNew;
                        iteration++;
                    }

                    double smooth = iteration + 1 - Math.Log(Math.Log(Math.Sqrt(zx * zx + zy * zy)) / Math.Log(bail)) / Math.Log(2);
                    window.DrawPoint(x, y, GetColor(smooth / max));

                    if (x % 32 == 0)
                    {
                        ProcessManager.Yield();
                    }
                }

                if (y % 8 == 0)
                {
                    wm.Update(window);
                }
            }

            wm.Update(window);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 256);
            wm.AddWindow(window);
            window.Title = "Mandelbrot";
            window.CanResize = true;
            window.Closing = TryStop;
            window.UserResized = RenderMandelbrot;

            RenderMandelbrot();
        }

        public override void Run()
        {

        }
    }
}
