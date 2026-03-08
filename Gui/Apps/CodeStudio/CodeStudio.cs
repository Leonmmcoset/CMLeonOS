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
using System.Drawing;
using Cosmos.System.Graphics;

namespace CMLeonOS.Gui.Apps.CodeStudio
{
    internal class CodeStudio : Process
    {
        internal CodeStudio() : base("CodeStudio", ProcessType.Application) { }

        Window splash;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.CodeStudio.Splash.bmp")]
        private static byte[] _splashBytes;
        private static Bitmap splashBitmap = new Bitmap(_splashBytes);

        private Ide ide;

        private bool ideCreated = false;

        public override void Start()
        {
            base.Start();
            splash = new Window(this, 372, 250, 535, 300);
            wm.AddWindow(splash);

            splash.DrawImage(splashBitmap, 0, 0);
            //splash.DrawString("Starting...", Color.White, 20, splash.Height - 16 - 20);

            wm.Update(splash);
        }

        public override void Run()
        {
            if (!ideCreated)
            {
                ide = new Ide(this, wm);
                ide.Start();
                wm.RemoveWindow(splash);
                ideCreated = true;
            }
        }
    }
}
