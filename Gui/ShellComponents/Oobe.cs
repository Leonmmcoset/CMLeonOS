using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;
using Sys = Cosmos.System;

namespace CMLeonOS.Gui.ShellComponents
{
    internal class Oobe : Process
    {
        internal Oobe() : base("OOBE", ProcessType.Application) { }

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private AppWindow window;
        private readonly List<Window> stepWindows = new List<Window>();

        private int step = 0;
        private bool acceptedTerms = false;

        private TextBox usernameBox;
        private TextBox passwordBox;
        private TextBox confirmBox;
        private TextBox hostnameBox;
        private TextBlock statusText;

        private Button backButton;
        private Button nextButton;

        private void AddStepWindow(Window w)
        {
            stepWindows.Add(w);
            wm.AddWindow(w);
        }

        private void ClearStepWindows()
        {
            for (int i = 0; i < stepWindows.Count; i++)
            {
                wm.RemoveWindow(stepWindows[i]);
            }
            stepWindows.Clear();
        }

        private void ShowStatus(string text, Color color)
        {
            statusText.Text = text;
            statusText.Foreground = color;
            statusText.Render();
        }

        private void RenderStep()
        {
            ClearStepWindows();

            window.Clear(UITheme.Surface);
            window.DrawRectangle(0, 0, window.Width, window.Height, UITheme.SurfaceBorder);
            window.DrawString("CMLeonOS Setup", UITheme.TextPrimary, 16, 12);
            window.DrawString($"Step {step + 1}/4", UITheme.TextSecondary, window.Width - 92, 12);

            int contentX = 16;
            int contentY = 44;
            int contentW = window.Width - 32;
            int contentH = window.Height - 108;

            Window content = new Window(this, window, contentX, contentY, contentW, contentH);
            content.Clear(Color.FromArgb(246, 249, 253));
            content.DrawRectangle(0, 0, content.Width, content.Height, UITheme.SurfaceBorder);
            AddStepWindow(content);

            switch (step)
            {
                case 0:
                    content.DrawString("Welcome", UITheme.TextPrimary, 12, 12);
                    content.DrawString("Let's configure your first administrator account.", UITheme.TextSecondary, 12, 34);
                    content.DrawString("Press Next to continue.", UITheme.TextSecondary, 12, 56);
                    backButton.Text = "Cancel";
                    nextButton.Text = "Next";
                    break;

                case 1:
                    content.DrawString("Terms and Conditions", UITheme.TextPrimary, 12, 12);
                    content.DrawString("Please read and accept to continue.", UITheme.TextSecondary, 12, 34);

                    TextBox terms = new TextBox(content, 12, 58, content.Width - 24, content.Height - 112);
                    terms.ReadOnly = true;
                    terms.MultiLine = true;
                    terms.Text =
                        "1. This system is provided as-is without warranty.\n" +
                        "2. You are responsible for your data and backups.\n" +
                        "3. Unauthorized access attempts may be logged.\n" +
                        "4. Administrators can access local system data.\n" +
                        "5. Data is stored locally on this device.\n" +
                        "6. Updates may change behavior and compatibility.\n" +
                        "7. Use implies acceptance of these terms.";
                    AddStepWindow(terms);

                    Switch accept = new Switch(content, 12, content.Height - 44, content.Width - 24, 18);
                    accept.Text = "I accept these terms";
                    accept.Checked = acceptedTerms;
                    accept.CheckBoxChanged = v => acceptedTerms = v;
                    AddStepWindow(accept);

                    backButton.Text = "Back";
                    nextButton.Text = "Next";
                    break;

                case 2:
                    content.DrawString("Administrator Account", UITheme.TextPrimary, 12, 12);
                    content.DrawString("Create your first admin user.", UITheme.TextSecondary, 12, 34);

                    TextBlock userLabel = new TextBlock(content, 12, 66, 94, 20);
                    userLabel.Text = "Username";
                    userLabel.Foreground = UITheme.TextSecondary;
                    userLabel.Background = Color.FromArgb(246, 249, 253);
                    AddStepWindow(userLabel);

                    usernameBox = new TextBox(content, 110, 64, content.Width - 122, 24);
                    AddStepWindow(usernameBox);

                    TextBlock passLabel = new TextBlock(content, 12, 100, 94, 20);
                    passLabel.Text = "Password";
                    passLabel.Foreground = UITheme.TextSecondary;
                    passLabel.Background = Color.FromArgb(246, 249, 253);
                    AddStepWindow(passLabel);

                    passwordBox = new TextBox(content, 110, 98, content.Width - 122, 24);
                    passwordBox.Shield = true;
                    AddStepWindow(passwordBox);

                    TextBlock confirmLabel = new TextBlock(content, 12, 134, 94, 20);
                    confirmLabel.Text = "Confirm";
                    confirmLabel.Foreground = UITheme.TextSecondary;
                    confirmLabel.Background = Color.FromArgb(246, 249, 253);
                    AddStepWindow(confirmLabel);

                    confirmBox = new TextBox(content, 110, 132, content.Width - 122, 24);
                    confirmBox.Shield = true;
                    AddStepWindow(confirmBox);

                    content.DrawString("Username cannot include: < > : \" | ? * / \\ or space", UITheme.TextSecondary, 12, 168);

                    backButton.Text = "Back";
                    nextButton.Text = "Next";
                    break;

                case 3:
                    content.DrawString("Device Name", UITheme.TextPrimary, 12, 12);
                    content.DrawString("Set a hostname for this device.", UITheme.TextSecondary, 12, 34);

                    TextBlock hostLabel = new TextBlock(content, 12, 74, 94, 20);
                    hostLabel.Text = "Hostname";
                    hostLabel.Foreground = UITheme.TextSecondary;
                    hostLabel.Background = Color.FromArgb(246, 249, 253);
                    AddStepWindow(hostLabel);

                    hostnameBox = new TextBox(content, 110, 72, content.Width - 122, 24);
                    AddStepWindow(hostnameBox);

                    content.DrawString("Example: CMLEON-PC", UITheme.TextSecondary, 12, 108);

                    backButton.Text = "Back";
                    nextButton.Text = "Finish";
                    break;
            }

            wm.Update(window);
        }

