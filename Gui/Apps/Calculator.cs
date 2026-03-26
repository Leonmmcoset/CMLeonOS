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

using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class Calculator : Process
    {
        internal Calculator() : base("Calculator", ProcessType.Application) { }

        private enum Operator
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }

        private AppWindow window;
        private TextBlock expressionBlock;
        private TextBlock displayBlock;
        private StatusBar statusBar;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private const int padding = 12;
        private const int buttonWidth = 68;
        private const int buttonHeight = 34;
        private const int buttonGap = 8;
        private const int displayHeight = 72;
        private const int statusHeight = 24;

        private long num1 = 0;
        private long num2 = 0;
        private Operator op = Operator.None;
        private bool overwriteNextDigit = false;

        private string OperatorText
        {
            get
            {
                return op switch
                {
                    Operator.Add => "+",
                    Operator.Subtract => "-",
                    Operator.Multiply => "*",
                    Operator.Divide => "/",
                    _ => string.Empty
                };
            }
        }

        private void UpdateDisplay()
        {
            if (op == Operator.None)
            {
                expressionBlock.Text = "Current Value";
                displayBlock.Text = num1.ToString();
            }
            else
            {
                expressionBlock.Text = $"{num1} {OperatorText}";
                displayBlock.Text = num2.ToString();
            }

            string state = op == Operator.None ? "Ready" : $"Operator: {OperatorText}";
            statusBar.Text = state;
            statusBar.DetailText = "Integer Mode";
        }

        private void ShowError(string text)
        {
            new MessageBox(this, "Calculator", text).Show();
        }

        private void InputDigit(int digit)
        {
            if (op == Operator.None)
            {
                if (overwriteNextDigit)
                {
                    num1 = digit;
                    overwriteNextDigit = false;
                }
                else
                {
                    num1 = (num1 * 10) + digit;
                }
            }
            else
            {
                if (overwriteNextDigit)
                {
                    num2 = digit;
                    overwriteNextDigit = false;
                }
                else
                {
                    num2 = (num2 * 10) + digit;
                }
            }

            UpdateDisplay();
        }

        private void InputOperator(Operator nextOperator)
        {
            if (op != Operator.None && !overwriteNextDigit)
            {
                InputEquals();
            }

            op = nextOperator;
            num2 = 0;
            overwriteNextDigit = false;
            UpdateDisplay();
        }

        private void InputBackspace()
        {
            if (op == Operator.None)
            {
                num1 /= 10;
            }
            else
            {
                num2 /= 10;
            }

            UpdateDisplay();
        }

        private void InputClear()
        {
            num1 = 0;
            num2 = 0;
            op = Operator.None;
            overwriteNextDigit = false;
            UpdateDisplay();
        }

        private void InputClearEntry()
        {
            if (op == Operator.None)
            {
                num1 = 0;
            }
            else
            {
                num2 = 0;
            }

            overwriteNextDigit = false;
            UpdateDisplay();
        }

        private void InputEquals()
        {
            if (op == Operator.None)
            {
                return;
            }

            long result = num1;
            switch (op)
            {
                case Operator.Add:
                    result = num1 + num2;
                    break;
                case Operator.Subtract:
                    result = num1 - num2;
                    break;
                case Operator.Multiply:
                    result = num1 * num2;
                    break;
                case Operator.Divide:
                    if (num2 == 0)
                    {
                        ShowError("Cannot divide by zero.");
                        return;
                    }
                    result = num1 / num2;
                    break;
            }

            num1 = result;
            num2 = 0;
            op = Operator.None;
            overwriteNextDigit = true;
            UpdateDisplay();
        }

        private Button CreateButton(string text, int col, int row, Action onClick, bool accent = false, bool neutral = false)
        {
            int x = padding + (col * (buttonWidth + buttonGap));
            int y = padding + displayHeight + padding + (row * (buttonHeight + buttonGap));

            Button button = new Button(window, x, y, buttonWidth, buttonHeight);
            button.Text = text;
            if (accent)
            {
                button.Background = UITheme.Accent;
                button.Border = UITheme.AccentDark;
                button.Foreground = Color.White;
            }
            else if (neutral)
            {
                button.Background = UITheme.SurfaceMuted;
                button.Border = UITheme.SurfaceBorder;
                button.Foreground = UITheme.TextPrimary;
            }
            button.OnClick = (_, _) => onClick();
            wm.AddWindow(button);
            return button;
        }

        public override void Start()
        {
            base.Start();

            int width = padding * 2 + (buttonWidth * 4) + (buttonGap * 3);
            int buttonRows = 5;
            int height = padding + displayHeight + padding + (buttonRows * buttonHeight) + ((buttonRows - 1) * buttonGap) + padding + statusHeight;

            window = new AppWindow(this, 260, 170, width, height);
            window.Title = "Calculator";
            window.Icon = AppManager.GetAppMetadata("Calculator").Icon;
            window.Closing = TryStop;
            wm.AddWindow(window);

            expressionBlock = new TextBlock(window, padding, padding, width - (padding * 2), 18);
            expressionBlock.Background = UITheme.Surface;
            expressionBlock.Foreground = UITheme.TextSecondary;
            expressionBlock.HorizontalAlignment = Alignment.End;
            wm.AddWindow(expressionBlock);

            displayBlock = new TextBlock(window, padding, padding + 22, width - (padding * 2), 42);
            displayBlock.Background = UITheme.Surface;
            displayBlock.Foreground = UITheme.TextPrimary;
            displayBlock.HorizontalAlignment = Alignment.End;
            displayBlock.VerticalAlignment = Alignment.Middle;
            wm.AddWindow(displayBlock);

            Separator separator = new Separator(window, padding, padding + displayHeight, width - (padding * 2), 8);
            separator.Background = UITheme.Surface;
            separator.Foreground = UITheme.SurfaceBorder;
            wm.AddWindow(separator);

            CreateButton("C", 0, 0, InputClear, neutral: true);
            CreateButton("Bk", 1, 0, InputBackspace, neutral: true);
            CreateButton("/", 2, 0, () => InputOperator(Operator.Divide), neutral: true);
            CreateButton("*", 3, 0, () => InputOperator(Operator.Multiply), neutral: true);

            CreateButton("7", 0, 1, () => InputDigit(7));
            CreateButton("8", 1, 1, () => InputDigit(8));
            CreateButton("9", 2, 1, () => InputDigit(9));
            CreateButton("-", 3, 1, () => InputOperator(Operator.Subtract), neutral: true);

            CreateButton("4", 0, 2, () => InputDigit(4));
            CreateButton("5", 1, 2, () => InputDigit(5));
            CreateButton("6", 2, 2, () => InputDigit(6));
            CreateButton("+", 3, 2, () => InputOperator(Operator.Add), neutral: true);

            CreateButton("1", 0, 3, () => InputDigit(1));
            CreateButton("2", 1, 3, () => InputDigit(2));
            CreateButton("3", 2, 3, () => InputDigit(3));
            CreateButton("=", 3, 3, InputEquals, accent: true);

            CreateButton("0", 0, 4, () => InputDigit(0));
            CreateButton("00", 1, 4, () =>
            {
                InputDigit(0);
                InputDigit(0);
            });
            CreateButton("CE", 2, 4, InputClearEntry, neutral: true);
            CreateButton("=", 3, 4, InputEquals, accent: true);

            statusBar = new StatusBar(window, 0, height - statusHeight, width, statusHeight);
            wm.AddWindow(statusBar);

            InputClear();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
