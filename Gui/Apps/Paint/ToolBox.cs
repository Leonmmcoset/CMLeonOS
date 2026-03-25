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

using CMLeonOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps.Paint
{
    internal class ToolBox : Window
    {
        private Paint paintInstance;

        private Table table;

        internal Tool SelectedTool;

        internal readonly List<Tool> Tools = new List<Tool>()
        {
            new Tools.Pencil(),
            new Tools.CircleBrush(),
            new Tools.Eraser(),
            new Tools.LineTool(),
            new Tools.RectangleTool(),
            new Tools.FilledRectangleTool(),
            new Tools.FilledCircleTool()
        };

        private void TableClicked(int x, int y)
        {
            Tool tool = table.Cells[table.SelectedCellIndex].Tag as Tool;

            if (tool != SelectedTool)
            {
                SelectedTool.Deselected();
                SelectedTool = tool;
            }
        }

        internal ToolBox(Paint paint, int x, int y, int width, int height) : base(paint, x, y, width, height)
        {
            paintInstance = paint;


            Clear(Color.FromArgb(107, 107, 107));
            DrawString("Toolbox", Color.White, 8, 8);

            table = new Table(this, 0, 32, Width, Height - 32);
            table.AllowDeselection = false;
            table.CellHeight = 24;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Tool tool in Tools)
            {
                table.Cells.Add(new TableCell(tool.Name, tool));
            }

            SelectedTool = Tools[0];
            table.SelectedCellIndex = 0;

            table.Render();

            WM.AddWindow(this);
            WM.AddWindow(table);
        }
    }
}
