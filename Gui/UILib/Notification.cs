using CMLeonOS;
using CMLeonOS.UILib.Animations;
using System.Drawing;

namespace CMLeonOS.Gui.UILib
{
    internal class Notification
    {
        internal Notification(Process process, string title, string message)
        {
            owner = process;
            Title = title;
            Message = message;
        }

        private readonly Process owner;
        internal string Title { get; }
        internal string Message { get; }
        internal void Show()
        {
            ProcessManager.AddProcess(owner, new NotificationProcess(owner, Title, Message)).Start();
        }

        private class NotificationProcess : Process
        {
            internal NotificationProcess(Process parent, string title, string message) : base("Notification", ProcessType.Background, parent)
            {
                this.title = title;
                this.message = message;
            }

            private readonly string title;
            private readonly string message;

            private Window window;
            private Button closeButton;
            private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
            private bool closeAnimationRunning = false;
            private int targetX;
            private int targetY;

            public override void Start()
            {
                base.Start();

                int width = 280;
                int height = 84;
                targetX = (int)wm.ScreenWidth - width - 16;
                targetY = (int)wm.ScreenHeight - height - 44;
                int startX = (int)wm.ScreenWidth + 12;

                window = new Window(this, startX, targetY, width, height);
                window.Clear(UITheme.WindowTitleBackground);
                window.DrawFilledRectangle(0, 0, window.Width, window.Height, UITheme.WindowTitleBackground);
                window.DrawRectangle(0, 0, window.Width, window.Height, UITheme.SurfaceBorder);
                window.DrawFilledRectangle(0, 0, window.Width, 22, UITheme.Accent);
                window.DrawString(title, UITheme.WindowTitleText, 10, 4);
                window.DrawString(message, Color.White, 10, 30);
                wm.AddWindow(window);

                closeButton = new Button(window, window.Width - 54, window.Height - 28, 44, 20);
                closeButton.Text = "Close";
                closeButton.OnClick = (_, _) => BeginCloseAnimation();
                wm.AddWindow(closeButton);

                wm.Update(window);
                StartOpenAnimation(startX, targetY, width, height);
            }

            private void StartOpenAnimation(int startX, int startY, int width, int height)
            {
                MovementAnimation animation = new MovementAnimation(window)
                {
                    From = new Rectangle(startX, startY, width, height),
                    To = new Rectangle(targetX, targetY, width, height),
                    Duration = 10,
                    EasingType = EasingType.Sine,
                    EasingDirection = EasingDirection.Out
                };
                animation.Completed = () =>
                {
                    wm.Update(window);
                    closeButton.Render();
                };
                animation.Start();
            }

            private void BeginCloseAnimation()
            {
                if (closeAnimationRunning || window == null)
                {
                    return;
                }

                closeAnimationRunning = true;

                int endX = (int)wm.ScreenWidth + 12;
                int width = window.Width;
                int height = window.Height;

                MovementAnimation animation = new MovementAnimation(window)
                {
                    From = new Rectangle(window.X, window.Y, width, height),
                    To = new Rectangle(endX, targetY, width, height),
                    Duration = 9,
                    EasingType = EasingType.Sine,
                    EasingDirection = EasingDirection.In
                };
                animation.Completed = TryStop;
                animation.Start();
            }

            public override void Run()
            {
            }

            public override void Stop()
            {
                base.Stop();
                if (closeButton != null)
                {
                    wm.RemoveWindow(closeButton);
                }
                if (window != null)
                {
                    wm.RemoveWindow(window);
                }
            }
        }
    }
}
