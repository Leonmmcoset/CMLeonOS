using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System.Collections.Generic;
using System.Drawing;

namespace CMLeonOS.Gui.Apps
{
    internal class UILibGallery : Process
    {
        internal UILibGallery() : base("UILib Gallery", ProcessType.Application) { }

        private AppWindow window;
        private Table categoryTable;
        private Window previewHost;
        private Window header;

        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();
        private readonly List<Window> demoWindows = new List<Window>();

        private FileBrowser fileBrowser;
        private ToolTip toolTip;
        private string headerTitle = "UILib Gallery";
        private string headerDescription = "Browse and test the current UILib controls.";

        private const int sidebarWidth = 164;
        private const int headerHeight = 58;

        private void SetHeader(string title, string description)
        {
            headerTitle = title;
            headerDescription = description;
            RenderHeader();
        }

        private void RenderHeader()
        {
            header.Clear(Color.FromArgb(235, 241, 248));
            header.DrawRectangle(0, 0, header.Width, header.Height, Color.FromArgb(180, 192, 208));
            header.DrawString(headerTitle, Color.FromArgb(28, 38, 52), 12, 10);
            header.DrawString(headerDescription, Color.FromArgb(97, 110, 126), 12, 30);
            wm.Update(header);
        }

        private void ClearPreview()
        {
            for (int i = 0; i < demoWindows.Count; i++)
            {
                wm.RemoveWindow(demoWindows[i]);
            }
            demoWindows.Clear();

            previewHost.Clear(Color.FromArgb(250, 252, 255));
            previewHost.DrawRectangle(0, 0, previewHost.Width, previewHost.Height, Color.FromArgb(192, 204, 221));
            wm.Update(previewHost);
        }

        private void AddDemo(Window control)
        {
            demoWindows.Add(control);
            wm.AddWindow(control);
        }

        private void ShowButtonsDemo()
        {
            ClearPreview();
            SetHeader("Button", "Standard action buttons with theme colours and icon support.");

            TextBlock label = new TextBlock(previewHost, 20, 18, 360, 20);
            label.Text = "Buttons";
            label.Foreground = UITheme.TextPrimary;
            AddDemo(label);

            Button primary = new Button(previewHost, 20, 48, 120, 28);
            primary.Text = "Primary";
            primary.OnClick = (_, _) => SetHeader("Button", "Primary button clicked.");
            AddDemo(primary);

            Button success = new Button(previewHost, 152, 48, 120, 28);
            success.Text = "Success";
            success.Background = UITheme.Success;
            success.Border = Color.FromArgb(31, 110, 72);
            success.OnClick = (_, _) => SetHeader("Button", "Success button clicked.");
            AddDemo(success);

            Button neutral = new Button(previewHost, 284, 48, 120, 28);
            neutral.Text = "Neutral";
            neutral.Background = UITheme.SurfaceMuted;
            neutral.Border = UITheme.SurfaceBorder;
            neutral.Foreground = UITheme.TextPrimary;
            neutral.OnClick = (_, _) => SetHeader("Button", "Neutral button clicked.");
            AddDemo(neutral);

            ImageBlock iconPreview = new ImageBlock(previewHost, 20, 92, 32, 32);
            iconPreview.Image = AppManager.DefaultAppIcon.Resize(32, 32);
            AddDemo(iconPreview);
        }

        private void ShowInputsDemo()
        {
            ClearPreview();
            SetHeader("Input Controls", "Text input, checkbox, switch and slider controls.");

            TextBox textBox = new TextBox(previewHost, 20, 24, 220, 24);
            textBox.PlaceholderText = "Type something";
            textBox.Changed = () => SetHeader("Input Controls", "TextBox value changed.");
            AddDemo(textBox);

            TextBox multi = new TextBox(previewHost, 20, 58, 300, 86);
            multi.MultiLine = true;
            multi.Text = "Multi-line TextBox\nUILib demo";
            AddDemo(multi);

            CheckBox checkBox = new CheckBox(previewHost, 340, 24, 180, 20);
            checkBox.Text = "Enable feature";
            checkBox.CheckBoxChanged = (value) => SetHeader("Input Controls", value ? "CheckBox checked." : "CheckBox unchecked.");
            AddDemo(checkBox);

            Switch toggle = new Switch(previewHost, 340, 58, 180, 20);
            toggle.Text = "Animated switch";
            toggle.CheckBoxChanged = (value) => SetHeader("Input Controls", value ? "Switch enabled." : "Switch disabled.");
            AddDemo(toggle);

            RangeSlider slider = new RangeSlider(previewHost, 340, 96, 220, 30, 0f, 30f, 100f);
            slider.Changed = (value) => SetHeader("Input Controls", "RangeSlider value: " + ((int)value).ToString());
            AddDemo(slider);
        }

