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
