using CMLeonOS;
using CMLeonOS.Gui.UILib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CMLeonOS.Gui.Apps
{
    internal class CodeBlocks : Process
    {
        internal CodeBlocks() : base("CodeBlocks", ProcessType.Application) { }

        private const string ProjectMagic = "CBLK1";

        private class Block
        {
            internal string Type;
            internal int A;
            internal int B;
            internal string Text;
        }

        private AppWindow window;
        private readonly WindowManager wm = ProcessManager.GetProcess<WindowManager>();

        private Table paletteTable;
        private Table programTable;
        private Table outputTable;
        private TextBox argTextBox;
        private NumericUpDown argA;
        private NumericUpDown argB;
        private TextBox statusBox;
        private FileBrowser fileBrowser;
        private readonly List<Button> actionButtons = new List<Button>();

        private readonly List<Block> program = new List<Block>();
        private readonly Dictionary<string, int> vars = new Dictionary<string, int>();
        private string projectPath = null;
        private bool modified = false;

        private readonly string[] palette = new[]
        {
            "Say (text)",
            "Set Counter (A)",
            "Add Counter (A)",
            "Sub Counter (A)",
            "Mul Counter (A)",
            "Div Counter by A",
            "Counter %= A",
            "Counter += A*B",
            "Counter = abs(Counter)",
            "Counter = random(A..B)",
            "Say Counter",
            "Wait (ms=A)",
            "Repeat Say (A times)",
            "If Counter > A -> Say(text)",
            "If Counter == A -> Say(text)",
            "Beep",
            "Set Var(text)=A",
            "Add Var(text)+=A",
            "Mul Var(text)*=A",
            "Swap Counter <-> Var(text)",
            "Say Var(text)",
            "Clear Output"
        };

        private void UpdateTitle()
        {
            string name = string.IsNullOrWhiteSpace(projectPath) ? "Untitled.cblk" : Path.GetFileName(projectPath);
            window.Title = modified ? $"{name}* - CodeBlocks" : $"{name} - CodeBlocks";
        }

        private void SetStatus(string text)
        {
            statusBox.Text = text;
            statusBox.Render();
            wm.Update(statusBox);
        }

        private void MarkModified()
        {
            modified = true;
            UpdateTitle();
        }

        private void Layout()
        {
            int side = 8;
            int top = 34;
            int gap = 8;
            int leftWidth = 240;
            int statusHeight = 24;
            int actionHeight = 26;
            int bottomHeight = 156;

            int rightX = side + leftWidth + gap;
            int rightWidth = window.Width - rightX - side;

            int listsHeight = window.Height - top - actionHeight - bottomHeight - statusHeight - (gap * 3);
            if (listsHeight < 120)
            {
                listsHeight = 120;
            }

            paletteTable.MoveAndResize(side, top, leftWidth, listsHeight);
            paletteTable.Render();

            programTable.MoveAndResize(rightX, top, rightWidth, listsHeight);
            programTable.Render();

            int actionY = top + listsHeight + gap;
            int x = side;
            for (int i = 0; i < actionButtons.Count; i++)
            {
                actionButtons[i].MoveAndResize(x, actionY, actionButtons[i].Width, actionHeight, sendWMEvent: false);
                x += actionButtons[i].Width + 6;
            }

            int controlsY = actionY + actionHeight + gap;
            argTextBox.MoveAndResize(side, controlsY, 260, 24);
            argA.MoveAndResize(side + 268, controlsY, 96, 24);
            argB.MoveAndResize(side + 372, controlsY, 96, 24);

            outputTable.MoveAndResize(side, controlsY + 32, window.Width - side * 2, bottomHeight - 32);
            outputTable.Render();

            statusBox.MoveAndResize(side, window.Height - statusHeight - 4, window.Width - side * 2, statusHeight);
            statusBox.Render();
        }

        private void RefreshProgramTable()
        {
            programTable.Cells.Clear();
            for (int i = 0; i < program.Count; i++)
            {
                programTable.Cells.Add(new TableCell($"{i:D2}  {DescribeBlock(program[i])}", i));
            }
            programTable.Render();
            wm.Update(programTable);
        }

        private string DescribeBlock(Block b)
        {
            switch (b.Type)
            {
                case "Say": return $"Say \"{b.Text}\"";
                case "SetCounter": return $"Set Counter = {b.A}";
                case "AddCounter": return $"Add Counter by {b.A}";
                case "SubCounter": return $"Sub Counter by {b.A}";
                case "MulCounter": return $"Counter *= {b.A}";
                case "DivCounter": return $"Counter /= {b.A}";
                case "ModCounter": return $"Counter %= {b.A}";
                case "CounterAddMul": return $"Counter += {b.A}*{b.B}";
                case "AbsCounter": return "Counter = abs(Counter)";
                case "RandCounter": return $"Counter = random({b.A}..{b.B})";
                case "SayCounter": return "Say Counter";
                case "Wait": return $"Wait {b.A}ms";
                case "RepeatSay": return $"Repeat Say {b.A} times: \"{b.Text}\"";
                case "IfCounterGtSay": return $"If Counter > {b.A} Say \"{b.Text}\"";
                case "IfCounterEqSay": return $"If Counter == {b.A} Say \"{b.Text}\"";
                case "Beep": return "Beep";
                case "SetVar": return $"Set Var[{b.Text}] = {b.A}";
                case "AddVar": return $"Add Var[{b.Text}] += {b.A}";
                case "MulVar": return $"Mul Var[{b.Text}] *= {b.A}";
                case "SwapCounterVar": return $"Swap Counter <-> Var[{b.Text}]";
                case "SayVar": return $"Say Var[{b.Text}]";
                case "ClearOutput": return "Clear Output";
                default: return b.Type;
            }
        }

        private Block CreateBlockByPaletteIndex(int index)
        {
            string t = string.IsNullOrWhiteSpace(argTextBox.Text) ? "x" : argTextBox.Text;
            switch (index)
            {
                case 0: return new Block { Type = "Say", Text = string.IsNullOrWhiteSpace(argTextBox.Text) ? "Hello" : argTextBox.Text };
                case 1: return new Block { Type = "SetCounter", A = argA.Value };
                case 2: return new Block { Type = "AddCounter", A = argA.Value };
                case 3: return new Block { Type = "SubCounter", A = argA.Value };
                case 4: return new Block { Type = "MulCounter", A = argA.Value };
                case 5: return new Block { Type = "DivCounter", A = argA.Value == 0 ? 1 : argA.Value };
                case 6: return new Block { Type = "ModCounter", A = argA.Value == 0 ? 1 : Math.Abs(argA.Value) };
                case 7: return new Block { Type = "CounterAddMul", A = argA.Value, B = argB.Value };
                case 8: return new Block { Type = "AbsCounter" };
                case 9: return new Block { Type = "RandCounter", A = argA.Value, B = argB.Value };
                case 10: return new Block { Type = "SayCounter" };
                case 11: return new Block { Type = "Wait", A = Math.Max(0, argA.Value) };
                case 12: return new Block { Type = "RepeatSay", A = Math.Max(0, argA.Value), Text = string.IsNullOrWhiteSpace(argTextBox.Text) ? "Hello" : argTextBox.Text };
                case 13: return new Block { Type = "IfCounterGtSay", A = argA.Value, Text = string.IsNullOrWhiteSpace(argTextBox.Text) ? "Counter is greater" : argTextBox.Text };
                case 14: return new Block { Type = "IfCounterEqSay", A = argA.Value, Text = string.IsNullOrWhiteSpace(argTextBox.Text) ? "Counter equals value" : argTextBox.Text };
                case 15: return new Block { Type = "Beep" };
                case 16: return new Block { Type = "SetVar", Text = t, A = argA.Value };
                case 17: return new Block { Type = "AddVar", Text = t, A = argA.Value };
                case 18: return new Block { Type = "MulVar", Text = t, A = argA.Value };
                case 19: return new Block { Type = "SwapCounterVar", Text = t };
                case 20: return new Block { Type = "SayVar", Text = t };
                case 21: return new Block { Type = "ClearOutput" };
                default: return null;
            }
        }

        private void AddSelectedBlock()
        {
            int idx = paletteTable.SelectedCellIndex;
            if (idx < 0 || idx >= palette.Length) { SetStatus("Select a block from palette."); return; }
            Block b = CreateBlockByPaletteIndex(idx);
            if (b == null) { SetStatus("Cannot create block."); return; }
            program.Add(b);
            RefreshProgramTable();
            MarkModified();
            SetStatus("Block added.");
        }

        private void DeleteSelectedBlock()
        {
            int idx = programTable.SelectedCellIndex;
            if (idx < 0 || idx >= program.Count) { SetStatus("Select a block in program."); return; }
            program.RemoveAt(idx);
            RefreshProgramTable();
            MarkModified();
            SetStatus("Block removed.");
        }

        private void MoveSelectedBlock(int dir)
        {
            int idx = programTable.SelectedCellIndex;
            if (idx < 0 || idx >= program.Count) { SetStatus("Select a block in program."); return; }
            int target = idx + dir;
            if (target < 0 || target >= program.Count) { return; }
            Block tmp = program[idx];
            program[idx] = program[target];
            program[target] = tmp;
            RefreshProgramTable();
            programTable.SelectedCellIndex = target;
            wm.Update(programTable);
            MarkModified();
            SetStatus("Block moved.");
        }

        private int GetVar(string name)
        {
            string key = (name ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(key)) key = "x";
            if (!vars.ContainsKey(key)) vars[key] = 0;
            return vars[key];
        }

        private void SetVar(string name, int value)
        {
            string key = (name ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(key)) key = "x";
            vars[key] = value;
        }

        private void AppendOutput(string line)
        {
            outputTable.Cells.Add(new TableCell(line));
            outputTable.ScrollToBottom();
            outputTable.Render();
            wm.Update(outputTable);
        }

        private void ResetOutput()
        {
            outputTable.Cells.Clear();
            outputTable.Cells.Add(new TableCell("Output..."));
            outputTable.Render();
            wm.Update(outputTable);
        }

        private void RunProgram()
        {
            if (program.Count == 0) { SetStatus("Program is empty."); return; }

            vars.Clear();
            vars["counter"] = 0;
            SetStatus("Running...");

            for (int i = 0; i < program.Count; i++)
            {
                Block b = program[i];
                switch (b.Type)
                {
                    case "Say":
                        AppendOutput("[Say] " + b.Text); break;
                    case "SetCounter":
                        vars["counter"] = b.A; AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "AddCounter":
                        vars["counter"] += b.A; AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "SubCounter":
                        vars["counter"] -= b.A; AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "MulCounter":
                        vars["counter"] *= b.A; AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "DivCounter":
                        {
                            int d = b.A == 0 ? 1 : b.A;
                            vars["counter"] /= d;
                            AppendOutput("[Var] counter = " + vars["counter"]);
                            break;
                        }
                    case "ModCounter":
                        {
                            int d = b.A == 0 ? 1 : Math.Abs(b.A);
                            vars["counter"] %= d;
                            AppendOutput("[Var] counter = " + vars["counter"]);
                            break;
                        }
                    case "CounterAddMul":
                        vars["counter"] += b.A * b.B; AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "AbsCounter":
                        vars["counter"] = Math.Abs(vars["counter"]); AppendOutput("[Var] counter = " + vars["counter"]); break;
                    case "RandCounter":
                        {
                            int min = b.A;
                            int max = b.B;
                            if (min > max) { int t = min; min = max; max = t; }
                            vars["counter"] = new Random((int)DateTime.Now.Ticks + i).Next(min, max + 1);
                            AppendOutput("[Var] counter = " + vars["counter"]);
                            break;
                        }
                    case "SayCounter":
                        AppendOutput("[Counter] " + vars["counter"]); break;
                    case "Wait":
                        {
                            int ms = b.A; if (ms < 0) ms = 0; if (ms > 2000) ms = 2000;
                            System.Threading.Thread.Sleep(ms);
                            AppendOutput("[Wait] " + ms + "ms");
                            break;
                        }
                    case "RepeatSay":
                        {
                            int count = b.A; if (count < 0) count = 0; if (count > 100) count = 100;
                            for (int n = 0; n < count; n++) AppendOutput("[Repeat] " + b.Text);
                            break;
                        }
                    case "IfCounterGtSay":
                        if (vars["counter"] > b.A) AppendOutput("[If>] " + b.Text); break;
                    case "IfCounterEqSay":
                        if (vars["counter"] == b.A) AppendOutput("[If==] " + b.Text); break;
                    case "Beep":
                        Console.Beep(); AppendOutput("[Beep]"); break;
                    case "SetVar":
                        SetVar(b.Text, b.A); AppendOutput("[Var] " + b.Text + " = " + GetVar(b.Text)); break;
                    case "AddVar":
                        SetVar(b.Text, GetVar(b.Text) + b.A); AppendOutput("[Var] " + b.Text + " = " + GetVar(b.Text)); break;
                    case "MulVar":
                        SetVar(b.Text, GetVar(b.Text) * b.A); AppendOutput("[Var] " + b.Text + " = " + GetVar(b.Text)); break;
                    case "SwapCounterVar":
                        {
                            int vv = GetVar(b.Text);
                            int cc = vars["counter"];
                            SetVar(b.Text, cc);
                            vars["counter"] = vv;
                            AppendOutput("[Swap] counter <-> " + b.Text);
                            break;
                        }
                    case "SayVar":
                        AppendOutput("[Var] " + b.Text + " = " + GetVar(b.Text)); break;
                    case "ClearOutput":
                        ResetOutput(); break;
                }
            }

            SetStatus("Done.");
        }

        private string Escape(string text)
        {
            if (text == null) return string.Empty;
            return text.Replace("\\", "\\\\").Replace("|", "\\p").Replace("\n", "\\n").Replace("\r", "");
        }

        private string Unescape(string text)
        {
            if (text == null) return string.Empty;
            return text.Replace("\\n", "\n").Replace("\\p", "|").Replace("\\\\", "\\");
        }

        private void NewProject()
        {
            program.Clear();
            projectPath = null;
            modified = false;
            RefreshProgramTable();
            ResetOutput();
            UpdateTitle();
            SetStatus("New project.");
        }

        private void SaveProjectTo(string path)
        {
            List<string> lines = new List<string> { ProjectMagic };
            for (int i = 0; i < program.Count; i++)
            {
                Block b = program[i];
                lines.Add(b.Type + "|" + b.A + "|" + b.B + "|" + Escape(b.Text));
            }
            File.WriteAllLines(path, lines.ToArray());
            projectPath = path;
            modified = false;
            UpdateTitle();
            SetStatus("Saved: " + Path.GetFileName(path));
        }

        private void SaveProject()
        {
            if (string.IsNullOrWhiteSpace(projectPath)) { SaveProjectAs(); return; }
            SaveProjectTo(projectPath);
        }

        private void SaveProjectAs()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (string.IsNullOrWhiteSpace(selectedPath)) return;
                string finalPath = selectedPath.EndsWith(".cblk") ? selectedPath : selectedPath + ".cblk";
                SaveProjectTo(finalPath);
            }, selectDirectoryOnly: true);
            fileBrowser.Show();
        }

        private void LoadProjectPath(string path)
        {
            if (!File.Exists(path)) { SetStatus("Project file not found."); return; }
            string[] lines = File.ReadAllLines(path);
            if (lines.Length == 0 || lines[0].Trim() != ProjectMagic) { SetStatus("Invalid project format."); return; }

            program.Clear();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] p = line.Split('|');
                if (p.Length < 4) continue;
                int a = 0; int b = 0;
                int.TryParse(p[1], out a);
                int.TryParse(p[2], out b);
                program.Add(new Block { Type = p[0], A = a, B = b, Text = Unescape(p[3]) });
            }

            projectPath = path;
            modified = false;
            RefreshProgramTable();
            UpdateTitle();
            SetStatus("Loaded: " + Path.GetFileName(path));
        }

        private void LoadProject()
        {
            fileBrowser = new FileBrowser(this, wm, (string selectedPath) =>
            {
                if (!string.IsNullOrWhiteSpace(selectedPath)) LoadProjectPath(selectedPath);
            });
            fileBrowser.Show();
        }

        public override void Start()
        {
            base.Start();

            window = new AppWindow(this, 140, 80, 980, 620);
            window.Title = "Untitled.cblk - CodeBlocks";
            window.Icon = AppManager.DefaultAppIcon;
            window.CanResize = true;
            window.UserResized = Layout;
            window.Closing = TryStop;
            wm.AddWindow(window);

            window.DrawString("CodeBlocks - Visual Programming", Color.FromArgb(90, 102, 122), 10, 10);

            paletteTable = new Table(window, 8, 34, 240, 300);
            paletteTable.CellHeight = 24;
            paletteTable.AllowDeselection = false;
            for (int i = 0; i < palette.Length; i++) paletteTable.Cells.Add(new TableCell(palette[i], i));
            wm.AddWindow(paletteTable);

            programTable = new Table(window, 256, 34, 700, 300);
            programTable.CellHeight = 24;
            programTable.AllowDeselection = false;
            wm.AddWindow(programTable);

            Button addBtn = new Button(window, 8, 350, 110, 26) { Text = "Add Block" };
            addBtn.OnClick = (_, _) => AddSelectedBlock(); wm.AddWindow(addBtn);
            Button delBtn = new Button(window, 124, 350, 110, 26) { Text = "Delete" };
            delBtn.OnClick = (_, _) => DeleteSelectedBlock(); wm.AddWindow(delBtn);
            Button upBtn = new Button(window, 240, 350, 70, 26) { Text = "Up" };
            upBtn.OnClick = (_, _) => MoveSelectedBlock(-1); wm.AddWindow(upBtn);
            Button downBtn = new Button(window, 316, 350, 70, 26) { Text = "Down" };
            downBtn.OnClick = (_, _) => MoveSelectedBlock(1); wm.AddWindow(downBtn);
            Button runBtn = new Button(window, 392, 350, 90, 26) { Text = "Run" };
            runBtn.OnClick = (_, _) => RunProgram(); wm.AddWindow(runBtn);
            Button clearBtn = new Button(window, 488, 350, 90, 26) { Text = "Clear" };
            clearBtn.OnClick = (_, _) => { program.Clear(); RefreshProgramTable(); MarkModified(); ResetOutput(); SetStatus("Program cleared."); }; wm.AddWindow(clearBtn);
            Button newBtn = new Button(window, 584, 350, 70, 26) { Text = "New" };
            newBtn.OnClick = (_, _) => NewProject(); wm.AddWindow(newBtn);
            Button loadBtn = new Button(window, 660, 350, 70, 26) { Text = "Load" };
            loadBtn.OnClick = (_, _) => LoadProject(); wm.AddWindow(loadBtn);
            Button saveBtn = new Button(window, 736, 350, 70, 26) { Text = "Save" };
            saveBtn.OnClick = (_, _) => SaveProject(); wm.AddWindow(saveBtn);
            Button saveAsBtn = new Button(window, 812, 350, 96, 26) { Text = "Save As" };
            saveAsBtn.OnClick = (_, _) => SaveProjectAs(); wm.AddWindow(saveAsBtn);

            actionButtons.Add(addBtn); actionButtons.Add(delBtn); actionButtons.Add(upBtn); actionButtons.Add(downBtn);
            actionButtons.Add(runBtn); actionButtons.Add(clearBtn); actionButtons.Add(newBtn); actionButtons.Add(loadBtn);
            actionButtons.Add(saveBtn); actionButtons.Add(saveAsBtn);

            argTextBox = new TextBox(window, 8, 382, 260, 24) { PlaceholderText = "Text / variable name" };
            wm.AddWindow(argTextBox);
            argA = new NumericUpDown(window, 276, 382, 96, 24) { Minimum = -9999, Maximum = 9999, Value = 1 };
            wm.AddWindow(argA);
            argB = new NumericUpDown(window, 380, 382, 96, 24) { Minimum = -9999, Maximum = 9999, Value = 0 };
            wm.AddWindow(argB);

            outputTable = new Table(window, 8, 414, 960, 130);
            outputTable.CellHeight = 20;
            outputTable.AllowSelection = false;
            outputTable.AllowDeselection = false;
            outputTable.ScrollbarThickness = 14;
            outputTable.Cells.Add(new TableCell("Output..."));
            wm.AddWindow(outputTable);

            statusBox = new TextBox(window, 8, 586, 960, 24) { ReadOnly = true, Text = "Ready." };
            wm.AddWindow(statusBox);

            Layout();
            RefreshProgramTable();
            UpdateTitle();
            wm.Update(window);
        }

        public override void Run()
        {
        }
    }
}
