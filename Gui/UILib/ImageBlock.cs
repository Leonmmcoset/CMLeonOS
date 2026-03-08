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
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class ImageBlock : Control
    {
        public ImageBlock(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        private Bitmap _image;
        internal Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                Render();
            }
        }

        private bool _alpha = false;
        internal bool Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
                Render();
            }
        }

        internal override void Render()
        {
            if (_image == null)
            {
                Clear(Color.Gray);
                WM.Update(this);
                return;
            }

            if (_alpha)
            {
                DrawImageAlpha(_image, 0, 0);
            }
            else
            {
                DrawImage(_image, 0, 0);
            }

            WM.Update(this);
        }
    }
}
