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
    internal class ColourPicker : Window
    {
        private Paint paintInstance;

        private Table table;

        internal readonly List<Color> Colours = new List<Color>()
        {
            Color.Black,
            Color.White,
            Color.Red,
            Color.Blue,
            Color.Orange,
            Color.Green,
            Color.Pink,
            Color.Gray,
            Color.Purple,
            Color.DarkGoldenrod,
            Color.DarkGray,
            Color.DarkGreen,
            Color.DarkCyan,
            Color.Cyan,
            Color.BlueViolet,
            Color.AliceBlue,
            Color.Brown,
            Color.CornflowerBlue,
            Color.Azure,
            Color.Beige,
            Color.DarkBlue,
            Color.DarkSlateBlue,
            Color.SeaGreen
        };

        private void TableClicked(int x, int y)
        {
            // Clear 'Selected' text on previously selected colour.
            foreach (var cell in table.Cells)
            {
                cell.Text = string.Empty;
            }

            var selectedCell = table.Cells[table.SelectedCellIndex];
            Color color = (Color)selectedCell.Tag;

            paintInstance.SelectedColor = color;

            selectedCell.Text = "Selected";

            table.Render();
        }

        internal ColourPicker(Paint paint, int x, int y, int width, int height) : base(paint, x, y, width, height)
        {
            paintInstance = paint;

            Clear(Color.FromArgb(107, 107, 107));
            DrawString("Colours", Color.White, 8, 8);

            table = new Table(this, 0, 32, Width, Height - 32);
            table.AllowDeselection = false;
            table.CellHeight = 20;
            table.TextAlignment = Alignment.Middle;
            table.OnClick = TableClicked;

            foreach (Color colour in Colours)
            {
                TableCell cell = new(string.Empty, tag: colour);
                cell.BackgroundColourOverride = colour;
                cell.ForegroundColourOverride = colour.GetForegroundColour();
                if (colour == paint.SelectedColor)
                {
                    cell.Text = "Selected";
                }
                table.Cells.Add(cell);
            }

            table.Render();

            WM.AddWindow(this);
            WM.AddWindow(table);
        }
    }
}
