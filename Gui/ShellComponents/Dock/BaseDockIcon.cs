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
    internal abstract class BaseDockIcon
    {
        internal BaseDockIcon(Bitmap image, bool doAnimation = true)
        {
            if (doAnimation)
            {
                Size = 1;
            }
            else
            {
                // Skip to the end of the animation.
                Size = Dock.IconSize;
            }

            Image = image;
        }

        private double _size;
        internal double Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;

                if (_image != null)
                {
                    SizedImage = _image.ResizeWidthKeepRatio((uint)Size);
                }
            }
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
                SizedImage = value.ResizeWidthKeepRatio((uint)Size);
            }
        }


        internal Bitmap SizedImage { get; private set; }

        internal bool Closing { get; set; } = false;

        internal bool CloseAnimationComplete
        {
            get
            {
                return Closing && (int)_size == 1;
            }
        }

        /// <summary>
        /// Run the dock icon's animation.
        /// </summary>
        /// <returns>If the dock needs to be rerendered.</returns>
        internal bool RunAnimation()
        {
            int oldSize = (int)Size;
            int goalSize = Closing ? 1 : Dock.IconSize;
            Size += (goalSize - Size) / 16;

            return (int)Size != (int)oldSize;
        }

        internal abstract void Clicked();
    }
}
