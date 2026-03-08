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

using System;

namespace CMLeonOS.Commands.Utility
{
    public static class CalcCommand
    {
        public static void Calculate(string expression, Action<string> showError)
        {
            try
            {
                var parts = expression.Split(' ');
                if (parts.Length == 3)
                {
                    double num1 = double.Parse(parts[0]);
                    string op = parts[1];
                    double num2 = double.Parse(parts[2]);
                    double result = 0;

                    switch (op)
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
                                showError("Division by zero");
                                return;
                            }
                            break;
                        default:
                            showError("Invalid operator. Use +, -, *, /");
                            return;
                    }

                    Console.WriteLine($"Result: {result}");
                }
                else
                {
                    showError("Invalid expression. Use format: calc <num> <op> <num>");
                }
            }
            catch (Exception ex)
            {
                showError($"Error: {ex.Message}");
            }
        }
    }
}
