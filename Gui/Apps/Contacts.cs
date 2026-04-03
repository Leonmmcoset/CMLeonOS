using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace CMLeonOS.Gui.Apps
{
    internal class Contacts : Process
    {
        internal Contacts() : base("Contacts", ProcessType.Application) { }

        private sealed class ContactEntry
        {
            internal string Name = string.Empty;
            internal string Phone = string.Empty;
            internal string Email = string.Empty;
            internal string Address = string.Empty;
            internal string Notes = string.Empty;
        }

        private const string DataPath = @"0:\contacts.dat";
        private const int ToolbarHeight = 34;
        private const int HeaderHeight = 52;
        private const int Padding = 8;
        private const int ButtonWidth = 82;
        private const int ButtonHeight = 24;
        private const int LabelWidth = 64;
        private const int RowHeight = 24;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly List<ContactEntry> contacts = new List<ContactEntry>();

        private AppWindow window;
        private Window header;
        private Table contactTable;

        private Button newButton;
        private Button saveButton;
        private Button deleteButton;
        private Button refreshButton;

        private TextBlock nameLabel;
        private TextBlock phoneLabel;
        private TextBlock emailLabel;
        private TextBlock addressLabel;
        private TextBlock notesLabel;

        private TextBox nameBox;
        private TextBox phoneBox;
        private TextBox emailBox;
        private TextBox addressBox;
        private TextBox notesBox;

        private string statusText = @"Store file: 0:\contacts.dat";

        private void SetStatus(string text)
        {
            statusText = text ?? string.Empty;
            RenderHeader();
        }

        private void RenderHeader()
        {
            header.Clear(Color.FromArgb(235, 241, 248));
            header.DrawRectangle(0, 0, header.Width, header.Height, Color.FromArgb(180, 192, 208));
            header.DrawString("Contacts", Color.FromArgb(28, 38, 52), 12, 10);
            header.DrawString(statusText, Color.FromArgb(97, 110, 126), 12, 30);
            wm.Update(header);
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

        private void LoadContacts()
        {
            contacts.Clear();

            if (!File.Exists(DataPath))
            {
                SetStatus(@"No contact file yet. Click Save to create 0:\contacts.dat");
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

                string[] fields = SplitEscapedTabs(line);
                ContactEntry entry = new ContactEntry();
                entry.Name = fields.Length > 0 ? Unescape(fields[0]) : string.Empty;
                entry.Phone = fields.Length > 1 ? Unescape(fields[1]) : string.Empty;
                entry.Email = fields.Length > 2 ? Unescape(fields[2]) : string.Empty;
                entry.Address = fields.Length > 3 ? Unescape(fields[3]) : string.Empty;
                entry.Notes = fields.Length > 4 ? Unescape(fields[4]) : string.Empty;

                if (!string.IsNullOrWhiteSpace(entry.Name))
                {
                    contacts.Add(entry);
                }
            }

            SetStatus($"Loaded {contacts.Count} contact(s).");
        }

        private void SaveContacts()
        {
            string[] lines = new string[contacts.Count];
            for (int i = 0; i < contacts.Count; i++)
            {
                ContactEntry c = contacts[i];
                lines[i] = string.Join("\t", new string[]
                {
                    Escape(c.Name),
                    Escape(c.Phone),
                    Escape(c.Email),
                    Escape(c.Address),
                    Escape(c.Notes)
                });
            }

            File.WriteAllLines(DataPath, lines);
        }

        private void PopulateTable(int preferredSelection = -1)
        {
            contactTable.Cells.Clear();
            contactTable.SelectedCellIndex = -1;

            for (int i = 0; i < contacts.Count; i++)
            {
                ContactEntry c = contacts[i];
                string display = string.IsNullOrWhiteSpace(c.Phone) ? c.Name : $"{c.Name}  ({c.Phone})";
                contactTable.Cells.Add(new TableCell(display, c.Name));
            }

            if (preferredSelection >= 0 && preferredSelection < contacts.Count)
            {
                contactTable.SelectedCellIndex = preferredSelection;
            }

            contactTable.Render();
            ContactSelected(contactTable.SelectedCellIndex);
        }

        private void ClearEditor()
        {
            nameBox.Text = string.Empty;
            phoneBox.Text = string.Empty;
            emailBox.Text = string.Empty;
            addressBox.Text = string.Empty;
            notesBox.Text = string.Empty;
        }

        private ContactEntry ReadEditor()
        {
            ContactEntry entry = new ContactEntry();
            entry.Name = (nameBox.Text ?? string.Empty).Trim();
            entry.Phone = (phoneBox.Text ?? string.Empty).Trim();
            entry.Email = (emailBox.Text ?? string.Empty).Trim();
            entry.Address = (addressBox.Text ?? string.Empty).Trim();
            entry.Notes = notesBox.Text ?? string.Empty;
            return entry;
        }

        private void LoadEditor(ContactEntry entry)
        {
            if (entry == null)
            {
                ClearEditor();
                return;
            }

            nameBox.Text = entry.Name;
            phoneBox.Text = entry.Phone;
            emailBox.Text = entry.Email;
            addressBox.Text = entry.Address;
            notesBox.Text = entry.Notes;
        }

        private void ContactSelected(int index)
        {
            if (index < 0 || index >= contacts.Count)
            {
                SetStatus($"Loaded {contacts.Count} contact(s).");
                ClearEditor();
                return;
            }

            LoadEditor(contacts[index]);
            SetStatus($"Selected: {contacts[index].Name}");
        }

        private void NewClicked(int x, int y)
        {
            contactTable.SelectedCellIndex = -1;
            ClearEditor();
            SetStatus("Create mode: fill fields and click Save.");
        }

        private void SaveClicked(int x, int y)
        {
            ContactEntry updated = ReadEditor();
            if (string.IsNullOrWhiteSpace(updated.Name))
            {
                new MessageBox(this, "Contacts", "Name cannot be empty.").Show();
                return;
            }

            int selected = contactTable.SelectedCellIndex;
            int targetIndex;
            if (selected >= 0 && selected < contacts.Count)
            {
                contacts[selected] = updated;
                targetIndex = selected;
                SetStatus($"Updated: {updated.Name}");
            }
            else
            {
                contacts.Add(updated);
                targetIndex = contacts.Count - 1;
                SetStatus($"Added: {updated.Name}");
            }

            SaveContacts();
            PopulateTable(targetIndex);
        }

        private void DeleteClicked(int x, int y)
        {
            int selected = contactTable.SelectedCellIndex;
            if (selected < 0 || selected >= contacts.Count)
            {
                SetStatus("Select a contact first.");
                return;
            }

            string name = contacts[selected].Name;
            contacts.RemoveAt(selected);
            SaveContacts();

            int next = selected;
            if (next >= contacts.Count) next = contacts.Count - 1;
            PopulateTable(next);
            SetStatus($"Deleted: {name}");
        }

        private void RefreshClicked(int x, int y)
        {
            LoadContacts();
            PopulateTable(-1);
        }

        private void Relayout()
        {
            int topY = Padding;
            int right = window.Width - Padding;

            newButton.MoveAndResize(right - ((ButtonWidth * 4) + (Padding * 3)), topY, ButtonWidth, ButtonHeight);
            saveButton.MoveAndResize(right - ((ButtonWidth * 3) + (Padding * 2)), topY, ButtonWidth, ButtonHeight);
            deleteButton.MoveAndResize(right - ((ButtonWidth * 2) + Padding), topY, ButtonWidth, ButtonHeight);
            refreshButton.MoveAndResize(right - ButtonWidth, topY, ButtonWidth, ButtonHeight);

            header.MoveAndResize(0, ToolbarHeight, window.Width, HeaderHeight);

            int contentY = ToolbarHeight + HeaderHeight + Padding;
            int contentHeight = window.Height - contentY - Padding;
            int listWidth = Math.Max(220, (window.Width * 2) / 5);
            int editorX = listWidth + (Padding * 2);
            int editorWidth = window.Width - editorX - Padding;

            contactTable.MoveAndResize(Padding, contentY, listWidth - Padding, contentHeight);

            int y = contentY;
            nameLabel.MoveAndResize(editorX, y + 2, LabelWidth, 20);
            nameBox.MoveAndResize(editorX + LabelWidth, y, editorWidth - LabelWidth, RowHeight);

            y += RowHeight + Padding;
            phoneLabel.MoveAndResize(editorX, y + 2, LabelWidth, 20);
            phoneBox.MoveAndResize(editorX + LabelWidth, y, editorWidth - LabelWidth, RowHeight);

            y += RowHeight + Padding;
            emailLabel.MoveAndResize(editorX, y + 2, LabelWidth, 20);
            emailBox.MoveAndResize(editorX + LabelWidth, y, editorWidth - LabelWidth, RowHeight);

            y += RowHeight + Padding;
            addressLabel.MoveAndResize(editorX, y + 2, LabelWidth, 20);
            addressBox.MoveAndResize(editorX + LabelWidth, y, editorWidth - LabelWidth, RowHeight);

            y += RowHeight + Padding;
            notesLabel.MoveAndResize(editorX, y + 2, LabelWidth, 20);
            notesBox.MoveAndResize(editorX + LabelWidth, y, editorWidth - LabelWidth, Math.Max(72, contentY + contentHeight - y));

            newButton.Render();
            saveButton.Render();
            deleteButton.Render();
            refreshButton.Render();
            RenderHeader();
            contactTable.Render();
            nameLabel.Render();
            phoneLabel.Render();
            emailLabel.Render();
            addressLabel.Render();
            notesLabel.Render();
            nameBox.Render();
            phoneBox.Render();
            emailBox.Render();
            addressBox.Render();
            notesBox.Render();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 190, 100, 900, 520);
            window.Title = "Contacts";
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

            refreshButton = new Button(window, 0, 0, 1, 1);
            refreshButton.Text = "Refresh";
            refreshButton.OnClick = RefreshClicked;
            wm.AddWindow(refreshButton);

            header = new Window(this, window, 0, ToolbarHeight, window.Width, HeaderHeight);
            wm.AddWindow(header);

            contactTable = new Table(window, 0, 0, 1, 1);
            contactTable.CellHeight = 24;
            contactTable.Background = Color.White;
            contactTable.Foreground = Color.Black;
            contactTable.Border = Color.FromArgb(185, 194, 207);
            contactTable.SelectedBackground = Color.FromArgb(216, 231, 255);
            contactTable.SelectedBorder = Color.FromArgb(94, 138, 216);
            contactTable.SelectedForeground = Color.Black;
            contactTable.TableCellSelected = ContactSelected;
            wm.AddWindow(contactTable);

            nameLabel = new TextBlock(window, 0, 0, LabelWidth, 20);
            nameLabel.Text = "Name";
            nameLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(nameLabel);

            phoneLabel = new TextBlock(window, 0, 0, LabelWidth, 20);
            phoneLabel.Text = "Phone";
            phoneLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(phoneLabel);

            emailLabel = new TextBlock(window, 0, 0, LabelWidth, 20);
            emailLabel.Text = "Email";
            emailLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(emailLabel);

            addressLabel = new TextBlock(window, 0, 0, LabelWidth, 20);
            addressLabel.Text = "Address";
            addressLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(addressLabel);

            notesLabel = new TextBlock(window, 0, 0, LabelWidth, 20);
            notesLabel.Text = "Notes";
            notesLabel.Foreground = UITheme.TextSecondary;
            wm.AddWindow(notesLabel);

            nameBox = new TextBox(window, 0, 0, 1, 1);
            wm.AddWindow(nameBox);

            phoneBox = new TextBox(window, 0, 0, 1, 1);
            wm.AddWindow(phoneBox);

            emailBox = new TextBox(window, 0, 0, 1, 1);
            wm.AddWindow(emailBox);

            addressBox = new TextBox(window, 0, 0, 1, 1);
            wm.AddWindow(addressBox);

            notesBox = new TextBox(window, 0, 0, 1, 1);
            notesBox.MultiLine = true;
            wm.AddWindow(notesBox);

            LoadContacts();
            PopulateTable(-1);
            Relayout();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