        private void BackClicked(int x, int y)
        {
            if (step == 0)
            {
                Sys.Power.Reboot();
                return;
            }

            step--;
            ShowStatus(string.Empty, UITheme.TextSecondary);
            RenderStep();
        }

        private void NextClicked(int x, int y)
        {
            if (step == 1 && !acceptedTerms)
            {
                ShowStatus("Please accept terms to continue.", UITheme.Warning);
                return;
            }

            if (step == 2)
            {
                if (string.IsNullOrWhiteSpace(usernameBox.Text))
                {
                    ShowStatus("Username cannot be empty.", UITheme.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(passwordBox.Text))
                {
                    ShowStatus("Password cannot be empty.", UITheme.Warning);
                    return;
                }
                if (passwordBox.Text != confirmBox.Text)
                {
                    ShowStatus("Passwords do not match.", UITheme.Warning);
                    return;
                }
            }

            if (step == 3)
            {
                string error;
                if (!Kernel.userSystem.CreateInitialAdminFromOobe(usernameBox.Text, passwordBox.Text, hostnameBox.Text, out error))
                {
                    ShowStatus("Setup failed: " + error, Color.FromArgb(198, 72, 72));
                    return;
                }

                ShowStatus("Setup complete. Rebooting...", UITheme.Success);
                Sys.Power.Reboot();
                return;
            }

            step++;
            ShowStatus(string.Empty, UITheme.TextSecondary);
            RenderStep();
        }

        public override void Start()
        {
            base.Start();

            int width = 760;
            int height = 500;
            window = new AppWindow(this, (int)(wm.ScreenWidth / 2 - width / 2), (int)(wm.ScreenHeight / 2 - height / 2), width, height);
            window.Title = "CMLeonOS OOBE";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanClose = false;
            window.CanMove = false;
            window.CanResize = false;
            wm.AddWindow(window);

            backButton = new Button(window, 16, window.Height - 42, 100, 26);
            backButton.Text = "Cancel";
            backButton.OnClick = BackClicked;
            wm.AddWindow(backButton);

            nextButton = new Button(window, window.Width - 116, window.Height - 42, 100, 26);
            nextButton.Text = "Next";
            nextButton.OnClick = NextClicked;
            wm.AddWindow(nextButton);

            statusText = new TextBlock(window, 126, window.Height - 40, window.Width - 252, 20);
            statusText.Text = string.Empty;
            statusText.Background = UITheme.Surface;
            statusText.Foreground = UITheme.TextSecondary;
            wm.AddWindow(statusText);

            RenderStep();
        }

        public override void Run()
        {
        }
    }
}
