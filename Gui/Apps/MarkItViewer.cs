using CMLeonOS;
using CMLeonOS.Gui.SmoothMono;
using CMLeonOS.Gui.UILib;
using CMLeonOS.Logger;
using CMLeonOS.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class MarkItViewer : Process
    {
        internal MarkItViewer() : base("MarkIt Viewer", ProcessType.Application) { }

        internal MarkItViewer(string path) : base("MarkIt Viewer", ProcessType.Application)
        {
            initialPath = path;
        }

        private struct StyledGlyph
        {
            internal char Character;
            internal Color Foreground;
            internal Color Background;
            internal bool HasBackground;
        }

        private class StyledLine
        {
            internal List<StyledGlyph> Glyphs { get; } = new List<StyledGlyph>();
        }

        private AppWindow window;
        private Window statusHeader;
        private Window contentView;
        private Button openButton;
        private Button upButton;
        private Button downButton;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private FileBrowser fileBrowser;

        private readonly List<StyledLine> lines = new List<StyledLine>();
        private int scrollLine = 0;
        private string currentPath = string.Empty;
        private string initialPath = string.Empty;
        private string statusText = "Open a MarkIt file to preview.";

        private const int toolbarHeight = 32;
        private const int headerHeight = 50;
        private const int padding = 8;
        private const int buttonWidth = 64;
        private const int buttonHeight = 24;
        private const int contentInset = 8;

        private static bool MatchesAt(string text, int index, string token)
        {
            if (index < 0 || token == null || index + token.Length > text.Length)
            {
                return false;
            }

            for (int i = 0; i < token.Length; i++)
            {
                if (text[index + i] != token[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static Color ParseColor(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                return UITheme.TextPrimary;
            }

            string name = colorName.ToLower().Trim();
            switch (name)
            {
                case "black": return Color.Black;
                case "darkblue": return Color.DarkBlue;
                case "darkgreen": return Color.DarkGreen;
                case "darkcyan": return Color.DarkCyan;
                case "darkred": return Color.DarkRed;
                case "darkmagenta": return Color.DarkMagenta;
                case "darkyellow": return Color.FromArgb(112, 96, 0);
                case "gray": return Color.Gray;
                case "darkgray": return Color.DarkGray;
                case "blue": return Color.Blue;
                case "green": return Color.Green;
                case "cyan": return Color.Cyan;
                case "red": return Color.Red;
                case "magenta": return Color.Magenta;
                case "yellow": return Color.Yellow;
                case "white": return Color.White;
                default: return UITheme.TextPrimary;
            }
        }

        private void SetStatus(string text)
        {
            statusText = text;
            RenderHeader();
        }

        private void RenderHeader()
        {
            statusHeader.Clear(Color.FromArgb(235, 241, 248));
            statusHeader.DrawRectangle(0, 0, statusHeader.Width, statusHeader.Height, Color.FromArgb(180, 192, 208));
            statusHeader.DrawString("MarkIt Viewer", Color.FromArgb(28, 38, 52), 12, 8);
            statusHeader.DrawString(statusText, Color.FromArgb(97, 110, 126), 12, 26);
            wm.Update(statusHeader);
        }

        private void Relayout()
        {
            openButton.MoveAndResize(window.Width - ((buttonWidth * 3) + (padding * 3)), padding, buttonWidth, buttonHeight);
            upButton.MoveAndResize(window.Width - ((buttonWidth * 2) + (padding * 2)), padding, buttonWidth, buttonHeight);
            downButton.MoveAndResize(window.Width - (buttonWidth + padding), padding, buttonWidth, buttonHeight);

            int contentY = toolbarHeight + headerHeight;
            statusHeader.MoveAndResize(0, toolbarHeight, window.Width, headerHeight);
            contentView.MoveAndResize(0, contentY, window.Width, window.Height - contentY);

            openButton.Render();
            upButton.Render();
            downButton.Render();
            RenderHeader();
            RenderContent();
        }

        private void ResetDocument()
        {
            lines.Clear();
            lines.Add(new StyledLine());
            scrollLine = 0;
            currentPath = string.Empty;
            window.Title = "MarkIt Viewer";
            SetStatus("Open a MarkIt file to preview.");
            RenderContent();
        }

        private void ParseDocument(string content)
        {
            lines.Clear();
            lines.Add(new StyledLine());

            Color currentForeground = UITheme.TextPrimary;
            Color currentBackground = UITheme.Surface;
            bool hasBackground = false;

            int i = 0;
            while (i < content.Length)
            {
                if (MatchesAt(content, i, "{/color}"))
                {
                    currentForeground = UITheme.TextPrimary;
                    i += 8;
                    continue;
                }

                if (MatchesAt(content, i, "{/bg}"))
                {
                    hasBackground = false;
                    i += 5;
                    continue;
                }

                if (MatchesAt(content, i, "{color:"))
                {
                    int end = content.IndexOf('}', i);
                    if (end > i + 7)
                    {
                        currentForeground = ParseColor(content.Substring(i + 7, end - (i + 7)));
                        i = end + 1;
                        continue;
                    }
                }

                if (MatchesAt(content, i, "{bg:"))
                {
                    int end = content.IndexOf('}', i);
                    if (end > i + 4)
                    {
                        currentBackground = ParseColor(content.Substring(i + 4, end - (i + 4)));
                        hasBackground = true;
                        i = end + 1;
                        continue;
                    }
                }

                char ch = content[i];
                if (ch == '\r')
                {
                    i++;
                    continue;
                }

                if (ch == '\n')
                {
                    lines.Add(new StyledLine());
                    i++;
                    continue;
                }

                lines[lines.Count - 1].Glyphs.Add(new StyledGlyph
                {
                    Character = ch,
                    Foreground = currentForeground,
                    Background = currentBackground,
                    HasBackground = hasBackground
                });
                i++;
            }

            if (lines.Count == 0)
            {
                lines.Add(new StyledLine());
            }
        }

        private int VisibleLineCount()
        {
            int visible = (contentView.Height - (contentInset * 2)) / FontData.Height;
            return Math.Max(1, visible);
        }

        private int VisibleColumnCount()
        {
            int visible = (contentView.Width - (contentInset * 2)) / FontData.Width;
            return Math.Max(1, visible);
        }

        private void ClampScroll()
        {
            int maxScroll = Math.Max(0, lines.Count - VisibleLineCount());
            if (scrollLine < 0)
            {
                scrollLine = 0;
            }
            if (scrollLine > maxScroll)
            {
                scrollLine = maxScroll;
            }
        }

        private void RenderContent()
        {
            contentView.Clear(Color.FromArgb(252, 254, 255));
            contentView.DrawRectangle(0, 0, contentView.Width, contentView.Height, Color.FromArgb(192, 204, 221));

            ClampScroll();

            int visibleLines = VisibleLineCount();
            int visibleColumns = VisibleColumnCount();
            int startLine = scrollLine;
            int endLineExclusive = Math.Min(lines.Count, startLine + visibleLines);

            int drawY = contentInset;
            for (int lineIndex = startLine; lineIndex < endLineExclusive; lineIndex++)
            {
                StyledLine line = lines[lineIndex];
                int drawX = contentInset;
                int maxColumns = Math.Min(visibleColumns, line.Glyphs.Count);

                for (int i = 0; i < maxColumns; i++)
                {
                    StyledGlyph glyph = line.Glyphs[i];
                    if (glyph.HasBackground)
                    {
                        contentView.DrawFilledRectangle(drawX, drawY, FontData.Width, FontData.Height, glyph.Background);
                    }
                    contentView.DrawString(glyph.Character.ToString(), glyph.Foreground, drawX, drawY);
                    drawX += FontData.Width;
                }

                drawY += FontData.Height;
            }

            wm.Update(contentView);
        }

        private void UpdateStatusWithPosition()
        {
            if (lines.Count == 0)
            {
                SetStatus("No content.");
                return;
            }

            int visible = VisibleLineCount();
            int start = Math.Min(lines.Count, scrollLine + 1);
            int end = Math.Min(lines.Count, scrollLine + visible);
            if (string.IsNullOrWhiteSpace(currentPath))
            {
                SetStatus($"Lines {start}-{end} / {lines.Count}");
                return;
            }

            SetStatus($"{Path.GetFileName(currentPath)}  |  Lines {start}-{end} / {lines.Count}");
        }

        private void ScrollUp(int x, int y)
        {
            scrollLine--;
            ClampScroll();
            RenderContent();
            UpdateStatusWithPosition();
        }

        private void ScrollDown(int x, int y)
        {
            scrollLine++;
            ClampScroll();
            RenderContent();
            UpdateStatusWithPosition();
        }

        private bool LoadFile(string path, bool showPopup = true)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                if (showPopup)
                {
                    new MessageBox(this, "MarkIt Viewer", "Path is empty.").Show();
                }
                return false;
            }

            string sanitizedPath = PathUtil.Sanitize(path.Trim());
            if (!File.Exists(sanitizedPath))
            {
                if (showPopup)
                {
                    new MessageBox(this, "MarkIt Viewer", "File not found.").Show();
                }
                return false;
            }

            try
            {
                string content = File.ReadAllText(sanitizedPath);
                ParseDocument(content);
                scrollLine = 0;
                currentPath = sanitizedPath;
                window.Title = $"MarkIt Viewer - {Path.GetFileName(sanitizedPath)}";
                RenderContent();
                UpdateStatusWithPosition();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Logger.Instance.Error("MarkItViewer", $"Failed to open file: {ex.Message}");
                if (showPopup)
                {
                    new MessageBox(this, "MarkIt Viewer", "Unable to open this file.").Show();
                }
                return false;
            }
        }

        private void OpenDialog()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    LoadFile(selectedPath);
                }
            });
            fileBrowser.Show();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 180, 110, 760, 500);
            window.Title = "MarkIt Viewer";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Relayout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            openButton = new Button(window, 0, 0, 1, 1);
            openButton.Text = "Open";
            openButton.OnClick = (_, _) => OpenDialog();
            wm.AddWindow(openButton);

            upButton = new Button(window, 0, 0, 1, 1);
            upButton.Text = "Up";
            upButton.OnClick = ScrollUp;
            wm.AddWindow(upButton);

            downButton = new Button(window, 0, 0, 1, 1);
            downButton.Text = "Down";
            downButton.OnClick = ScrollDown;
            wm.AddWindow(downButton);

            statusHeader = new Window(this, window, 0, toolbarHeight, 1, 1);
            wm.AddWindow(statusHeader);

            contentView = new Window(this, window, 0, toolbarHeight + headerHeight, 1, 1);
            wm.AddWindow(contentView);

            ResetDocument();
            Relayout();

            if (!string.IsNullOrWhiteSpace(initialPath))
            {
                LoadFile(initialPath, showPopup: false);
            }

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
