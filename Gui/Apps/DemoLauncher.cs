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
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class DemoLauncher : Process
    {
        internal DemoLauncher() : base("DemoLauncher", ProcessType.Application) { }

        AppWindow window;

        Table table;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private static class Icons
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.AppIcons.Demos.Starfield.bmp")]
            private static byte[] _iconBytes_Starfield;
            internal static Bitmap Icon_Starfield = new Bitmap(_iconBytes_Starfield);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.AppIcons.Demos.Mandelbrot.bmp")]
            private static byte[] _iconBytes_Mandelbrot;
            internal static Bitmap Icon_Mandelbrot = new Bitmap(_iconBytes_Mandelbrot);
        }

        List<AppMetadata> demoApps = new()
        {
            new AppMetadata("Starfield", () => { return new Apps.Demos.Starfield(); }, Icons.Icon_Starfield, Color.Black ),
            new AppMetadata("Mandelbrot", () => { return new Apps.Demos.Mandelbrot(); }, Icons.Icon_Mandelbrot, Color.Black ),
        };

        private const string message = "Demo Launcher";

        private void PopulateTable()
        {
            table.Cells.Clear();
            foreach (AppMetadata app in demoApps)
            {
                table.Cells.Add(new TableCell(app.Icon.Resize(32, 32), app.Name));
            }
            table.Render();
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 256, 256, 384, 256);
            wm.AddWindow(window);
            window.Title = "Demos";
            window.Icon = AppManager.GetAppMetadata("Demos").Icon;
            window.Closing = TryStop;

            window.Clear(Color.FromArgb(56, 56, 71));

            window.DrawString(message, Color.White, (window.Width / 2) - ((FontData.Width * message.Length) / 2), 12);

            table = new Table(window, 12, 40, window.Width - 24, window.Height - 52);
            table.Background = Color.FromArgb(56, 56, 71);
            table.Border = Color.FromArgb(56, 56, 71);
            table.Foreground = Color.White;
            table.CellHeight = 32;
            table.OnDoubleClick = (int x, int y) =>
            {
                if (table.SelectedCellIndex != -1)
                {
                    AppMetadata app = demoApps[table.SelectedCellIndex];
                    ProcessManager.AddProcess(app.CreateProcess()).Start();
                }
            };
            PopulateTable();
            wm.AddWindow(table);

            wm.Update(window);
        }

        public override void Run()
        {

        }
    }
}
