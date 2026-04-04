using Cosmos.System;
using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class PingPong : Process
    {
        internal PingPong() : base("Ping Pong", ProcessType.Application) { }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly Random random = new Random();

        private AppWindow window;
        private Button resetButton;
        private Button pauseButton;

        private int playerScore = 0;
        private int aiScore = 0;
        private bool paused = false;

        private const int ArenaX = 14;
        private const int ArenaY = 28;
        private const int ArenaWidth = 620;
        private const int ArenaHeight = 320;

        private const int PaddleWidth = 10;
        private const int PaddleHeight = 74;
        private const int BallSize = 8;

        private int playerY;
        private int aiY;
        private float ballX;
        private float ballY;
        private float ballVx;
        private float ballVy;

        private bool dirty = true;

        private void ResetBall(int direction)
        {
            ballX = ArenaX + (ArenaWidth / 2f) - (BallSize / 2f);
            ballY = ArenaY + (ArenaHeight / 2f) - (BallSize / 2f);

            float speed = 1.8f;
            ballVx = speed * direction;
            ballVy = ((float)random.NextDouble() * 2.0f - 1.0f) * 1.1f;
            if (Math.Abs(ballVy) < 0.35f)
            {
                ballVy = ballVy < 0 ? -0.35f : 0.35f;
            }
        }

        private void ResetGame()
        {
            playerScore = 0;
            aiScore = 0;
            playerY = ArenaY + (ArenaHeight / 2) - (PaddleHeight / 2);
            aiY = playerY;
            paused = false;
            pauseButton.Text = "Pause";
            ResetBall(random.Next(0, 2) == 0 ? -1 : 1);
            dirty = true;
        }

        private void MovePlayer(int dy)
        {
            playerY += dy;
            int minY = ArenaY;
            int maxY = ArenaY + ArenaHeight - PaddleHeight;
            if (playerY < minY) playerY = minY;
            if (playerY > maxY) playerY = maxY;
            dirty = true;
        }

        private void UpdateAi()
        {
            int aiCenter = aiY + (PaddleHeight / 2);
            int ballCenter = (int)ballY + (BallSize / 2);
            int diff = ballCenter - aiCenter;

            int maxStep = 4;
            if (diff > 0) aiY += Math.Min(maxStep, diff);
            else if (diff < 0) aiY -= Math.Min(maxStep, -diff);

            int minY = ArenaY;
            int maxY = ArenaY + ArenaHeight - PaddleHeight;
            if (aiY < minY) aiY = minY;
            if (aiY > maxY) aiY = maxY;
        }

        private void BounceFromPaddle(int paddleY, bool fromLeft)
        {
            float paddleCenter = paddleY + (PaddleHeight / 2f);
            float ballCenter = ballY + (BallSize / 2f);
            float offset = (ballCenter - paddleCenter) / (PaddleHeight / 2f);
            if (offset < -1f) offset = -1f;
            if (offset > 1f) offset = 1f;

            ballVy += offset * 1.6f;
            float maxVy = 3.8f;
            if (ballVy < -maxVy) ballVy = -maxVy;
            if (ballVy > maxVy) ballVy = maxVy;

            if (fromLeft)
            {
                ballVx = Math.Abs(ballVx) + 0.05f;
            }
            else
            {
                ballVx = -Math.Abs(ballVx) - 0.05f;
            }
        }

        private void UpdateBall()
        {
            ballX += ballVx;
            ballY += ballVy;

            if (ballY <= ArenaY)
            {
                ballY = ArenaY;
                ballVy = Math.Abs(ballVy);
            }
            else if (ballY + BallSize >= ArenaY + ArenaHeight)
            {
                ballY = ArenaY + ArenaHeight - BallSize;
                ballVy = -Math.Abs(ballVy);
            }

            int playerX = ArenaX + 12;
            int aiX = ArenaX + ArenaWidth - 12 - PaddleWidth;

            if (ballVx < 0
                && ballX <= playerX + PaddleWidth
                && ballX + BallSize >= playerX
                && ballY + BallSize >= playerY
                && ballY <= playerY + PaddleHeight)
            {
                ballX = playerX + PaddleWidth;
                BounceFromPaddle(playerY, fromLeft: true);
            }

            if (ballVx > 0
                && ballX + BallSize >= aiX
                && ballX <= aiX + PaddleWidth
                && ballY + BallSize >= aiY
                && ballY <= aiY + PaddleHeight)
            {
                ballX = aiX - BallSize;
                BounceFromPaddle(aiY, fromLeft: false);
            }

            if (ballX < ArenaX - 4)
            {
                aiScore++;
                ResetBall(direction: -1);
            }
            else if (ballX > ArenaX + ArenaWidth + 4)
            {
                playerScore++;
                ResetBall(direction: 1);
            }

            dirty = true;
        }

        private void OnKeyPressed(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.W:
                case ConsoleKeyEx.UpArrow:
                    MovePlayer(-20);
                    break;
                case ConsoleKeyEx.S:
                case ConsoleKeyEx.DownArrow:
                    MovePlayer(20);
                    break;
                case ConsoleKeyEx.Spacebar:
                    paused = !paused;
                    pauseButton.Text = paused ? "Resume" : "Pause";
                    dirty = true;
                    break;
                case ConsoleKeyEx.R:
                    ResetGame();
                    break;
            }
        }

        private void Render()
        {
            window.Clear(Color.FromArgb(18, 22, 30));

            window.DrawFilledRectangle(ArenaX - 2, ArenaY - 2, ArenaWidth + 4, ArenaHeight + 4, Color.FromArgb(62, 74, 94));
            window.DrawFilledRectangle(ArenaX, ArenaY, ArenaWidth, ArenaHeight, Color.FromArgb(28, 33, 45));
            window.DrawRectangle(ArenaX, ArenaY, ArenaWidth, ArenaHeight, Color.FromArgb(100, 116, 142));

            int centerX = ArenaX + (ArenaWidth / 2);
            for (int y = ArenaY + 4; y < ArenaY + ArenaHeight - 4; y += 16)
            {
                window.DrawFilledRectangle(centerX - 1, y, 2, 9, Color.FromArgb(84, 98, 122));
            }

            int playerX = ArenaX + 12;
            int aiX = ArenaX + ArenaWidth - 12 - PaddleWidth;
            window.DrawFilledRectangle(playerX, playerY, PaddleWidth, PaddleHeight, Color.FromArgb(212, 226, 252));
            window.DrawFilledRectangle(aiX, aiY, PaddleWidth, PaddleHeight, Color.FromArgb(252, 216, 184));

            window.DrawFilledRectangle((int)ballX, (int)ballY, BallSize, BallSize, Color.FromArgb(255, 255, 255));
            window.DrawRectangle((int)ballX, (int)ballY, BallSize, BallSize, Color.FromArgb(132, 146, 172));

            window.DrawString("Ping Pong", Color.FromArgb(223, 233, 250), 16, 8);
            window.DrawString($"Player {playerScore} : {aiScore} AI", Color.White, ArenaX + (ArenaWidth / 2) - 76, 8);
            window.DrawString("Controls: W/S or Up/Down | Space Pause | R Reset", Color.FromArgb(180, 196, 220), 16, ArenaY + ArenaHeight + 8);

            if (paused)
            {
                int boxW = 180;
                int boxH = 54;
                int boxX = ArenaX + (ArenaWidth / 2) - (boxW / 2);
                int boxY = ArenaY + (ArenaHeight / 2) - (boxH / 2);
                window.DrawFilledRectangle(boxX, boxY, boxW, boxH, Color.FromArgb(20, 20, 26));
                window.DrawRectangle(boxX, boxY, boxW, boxH, Color.FromArgb(112, 132, 165));
                window.DrawString("Paused", Color.FromArgb(230, 238, 255), boxX + 64, boxY + 12);
                window.DrawString("Press Space to resume", Color.FromArgb(190, 204, 232), boxX + 16, boxY + 30);
            }

            wm.Update(window);
            dirty = false;
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 120, 90, 650, 390);
            window.Title = "Ping Pong";
            window.Icon = AppManager.DefaultAppIcon;
            window.Closing = TryStop;
            window.OnKeyPressed = OnKeyPressed;
            wm.AddWindow(window);

            resetButton = new Button(window, 456, 356, 88, 24);
            resetButton.Text = "Reset";
            resetButton.OnClick = (_, _) => ResetGame();
            wm.AddWindow(resetButton);

            pauseButton = new Button(window, 550, 356, 88, 24);
            pauseButton.Text = "Pause";
            pauseButton.OnClick = (_, _) =>
            {
                paused = !paused;
                pauseButton.Text = paused ? "Resume" : "Pause";
                dirty = true;
            };
            wm.AddWindow(pauseButton);

            ResetGame();
            Render();
        }

        public override void Run()
        {
            if (!paused)
            {
                UpdateAi();
                UpdateBall();
            }

            if (dirty)
            {
                Render();
            }
        }
    }
}
