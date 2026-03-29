using CMLeonOS;
using CMLeonOS.Gui.UILib;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using System.IO;
using UniLua;

namespace CMLeonOS.Gui.Apps.CodeStudio
{
    internal class Ide
    {
        internal Ide(Process process, WindowManager wm)
        {
            this.process = process;
            this.wm = wm;
        }

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.AppIcons.CodeStudio.bmp")]
        private static byte[] _iconBytes;
        private static Bitmap iconBitmap = new Bitmap(_iconBytes);

        [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "CMLeonOS.Gui.Resources.CodeStudio.Run.bmp")]
        private static byte[] _runBytes;
        private static Bitmap runBitmap = new Bitmap(_runBytes);

        private readonly Process process;
        private readonly WindowManager wm;

        private AppWindow mainWindow;
        private Button runButton;
        private ShortcutBar shortcutBar;
        private CodeEditor editor;
        private TextBox problems;
        private TextBox output;
        private TextBox statusBar;
        private Table sideTable;
        private FileBrowser fileBrowser;

        private string path = null;
        private bool modified = false;

        private static class Layout
        {
            internal const int TopBarHeight = 30;
            internal const int BottomBarHeight = 24;
            internal const int Gap = 6;
            internal const int Side = 8;
            internal const int RunWidth = 72;
            internal const int SidePanelWidth = 190;
            internal const int PanelTitleHeight = 18;
            internal const int BottomPanelHeight = 150;
        }

        private static class Theme
        {
            internal static readonly Color FrameBg = Color.FromArgb(34, 40, 52);
            internal static readonly Color ToolbarBg = Color.FromArgb(27, 33, 44);
            internal static readonly Color EditorBg = Color.FromArgb(19, 24, 34);
            internal static readonly Color PanelBg = Color.FromArgb(23, 29, 39);
            internal static readonly Color PanelBorder = Color.FromArgb(66, 78, 97);
            internal static readonly Color TitleText = Color.FromArgb(220, 228, 240);
            internal static readonly Color HintText = Color.FromArgb(157, 171, 190);
            internal static readonly Color OkText = Color.FromArgb(142, 219, 160);
            internal static readonly Color ErrorText = Color.FromArgb(255, 153, 164);
        }

        private void UpdateTitle()
        {
            if (path == null)
            {
                mainWindow.Title = "Untitled - CodeStudio";
                return;
            }

            string fileName = Path.GetFileName(path);
            mainWindow.Title = modified ? $"{fileName}* - CodeStudio" : $"{fileName} - CodeStudio";
        }

        private void SetStatus(string text)
        {
            statusBar.Text = text;
            statusBar.Render();
            wm.Update(statusBar);
        }

        private void TextChanged()
        {
            modified = true;
            UpdateTitle();
            SetStatus("Edited");
        }

        private void RenderChrome()
        {
            mainWindow.Clear(Theme.FrameBg);
            mainWindow.DrawFilledRectangle(0, 0, mainWindow.Width, Layout.TopBarHeight, Theme.ToolbarBg);
            mainWindow.DrawRectangle(0, 0, mainWindow.Width, Layout.TopBarHeight, Theme.PanelBorder);

            int contentY = Layout.TopBarHeight + Layout.Gap;
            int contentH = mainWindow.Height - contentY - Layout.BottomBarHeight - Layout.Gap;
            int leftW = Layout.SidePanelWidth;
            int rightX = Layout.Side + leftW + Layout.Gap;
            int rightW = mainWindow.Width - rightX - Layout.Side;

            int editorTop = contentY + Layout.PanelTitleHeight;
            int editorH = contentH - Layout.BottomPanelHeight - Layout.Gap - Layout.PanelTitleHeight;
            int bottomY = editorTop + editorH + Layout.Gap + Layout.PanelTitleHeight;
            int halfW = (rightW - Layout.Gap) / 2;

            mainWindow.DrawString("Workspace", Theme.TitleText, Layout.Side + 2, contentY + 2);
            mainWindow.DrawString("Editor", Theme.TitleText, rightX + 2, contentY + 2);
            mainWindow.DrawString("Problems", Theme.TitleText, rightX + 2, bottomY - Layout.PanelTitleHeight + 2);
            mainWindow.DrawString("Output", Theme.TitleText, rightX + halfW + Layout.Gap + 2, bottomY - Layout.PanelTitleHeight + 2);
        }

