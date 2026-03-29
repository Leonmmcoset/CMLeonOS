using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class MusicEditor : Process
    {
        internal MusicEditor() : base("Music Editor", ProcessType.Application) { }
        internal MusicEditor(string openPath) : base("Music Editor", ProcessType.Application)
        {
            path = openPath;
        }

        private class NoteEvent
        {
            public int Tick;
            public string Note = "C4";
            public int DurationMs = 200;
        }

        private const string FileMagic = "MUS1";
        private const int DefaultTickMs = 200;

        private AppWindow window;
        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private ShortcutBar shortcutBar;
        private Table notesTable;
        private NumericUpDown tickInput;
        private NumericUpDown durationInput;
        private Dropdown noteDropdown;
        private TextBox statusText;

        private FileBrowser fileBrowser;
        private string path;
        private bool modified;

        private readonly List<NoteEvent> notes = new List<NoteEvent>();
        private readonly List<NoteEvent> playback = new List<NoteEvent>();
        private bool playing;
        private DateTime playbackStart;
        private int playIndex;

        private readonly string[] noteNames = new[]
        {
            "C3","D3","E3","F3","G3","A3","B3",
            "C4","D4","E4","F4","G4","A4","B4",
            "C5","D5","E5","F5","G5","A5","B5"
        };

        private void UpdateTitle()
        {
            string name = string.IsNullOrWhiteSpace(path) ? "Untitled.mus" : Path.GetFileName(path);
            window.Title = modified ? $"{name}* - Music Editor" : $"{name} - Music Editor";
        }

        private void UpdateStatus(string text)
        {
            statusText.Text = text;
            statusText.Render();
        }

        private void RefreshTable()
        {
            notesTable.Cells.Clear();
            for (int i = 0; i < notes.Count; i++)
            {
                NoteEvent n = notes[i];
                notesTable.Cells.Add(new TableCell($"{i:D2} | t={n.Tick} | {n.Note} | {n.DurationMs}ms", i));
            }
            notesTable.Render();
            wm.Update(notesTable);
        }

        private void MarkModified()
        {
            modified = true;
            UpdateTitle();
        }

        private void NewProject()
        {
            StopPlayback();
            notes.Clear();
            path = null;
            modified = false;
            RefreshTable();
            UpdateStatus("New project.");
            UpdateTitle();
        }

        private void AddNote()
        {
            NoteEvent n = new NoteEvent
            {
                Tick = tickInput.Value,
                DurationMs = durationInput.Value,
                Note = noteDropdown.SelectedIndex >= 0 ? noteDropdown.SelectedText : "C4"
            };

            notes.Add(n);
            SortNotesByTick(notes);
            RefreshTable();
            MarkModified();
            UpdateStatus($"Added {n.Note} at tick {n.Tick}.");
        }

        private void RemoveSelected()
        {
            int idx = notesTable.SelectedCellIndex;
            if (idx < 0 || idx >= notes.Count)
            {
                UpdateStatus("No note selected.");
                return;
            }

            notes.RemoveAt(idx);
            RefreshTable();
            MarkModified();
            UpdateStatus("Removed selected note.");
        }

        private void SaveToPath(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                UpdateStatus("Invalid path.");
                return;
            }

            List<string> lines = new List<string>();
            lines.Add(FileMagic);
            lines.Add($"TICK_MS={DefaultTickMs}");
            for (int i = 0; i < notes.Count; i++)
            {
                NoteEvent n = notes[i];
                lines.Add($"{n.Tick}|{n.Note}|{n.DurationMs}");
            }

            File.WriteAllLines(targetPath, lines.ToArray());
            path = targetPath;
            modified = false;
            UpdateTitle();
            UpdateStatus($"Saved: {Path.GetFileName(targetPath)}");
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                SaveAs();
                return;
            }
            SaveToPath(path);
        }

        private void SaveAs()
        {
            fileBrowser = new FileBrowser(this, wm, (selectedPath) =>
            {
                if (string.IsNullOrWhiteSpace(selectedPath))
                {
                    return;
                }

                string finalPath = selectedPath;
                if (!finalPath.EndsWith(".mus"))
                {
                    finalPath += ".mus";
                }
                SaveToPath(finalPath);
            }, selectDirectoryOnly: true);
            fileBrowser.Show();
        }

        private void OpenFromPath(string openPath)
        {
            if (!File.Exists(openPath))
            {
                new MessageBox(this, "Music Editor", "File does not exist.").Show();
                return;
            }

            string[] lines = File.ReadAllLines(openPath);
            if (lines.Length == 0 || lines[0].Trim() != FileMagic)
            {
                new MessageBox(this, "Music Editor", "Invalid .mus format.").Show();
                return;
            }

            StopPlayback();
            notes.Clear();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("TICK_MS="))
                {
                    continue;
                }

                string[] parts = line.Split('|');
                if (parts.Length < 3)
                {
                    continue;
                }

                if (!int.TryParse(parts[0], out int tick))
                {
                    continue;
                }
                if (!int.TryParse(parts[2], out int duration))
                {
                    continue;
                }

                if (duration < 10)
                {
                    duration = 10;
                }

                notes.Add(new NoteEvent
                {
                    Tick = tick,
                    Note = parts[1],
                    DurationMs = duration
                });
            }

            SortNotesByTick(notes);
            path = openPath;
            modified = false;
            RefreshTable();
            UpdateTitle();
            UpdateStatus($"Opened: {Path.GetFileName(openPath)}");
        }

        private void Open()
        {
            fileBrowser = new FileBrowser(this, wm, (selectedPath) =>
            {
                if (string.IsNullOrWhiteSpace(selectedPath))
                {
                    return;
                }

                OpenFromPath(selectedPath);
            });
            fileBrowser.Show();
        }

        private int GetNoteFrequency(string note)
        {
            switch ((note ?? string.Empty).Trim().ToUpper())
            {
                case "C3": return 131;
                case "D3": return 147;
                case "E3": return 165;
                case "F3": return 175;
                case "G3": return 196;
                case "A3": return 220;
                case "B3": return 247;
                case "C4": return 262;
                case "D4": return 294;
                case "E4": return 330;
                case "F4": return 349;
                case "G4": return 392;
                case "A4": return 440;
                case "B4": return 494;
                case "C5": return 523;
                case "D5": return 587;
                case "E5": return 659;
                case "F5": return 698;
                case "G5": return 784;
                case "A5": return 880;
                case "B5": return 988;
                default: return 440;
            }
        }

        private void Play()
        {
            if (notes.Count == 0)
            {
                UpdateStatus("No notes to play.");
                return;
            }

            playback.Clear();
            for (int i = 0; i < notes.Count; i++)
            {
                playback.Add(new NoteEvent
                {
                    Tick = notes[i].Tick,
                    Note = notes[i].Note,
                    DurationMs = notes[i].DurationMs
                });
            }
            SortNotesByTick(playback);

            playIndex = 0;
            playbackStart = DateTime.Now;
            playing = true;
            UpdateStatus("Playing...");
        }

        private void StopPlayback()
        {
            if (!playing)
            {
                return;
            }

            playing = false;
            playIndex = 0;
            playback.Clear();
            UpdateStatus("Stopped.");
        }

        private void Layout()
        {
            int topBarHeight = 24;
            int controlY = topBarHeight + 8;
            int rowHeight = 26;

            shortcutBar.MoveAndResize(0, 0, window.Width, topBarHeight);
            shortcutBar.Render();

            tickInput.MoveAndResize(8, controlY, 100, rowHeight);
            noteDropdown.MoveAndResize(116, controlY, 100, rowHeight);
            durationInput.MoveAndResize(224, controlY, 110, rowHeight);

            int tableY = controlY + rowHeight + 8;
            int statusHeight = 24;
            notesTable.MoveAndResize(8, tableY, window.Width - 16, window.Height - tableY - statusHeight - 10);
            notesTable.Render();

            statusText.MoveAndResize(8, window.Height - statusHeight - 6, window.Width - 16, statusHeight);
            statusText.Render();
        }

        private void SortNotesByTick(List<NoteEvent> list)
        {
            // Avoid List<T>.Sort for IL2CPU compatibility.
            for (int i = 1; i < list.Count; i++)
            {
                NoteEvent key = list[i];
                int j = i - 1;
                while (j >= 0 && list[j].Tick > key.Tick)
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = key;
            }
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 180, 100, 760, 460);
            window.Title = "Untitled.mus - Music Editor";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Layout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            shortcutBar = new ShortcutBar(window, 0, 0, window.Width, 24);
            shortcutBar.Cells.Add(new ShortcutBarCell("New", NewProject));
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", Open));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAs));
            shortcutBar.Cells.Add(new ShortcutBarCell("Add Note", AddNote));
            shortcutBar.Cells.Add(new ShortcutBarCell("Remove", RemoveSelected));
            shortcutBar.Cells.Add(new ShortcutBarCell("Play", Play));
            shortcutBar.Cells.Add(new ShortcutBarCell("Stop", StopPlayback));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            tickInput = new NumericUpDown(window, 8, 32, 100, 26);
            tickInput.Minimum = 0;
            tickInput.Maximum = 4096;
            tickInput.Value = 0;
            tickInput.Step = 1;
            wm.AddWindow(tickInput);

            noteDropdown = new Dropdown(window, 116, 32, 100, 26);
            noteDropdown.PlaceholderText = "Note";
            for (int i = 0; i < noteNames.Length; i++)
            {
                noteDropdown.Items.Add(noteNames[i]);
            }
            noteDropdown.RefreshItems();
            noteDropdown.SelectedIndex = 7; // C4
            wm.AddWindow(noteDropdown);

            durationInput = new NumericUpDown(window, 224, 32, 110, 26);
            durationInput.Minimum = 10;
            durationInput.Maximum = 2000;
            durationInput.Step = 10;
            durationInput.Value = 200;
            wm.AddWindow(durationInput);

            notesTable = new Table(window, 8, 68, window.Width - 16, window.Height - 100);
            notesTable.CellHeight = 22;
            notesTable.AllowDeselection = false;
            notesTable.TableCellSelected = (index) =>
            {
                if (index < 0 || index >= notes.Count)
                {
                    return;
                }
                NoteEvent n = notes[index];
                tickInput.Value = n.Tick;
                durationInput.Value = n.DurationMs;
                for (int i = 0; i < noteDropdown.Items.Count; i++)
                {
                    if (noteDropdown.Items[i] == n.Note)
                    {
                        noteDropdown.SelectedIndex = i;
                        break;
                    }
                }
                UpdateStatus($"Selected {n.Note} at tick {n.Tick}.");
            };
            wm.AddWindow(notesTable);

            statusText = new TextBox(window, 8, window.Height - 30, window.Width - 16, 24);
            statusText.ReadOnly = true;
            statusText.Text = "Ready. Add notes and press Play.";
            wm.AddWindow(statusText);

            Layout();
            RefreshTable();
            UpdateTitle();

            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                OpenFromPath(path);
            }

            wm.Update(window);
        }

        public override void Run()
        {
            if (!playing)
            {
                return;
            }

            if (playIndex >= playback.Count)
            {
                playing = false;
                UpdateStatus("Playback complete.");
                return;
            }

            int elapsedMs = (int)(DateTime.Now - playbackStart).TotalMilliseconds;
            NoteEvent next = playback[playIndex];
            int targetMs = next.Tick * DefaultTickMs;
            if (elapsedMs < targetMs)
            {
                return;
            }

            int frequency = GetNoteFrequency(next.Note);
            int duration = next.DurationMs;
            if (duration < 20) duration = 20;
            if (duration > 500) duration = 500;

            // Cosmos beep is mono beeper output; use short pulse per note.
            Console.Beep();

            playIndex++;
            if (playIndex >= playback.Count)
            {
                playing = false;
                UpdateStatus("Playback complete.");
            }
            else
            {
                UpdateStatus($"Playing: {next.Note} ({frequency}Hz, {duration}ms)");
            }
        }

        public override void Stop()
        {
            StopPlayback();
            base.Stop();
        }
    }
}
