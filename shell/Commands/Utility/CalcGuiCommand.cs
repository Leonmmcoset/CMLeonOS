using System;
using System.Collections.Generic;
using CMLeonOS.UI;

namespace CMLeonOS.Commands.Utility
{
    public static class CalcGUICommand
    {
        private static string currentInput = "0";
        private static string previousInput = "";
        private static string currentOperation = "";
        private static bool newInput = true;
        private static int selectedButton = 0;

        private static readonly List<string> buttons = new List<string>
        {
            "7", "8", "9", "/",
            "4", "5", "6", "*",
            "1", "2", "3", "-",
            "C", "0", "=", "+"
        };

        public static void ShowCalculator()
        {
            Console.Clear();
            
            var window = new Window(
                new Rect(5, 2, 40, 20),
                "Calculator",
                () => { },
                true
            );
            window.Render();

            bool running = true;
            while (running)
            {
                RenderCalculator();
                
                var key = Console.ReadKey(true);
                
                switch (key.Key)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        PressButton("0");
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        PressButton("1");
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        PressButton("2");
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        PressButton("3");
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        PressButton("4");
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        PressButton("5");
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        PressButton("6");
                        break;
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        PressButton("7");
                        break;
                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        PressButton("8");
                        break;
                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                        PressButton("9");
                        break;
                    case ConsoleKey.Add:
                        PressButton("+");
                        break;
                    case ConsoleKey.Subtract:
                        PressButton("-");
                        break;
                    case ConsoleKey.Multiply:
                        PressButton("*");
                        break;
                    case ConsoleKey.Divide:
                        PressButton("/");
                        break;
                    case ConsoleKey.Enter:
                        PressButton("=");
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        running = false;
                        break;
                    case ConsoleKey.Backspace:
                        if (currentInput.Length > 1)
                        {
                            currentInput = currentInput.Substring(0, currentInput.Length - 1);
                        }
                        else if (currentInput.Length == 1)
                        {
                            currentInput = "0";
                        }
                        else
                        {
                            currentInput = "0";
                        }
                        break;
                    case ConsoleKey.C:
                        PressButton("C");
                        break;
                    case ConsoleKey.UpArrow:
                        selectedButton = Math.Max(0, selectedButton - 4);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedButton = Math.Min(15, selectedButton + 4);
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedButton = Math.Max(0, selectedButton - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        selectedButton = Math.Min(15, selectedButton + 1);
                        break;
                }
            }
            
            Console.Clear();
        }

        private static void RenderCalculator()
        {
            var window = new Window(
                new Rect(5, 2, 40, 20),
                "Calculator",
                () => { },
                true
            );
            window.Render();

            TUIHelper.SetColors(ConsoleColor.White, ConsoleColor.DarkBlue);
            
            Console.SetCursorPosition(7, 4);
            Console.Write(new string(' ', 36));
            Console.SetCursorPosition(7, 4);
            Console.Write(currentInput);

            int startX = 7;
            int startY = 7;
            int buttonWidth = 8;
            int buttonHeight = 3;

            for (int i = 0; i < buttons.Count; i++)
            {
                int row = i / 4;
                int col = i % 4;
                
                int x = startX + col * (buttonWidth + 1);
                int y = startY + row * (buttonHeight + 1);

                bool isSelected = (i == selectedButton);
                ConsoleColor bgColor = isSelected ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
                ConsoleColor fgColor = isSelected ? ConsoleColor.Black : ConsoleColor.White;

                TUIHelper.SetColors(fgColor, bgColor);
                
                for (int py = 0; py < buttonHeight; py++)
                {
                    Console.SetCursorPosition(x, y + py);
                    for (int px = 0; px < buttonWidth; px++)
                    {
                        if (py == 0 || py == buttonHeight - 1 || px == 0 || px == buttonWidth - 1)
                        {
                            Console.Write(isSelected ? " " : "+");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                }

                string buttonText = buttons[i];
                int textX = x + (buttonWidth - buttonText.Length) / 2;
                int textY = y + buttonHeight / 2;
                Console.SetCursorPosition(textX, textY);
                Console.Write(buttonText);
            }

            TUIHelper.SetColors(ConsoleColor.Gray, ConsoleColor.DarkBlue);
            Console.SetCursorPosition(7, 18);
            Console.Write("Arrow keys to navigate, Enter to select");
            Console.SetCursorPosition(7, 19);
            Console.Write("Esc or Q to exit");
        }

        private static void PressButton(string button)
        {
            switch (button)
            {
                case "C":
                    currentInput = "0";
                    previousInput = "";
                    currentOperation = "";
                    newInput = true;
                    break;

                case "=":
                    if (!string.IsNullOrEmpty(currentOperation) && !string.IsNullOrEmpty(previousInput))
                    {
                        try
                        {
                            double num1 = double.Parse(previousInput);
                            double num2 = double.Parse(currentInput);
                            double result = 0;

                            switch (currentOperation)
                            {
                                case "+":
                                    result = num1 + num2;
                                    break;
                                case "-":
                                    result = num1 - num2;
                                    break;
                                case "*":
                                    result = num1 * num2;
                                    break;
                                case "/":
                                    if (num2 != 0)
                                    {
                                        result = num1 / num2;
                                    }
                                    else
                                    {
                                        currentInput = "Error";
                                        return;
                                    }
                                    break;
                            }

                            currentInput = result.ToString();
                            previousInput = "";
                            currentOperation = "";
                            newInput = true;
                        }
                        catch
                        {
                            currentInput = "Error";
                        }
                    }
                    break;

                case "+":
                case "-":
                case "*":
                case "/":
                    if (string.IsNullOrEmpty(currentInput))
                    {
                        currentInput = "0";
                    }
                    
                    if (string.IsNullOrEmpty(previousInput) || string.IsNullOrEmpty(currentOperation))
                    {
                        previousInput = currentInput;
                        currentOperation = button;
                        newInput = true;
                    }
                    else
                    {
                        try
                        {
                            double num1 = double.Parse(previousInput);
                            double num2 = double.Parse(currentInput);
                            double result = 0;

                            switch (currentOperation)
                            {
                                case "+":
                                    result = num1 + num2;
                                    break;
                                case "-":
                                    result = num1 - num2;
                                    break;
                                case "*":
                                    result = num1 * num2;
                                    break;
                                case "/":
                                    if (num2 != 0)
                                    {
                                        result = num1 / num2;
                                    }
                                    else
                                    {
                                        currentInput = "Error";
                                        return;
                                    }
                                    break;
                            }

                            previousInput = result.ToString();
                            currentOperation = button;
                            newInput = false;
                        }
                        catch
                        {
                            currentInput = "Error";
                        }
                    }
                    break;

                default:
                    if (newInput || currentInput == "0" || currentInput == "Error")
                    {
                        currentInput = button;
                        newInput = false;
                    }
                    else
                    {
                        if (currentInput.Length < 10)
                        {
                            currentInput += button;
                        }
                    }
                    break;
            }
        }
    }
}