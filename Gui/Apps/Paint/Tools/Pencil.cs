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

namespace CMLeonOS.Gui.Apps.Paint.Tools
{
    internal class Pencil : Tool
    {
        public Pencil() : base("Pencil")
        {
        }

        private bool joinLine;
        private int joinX;
        private int joinY;

        internal override void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY)
        {
            if (mouseState == MouseState.Left)
            {
                if (joinLine)
                {
                    canvas.DrawLine(joinX, joinY, mouseX, mouseY, paint.SelectedColor);
                }
                else
                {
                    canvas.DrawPoint(mouseX, mouseY, paint.SelectedColor);
                }
                joinLine = true;
                joinX = mouseX;
                joinY = mouseY;
            }
            else
            {
                joinLine = false;
            }
        }

        internal override void Deselected()
        {
            joinLine = false;
        }
    }
}