        private void ShowDropdownDemo()
        {
            ClearPreview();
            SetHeader("Dropdown", "Single-select dropdown list built on top of Table.");

            Dropdown dropdown = new Dropdown(previewHost, 20, 28, 200, 24);
            dropdown.PlaceholderText = "Choose a theme";
            dropdown.Items.Add("Ocean");
            dropdown.Items.Add("Sunrise");
            dropdown.Items.Add("Forest");
            dropdown.Items.Add("Mono");
            dropdown.RefreshItems();
            dropdown.SelectionChanged = (index, text) => SetHeader("Dropdown", "Selected item: " + text);
            AddDemo(dropdown);

            TextBlock info = new TextBlock(previewHost, 20, 66, 360, 20);
            info.Text = "Click the field to expand the dropdown.";
            info.Foreground = UITheme.TextSecondary;
            AddDemo(info);
        }

        private void ShowTabsDemo()
        {
            ClearPreview();
            SetHeader("Tabs", "Tab strip control for switching between sections.");

            Tabs tabs = new Tabs(previewHost, 20, 24, 360, 28);
            tabs.Items.Add("General");
            tabs.Items.Add("Appearance");
            tabs.Items.Add("Advanced");
            tabs.RefreshItems();
            tabs.SelectionChanged = (index, text) => SetHeader("Tabs", "Selected tab: " + text);
            AddDemo(tabs);

            TextBlock block = new TextBlock(previewHost, 20, 70, 360, 48);
            block.Text = "Tabs are ideal for settings and editors.";
            block.Background = UITheme.AccentLight;
            block.Foreground = UITheme.TextPrimary;
            block.HorizontalAlignment = Alignment.Middle;
            block.VerticalAlignment = Alignment.Middle;
            AddDemo(block);
        }

        private void ShowNumericDemo()
        {
            ClearPreview();
            SetHeader("NumericUpDown", "Increment and decrement integer values without typing.");

            NumericUpDown numeric = new NumericUpDown(previewHost, 20, 24, 120, 32);
            numeric.Minimum = 0;
            numeric.Maximum = 256;
            numeric.Step = 8;
            numeric.Value = 64;
            numeric.Changed = (value) => SetHeader("NumericUpDown", "Value changed to " + value.ToString());
            AddDemo(numeric);

            TextBlock hint = new TextBlock(previewHost, 160, 28, 320, 24);
            hint.Text = "Click + / - to change the value.";
            hint.Foreground = UITheme.TextSecondary;
            AddDemo(hint);
        }

        private void ShowProgressDemo()
        {
            ClearPreview();
            SetHeader("ProgressRing", "Animated busy indicator for indeterminate loading states.");

            ProgressRing ring = new ProgressRing(previewHost, 24, 24, 56, 56);
            ring.Active = true;
            AddDemo(ring);

            TextBlock hint = new TextBlock(previewHost, 100, 38, 320, 24);
            hint.Text = "ProgressRing keeps animating while Active=true.";
            hint.Foreground = UITheme.TextSecondary;
            AddDemo(hint);
        }

        private void ShowTableDemo()
        {
            ClearPreview();
            SetHeader("Table", "Selectable list/table control with scrolling and icons.");

            Table table = new Table(previewHost, 20, 24, 360, 180);
            table.CellHeight = 24;
            table.Cells.Add(new TableCell(AppManager.DefaultAppIcon.Resize(20, 20), "First Item", "First"));
            table.Cells.Add(new TableCell(AppManager.DefaultAppIcon.Resize(20, 20), "Second Item", "Second"));
            table.Cells.Add(new TableCell(AppManager.DefaultAppIcon.Resize(20, 20), "Third Item", "Third"));
            table.Cells.Add(new TableCell("Text-only row", "Text"));
            table.TableCellSelected = (index) =>
            {
                if (index >= 0 && index < table.Cells.Count)
                {
                    SetHeader("Table", "Selected row: " + table.Cells[index].Text);
                }
            };
            table.Render();
            AddDemo(table);
        }

