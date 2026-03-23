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
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Switch : CheckBox
    {
        public Switch(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = SwitchDown;
            OnClick = null;
        }

        private const int maximumToggleDrag = 4;
        private const int switchWidth = 34;
        private const int switchHeight = 18;
        private const int knobSize = 14;

        private int lastMouseX = 0;
        private int totalDragged = 0;
        private bool held = false;

        private void SwitchDown(int x, int y)
        {
            lastMouseX = (int)MouseManager.X;
            totalDragged = 0;
            held = true;
            Render();
        }

        private void Release()
        {
            held = false;
            if (totalDragged <= maximumToggleDrag)
            {
                // Interpret as a toggle.
                Checked = !Checked;
            }
            else
            {
                // Interpret as a drag rather than a toggle,
                // setting the Checked state based on where
                // the switch knob is.
                Checked = knobX >= (switchWidth - knobSize) / 2d;
            }
        }

        private double knobX = -1;
        private double knobGoal = 0;

        internal override void Render()
        {
            knobGoal = Checked ? switchWidth - knobSize : 0;

            if (held && MouseManager.MouseState != MouseState.Left)
            {
                Release();
            }

            if (held)
            {
                int diff = (int)(MouseManager.X - lastMouseX);
                lastMouseX = (int)MouseManager.X;
                totalDragged += Math.Abs(diff);
                knobX = Math.Clamp(knobX + diff, 0, switchWidth - knobSize);

                WM.UpdateQueue.Enqueue(this);
            }
            else
            {
                double oldKnobX = knobX;
                if (knobX == -1)
                {
                    knobX = knobGoal;
                }
                else
                {
                    double diff = knobGoal - knobX;
                    double move = diff / 8d;
                    knobX += move;
                }
                if (Math.Abs(knobX - oldKnobX) < 0.25)
                {
                    knobX = knobGoal;
                }
                else
                {
                    WM.UpdateQueue.Enqueue(this);
                }
            }

            Clear(Background);

            int switchX = 0;
            int switchY = (Height / 2) - (switchHeight / 2);

            int textX = switchWidth + 8;
            int textY = (Height / 2) - (16 / 2);

            DrawFilledRectangle(switchX, switchY, switchWidth, switchHeight, Checked ? UITheme.Accent : UITheme.SurfaceMuted);
            DrawRectangle(switchX, switchY, switchWidth, switchHeight, Checked ? UITheme.AccentDark : UITheme.SurfaceBorder);

            int knobY = switchY + ((switchHeight - knobSize) / 2);
            DrawFilledRectangle((int)knobX + 1, knobY, knobSize, knobSize, Color.White);
            DrawRectangle((int)knobX + 1, knobY, knobSize, knobSize, UITheme.SurfaceBorder);

            DrawString(Text, Foreground, textX, textY);

            WM.Update(this);
        }
    }
}
