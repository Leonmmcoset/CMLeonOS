using System;
using System.Collections.Generic;

namespace CMLeonOS.Commands
{
    public static class LabyrinthCommand
    {
        private static int playerX = 1;
        private static int playerY = 1;
        private static int exitX;
        private static int exitY;
        private static int width = 15;
        private static int height = 15;
        private static List<string> maze = new List<string>();
        private static Random random = new Random();

        public static void ProcessLabyrinth()
        {
            GenerateMaze();
            playerX = 1;
            playerY = 1;
            bool gameRunning = true;

            Console.Clear();
            Console.WriteLine("Labyrinth - Maze Escape Game");
            Console.WriteLine("Use arrow keys (↑ ↓ ← →) to move, press ESC to exit");
            Console.WriteLine();

            while (gameRunning)
            {
                DisplayMaze();
                Console.SetCursorPosition(0, height + 2);
                Console.Write($"Position: ({playerX}, {playerY})");
                
                var key = Console.ReadKey(true);
                
                if (key.Key == ConsoleKey.Escape)
                {
                    gameRunning = false;
                    Console.WriteLine();
                    Console.WriteLine("Game Over!");
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (playerY > 1 && GetMazeChar(playerY - 1, playerX) != '#')
                    {
                        playerY--;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (playerY < height && GetMazeChar(playerY + 1, playerX) != '#')
                    {
                        playerY++;
                    }
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (playerX > 1 && GetMazeChar(playerY, playerX - 1) != '#')
                    {
                        playerX--;
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (playerX < width && GetMazeChar(playerY, playerX + 1) != '#')
                    {
                        playerX++;
                    }
                }

                if (playerX == exitX && playerY == exitY)
                {
                    gameRunning = false;
                    Console.Clear();
                    Console.WriteLine("Congratulations! You have successfully escaped the maze!");
                    Console.WriteLine($"Steps: {GetStepCount()}");
                }
            }
        }

        private static void GenerateMaze()
        {
            maze.Clear();
            
            for (int i = 0; i < height + 2; i++)
            {
                string row = "";
                for (int j = 0; j < width + 2; j++)
                {
                    if (i == 0 || i == height + 1 || j == 0 || j == width + 1)
                    {
                        row += "#";
                    }
                    else
                    {
                        row += "#";
                    }
                }
                maze.Add(row);
            }

            GenerateMazeRecursive(1, 1);

            bool exitPlaced = false;
            while (!exitPlaced)
            {
                int startSide = random.Next(0, 4);
                if (startSide == 0)
                {
                    playerX = 1;
                    playerY = 1;
                }
                else if (startSide == 1)
                {
                    playerX = width;
                    playerY = 1;
                }
                else if (startSide == 2)
                {
                    playerX = 1;
                    playerY = height;
                }
                else if (startSide == 3)
                {
                    playerX = width;
                    playerY = height;
                }

                exitX = random.Next(1, width);
                exitY = random.Next(1, height);
                while (GetMazeChar(exitY, exitX) == '#')
                {
                    exitX = random.Next(1, width);
                    exitY = random.Next(1, height);
                }
                SetMazeChar(exitY, exitX, 'E');
                exitPlaced = true;
            }

            SetMazeChar(playerY, playerX, 'P');
        }

        private static void GenerateMazeRecursive(int x, int y)
        {
            SetMazeChar(y, x, ' ');
            
            List<int[]> directions = new List<int[]>
            {
                new int[] {0, -2},
                new int[] {0, 2},
                new int[] {-2, 0},
                new int[] {2, 0}
            };

            for (int i = directions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int[] temp = directions[i];
                directions[i] = directions[j];
                directions[j] = temp;
            }

            foreach (int[] dir in directions)
            {
                int nx = x + dir[0];
                int ny = y + dir[1];

                if (nx > 0 && nx < width + 1 && ny > 0 && ny < height + 1 && GetMazeChar(ny, nx) == '#')
                {
                    SetMazeChar(y + dir[1] / 2, x + dir[0] / 2, ' ');
                    GenerateMazeRecursive(nx, ny);
                }
            }
        }

        private static char GetMazeChar(int row, int col)
        {
            if (row >= 0 && row < maze.Count && col >= 0 && col < maze[row].Length)
            {
                return maze[row][col];
            }
            return '#';
        }

        private static void SetMazeChar(int row, int col, char value)
        {
            if (row >= 0 && row < maze.Count && col >= 0 && col < maze[row].Length)
            {
                char[] chars = maze[row].ToCharArray();
                chars[col] = value;
                maze[row] = new string(chars);
            }
        }

        private static void DisplayMaze()
        {
            Console.SetCursorPosition(0, 0);
            
            for (int i = 0; i < maze.Count; i++)
            {
                for (int j = 0; j < maze[i].Length; j++)
                {
                    if (i == playerY && j == playerX)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('@');
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (i == exitY && j == exitX)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('E');
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (i == exitY && j == exitX + 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('X');
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write(GetMazeChar(i, j));
                    }
                }
                Console.WriteLine();
            }
        }

        private static int GetStepCount()
        {
            int steps = 0;
            for (int i = 0; i < maze.Count; i++)
            {
                for (int j = 0; j < maze[i].Length; j++)
                {
                    if (GetMazeChar(i, j) == '.')
                    {
                        steps++;
                    }
                }
            }
            return steps;
        }
    }
}
