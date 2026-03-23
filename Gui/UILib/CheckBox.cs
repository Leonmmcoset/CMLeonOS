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
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class CheckBox : Control
    {
        public CheckBox(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnClick = CheckBoxClicked;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.Check.bmp")]
        private static byte[] checkBytes;
        private static Bitmap checkBitmap = new Bitmap(checkBytes);

        internal Action CheckBoxChecked;
        internal Action CheckBoxUnchecked;
        internal Action<bool> CheckBoxChanged;

        private const int iconSize = 16;

        private bool _checked = false;
        internal bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                if (_checked)
                {
                    CheckBoxChecked?.Invoke();
                }
                else
                {
                    CheckBoxUnchecked?.Invoke();
                }
                CheckBoxChanged?.Invoke(_checked);
                Render();
            }
        }

        private string _text = "CheckBox";
        internal string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Render();
            }
        }

        private Color _background = UITheme.Surface;
        internal Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                Render();
            }
        }

        private Color _foreground = UITheme.TextPrimary;
        internal Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                Render();
            }
        }

        private void CheckBoxClicked(int x, int y)
        {
            Checked = !Checked;
        }

        internal override void Render()
        {
            Clear(Background);

            int iconX = 0;
            int iconY = (Height / 2) - (iconSize / 2);

            int textX = iconSize + 8;
            int textY = (Height / 2) - (16 / 2);

            DrawFilledRectangle(iconX, iconY, iconSize, iconSize, _checked ? UITheme.AccentLight : UITheme.SurfaceMuted);
            DrawRectangle(iconX, iconY, iconSize, iconSize, _checked ? UITheme.Accent : UITheme.SurfaceBorder);
            if (_checked)
            {
                DrawImageAlpha(checkBitmap, iconX, iconY);
            }

            DrawString(Text, _foreground, textX, textY);

            WM.Update(this);
        }
    }
}
