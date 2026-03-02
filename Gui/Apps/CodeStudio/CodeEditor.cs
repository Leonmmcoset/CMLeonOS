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
