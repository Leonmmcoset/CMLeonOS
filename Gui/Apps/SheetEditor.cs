using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace CMLeonOS.Gui.Apps
{
    internal class SheetEditor : Process
    {
        internal SheetEditor() : base("Sheet Editor", ProcessType.Application) { }

        internal SheetEditor(string path) : base("Sheet Editor", ProcessType.Application)
        {
            startupPath = path;
        }

        private const int MaxRows = 200;
        private const int MaxCols = 52;
        private const int ToolbarHeight = 30;
        private const int StatusHeight = 24;
        private const int Padding = 6;
        private const int ButtonWidth = 82;
        private const int RowHeaderWidth = 44;
        private const int ColHeaderHeight = 22;
        private const int CellWidth = 92;
        private const int CellHeight = 22;
        private const string FileMagic = "CMSHEET1";

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private AppWindow window;
        private Window grid;
        private TextBox editorBox;
        private TextBlock cellLabel;
        private StatusBar statusBar;

        private Button newButton;
        private Button openButton;
        private Button saveButton;
        private Button saveAsButton;
        private Button applyButton;
        private Button leftButton;
        private Button rightButton;
        private Button upButton;
        private Button downButton;

        private FileBrowser fileBrowser;

        private readonly string[] cells = new string[MaxRows * MaxCols];
        private int selectedRow = 0;
        private int selectedCol = 0;
        private int startRow = 0;
        private int startCol = 0;
        private string currentPath = null;
        private bool modified = false;
        private readonly string startupPath = null;

        private static int Index(int row, int col)
        {
            return (row * MaxCols) + col;
        }

        private static string Escape(string value)
        {
            string v = value ?? string.Empty;
            v = v.Replace("\\", "\\\\");
            v = v.Replace("\t", "\\t");
            v = v.Replace("\n", "\\n");
            v = v.Replace("\r", string.Empty);
            return v;
        }

        private static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(value.Length);
            bool esc = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!esc)
                {
                    if (c == '\\')
                    {
                        esc = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    switch (c)
                    {
                        case 't': sb.Append('\t'); break;
                        case 'n': sb.Append('\n'); break;
                        case '\\': sb.Append('\\'); break;
                        default: sb.Append(c); break;
                    }
                    esc = false;
                }
            }
            if (esc) sb.Append('\\');
            return sb.ToString();
        }

        private static string[] SplitTabs(string line)
        {
            string[] parts = new string[3];
            int part = 0;
            StringBuilder sb = new StringBuilder();
            bool esc = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (!esc)
                {
                    if (c == '\\')
                    {
                        esc = true;
                        continue;
                    }
                    if (c == '\t' && part < 2)
                    {
                        parts[part++] = sb.ToString();
                        sb.Clear();
                        continue;
                    }
                    sb.Append(c);
                }
                else
                {
                    sb.Append('\\');
                    sb.Append(c);
                    esc = false;
                }
            }
            parts[part] = sb.ToString();
            if (parts[0] == null) parts[0] = string.Empty;
            if (parts[1] == null) parts[1] = string.Empty;
            if (parts[2] == null) parts[2] = string.Empty;
            return parts;
        }

        private void SetStatus(string text)
        {
            statusBar.Text = text;
            statusBar.DetailText = currentPath ?? "Unsaved (.cws)";
            statusBar.Render();
        }

        private void UpdateTitle()
        {
            string baseName = currentPath == null ? "Untitled.cws" : Path.GetFileName(currentPath);
            window.Title = modified ? (baseName + "* - Sheet Editor") : (baseName + " - Sheet Editor");
            cellLabel.Text = CellName(selectedRow, selectedCol);
            cellLabel.Render();
        }

        private static string ColumnName(int col)
        {
            col = Math.Max(0, Math.Min(MaxCols - 1, col));
            if (col < 26)
            {
                return ((char)('A' + col)).ToString();
            }
            return ((char)('A' + ((col / 26) - 1))).ToString() + ((char)('A' + (col % 26))).ToString();
        }

        private static string CellName(int row, int col)
        {
            return ColumnName(col) + (row + 1).ToString();
        }

        private static string ClipText(string text, int maxChars)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            string singleLine = text.Replace('\n', ' ');
            if (singleLine.Length <= maxChars) return singleLine;
            if (maxChars <= 1) return ".";
            return singleLine.Substring(0, maxChars - 1) + ".";
        }

        private int VisibleCols()
        {
            int v = (grid.Width - RowHeaderWidth) / CellWidth;
            return Math.Max(1, v);
        }

        private int VisibleRows()
        {
            int v = (grid.Height - ColHeaderHeight) / CellHeight;
            return Math.Max(1, v);
        }

        private void EnsureSelectionVisible()
        {
            int visCols = VisibleCols();
            int visRows = VisibleRows();
            if (selectedCol < startCol) startCol = selectedCol;
            if (selectedCol >= startCol + visCols) startCol = selectedCol - visCols + 1;
            if (selectedRow < startRow) startRow = selectedRow;
            if (selectedRow >= startRow + visRows) startRow = selectedRow - visRows + 1;
            if (startCol < 0) startCol = 0;
            if (startRow < 0) startRow = 0;
            if (startCol > MaxCols - 1) startCol = MaxCols - 1;
            if (startRow > MaxRows - 1) startRow = MaxRows - 1;
        }

        private void RenderGrid()
        {
            grid.Clear(Color.White);

            int visCols = VisibleCols();
            int visRows = VisibleRows();
            int maxCol = Math.Min(MaxCols, startCol + visCols);
            int maxRow = Math.Min(MaxRows, startRow + visRows);

            grid.DrawFilledRectangle(0, 0, grid.Width, ColHeaderHeight, Color.FromArgb(240, 245, 252));
            grid.DrawFilledRectangle(0, 0, RowHeaderWidth, grid.Height, Color.FromArgb(240, 245, 252));
            grid.DrawRectangle(0, 0, grid.Width, grid.Height, Color.FromArgb(182, 194, 210));

            for (int c = startCol; c < maxCol; c++)
            {
                int x = RowHeaderWidth + ((c - startCol) * CellWidth);
                string label = ColumnName(c);
                grid.DrawRectangle(x, 0, CellWidth, ColHeaderHeight, Color.FromArgb(198, 208, 223));
                grid.DrawString(label, Color.FromArgb(44, 60, 82), x + (CellWidth / 2) - (label.Length * 4), 3);
            }

            for (int r = startRow; r < maxRow; r++)
            {
                int y = ColHeaderHeight + ((r - startRow) * CellHeight);
                string label = (r + 1).ToString();
                grid.DrawRectangle(0, y, RowHeaderWidth, CellHeight, Color.FromArgb(198, 208, 223));
                grid.DrawString(label, Color.FromArgb(44, 60, 82), RowHeaderWidth - 6 - (label.Length * 8), y + 3);
            }

            for (int r = startRow; r < maxRow; r++)
            {
                int y = ColHeaderHeight + ((r - startRow) * CellHeight);
                for (int c = startCol; c < maxCol; c++)
                {
                    int x = RowHeaderWidth + ((c - startCol) * CellWidth);
                    bool selected = (r == selectedRow && c == selectedCol);
                    Color border = selected ? Color.FromArgb(94, 138, 216) : Color.FromArgb(204, 214, 226);
                    Color back = selected ? Color.FromArgb(227, 238, 255) : Color.White;

                    grid.DrawFilledRectangle(x, y, CellWidth, CellHeight, border);
                    grid.DrawFilledRectangle(x + 1, y + 1, CellWidth - 2, CellHeight - 2, back);

                    string cellText = cells[Index(r, c)] ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(cellText))
                    {
                        grid.DrawString(ClipText(cellText, 10), Color.FromArgb(28, 38, 52), x + 4, y + 3);
                    }
                }
            }

            wm.Update(grid);
        }

        private void SelectCell(int row, int col)
        {
            selectedRow = Math.Max(0, Math.Min(MaxRows - 1, row));
            selectedCol = Math.Max(0, Math.Min(MaxCols - 1, col));
            EnsureSelectionVisible();
            editorBox.Text = cells[Index(selectedRow, selectedCol)] ?? string.Empty;
            UpdateTitle();
            RenderGrid();
        }

        private void GridDown(int x, int y)
        {
            if (x < RowHeaderWidth || y < ColHeaderHeight)
            {
                return;
            }

            int col = startCol + ((x - RowHeaderWidth) / CellWidth);
            int row = startRow + ((y - ColHeaderHeight) / CellHeight);
            if (row < 0 || row >= MaxRows || col < 0 || col >= MaxCols)
            {
                return;
            }
            SelectCell(row, col);
        }

        private void ApplyEdit()
        {
            int idx = Index(selectedRow, selectedCol);
            string before = cells[idx] ?? string.Empty;
            string after = editorBox.Text ?? string.Empty;
            if (before == after) return;
            cells[idx] = after;
            modified = true;
            UpdateTitle();
            RenderGrid();
            SetStatus("Cell updated: " + CellName(selectedRow, selectedCol));
        }

        private void ClearSheet()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = string.Empty;
            }
            selectedRow = 0;
            selectedCol = 0;
            startRow = 0;
            startCol = 0;
            editorBox.Text = string.Empty;
            modified = false;
            currentPath = null;
            UpdateTitle();
            RenderGrid();
            SetStatus("New sheet.");
        }

        private void SaveToPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FileMagic);
            sb.AppendLine(MaxRows.ToString() + "\t" + MaxCols.ToString());

            for (int r = 0; r < MaxRows; r++)
            {
                for (int c = 0; c < MaxCols; c++)
                {
                    string val = cells[Index(r, c)] ?? string.Empty;
                    if (val.Length == 0) continue;
                    sb.Append(r.ToString());
                    sb.Append('\t');
                    sb.Append(c.ToString());
                    sb.Append('\t');
                    sb.AppendLine(Escape(val));
                }
            }

            File.WriteAllText(path, sb.ToString());
            currentPath = path;
            modified = false;
            UpdateTitle();
            SetStatus("Saved.");
        }

        private void Save()
        {
            if (currentPath == null)
            {
                SaveAs();
                return;
            }
            SaveToPath(currentPath);
        }

        private void SaveAs()
        {
            fileBrowser = new FileBrowser(this, wm, (selectedPath) =>
            {
                if (string.IsNullOrWhiteSpace(selectedPath)) return;
                SaveToPath(selectedPath);
            }, selectDirectoryOnly: true);
            fileBrowser.Show();
        }

        private void OpenFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                new MessageBox(this, "Sheet Editor", "File does not exist.").Show();
                return;
            }

            string[] lines = File.ReadAllLines(path);
            if (lines.Length < 2 || lines[0] != FileMagic)
            {
                new MessageBox(this, "Sheet Editor", "Unsupported file format.").Show();
                return;
            }

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = string.Empty;
            }

            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = SplitTabs(line);

                if (!int.TryParse(parts[0], out int r)) continue;
                if (!int.TryParse(parts[1], out int c)) continue;
                if (r < 0 || r >= MaxRows || c < 0 || c >= MaxCols) continue;

                cells[Index(r, c)] = Unescape(parts[2]);
            }

            currentPath = path;
            modified = false;
            selectedRow = 0;
            selectedCol = 0;
            startRow = 0;
            startCol = 0;
            editorBox.Text = string.Empty;
            UpdateTitle();
            RenderGrid();
            SetStatus("Opened.");
        }

        private void OpenPrompt()
        {
            fileBrowser = new FileBrowser(this, wm, (selectedPath) =>
            {
                if (string.IsNullOrWhiteSpace(selectedPath)) return;
                OpenFromPath(selectedPath);
            });
            fileBrowser.Show();
        }

        private void Layout()
        {
            int topY = Padding;
            int x = Padding;
            newButton.MoveAndResize(x, topY, ButtonWidth, 22); x += ButtonWidth + 4;
            openButton.MoveAndResize(x, topY, ButtonWidth, 22); x += ButtonWidth + 4;
            saveButton.MoveAndResize(x, topY, ButtonWidth, 22); x += ButtonWidth + 4;
            saveAsButton.MoveAndResize(x, topY, ButtonWidth, 22); x += ButtonWidth + 6;

            cellLabel.MoveAndResize(x, topY + 3, 52, 20); x += 54;
            editorBox.MoveAndResize(x, topY, Math.Max(180, window.Width - x - 210), 22);
            applyButton.MoveAndResize(window.Width - 198, topY, 70, 22);

            leftButton.MoveAndResize(window.Width - 122, topY, 26, 22);
            rightButton.MoveAndResize(window.Width - 92, topY, 26, 22);
            upButton.MoveAndResize(window.Width - 62, topY, 26, 22);
            downButton.MoveAndResize(window.Width - 32, topY, 26, 22);

            int gridY = ToolbarHeight;
            grid.MoveAndResize(0, gridY, window.Width, window.Height - gridY - StatusHeight);
            statusBar.MoveAndResize(0, window.Height - StatusHeight, window.Width, StatusHeight);

            EnsureSelectionVisible();
            RenderGrid();
            statusBar.Render();
            cellLabel.Render();
            editorBox.Render();
            newButton.Render();
            openButton.Render();
            saveButton.Render();
            saveAsButton.Render();
            applyButton.Render();
            leftButton.Render();
            rightButton.Render();
            upButton.Render();
            downButton.Render();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 150, 80, 980, 560);
            window.Title = "Sheet Editor - Untitled.cws";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Layout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            newButton = new Button(window, 0, 0, 1, 1);
            newButton.Text = "New";
            newButton.OnClick = (_, _) => ClearSheet();
            wm.AddWindow(newButton);

            openButton = new Button(window, 0, 0, 1, 1);
            openButton.Text = "Open";
            openButton.OnClick = (_, _) => OpenPrompt();
            wm.AddWindow(openButton);

            saveButton = new Button(window, 0, 0, 1, 1);
            saveButton.Text = "Save";
            saveButton.OnClick = (_, _) => Save();
            wm.AddWindow(saveButton);

            saveAsButton = new Button(window, 0, 0, 1, 1);
            saveAsButton.Text = "Save As";
            saveAsButton.OnClick = (_, _) => SaveAs();
            wm.AddWindow(saveAsButton);

            cellLabel = new TextBlock(window, 0, 0, 1, 1);
            cellLabel.Text = "A1";
            cellLabel.Foreground = UITheme.TextPrimary;
            wm.AddWindow(cellLabel);

            editorBox = new TextBox(window, 0, 0, 1, 1);
            editorBox.PlaceholderText = "Cell value";
            editorBox.Submitted = ApplyEdit;
            wm.AddWindow(editorBox);

            applyButton = new Button(window, 0, 0, 1, 1);
            applyButton.Text = "Apply";
            applyButton.OnClick = (_, _) => ApplyEdit();
            wm.AddWindow(applyButton);

            leftButton = new Button(window, 0, 0, 1, 1);
            leftButton.Text = "<";
            leftButton.OnClick = (_, _) =>
            {
                startCol = Math.Max(0, startCol - 1);
                RenderGrid();
            };
            wm.AddWindow(leftButton);

            rightButton = new Button(window, 0, 0, 1, 1);
            rightButton.Text = ">";
            rightButton.OnClick = (_, _) =>
            {
                startCol = Math.Min(MaxCols - 1, startCol + 1);
                RenderGrid();
            };
            wm.AddWindow(rightButton);

            upButton = new Button(window, 0, 0, 1, 1);
            upButton.Text = "^";
            upButton.OnClick = (_, _) =>
            {
                startRow = Math.Max(0, startRow - 1);
                RenderGrid();
            };
            wm.AddWindow(upButton);

            downButton = new Button(window, 0, 0, 1, 1);
            downButton.Text = "v";
            downButton.OnClick = (_, _) =>
            {
                startRow = Math.Min(MaxRows - 1, startRow + 1);
                RenderGrid();
            };
            wm.AddWindow(downButton);

            grid = new Window(this, window, 0, ToolbarHeight, window.Width, window.Height - ToolbarHeight - StatusHeight);
            grid.OnDown = GridDown;
            wm.AddWindow(grid);

            statusBar = new StatusBar(window, 0, window.Height - StatusHeight, window.Width, StatusHeight);
            statusBar.Text = "Ready";
            statusBar.DetailText = "Unsaved (.cws)";
            wm.AddWindow(statusBar);

            ClearSheet();
            Layout();

            if (!string.IsNullOrWhiteSpace(startupPath))
            {
                OpenFromPath(startupPath);
            }

            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