        private void ShowBarsDemo()
        {
            ClearPreview();
            SetHeader("Bars And Blocks", "ShortcutBar, TextBlock and ImageBlock examples.");

            ShortcutBar shortcutBar = new ShortcutBar(previewHost, 20, 20, 360, 24);
            shortcutBar.Background = UITheme.SurfaceMuted;
            shortcutBar.Foreground = UITheme.TextPrimary;
            shortcutBar.Cells.Add(new ShortcutBarCell("File", () => SetHeader("Bars And Blocks", "ShortcutBar: File")));
            shortcutBar.Cells.Add(new ShortcutBarCell("Edit", () => SetHeader("Bars And Blocks", "ShortcutBar: Edit")));
            shortcutBar.Cells.Add(new ShortcutBarCell("View", () => SetHeader("Bars And Blocks", "ShortcutBar: View")));
            shortcutBar.Render();
            AddDemo(shortcutBar);

            TextBlock centered = new TextBlock(previewHost, 20, 62, 220, 48);
            centered.Text = "Centered TextBlock";
            centered.Background = UITheme.AccentLight;
            centered.Foreground = UITheme.TextPrimary;
            centered.HorizontalAlignment = Alignment.Middle;
            centered.VerticalAlignment = Alignment.Middle;
            AddDemo(centered);

            ImageBlock image = new ImageBlock(previewHost, 260, 62, 48, 48);
            image.Image = AppManager.DefaultAppIcon.Resize(48, 48);
            image.Alpha = true;
            AddDemo(image);

            StatusBar statusBar = new StatusBar(previewHost, 20, 132, 360, 24);
            statusBar.Text = "Ready";
            statusBar.DetailText = "UILib Demo";
            AddDemo(statusBar);

            Separator separator = new Separator(previewHost, 20, 172, 360, 8);
            separator.Background = Color.FromArgb(250, 252, 255);
            separator.Foreground = UITheme.SurfaceBorder;
            AddDemo(separator);
        }

        private void ShowTreeDemo()
        {
            ClearPreview();
            SetHeader("TreeView", "Hierarchical tree control for folders, settings and project structures.");

            TreeView tree = new TreeView(previewHost, 20, 24, 300, 180);

            TreeNode rootA = new TreeNode("System");
            rootA.Expanded = true;
            rootA.Children.Add(new TreeNode("Apps"));
            rootA.Children.Add(new TreeNode("Config"));

            TreeNode rootB = new TreeNode("User");
            rootB.Children.Add(new TreeNode("Desktop"));
            rootB.Children.Add(new TreeNode("Documents"));
            rootB.Children.Add(new TreeNode("Pictures"));

            tree.Nodes.Add(rootA);
            tree.Nodes.Add(rootB);
            tree.NodeSelected = (node) => SetHeader("TreeView", "Selected node: " + node.Text);
            tree.Render();
            AddDemo(tree);
        }

        private void ShowDialogsDemo()
        {
            ClearPreview();
            SetHeader("Dialogs", "Dialog-style components that open in separate windows.");

            Button messageButton = new Button(previewHost, 20, 28, 140, 28);
            messageButton.Text = "MessageBox";
            messageButton.OnClick = (_, _) =>
            {
                new MessageBox(this, "UILib Gallery", "This is a MessageBox demo.").Show();
            };
            AddDemo(messageButton);

            Button promptButton = new Button(previewHost, 172, 28, 140, 28);
            promptButton.Text = "PromptBox";
            promptButton.OnClick = (_, _) =>
            {
                PromptBox prompt = new PromptBox(this, "UILib Gallery", "Enter sample text.", "Hello", (value) =>
                {
                    SetHeader("Dialogs", "Prompt result: " + (string.IsNullOrWhiteSpace(value) ? "(empty)" : value));
                });
                prompt.Show();
            };
            AddDemo(promptButton);

            Button fileBrowserButton = new Button(previewHost, 324, 28, 140, 28);
            fileBrowserButton.Text = "FileBrowser";
            fileBrowserButton.OnClick = (_, _) =>
            {
                fileBrowser = new FileBrowser(this, wm, (selectedPath) =>
                {
                    SetHeader("Dialogs", "FileBrowser selected: " + (string.IsNullOrWhiteSpace(selectedPath) ? "(cancelled)" : selectedPath));
                });
                fileBrowser.Show();
            };
            AddDemo(fileBrowserButton);

            Button notificationButton = new Button(previewHost, 20, 68, 140, 28);
            notificationButton.Text = "Notification";
            notificationButton.OnClick = (_, _) =>
            {
                Notification notification = new Notification(this, "UILib Gallery", "This is a toast notification.");
                notification.Show();
            };
            AddDemo(notificationButton);

            Button toolTipButton = new Button(previewHost, 172, 68, 140, 28);
            toolTipButton.Text = "ToolTip";
            toolTipButton.OnClick = (_, _) =>
            {
                if (toolTip == null)
                {
                    toolTip = new ToolTip(this, wm);
                }
                toolTip.Show(toolTipButton, "Manual ToolTip demo");
                SetHeader("Dialogs", "ToolTip shown near the button.");
            };
            toolTipButton.OnUnfocused = () => toolTip?.Hide();
            AddDemo(toolTipButton);
        }

