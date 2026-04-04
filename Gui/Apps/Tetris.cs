using Cosmos.System;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class Tetris : Process
    {
        internal Tetris() : base("Tetris", ProcessType.Application) { }

        private const int BoardWidth = 10;
        private const int BoardHeight = 20;
        private const int CellSize = 16;
        private const int BoardX = 14;
        private const int BoardY = 24;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly Random random = new Random();

        private AppWindow window;
        private Button slowButton;
        private Button normalButton;
        private Button fastButton;
        private readonly int[] board = new int[BoardWidth * BoardHeight];

        private int[] curX = new int[4];
        private int[] curY = new int[4];
        private int curType = 0;
        private int curColor = 1;
        private bool gameOver = false;

        private int score = 0;
        private int lines = 0;
        private int level = 1;
        private int frameCounter = 0;
        private int dropFrames = 36;
        private int speedMode = 1; // 0: Slow, 1: Normal, 2: Fast

        private bool dirty = true;

        // Each piece has 4 blocks: (x0,y0,x1,y1,x2,y2,x3,y3)
        private static readonly int[] PieceTemplates = new int[]
        {
            // I
            -1,0, 0,0, 1,0, 2,0,
            // O
            0,0, 1,0, 0,1, 1,1,
            // T
            -1,0, 0,0, 1,0, 0,1,
            // S
            0,0, 1,0, -1,1, 0,1,
            // Z
            -1,0, 0,0, 0,1, 1,1,
            // J
            -1,0, 0,0, 1,0, 1,1,
            // L
            -1,0, 0,0, 1,0, -1,1,
        };

        private static readonly Color[] BlockColors = new Color[]
        {
            Color.FromArgb(28, 38, 52), // unused
            Color.FromArgb(86, 201, 231), // I
            Color.FromArgb(246, 218, 87), // O
            Color.FromArgb(188, 111, 255), // T
            Color.FromArgb(117, 209, 119), // S
            Color.FromArgb(239, 100, 100), // Z
            Color.FromArgb(88, 142, 255), // J
            Color.FromArgb(255, 168, 83), // L
        };

        private static int Index(int x, int y)
        {
            return (y * BoardWidth) + x;
        }

        private int GetBaseDropFrames()
        {
            if (speedMode == 0) return 48;
            if (speedMode == 2) return 24;
            return 36;
        }

        private int GetMinDropFrames()
        {
            if (speedMode == 0) return 14;
            if (speedMode == 2) return 7;
            return 10;
        }

        private void UpdateDropFramesFromMode()
        {
            int baseFrames = GetBaseDropFrames();
            int minFrames = GetMinDropFrames();
            dropFrames = Math.Max(minFrames, baseFrames - ((level - 1) * 2));
        }

        private string GetSpeedModeLabel()
        {
            if (speedMode == 0) return "Slow";
            if (speedMode == 2) return "Fast";
            return "Normal";
        }

        private void SetSpeedButtonStyle()
        {
            if (slowButton == null || normalButton == null || fastButton == null) return;

            slowButton.Background = speedMode == 0 ? UITheme.Accent : UITheme.SurfaceMuted;
            slowButton.Border = speedMode == 0 ? UITheme.AccentDark : UITheme.SurfaceBorder;
            slowButton.Foreground = speedMode == 0 ? Color.White : UITheme.TextPrimary;

            normalButton.Background = speedMode == 1 ? UITheme.Accent : UITheme.SurfaceMuted;
            normalButton.Border = speedMode == 1 ? UITheme.AccentDark : UITheme.SurfaceBorder;
            normalButton.Foreground = speedMode == 1 ? Color.White : UITheme.TextPrimary;

            fastButton.Background = speedMode == 2 ? UITheme.Accent : UITheme.SurfaceMuted;
            fastButton.Border = speedMode == 2 ? UITheme.AccentDark : UITheme.SurfaceBorder;
            fastButton.Foreground = speedMode == 2 ? Color.White : UITheme.TextPrimary;
        }

        private void SetSpeedMode(int mode)
        {
            speedMode = Math.Max(0, Math.Min(2, mode));
            UpdateDropFramesFromMode();
            SetSpeedButtonStyle();
            dirty = true;
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight;
        }

        private bool CanPlace(int[] testX, int[] testY)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = testX[i];
                int y = testY[i];
                if (!InBounds(x, y))
                {
                    return false;
                }
                if (board[Index(x, y)] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void SpawnPiece()
        {
            curType = random.Next(0, 7);
            curColor = curType + 1;

            int baseOffset = curType * 8;
            int spawnX = BoardWidth / 2;
            int spawnY = 1;
            for (int i = 0; i < 4; i++)
            {
                curX[i] = spawnX + PieceTemplates[baseOffset + (i * 2)];
                curY[i] = spawnY + PieceTemplates[baseOffset + (i * 2) + 1];
            }

            if (!CanPlace(curX, curY))
            {
                gameOver = true;
            }
            dirty = true;
        }

        private void LockPiece()
        {
            for (int i = 0; i < 4; i++)
            {
                if (InBounds(curX[i], curY[i]))
                {
                    board[Index(curX[i], curY[i])] = curColor;
                }
            }
            ClearLines();
            SpawnPiece();
        }

        private void ClearLines()
        {
            int cleared = 0;
            for (int y = BoardHeight - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < BoardWidth; x++)
                {
                    if (board[Index(x, y)] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (!full)
                {
                    continue;
                }

                for (int ty = y; ty > 0; ty--)
                {
                    for (int x = 0; x < BoardWidth; x++)
                    {
                        board[Index(x, ty)] = board[Index(x, ty - 1)];
                    }
                }
                for (int x = 0; x < BoardWidth; x++)
                {
                    board[Index(x, 0)] = 0;
                }

                cleared++;
                y++; // re-check same line after shift
            }

            if (cleared > 0)
            {
                lines += cleared;
                if (cleared == 1) score += 100 * level;
                else if (cleared == 2) score += 300 * level;
                else if (cleared == 3) score += 500 * level;
                else score += 800 * level;

                level = 1 + (lines / 10);
                UpdateDropFramesFromMode();
                dirty = true;
            }
        }

        private void MovePiece(int dx, int dy, bool lockOnFail)
        {
            if (gameOver) return;

            int[] tx = new int[4];
            int[] ty = new int[4];
            for (int i = 0; i < 4; i++)
            {
                tx[i] = curX[i] + dx;
                ty[i] = curY[i] + dy;
            }

            if (CanPlace(tx, ty))
            {
                curX = tx;
                curY = ty;
                dirty = true;
                return;
            }

            if (lockOnFail && dy > 0)
            {
                LockPiece();
            }
        }

        private void RotatePiece()
        {
            if (gameOver) return;
            if (curType == 1) return; // O

            int pivotX = curX[1];
            int pivotY = curY[1];

            int[] tx = new int[4];
            int[] ty = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int relX = curX[i] - pivotX;
                int relY = curY[i] - pivotY;
                tx[i] = pivotX - relY;
                ty[i] = pivotY + relX;
            }

            if (CanPlace(tx, ty))
            {
                curX = tx;
                curY = ty;
                dirty = true;
                return;
            }

            // simple wall kick
            for (int shift = -1; shift <= 1; shift += 2)
            {
                for (int i = 0; i < 4; i++) tx[i] += shift;
                if (CanPlace(tx, ty))
                {
                    curX = tx;
                    curY = ty;
                    dirty = true;
                    return;
                }
                for (int i = 0; i < 4; i++) tx[i] -= shift;
            }
        }

        private void HardDrop()
        {
            if (gameOver) return;
            while (true)
            {
                int oldY = curY[0];
                MovePiece(0, 1, false);
                if (curY[0] == oldY)
                {
                    LockPiece();
                    break;
                }
            }
            dirty = true;
        }

        private void ResetGame()
        {
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = 0;
            }
            score = 0;
            lines = 0;
            level = 1;
            UpdateDropFramesFromMode();
            frameCounter = 0;
            gameOver = false;
            SpawnPiece();
            dirty = true;
        }

        private void OnKeyPressed(KeyEvent key)
        {
            if (gameOver)
            {
                if (key.Key == ConsoleKeyEx.R)
                {
                    ResetGame();
                }
                return;
            }

            switch (key.Key)
            {
                case ConsoleKeyEx.LeftArrow:
                    MovePiece(-1, 0, false);
                    break;
                case ConsoleKeyEx.RightArrow:
                    MovePiece(1, 0, false);
                    break;
                case ConsoleKeyEx.DownArrow:
                    MovePiece(0, 1, true);
                    break;
                case ConsoleKeyEx.UpArrow:
                    RotatePiece();
                    break;
                case ConsoleKeyEx.Spacebar:
                    HardDrop();
                    break;
                case ConsoleKeyEx.D1:
                    SetSpeedMode(0);
                    break;
                case ConsoleKeyEx.D2:
                    SetSpeedMode(1);
                    break;
                case ConsoleKeyEx.D3:
                    SetSpeedMode(2);
                    break;
            }
        }

        private void DrawCell(int x, int y, Color color)
        {
            int px = BoardX + (x * CellSize);
            int py = BoardY + (y * CellSize);
            window.DrawFilledRectangle(px, py, CellSize, CellSize, color);
            window.DrawRectangle(px, py, CellSize, CellSize, Color.FromArgb(28, 38, 52));
        }

        private void Render()
        {
            window.Clear(Color.FromArgb(19, 22, 28));

            int boardPixelW = BoardWidth * CellSize;
            int boardPixelH = BoardHeight * CellSize;
            window.DrawFilledRectangle(BoardX - 2, BoardY - 2, boardPixelW + 4, boardPixelH + 4, Color.FromArgb(56, 64, 76));
            window.DrawFilledRectangle(BoardX, BoardY, boardPixelW, boardPixelH, Color.FromArgb(30, 34, 42));

            for (int y = 0; y < BoardHeight; y++)
            {
                for (int x = 0; x < BoardWidth; x++)
                {
                    int v = board[Index(x, y)];
                    if (v != 0)
                    {
                        DrawCell(x, y, BlockColors[v]);
                    }
                    else
                    {
                        int px = BoardX + (x * CellSize);
                        int py = BoardY + (y * CellSize);
                        window.DrawRectangle(px, py, CellSize, CellSize, Color.FromArgb(37, 43, 54));
                    }
                }
            }

            if (!gameOver)
            {
                for (int i = 0; i < 4; i++)
                {
                    DrawCell(curX[i], curY[i], BlockColors[curColor]);
                }
            }

            int sideX = BoardX + boardPixelW + 20;
            window.DrawString("TETRIS", Color.FromArgb(196, 212, 235), sideX, 30);
            window.DrawString("Score: " + score.ToString(), Color.White, sideX, 62);
            window.DrawString("Lines: " + lines.ToString(), Color.White, sideX, 82);
            window.DrawString("Level: " + level.ToString(), Color.White, sideX, 102);
            window.DrawString("Speed: " + GetSpeedModeLabel(), Color.White, sideX, 122);
            window.DrawString("Keys:", Color.FromArgb(186, 198, 216), sideX, 136);
            window.DrawString("Arrows Move", Color.FromArgb(186, 198, 216), sideX, 154);
            window.DrawString("Up Rotate", Color.FromArgb(186, 198, 216), sideX, 172);
            window.DrawString("Space Drop", Color.FromArgb(186, 198, 216), sideX, 190);
            window.DrawString("1/2/3 Speed", Color.FromArgb(186, 198, 216), sideX, 208);

            if (gameOver)
            {
                int boxW = 210;
                int boxH = 68;
                int boxX = (window.Width / 2) - (boxW / 2);
                int boxY = (window.Height / 2) - (boxH / 2);
                window.DrawFilledRectangle(boxX, boxY, boxW, boxH, Color.FromArgb(18, 18, 22));
                window.DrawRectangle(boxX, boxY, boxW, boxH, Color.FromArgb(210, 96, 96));
                window.DrawString("Game Over", Color.FromArgb(250, 140, 140), boxX + 60, boxY + 14);
                window.DrawString("Press R to restart", Color.White, boxX + 38, boxY + 36);
            }

            wm.Update(window);
            dirty = false;
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 170, 80, 430, 380);
            window.Title = "Tetris";
            window.Icon = AppManager.DefaultAppIcon;
            window.Closing = TryStop;
            window.OnKeyPressed = OnKeyPressed;
            wm.AddWindow(window);

            slowButton = new Button(window, 206, 338, 62, 20);
            slowButton.Text = "Slow";
            slowButton.OnClick = (_, _) => SetSpeedMode(0);
            wm.AddWindow(slowButton);

            normalButton = new Button(window, 274, 338, 72, 20);
            normalButton.Text = "Normal";
            normalButton.OnClick = (_, _) => SetSpeedMode(1);
            wm.AddWindow(normalButton);

            fastButton = new Button(window, 352, 338, 62, 20);
            fastButton.Text = "Fast";
            fastButton.OnClick = (_, _) => SetSpeedMode(2);
            wm.AddWindow(fastButton);

            SetSpeedButtonStyle();
            ResetGame();
            Render();
        }

        public override void Run()
        {
            if (!gameOver)
            {
                frameCounter++;
                if (frameCounter >= dropFrames)
                {
                    frameCounter = 0;
                    MovePiece(0, 1, true);
                }
            }

            if (dirty)
            {
                Render();
            }
        }
    }
}