        private int GetEditorHeight()
        {
            int contentY = Layout.TopBarHeight + Layout.Gap;
            int contentH = mainWindow.Height - contentY - Layout.BottomBarHeight - Layout.Gap;
            int available = contentH - Layout.BottomPanelHeight - Layout.Gap - Layout.PanelTitleHeight;
            if (available < 80)
            {
                return 80;
            }
            return available;
        }

        private void ReLayout()
        {
            int side = Layout.Side;
            int width = mainWindow.Width - (side * 2);
            int y = 0;

            runButton.MoveAndResize(side, y, Layout.RunWidth, Layout.TopBarHeight, sendWMEvent: false);
            shortcutBar.MoveAndResize(side + Layout.RunWidth + Layout.Gap, y, width - Layout.RunWidth - Layout.Gap, Layout.TopBarHeight, sendWMEvent: false);
            shortcutBar.Render();

            int contentY = Layout.TopBarHeight + Layout.Gap;
            int contentH = mainWindow.Height - contentY - Layout.BottomBarHeight - Layout.Gap;
            int leftW = Layout.SidePanelWidth;
            int rightX = side + leftW + Layout.Gap;
            int rightW = mainWindow.Width - rightX - side;

            sideTable.MoveAndResize(side, contentY + Layout.PanelTitleHeight, leftW, contentH - Layout.PanelTitleHeight, sendWMEvent: false);
            sideTable.Render();

            int editorY = contentY + Layout.PanelTitleHeight;
            int editorH = GetEditorHeight();
            editor.MoveAndResize(rightX, editorY, rightW, editorH, sendWMEvent: false);
            editor.MarkAllLines();
            editor.Render();

            int bottomY = editorY + editorH + Layout.Gap + Layout.PanelTitleHeight;
            int halfW = (rightW - Layout.Gap) / 2;
            problems.MoveAndResize(rightX, bottomY, halfW, Layout.BottomPanelHeight, sendWMEvent: false);
            problems.MarkAllLines();
            problems.Render();

            output.MoveAndResize(rightX + halfW + Layout.Gap, bottomY, rightW - halfW - Layout.Gap, Layout.BottomPanelHeight, sendWMEvent: false);
            output.MarkAllLines();
            output.Render();

            int statusY = mainWindow.Height - Layout.BottomBarHeight;
            statusBar.MoveAndResize(0, statusY, mainWindow.Width, Layout.BottomBarHeight, sendWMEvent: false);
            statusBar.Render();

            RenderChrome();
            wm.Update(mainWindow);
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null)
            {
                return;
            }

            if (readFile && !File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(process, "CodeStudio", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
                return;
            }

            path = newPath;

            if (readFile)
            {
                editor.Text = File.ReadAllText(path);

                string extension = Path.GetExtension(path).ToLower();
                editor.SetSyntaxHighlighting(extension == ".lua", extension);
                editor.MarkAllLines();
                editor.Render();

                modified = false;
            }

            UpdateTitle();
            SetStatus("Opened: " + Path.GetFileName(path));
        }

