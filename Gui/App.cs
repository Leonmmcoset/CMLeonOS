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
using CMLeonOS;
using System;
using System.Drawing;

namespace CMLeonOS.Gui
{
    internal class AppMetadata
    {
        public AppMetadata(string name, Func<Process> createProcess, Bitmap icon, Color themeColor)
        {
            Name = name;
            CreateProcess = createProcess;
            Icon = icon;
            ThemeColor = themeColor;
        }

        internal void Start(Process parent)
        {
            ProcessManager.AddProcess(parent, CreateProcess()).Start();
        }

        internal void Start()
        {
            ProcessManager.AddProcess(CreateProcess()).Start();
        }

        internal string Name { get; private set; }

        internal Func<Process> CreateProcess { get; private set; }

        internal Bitmap Icon { get; private set; }

        internal Color ThemeColor { get; private set; }
    }
}
