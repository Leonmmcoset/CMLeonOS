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

namespace CMLeonOS.Gui.ShellComponents.Dock
{
    internal class StartMenuDockIcon : BaseDockIcon
    {
        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Dock.StartMenu.bmp")]
        private static byte[] _iconBytes_StartMenu;
        internal static Bitmap Icon_StartMenu = new Bitmap(_iconBytes_StartMenu);

        internal StartMenuDockIcon() : base(
            image: Icon_StartMenu,
            doAnimation: false)
        {
        }

        internal override void Clicked()
        {
            StartMenu.CurrentStartMenu.ToggleStartMenu();
        }
    }
}
