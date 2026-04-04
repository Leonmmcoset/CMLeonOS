using Cosmos.System;
using CMLeonOS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class TerminalApp : Process
    {
        internal TerminalApp() : base("Terminal", ProcessType.Application) { }

        private const int Cols = 80;
        private const int Rows = 25;
        private const int CharW = 8;
        private const int CharH = 16;
        private const int Padding = 8;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private UILib.AppWindow window;
        private Window surface;
        private UILib.TextBox inputBox;

        private readonly List<string> lines = new List<string>();
        private string cwd = @"0:\";
        private bool dirty = true;

        private int ContentWidth => Cols * CharW;
        private int ContentHeight => Rows * CharH;

        private void PushLine(string text)
        {
            text = text ?? string.Empty;
            if (text.Length <= Cols)
            {
                lines.Add(text);
                return;
            }

            int i = 0;
            while (i < text.Length)
            {
                int take = Math.Min(Cols, text.Length - i);
                lines.Add(text.Substring(i, take));
                i += take;
            }
        }

        private void PrintPromptLine()
        {
            string user = UserSystem.CurrentLoggedInUser?.Username ?? "user";
            PushLine($"{user}@CMLeonOS {cwd}");
        }

        private void ClearScreen()
        {
            lines.Clear();
            PushLine("CMLeonOS GUI Terminal");
            PushLine("Type 'help' for commands.");
        }

        private void ExecuteCommand(string raw)
        {
            string commandLine = (raw ?? string.Empty).Trim();
            if (commandLine.Length == 0)
            {
                return;
            }

            string[] parts = commandLine.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string cmd = parts[0].ToLower();
            string args = parts.Length > 1 ? parts[1].Trim() : string.Empty;

            try
            {
                switch (cmd)
                {
                    case "help":
                        PushLine("help clear echo time date whoami ver pwd cd ls");
                        break;
                    case "clear":
                    case "cls":
                        ClearScreen();
                        break;
                    case "echo":
                        PushLine(args);
                        break;
                    case "time":
                        PushLine(DateTime.Now.ToString("HH:mm:ss"));
                        break;
                    case "date":
                        PushLine(DateTime.Now.ToString("yyyy-MM-dd"));
                        break;
                    case "whoami":
                        PushLine(UserSystem.CurrentLoggedInUser?.Username ?? "Not logged in");
                        break;
                    case "ver":
                    case "version":
                        PushLine(Kernel.Version);
                        break;
                    case "pwd":
                        PushLine(cwd);
                        break;
                    case "cd":
                        ChangeDirectory(args);
                        break;
                    case "ls":
                        ListDirectory(args);
                        break;
                    default:
                        PushLine("Unknown command: " + cmd);
                        break;
                }
            }
            catch (Exception ex)
            {
                PushLine("Error: " + ex.Message);
            }
        }

        private void ChangeDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                PushLine(cwd);
                return;
            }

            string target = path.Trim();
            string full;

            if (target == ".")
            {
                full = cwd;
            }
            else if (target == "..")
            {
                DirectoryInfo parent = Directory.GetParent(cwd);
                full = parent == null ? cwd : parent.FullName;
            }
            else if (target.StartsWith(@"0:\") || target.StartsWith(@"1:\"))
            {
                full = target;
            }
            else
            {
                full = Path.Combine(cwd, target);
            }

            if (!Directory.Exists(full))
            {
                PushLine("Directory not found: " + full);
                return;
            }

            cwd = full;
            PushLine(cwd);
        }

        private void ListDirectory(string path)
        {
            string target = string.IsNullOrWhiteSpace(path) ? cwd : path.Trim();
            string full;

            if (target.StartsWith(@"0:\") || target.StartsWith(@"1:\"))
            {
                full = target;
            }
            else
            {
                full = Path.Combine(cwd, target);
            }

            if (!Directory.Exists(full))
            {
                PushLine("Directory not found: " + full);
                return;
            }

            string[] dirs = Directory.GetDirectories(full);
            string[] files = Directory.GetFiles(full);

            PushLine("[" + full + "]");
            for (int i = 0; i < dirs.Length; i++)
            {
                PushLine("<DIR> " + Path.GetFileName(dirs[i]));
            }
            for (int i = 0; i < files.Length; i++)
            {
                PushLine("      " + Path.GetFileName(files[i]));
            }
        }

        private void Render()
        {
            surface.Clear(Color.Black);

            int visibleHistoryRows = Rows - 1;
            int start = Math.Max(0, lines.Count - visibleHistoryRows);
            for (int i = 0; i < visibleHistoryRows; i++)
            {
                int idx = start + i;
                if (idx >= lines.Count)
                {
                    break;
                }

                string text = lines[idx];
                if (text.Length > Cols)
                {
                    text = text.Substring(0, Cols);
                }
                surface.DrawString(text, Color.White, 0, i * CharH);
            }

            wm.Update(surface);
            dirty = false;
        }

        public override void Start()
        {
            base.Start();

            int width = ContentWidth + (Padding * 2);
            int inputHeight = 24;
            int height = ContentHeight + (Padding * 3) + inputHeight + 24;
            window = new UILib.AppWindow(this, 120, 60, width, height);
            window.Title = "Terminal 80x25";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = false;
            window.Closing = TryStop;
            wm.AddWindow(window);

            surface = new Window(this, window, Padding, Padding, ContentWidth, ContentHeight);
            wm.AddWindow(surface);

            inputBox = new UILib.TextBox(window, Padding, Padding + ContentHeight + Padding, ContentWidth, inputHeight);
            inputBox.PlaceholderText = "Enter command and press Enter...";
            inputBox.Submitted = () =>
            {
                string command = (inputBox.Text ?? string.Empty).Trim();
                if (command.Length > 0)
                {
                    PushLine("> " + command);
                    ExecuteCommand(command);
                }
                inputBox.Text = string.Empty;
                dirty = true;
            };
            wm.AddWindow(inputBox);

            wm.BringToFrontAndFocus(window);
            wm.SetFocus(inputBox);

            ClearScreen();
            PrintPromptLine();
            dirty = true;
            Render();
        }

        public override void Run()
        {
            if (dirty)
            {
                Render();
            }
        }
    }
}
