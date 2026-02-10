using System;
using System.Collections.Generic;
using System.Threading;

namespace CMLeonOS.Commands.Utility
{
    public static class SnakeCommand
    {
        private static readonly int width = 40;
        private static readonly int height = 20;
        private static readonly int snakeStartLength = 3;
        private static List<(int x, int y)> snake;
        private static (int x, int y) food;
        private static (int dx, int dy) direction;
        private static bool running;
        private static Random random;
        private static int score;
        private static bool gameOver;

        public static void PlaySnake()
        {
            Console.Clear();
            
            InitializeGame();
            
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    HandleInput(key);
                }
                
                Update();
                Render();
                
                if (gameOver)
                {
                    ShowGameOver();
                    break;
                }
                
                Thread.Sleep(100);
            }
            
            Console.Clear();
            Console.ResetColor();
        }

        private static void InitializeGame()
        {
            snake = new List<(int, int y)>();
            direction = (1, 0);
            random = new Random();
            score = 0;
            gameOver = false;
            running = true;
            
            int startX = width / 2;
            int startY = height / 2;
            
            for (int i = 0; i < snakeStartLength; i++)
            {
                snake.Add((startX - i, startY));
            }
            
            SpawnFood();
        }

        private static void HandleInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (direction.dy == 0)
                        direction = (0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    if (direction.dy == 0)
                        direction = (0, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction.dx == 0)
                        direction = (-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    if (direction.dx == 0)
                        direction = (1, 0);
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    running = false;
                    break;
            }
        }

        private static void Update()
        {
            if (!running || gameOver)
                return;
            
            int headX = snake[0].x + direction.dx;
            int headY = snake[0].y + direction.dy;
            
            if (headX < 0 || headX >= width || headY < 0 || headY >= height)
            {
                gameOver = true;
                return;
            }
            
            for (int i = 1; i < snake.Count; i++)
            {
                if (headX == snake[i].x && headY == snake[i].y)
                {
                    gameOver = true;
                    return;
                }
            }
            
            snake.Insert(0, (headX, headY));
            
            if (headX == food.x && headY == food.y)
            {
                score += 10;
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private static void Render()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var segment in snake)
            {
                Console.SetCursorPosition(segment.x, segment.y);
                Console.Write("#");
            }
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(food.x, food.y);
            Console.Write("O");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(0, height);
            Console.Write($"Score: {score} | Arrow keys to move | ESC or Q to quit");
            
            Console.ResetColor();
        }

        private static void SpawnFood()
        {
            bool validPosition = false;
            while (!validPosition)
            {
                food.x = random.Next(0, width);
                food.y = random.Next(0, height);
                
                validPosition = true;
                foreach (var segment in snake)
                {
                    if (food.x == segment.x && food.y == segment.y)
                    {
                        validPosition = false;
                        break;
                    }
                }
            }
        }

        private static void ShowGameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(width / 2 - 5, height / 2);
            Console.WriteLine("GAME OVER!");
            Console.SetCursorPosition(width / 2 - 8, height / 2 + 1);
            Console.WriteLine($"Final Score: {score}");
            Console.SetCursorPosition(width / 2 - 10, height / 2 + 3);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}