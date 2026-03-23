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
using Cosmos.System;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Button : Control
    {
        public Button(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
            OnDown = (_, _) =>
            {
                held = true;
                Render();
            };
        }

        internal enum ButtonImageLocation
        {
            AboveText,
            Left
        }

        private string _text = "Button";
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

        private ButtonImageLocation _imageLocation = ButtonImageLocation.AboveText;
        internal ButtonImageLocation ImageLocation
        {
            get
            {
                return _imageLocation;
            }
            set
            {
                _imageLocation = value;
                Render();
            }
        }

        private Color _background = UITheme.Accent;
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

        private Color _foreground = Color.White;
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

        private Color _border = UITheme.AccentDark;
        internal Color Border
        {
            get
            {
                return _border;
            }
            set
            {
                _border = value;
                Render();
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
                Render();
            }
        }

        private bool held = false;

        internal override void Render()
        {
            if (held && MouseManager.MouseState != MouseState.Left)
            {
                held = false;
            }

            Color buttonBackground = held
                ? Color.FromArgb(Math.Max(0, Background.R - 24), Math.Max(0, Background.G - 24), Math.Max(0, Background.B - 24))
                : Background;

            Clear(UITheme.Surface);
            DrawFilledRectangle(0, 0, Width, Height, buttonBackground);
            DrawHorizontalLine(Width - 2, 1, 1, Color.FromArgb(
                Math.Min(255, buttonBackground.R + 20),
                Math.Min(255, buttonBackground.G + 20),
                Math.Min(255, buttonBackground.B + 20)));

            if (_image != null)
            {
                switch (_imageLocation)
                {
                    case ButtonImageLocation.Left:
                    {
                        int imageWidth = (int)_image.Width;
                        int imageHeight = (int)_image.Height;
                        int textWidth = 8 * Text.Length;
                        int contentWidth = imageWidth + 6 + textWidth;
                        int imageX = Math.Max(4, (Width / 2) - (contentWidth / 2));
                        int imageY = (Height / 2) - (imageHeight / 2);
                        int textXLeft = imageX + imageWidth + 6;
                        int textYLeft = (Height / 2) - (16 / 2);

                        DrawImageAlpha(_image, imageX, imageY);
                        DrawString(Text, Foreground, textXLeft, textYLeft);
                        break;
                    }
                    case ButtonImageLocation.AboveText:
                    {
                        DrawImageAlpha(_image, (int)((Width / 2) - (_image.Width / 2)), Math.Max(1, (int)((Height / 2) - (_image.Height / 2) - 4)));
                        DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), Height - 17);
                        break;
                    }
                    default:
                        throw new Exception("Unrecognised image location in button.");
                }
            }
            else
            {
                DrawString(Text, Foreground, (Width / 2) - (4 * Text.Length), (Height / 2) - 8 + (held ? 1 : 0));
            }

            DrawRectangle(0, 0, Width, Height, Border);

            WM.Update(this);
        }
    }
}
