using CMLeonOS;
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using CMLeonOS.Utils;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class Notepad : Process
    {
        internal Notepad() : base("Notepad", ProcessType.Application) { }

        internal Notepad(string path) : base("Notepad", ProcessType.Application)
        {
            this.path = path;
        }

        AppWindow window;

        WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        SettingsService settingsService = ProcessManager.GetProcess<SettingsService>();
        TextBox textBox;

        ShortcutBar shortcutBar;

        FileBrowser fileBrowser;

        private string? path = null;

        private bool modified = false;

        private void TextChanged()
        {
            modified = true;

            UpdateTitle();
        }

        private void WindowResized()
        {
            textBox.Resize(window.Width, window.Height - 20);
            shortcutBar.Resize(window.Width, 20);

            shortcutBar.Render();

            textBox.MarkAllLines();
            textBox.Render();
        }

        private void UpdateTitle()
        {
            if (path == null)
            {
                window.Title = "Untitled - Notepad";
                return;
            }

            if (modified)
            {
                window.Title = $"{Path.GetFileName(path)}* - Notepad";
            }
            else
            {
                window.Title = $"{Path.GetFileName(path)} - Notepad";
            }
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null) return;

            if (readFile && !File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(this, "Notepad", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
            }

            path = newPath;

            if (readFile)
            {
                textBox.Text = File.ReadAllText(path);

                textBox.MarkAllLines();
                textBox.Render();

                modified = false;
            }

            UpdateTitle();
        }

        private void OpenFilePrompt()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (selectedPath != null)
                {
                    Open(selectedPath);
                }
            });
            fileBrowser.Show();
        }

        private void SaveAsPrompt()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (selectedPath != null)
                {
                    path = selectedPath;
                    Save();
                }
            }, selectDirectoryOnly: true);
            fileBrowser.Show();
        }

        private void Save()
        {
            if (path == null)
            {
                SaveAsPrompt();
                return;
            }

            File.WriteAllText(path, textBox.Text);

            modified = false;
            UpdateTitle();
        }

        private void ApplyTheme()
        {
            if (settingsService.DarkNotepad)
            {
                textBox.Background = Color.FromArgb(24, 24, 30);
                textBox.Foreground = Color.White;

                shortcutBar.Background = Color.FromArgb(56, 56, 71);
                shortcutBar.Foreground = Color.White;
            }
            else
            {
                textBox.Background = Color.White;
                textBox.Foreground = Color.Black;

                shortcutBar.Background = Color.LightGray;
                shortcutBar.Foreground = Color.Black;
            }

            textBox.MarkAllLines();
            textBox.Render();
        }

        private void OpenViewSettings()
        {
            AppWindow settingsWindow = new AppWindow(this, 320, 264, 256, 192);
            settingsWindow.DrawString("Notepad Settings", Color.DarkBlue, 12, 12);
            settingsWindow.DrawString($"Notepad v{Kernel.Version}", Color.DarkGray, 12, settingsWindow.Height - 12 - FontData.Height);
            wm.AddWindow(settingsWindow);
            settingsWindow.Title = "Notepad";

            Switch darkSwitch = new Switch(settingsWindow, 12, 40, settingsWindow.Width - 16, 20);
            darkSwitch.Text = "Dark theme";
            darkSwitch.Checked = settingsService.DarkNotepad;
            darkSwitch.CheckBoxChanged = (bool @checked) =>
            {
                settingsService.DarkNotepad = @checked;
                ApplyTheme();
            };
            wm.AddWindow(darkSwitch);

            wm.Update(settingsWindow);
        }

        public override void Start()
        {
            base.Start();
            window = new AppWindow(this, 320, 264, 384, 240);
            wm.AddWindow(window);
            UpdateTitle();
            window.Closing = TryStop;
            window.Icon = AppManager.GetAppMetadata("Notepad").Icon;
            window.CanResize = true;
            window.UserResized = WindowResized;

            shortcutBar = new ShortcutBar(window, 0, 0, window.Width, 20);
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", OpenFilePrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAsPrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("View", OpenViewSettings));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            textBox = new TextBox(window, 0, 20, window.Width, window.Height - 20);
            textBox.MultiLine = true;
            textBox.Changed = TextChanged;
            wm.AddWindow(textBox);

            ApplyTheme();

            Open(path);

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