        private void CategorySelected(int index)
        {
            switch (index)
            {
                case 0:
                    ShowButtonsDemo();
                    break;
                case 1:
                    ShowInputsDemo();
                    break;
                case 2:
                    ShowDropdownDemo();
                    break;
                case 3:
                    ShowTabsDemo();
                    break;
                case 4:
                    ShowNumericDemo();
                    break;
                case 5:
                    ShowProgressDemo();
                    break;
                case 6:
                    ShowTableDemo();
                    break;
                case 7:
                    ShowTreeDemo();
                    break;
                case 8:
                    ShowBarsDemo();
                    break;
                case 9:
                    ShowDialogsDemo();
                    break;
            }
        }

        private void Relayout()
        {
            categoryTable.MoveAndResize(0, 0, sidebarWidth, window.Height);
            header.MoveAndResize(sidebarWidth, 0, window.Width - sidebarWidth, headerHeight);
            previewHost.MoveAndResize(sidebarWidth, headerHeight, window.Width - sidebarWidth, window.Height - headerHeight);

            categoryTable.Render();
            RenderHeader();
            ClearPreview();

            if (categoryTable.SelectedCellIndex < 0)
            {
                categoryTable.SelectedCellIndex = 0;
            }
            CategorySelected(categoryTable.SelectedCellIndex);
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 180, 110, 860, 520);
            window.Title = "UILib Gallery";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Relayout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            categoryTable = new Table(window, 0, 0, sidebarWidth, window.Height);
            categoryTable.AllowDeselection = false;
            categoryTable.CellHeight = 26;
            categoryTable.TextAlignment = Alignment.Middle;
            categoryTable.Background = Color.FromArgb(242, 246, 252);
            categoryTable.Border = Color.FromArgb(182, 194, 210);
            categoryTable.SelectedBackground = Color.FromArgb(216, 231, 255);
            categoryTable.SelectedBorder = UITheme.Accent;
            categoryTable.SelectedForeground = UITheme.TextPrimary;
            categoryTable.TableCellSelected = CategorySelected;
            categoryTable.Cells.Add(new TableCell("Buttons"));
            categoryTable.Cells.Add(new TableCell("Inputs"));
            categoryTable.Cells.Add(new TableCell("Dropdown"));
            categoryTable.Cells.Add(new TableCell("Tabs"));
            categoryTable.Cells.Add(new TableCell("Numeric"));
            categoryTable.Cells.Add(new TableCell("Progress"));
            categoryTable.Cells.Add(new TableCell("Table"));
            categoryTable.Cells.Add(new TableCell("TreeView"));
            categoryTable.Cells.Add(new TableCell("Bars && Blocks"));
            categoryTable.Cells.Add(new TableCell("Dialogs"));
            wm.AddWindow(categoryTable);

            header = new Window(this, window, sidebarWidth, 0, window.Width - sidebarWidth, headerHeight);
            wm.AddWindow(header);

            previewHost = new Window(this, window, sidebarWidth, headerHeight, window.Width - sidebarWidth, window.Height - headerHeight);
            wm.AddWindow(previewHost);

            categoryTable.SelectedCellIndex = 0;
            Relayout();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
