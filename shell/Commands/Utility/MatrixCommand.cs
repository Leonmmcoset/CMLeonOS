using System;
using System.Threading;

namespace CMLeonOS.Commands.Utility
{
    public static class MatrixCommand
    {
        private static readonly string[] matrixChars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "@", "#", "$", "%", "&", "*", "(", ")", "-", "_", "+", "=", "{", "}", "[", "]", "|", "\\", ":", ";", "\"", "'", "<", ">", ",", ".", "?", "/", "~", "`" };
        private static readonly ConsoleColor matrixColor = ConsoleColor.Green;
        private static readonly ConsoleColor backgroundColor = ConsoleColor.Black;
        private static Random random = new Random();
        private static bool running = true;

        public static void ShowMatrix()
        {
            Console.Clear();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = matrixColor;
            Console.Clear();

            running = true;
            
            Console.WriteLine("Press ESC or Q to exit...");
            Thread.Sleep(2000);

            int consoleWidth = 80;
            int consoleHeight = 25;
            
            int[] columns = new int[consoleWidth];
            int[] columnSpeeds = new int[consoleWidth];
            
            for (int i = 0; i < consoleWidth; i++)
            {
                columns[i] = random.Next(consoleHeight / 2);
                columnSpeeds[i] = random.Next(1, 2);
            }

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Q)
                    {
                        running = false;
                        break;
                    }
                }

                for (int i = 0; i < consoleWidth; i++)
                {
                    columns[i] += columnSpeeds[i];
                    
                    if (columns[i] >= consoleHeight)
                    {
                        columns[i] = random.Next(consoleHeight / 2);
                        columnSpeeds[i] = random.Next(1, 2);
                    }

                    if (columns[i] > 0 && columns[i] < consoleHeight)
                    {
                        int charIndex = random.Next(matrixChars.Length);
                        Console.SetCursorPosition(i, columns[i]);
                        Console.Write(matrixChars[charIndex] + " ");
                    }
                }

                Thread.Sleep(100);
            }

            Console.Clear();
            Console.ResetColor();
        }
    }
}