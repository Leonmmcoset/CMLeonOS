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
using Cosmos.System.Graphics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMLeonOS.Gui.Apps.CodeStudio
{
    internal class CodeEditor : TextBox
    {
        private bool enableSyntaxHighlighting = false;
        private string fileExtension = "";

        public CodeEditor(Window parent, int x, int y, int width, int height) : base(parent, x, y, width, height)
        {
        }

        internal void SetSyntaxHighlighting(bool enable, string extension = "")
        {
            enableSyntaxHighlighting = enable;
            fileExtension = extension.ToLower();
            MarkAllLines();
            Render();
        }

        internal override void RenderLine(int lineIndex, string lineText, int lineY, int xOffset)
        {
            if (enableSyntaxHighlighting && fileExtension == ".lua")
            {
                var tokens = LuaSyntaxHighlighter.HighlightLine(lineText);
                int currentXOffset = xOffset;

                foreach (var token in tokens)
                {
                    if (currentXOffset + token.Text.Length * 8 > 0 && currentXOffset < Width)
                    {
                        DrawString(token.Text, token.Color, currentXOffset, lineY);
                    }
                    currentXOffset += token.Text.Length * 8;
                }
            }
            else
            {
                base.RenderLine(lineIndex, lineText, lineY, xOffset);
            }
        }
    }
}
