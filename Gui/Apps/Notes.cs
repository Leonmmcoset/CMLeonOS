using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace CMLeonOS.Gui.Apps
{
    internal class Notes : Process
    {
        internal Notes() : base("Notes", ProcessType.Application) { }

        private sealed class NoteItem
        {
            internal string Title = "Untitled";
            internal string Content = string.Empty;
            internal DateTime UpdatedAt = DateTime.Now;
        }

        private const string DataPath = @"0:\notes.dat";
        private const int ToolbarHeight = 34;
        private const int HeaderHeight = 40;
        private const int Padding = 8;
        private const int ButtonWidth = 84;
        private const int ButtonHeight = 24;
        private const int TitleRowHeight = 24;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly List<NoteItem> notes = new List<NoteItem>();

        private AppWindow window;
        private Window header;

        private Button newButton;
        private Button saveButton;
        private Button deleteButton;
        private Button reloadButton;

        private Table noteTable;
        private TextBox titleBox;
        private TextBox contentBox;

        private TextBlock titleLabel;
        private TextBlock contentLabel;

        private string status = @"Store: 0:\notes.dat";

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
            bool escaped = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!escaped)
                {
                    if (c == '\\')
                    {
                        escaped = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    continue;
                }

                switch (c)
                {
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case '\\':
                        sb.Append('\\');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
                escaped = false;
            }

            if (escaped)
            {
                sb.Append('\\');
            }

            return sb.ToString();
        }

        private static string[] SplitEscapedTabs(string line)
        {
            List<string> parts = new List<string>();
            StringBuilder current = new StringBuilder();
            bool escaped = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (escaped)
                {
                    current.Append('\\');
                    current.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (c == '\t')
                {
                    parts.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                current.Append(c);
            }

            if (escaped)
            {
                current.Append('\\');
            }

            parts.Add(current.ToString());
            return parts.ToArray();
        }

        private void SetStatus(string text)
        {
            status = text ?? string.Empty;
            RenderHeader();
        }

        private void RenderHeader()
        {
            header.Clear(Color.FromArgb(244, 246, 250));
            header.DrawRectangle(0, 0, header.Width, header.Height, Color.FromArgb(186, 194, 206));
            header.DrawString("Notes", Color.FromArgb(40, 49, 64), 12, 6);
            header.DrawString(status, Color.FromArgb(108, 117, 131), 12, 22);
            wm.Update(header);
        }

        private void LoadNotes()
        {
            notes.Clear();

            if (!File.Exists(DataPath))
            {
                SetStatus(@"No notes file yet. Save to create 0:\notes.dat");
                return;
            }

            string[] lines = File.ReadAllLines(DataPath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = SplitEscapedTabs(line);
                NoteItem item = new NoteItem();
                item.Title = parts.Length > 0 ? Unescape(parts[0]) : "Untitled";
                item.Content = parts.Length > 1 ? Unescape(parts[1]) : string.Empty;

                long ticks = 0;
                if (parts.Length > 2 && long.TryParse(parts[2], out ticks))
                {
                    item.UpdatedAt = new DateTime(ticks);
                }
                else
                {
                    item.UpdatedAt = DateTime.Now;
                }

                if (string.IsNullOrWhiteSpace(item.Title))
                {
                    item.Title = "Untitled";
                }

                notes.Add(item);
            }

            SetStatus($"Loaded {notes.Count} note(s)");
        }

        private void SaveNotes()
        {
            string[] lines = new string[notes.Count];
            for (int i = 0; i < notes.Count; i++)
            {
                NoteItem note = notes[i];
                lines[i] = string.Join("\t", new string[]
                {
                    Escape(note.Title),
                    Escape(note.Content),
                    note.UpdatedAt.Ticks.ToString()
                });
            }

            File.WriteAllLines(DataPath, lines);
        }

        private string BuildListLabel(NoteItem note)
        {
            string title = string.IsNullOrWhiteSpace(note.Title) ? "Untitled" : note.Title.Trim();
            if (title.Length > 22)
            {
                title = title.Substring(0, 22) + "...";
            }

            string date = note.UpdatedAt.ToString("MM-dd HH:mm");
            return $"{title}  {date}";
        }

        private void PopulateList(int preferredSelection)
        {
            noteTable.Cells.Clear();
            noteTable.SelectedCellIndex = -1;

            for (int i = 0; i < notes.Count; i++)
            {
                noteTable.Cells.Add(new TableCell(BuildListLabel(notes[i]), i));
            }

            if (preferredSelection >= 0 && preferredSelection < notes.Count)
            {
                noteTable.SelectedCellIndex = preferredSelection;
            }
            else if (notes.Count > 0)
            {
                noteTable.SelectedCellIndex = 0;
            }

            noteTable.Render();
            LoadCurrentSelection();
        }

        private void LoadCurrentSelection()
        {
            int idx = noteTable.SelectedCellIndex;
            if (idx < 0 || idx >= notes.Count)
            {
                titleBox.Text = string.Empty;
                contentBox.Text = string.Empty;
                SetStatus($"Loaded {notes.Count} note(s)");
                return;
            }

            NoteItem selected = notes[idx];
            titleBox.Text = selected.Title;
            contentBox.Text = selected.Content;
            SetStatus($"Selected: {selected.Title}");
        }

        private void NewClicked(int x, int y)
        {
            noteTable.SelectedCellIndex = -1;
            titleBox.Text = "Untitled";
            contentBox.Text = string.Empty;
            SetStatus("Create mode: edit and click Save");
            titleBox.Render();
            contentBox.Render();
        }

        private void SaveClicked(int x, int y)
        {
            string title = (titleBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Untitled";
            }

            string content = contentBox.Text ?? string.Empty;
            int idx = noteTable.SelectedCellIndex;
            if (idx >= 0 && idx < notes.Count)
            {
                notes[idx].Title = title;
                notes[idx].Content = content;
                notes[idx].UpdatedAt = DateTime.Now;
                SaveNotes();
                PopulateList(idx);
                SetStatus($"Updated: {title}");
                return;
            }

            NoteItem item = new NoteItem
            {
                Title = title,
                Content = content,
                UpdatedAt = DateTime.Now
            };
            notes.Insert(0, item);
            SaveNotes();
            PopulateList(0);
            SetStatus($"Created: {title}");
        }

        private void DeleteClicked(int x, int y)
        {
            int idx = noteTable.SelectedCellIndex;
            if (idx < 0 || idx >= notes.Count)
            {
                SetStatus("Select a note first");
                return;
            }

            string title = notes[idx].Title;
            notes.RemoveAt(idx);
            SaveNotes();

            int next = idx;
            if (next >= notes.Count)
            {
                next = notes.Count - 1;
            }
            PopulateList(next);
            SetStatus($"Deleted: {title}");
        }

        private void ReloadClicked(int x, int y)
        {
            LoadNotes();
            PopulateList(-1);
        }

        private void Relayout()
        {
            int right = window.Width - Padding;
            int topY = Padding;

            newButton.MoveAndResize(right - ((ButtonWidth * 4) + (Padding * 3)), topY, ButtonWidth, ButtonHeight);
            saveButton.MoveAndResize(right - ((ButtonWidth * 3) + (Padding * 2)), topY, ButtonWidth, ButtonHeight);
            deleteButton.MoveAndResize(right - ((ButtonWidth * 2) + Padding), topY, ButtonWidth, ButtonHeight);
            reloadButton.MoveAndResize(right - ButtonWidth, topY, ButtonWidth, ButtonHeight);

            header.MoveAndResize(0, ToolbarHeight, window.Width, HeaderHeight);

            int contentY = ToolbarHeight + HeaderHeight + Padding;
            int contentHeight = window.Height - contentY - Padding;
            int listWidth = Math.Max(220, (window.Width * 38) / 100);
            int editorX = listWidth + (Padding * 2);
            int editorWidth = window.Width - editorX - Padding;

            noteTable.MoveAndResize(Padding, contentY, listWidth - Padding, contentHeight);

            titleLabel.MoveAndResize(editorX, contentY + 2, 48, 20);
            titleBox.MoveAndResize(editorX + 48, contentY, editorWidth - 48, TitleRowHeight);

            int bodyY = contentY + TitleRowHeight + Padding;
            contentLabel.MoveAndResize(editorX, bodyY + 2, 60, 20);
            contentBox.MoveAndResize(editorX, bodyY + 22, editorWidth, Math.Max(80, contentY + contentHeight - (bodyY + 22)));

            newButton.Render();
            saveButton.Render();
            deleteButton.Render();
            reloadButton.Render();
            RenderHeader();
            noteTable.Render();
            titleLabel.Render();
            contentLabel.Render();
            titleBox.Render();
            contentBox.Render();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 180, 96, 930, 540);
            window.Title = "Notes";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Relayout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            newButton = new Button(window, 0, 0, 1, 1);
            newButton.Text = "New";
            newButton.OnClick = NewClicked;
            wm.AddWindow(newButton);

            saveButton = new Button(window, 0, 0, 1, 1);
            saveButton.Text = "Save";
            saveButton.OnClick = SaveClicked;
            wm.AddWindow(saveButton);

            deleteButton = new Button(window, 0, 0, 1, 1);
            deleteButton.Text = "Delete";
            deleteButton.OnClick = DeleteClicked;
            wm.AddWindow(deleteButton);

            reloadButton = new Button(window, 0, 0, 1, 1);
            reloadButton.Text = "Reload";
            reloadButton.OnClick = ReloadClicked;
            wm.AddWindow(reloadButton);

            header = new Window(this, window, 0, ToolbarHeight, window.Width, HeaderHeight);
            wm.AddWindow(header);

            noteTable = new Table(window, 0, 0, 1, 1);
            noteTable.CellHeight = 24;
            noteTable.Background = Color.White;
            noteTable.Foreground = Color.Black;
            noteTable.Border = Color.FromArgb(185, 194, 207);
            noteTable.SelectedBackground = Color.FromArgb(255, 240, 210);
            noteTable.SelectedBorder = Color.FromArgb(223, 164, 70);
            noteTable.SelectedForeground = Color.Black;
            noteTable.TableCellSelected = _ => LoadCurrentSelection();
            wm.AddWindow(noteTable);

            titleLabel = new TextBlock(window, 0, 0, 1, 1);
            titleLabel.Text = "Title";
            titleLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(titleLabel);

            contentLabel = new TextBlock(window, 0, 0, 1, 1);
            contentLabel.Text = "Content";
            contentLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(contentLabel);

            titleBox = new TextBox(window, 0, 0, 1, 1);
            wm.AddWindow(titleBox);

            contentBox = new TextBox(window, 0, 0, 1, 1);
            contentBox.MultiLine = true;
            wm.AddWindow(contentBox);

            LoadNotes();
            PopulateList(-1);
            Relayout();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