        private void OpenFilePrompt()
        {
            fileBrowser = new FileBrowser(process, wm, (string selectedPath) =>
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
            fileBrowser = new FileBrowser(process, wm, (string selectedPath) =>
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

            File.WriteAllText(path, editor.Text);
            modified = false;
            UpdateTitle();
            SetStatus("Saved: " + Path.GetFileName(path));
        }

        private void SideCommandSelected(int index)
        {
            if (index < 0 || index >= sideTable.Cells.Count)
            {
                return;
            }

            string cmd = sideTable.Cells[index].Text;
            if (cmd == "Open File")
            {
                OpenFilePrompt();
            }
            else if (cmd == "Save File")
            {
                Save();
            }
            else if (cmd == "Save As")
            {
                SaveAsPrompt();
            }
            else if (cmd == "Evaluate")
            {
                Evaluate();
            }
            else if (cmd == "Run Script")
            {
                RunClicked(0, 0);
            }
            else if (cmd == "Insert: Hello")
            {
                editor.Text += "\nprint(\"Hello from CodeStudio\")";
                editor.MarkAllLines();
                editor.Render();
                wm.Update(editor);
                TextChanged();
            }
            else if (cmd == "Insert: GUI Demo")
            {
                editor.Text += "\nlocal w = gui.window(\"Demo\", 100, 100, 260, 120)\n";
                editor.MarkAllLines();
                editor.Render();
                wm.Update(editor);
                TextChanged();
            }
        }

        private void RunClicked(int x, int y)
        {
            try
            {
                output.Text = "";

                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();

                LuaGuiLibrary.Initialize(wm, process);
                LuaGuiLibrary.Register(lua);

                lua.PushCSharpFunction(L =>
                {
                    int n = L.GetTop();
                    string result = "";
                    for (int i = 1; i <= n; i++)
                    {
                        if (i > 1)
                        {
                            result += "\t";
                        }
                        LuaType type = L.Type(i);
                        if (type == LuaType.LUA_TSTRING)
                        {
                            result += L.ToString(i);
                        }
                        else if (type == LuaType.LUA_TBOOLEAN)
                        {
                            result += L.ToBoolean(i) ? "true" : "false";
                        }
                        else if (type == LuaType.LUA_TNUMBER)
                        {
                            result += L.ToNumber(i).ToString();
                        }
                        else if (type == LuaType.LUA_TNIL)
                        {
                            result += "nil";
                        }
                    }
                    output.Text += result + "\n";
                    return 0;
                });
                lua.SetGlobal("print");

                UniLua.ThreadStatus loadResult = lua.L_LoadString(editor.Text);

                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                    if (callResult == UniLua.ThreadStatus.LUA_OK)
                    {
                        problems.Foreground = Theme.OkText;
                        problems.Text = "Execution successful.";
                        SetStatus("Run succeeded");
                    }
                    else
                    {
                        string errorMsg = lua.ToString(-1);
                        problems.Foreground = Theme.ErrorText;
                        problems.Text = "Script execution error: " + errorMsg;
                        SetStatus("Run failed");
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    problems.Foreground = Theme.ErrorText;
                    problems.Text = "Script load error: " + errorMsg;
                    SetStatus("Run failed");
                }
            }
            catch (Exception e)
            {
                problems.Foreground = Theme.ErrorText;
                problems.Text = e.Message;
                SetStatus("Run failed");
            }

            problems.MarkAllLines();
            problems.Render();
            output.MarkAllLines();
            output.Render();
            wm.Update(problems);
            wm.Update(output);
        }

        private void Evaluate()
        {
            try
            {
                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();
                UniLua.ThreadStatus loadResult = lua.L_LoadString(editor.Text);

                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    problems.Foreground = Theme.OkText;
                    problems.Text = "No syntax errors.";
                    SetStatus("Evaluate passed");
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    problems.Foreground = Theme.ErrorText;
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        problems.Text = "Script load error: Unknown error";
                    }
                    else
                    {
                        problems.Text = "Script load error: " + errorMsg;
                    }
                    SetStatus("Evaluate failed");
                }
            }
            catch (Exception e)
            {
                problems.Foreground = Theme.ErrorText;
                problems.Text = e.Message;
                SetStatus("Evaluate failed");
            }

            problems.MarkAllLines();
            problems.Render();
            wm.Update(problems);
        }

