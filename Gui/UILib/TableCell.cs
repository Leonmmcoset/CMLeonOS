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
    internal class TableCell
    {
        internal TableCell(string text)
        {
            Text = text;
        }

        internal TableCell(string text, object tag)
        {
            Text = text;
            Tag = tag;
        }

        internal TableCell(Bitmap image, string text)
        {
            Image = image;
            Text = text;
        }

        internal TableCell(Bitmap image, string text, object tag)
        {
            Image = image;
            Text = text;
            Tag = tag;
        }

        internal Bitmap Image { get; set; }

        internal string Text { get; set; } = string.Empty;

        internal object Tag { get; set; }

        internal Color? BackgroundColourOverride { get; set; } = null;

        internal Color? ForegroundColourOverride { get; set; } = null;
    }
}
