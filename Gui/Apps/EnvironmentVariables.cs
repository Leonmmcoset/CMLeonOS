using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class EnvironmentVariables : Process
    {
        internal EnvironmentVariables() : base("Environment Variables", ProcessType.Application) { }

        private AppWindow window;
        private Window header;
        private Table variableTable;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button refreshButton;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly EnvironmentVariableManager envManager = EnvironmentVariableManager.Instance;

        private readonly List<string> rowNames = new List<string>();
        private readonly List<string> rowValues = new List<string>();

        private const int padding = 8;
        private const int toolbarHeight = 32;
        private const int headerHeight = 54;
        private const int buttonWidth = 78;
        private const int buttonHeight = 24;

        private string statusText = "Manage variables stored in 0:\\system\\env.dat";

        private void SetStatus(string text)
        {
            statusText = text;
            RenderHeader();
        }

        private void RenderHeader()
        {
            header.Clear(Color.FromArgb(235, 241, 248));
            header.DrawRectangle(0, 0, header.Width, header.Height, Color.FromArgb(180, 192, 208));
            header.DrawString("Environment Variables", Color.FromArgb(28, 38, 52), 12, 10);
            header.DrawString(statusText, Color.FromArgb(97, 110, 126), 12, 30);
            wm.Update(header);
        }

        private void UpdateStatusFromSelection()
        {
            if (variableTable.SelectedCellIndex >= 0 && variableTable.SelectedCellIndex < rowNames.Count)
            {
                SetStatus($"Selected: {rowNames[variableTable.SelectedCellIndex]}={rowValues[variableTable.SelectedCellIndex]}");
                return;
            }

            SetStatus($"Loaded {rowNames.Count} variable(s) from 0:\\system\\env.dat");
        }

        private void PopulateTable()
        {
            string selectedName = null;
            if (variableTable.SelectedCellIndex >= 0 && variableTable.SelectedCellIndex < rowNames.Count)
            {
                selectedName = rowNames[variableTable.SelectedCellIndex];
            }

            rowNames.Clear();
            rowValues.Clear();
            variableTable.Cells.Clear();
            variableTable.SelectedCellIndex = -1;

            string[] variables = envManager.GetAllVariablesAsLines();
            for (int i = 0; i < variables.Length; i++)
            {
                string line = variables[i] ?? string.Empty;
                int equalIndex = line.IndexOf('=');
                string name = equalIndex >= 0 ? line.Substring(0, equalIndex) : line;
                string value = equalIndex >= 0 ? line.Substring(equalIndex + 1) : string.Empty;

                rowNames.Add(name);
                rowValues.Add(value);
            }

            for (int i = 0; i < rowNames.Count; i++)
            {
                variableTable.Cells.Add(new TableCell($"{rowNames[i]}={rowValues[i]}", rowNames[i]));
            }

            if (!string.IsNullOrWhiteSpace(selectedName))
            {
                for (int i = 0; i < rowNames.Count; i++)
                {
                    if (rowNames[i] == selectedName)
                    {
                        variableTable.SelectedCellIndex = i;
                        break;
                    }
                }
            }

            variableTable.Render();
            UpdateStatusFromSelection();
        }

        private void SelectionChanged(int index)
        {
            UpdateStatusFromSelection();
        }

        private void RefreshClicked(int x, int y)
        {
            PopulateTable();
        }

        private void AddClicked(int x, int y)
        {
            PromptBox namePrompt = new PromptBox(this, "Add Variable", "Enter the variable name.", "MY_VAR", (string name) =>
            {
                string trimmedName = (name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmedName))
                {
                    SetStatus("Variable name cannot be empty.");
                    return;
                }

                PromptBox valuePrompt = new PromptBox(this, "Add Variable", $"Enter the value for {trimmedName}.", "value", (string value) =>
                {
                    envManager.SetVariable(trimmedName, value ?? string.Empty);
                    PopulateTable();
                    SetStatus($"Added: {trimmedName}");
                });
                valuePrompt.Show();
            });
            namePrompt.Show();
        }

        private void EditClicked(int x, int y)
        {
            if (variableTable.SelectedCellIndex < 0 || variableTable.SelectedCellIndex >= rowNames.Count)
            {
                SetStatus("Select a variable first.");
                return;
            }

            string selectedName = rowNames[variableTable.SelectedCellIndex];
            string selectedValue = rowValues[variableTable.SelectedCellIndex];
            PromptBox valuePrompt = new PromptBox(this, "Edit Variable", $"Enter a new value for {selectedName}.", selectedValue, (string value) =>
            {
                envManager.SetVariable(selectedName, value ?? string.Empty);
                PopulateTable();
                SetStatus($"Updated: {selectedName}");
            });
            valuePrompt.Show();
        }

        private void DeleteClicked(int x, int y)
        {
            if (variableTable.SelectedCellIndex < 0 || variableTable.SelectedCellIndex >= rowNames.Count)
            {
                SetStatus("Select a variable first.");
                return;
            }

            string selectedName = rowNames[variableTable.SelectedCellIndex];
            bool deleted = envManager.DeleteVariable(selectedName);
            if (!deleted)
            {
                new MessageBox(this, "Environment Variables", "Unable to delete the selected variable.").Show();
                return;
            }

            PopulateTable();
            SetStatus($"Deleted: {selectedName}");
        }

        private void Relayout()
        {
            int topY = padding;
            addButton.MoveAndResize(window.Width - ((buttonWidth * 4) + (padding * 4)), topY, buttonWidth, buttonHeight);
            editButton.MoveAndResize(window.Width - ((buttonWidth * 3) + (padding * 3)), topY, buttonWidth, buttonHeight);
            deleteButton.MoveAndResize(window.Width - ((buttonWidth * 2) + (padding * 2)), topY, buttonWidth, buttonHeight);
            refreshButton.MoveAndResize(window.Width - (buttonWidth + padding), topY, buttonWidth, buttonHeight);

            header.MoveAndResize(0, toolbarHeight, window.Width, headerHeight);

            int tableY = toolbarHeight + headerHeight;
            variableTable.MoveAndResize(0, tableY, window.Width, window.Height - tableY);

            addButton.Render();
            editButton.Render();
            deleteButton.Render();
            refreshButton.Render();
            RenderHeader();
            variableTable.Render();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 220, 120, 760, 430);
            window.Title = "Environment Variables";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Relayout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            addButton = new Button(window, 0, 0, 1, 1);
            addButton.Text = "Add";
            addButton.OnClick = AddClicked;
            wm.AddWindow(addButton);

            editButton = new Button(window, 0, 0, 1, 1);
            editButton.Text = "Edit";
            editButton.OnClick = EditClicked;
            wm.AddWindow(editButton);

            deleteButton = new Button(window, 0, 0, 1, 1);
            deleteButton.Text = "Delete";
            deleteButton.OnClick = DeleteClicked;
            wm.AddWindow(deleteButton);

            refreshButton = new Button(window, 0, 0, 1, 1);
            refreshButton.Text = "Refresh";
            refreshButton.OnClick = RefreshClicked;
            wm.AddWindow(refreshButton);

            header = new Window(this, window, 0, toolbarHeight, window.Width, headerHeight);
            wm.AddWindow(header);

            variableTable = new Table(window, 0, toolbarHeight + headerHeight, window.Width, window.Height - toolbarHeight - headerHeight);
            variableTable.Background = Color.White;
            variableTable.Foreground = Color.Black;
            variableTable.Border = Color.FromArgb(185, 194, 207);
            variableTable.SelectedBackground = Color.FromArgb(216, 231, 255);
            variableTable.SelectedBorder = Color.FromArgb(94, 138, 216);
            variableTable.SelectedForeground = Color.Black;
            variableTable.CellHeight = 24;
            variableTable.TableCellSelected = SelectionChanged;
            wm.AddWindow(variableTable);

            PopulateTable();
            Relayout();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