        internal void Start()
        {
            mainWindow = new AppWindow(process, 96, 96, 920, 640);
            mainWindow.Icon = iconBitmap;
            mainWindow.Closing = process.TryStop;
            mainWindow.CanResize = true;
            mainWindow.UserResized = ReLayout;
            wm.AddWindow(mainWindow);
            UpdateTitle();

            runButton = new Button(mainWindow, Layout.Side, 0, Layout.RunWidth, Layout.TopBarHeight);
            runButton.Background = Theme.ToolbarBg;
            runButton.Border = Theme.PanelBorder;
            runButton.Foreground = Theme.TitleText;
            runButton.Text = "Run";
            runButton.Image = runBitmap;
            runButton.ImageLocation = Button.ButtonImageLocation.Left;
            runButton.OnClick = RunClicked;
            wm.AddWindow(runButton);

            shortcutBar = new ShortcutBar(mainWindow, Layout.Side + Layout.RunWidth + Layout.Gap, 0, 300, Layout.TopBarHeight);
            shortcutBar.Background = Theme.ToolbarBg;
            shortcutBar.Foreground = Theme.TitleText;
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", OpenFilePrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAsPrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Evaluate", Evaluate));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            editor = new CodeEditor(mainWindow, Layout.Side, Layout.TopBarHeight + Layout.Gap, 300, 200)
            {
                Background = Theme.EditorBg,
                Foreground = Color.White,
                Text = "print(\"Hello World!\")",
                Changed = TextChanged,
                MultiLine = true
            };
            editor.SetSyntaxHighlighting(true, ".lua");
            wm.AddWindow(editor);

            sideTable = new Table(mainWindow, Layout.Side, Layout.TopBarHeight + Layout.Gap + Layout.PanelTitleHeight, Layout.SidePanelWidth, 300);
            sideTable.CellHeight = 24;
            sideTable.AllowDeselection = false;
            sideTable.Background = Theme.PanelBg;
            sideTable.Border = Theme.PanelBorder;
            sideTable.Foreground = Theme.TitleText;
            sideTable.SelectedBackground = Color.FromArgb(54, 72, 103);
            sideTable.SelectedBorder = Color.FromArgb(88, 112, 152);
            sideTable.SelectedForeground = Color.White;
            sideTable.TableCellSelected = SideCommandSelected;
            sideTable.Cells.Add(new TableCell("Open File"));
            sideTable.Cells.Add(new TableCell("Save File"));
            sideTable.Cells.Add(new TableCell("Save As"));
            sideTable.Cells.Add(new TableCell("Evaluate"));
            sideTable.Cells.Add(new TableCell("Run Script"));
            sideTable.Cells.Add(new TableCell("Insert: Hello"));
            sideTable.Cells.Add(new TableCell("Insert: GUI Demo"));
            wm.AddWindow(sideTable);

            problems = new TextBox(mainWindow, Layout.Side, 280, 300, Layout.BottomPanelHeight)
            {
                Background = Theme.PanelBg,
                Foreground = Theme.HintText,
                Text = "Click Evaluate to check syntax.",
                ReadOnly = true,
                MultiLine = true
            };
            wm.AddWindow(problems);

            output = new TextBox(mainWindow, Layout.Side, 430, 300, Layout.BottomPanelHeight)
            {
                Background = Theme.PanelBg,
                Foreground = Theme.TitleText,
                Text = "Output will appear here...",
                ReadOnly = true,
                MultiLine = true
            };
            wm.AddWindow(output);

            statusBar = new TextBox(mainWindow, 0, mainWindow.Height - Layout.BottomBarHeight, mainWindow.Width, Layout.BottomBarHeight)
            {
                Background = Theme.ToolbarBg,
                Foreground = Theme.HintText,
                ReadOnly = true,
                Text = "Ready"
            };
            wm.AddWindow(statusBar);

            ReLayout();
            wm.Update(mainWindow);
        }
    }
}
