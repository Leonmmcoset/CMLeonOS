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
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Demos
{
    internal class Starfield : Process
    {
        internal Starfield() : base("Starfield", ProcessType.Application) { }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private readonly Random random = new Random();

        private readonly List<Star> stars = new List<Star>();

        private int timerId;

        private class Star
        {
            internal double X { get; set; }
            internal double Y { get; set; }
            internal double Z { get; set; }
            internal double Velocity { get; set; }

            internal Star(double x, double y, double z, double velocity)
            {
                X = x;
                Y = y;
                Z = z;
                Velocity = velocity;
            }
        }

        internal (double, double) TransformCoordinates(double x, double y, double z, double fov)
        {
            double screenX = x / (z * Math.Tan(fov / 2)) + 0.5;
            double screenY = y / (z * Math.Tan(fov / 2)) + 0.5;

            return (screenX, screenY);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 256, 256);
            wm.AddWindow(window);
            window.Title = "Starfield";
            window.CanResize = true;
            window.Closing = TryStop;

            for (int i = 0; i < 100; i++)
            {
                stars.Add(new Star(
                    x: random.NextDouble() * 2 - 1.5,
                    y: random.NextDouble() * 2 - 1.5,
                    z: 3,
                    velocity: random.NextDouble() * 0.05 + 0.05));
            }

            timerId = Cosmos.HAL.Global.PIT.RegisterTimer(new Cosmos.HAL.PIT.PITTimer(() =>
            {
                foreach (var star in stars)
                {
                    star.Z -= star.Velocity;

                    if (star.Z < 0)
                    {
                        star.X = random.NextDouble() * 2 - 1.5;
                        star.Y = random.NextDouble() * 2 - 1.5;
                        star.Z = 3;
                    }
                }
            }, (ulong)((1000 /* ms */ / 30) * 1e+6 /* ms -> ns */ ), true));
        }

        public override void Run()
        {
            window.Clear(Color.Black);

            foreach (var star in stars)
            {
                (double X, double Y) pos = TransformCoordinates(star.X, star.Y, star.Z, Math.PI / 2);

                int screenX = (int)((pos.X + 1) * (window.Width / 2));
                int screenY = (int)((pos.Y + 1) * (window.Height / 2));

                window.DrawPoint(screenX, screenY, Color.White);
            }

            wm.Update(window);
        }

        public override void Stop()
        {
            base.Stop();

            Cosmos.HAL.Global.PIT.UnregisterTimer(timerId);
        }
    }
}
