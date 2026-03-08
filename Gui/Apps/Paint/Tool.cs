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

namespace CMLeonOS.Gui.Apps.Paint
{
    internal abstract class Tool
    {
        internal Tool(string name)
        {
            Name = name;
        }

        internal abstract void Run(Paint paint, Window canvas, MouseState mouseState, int mouseX, int mouseY);

        internal virtual void Selected()
        {
        }

        internal virtual void Deselected()
        {
        }

        internal string Name { get; init; }
    }
}
