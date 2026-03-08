// The CMLeonOS Project (https://github.com/Leonmmcoset/CMLeonOS)
// Copyright (C) 2025-present LeonOS 2 Developer Team
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using CMLeonOS;
using CMLeonOS.Gui.UILib;
using Cosmos.System.Graphics;
using System.Drawing;
using System;
using System.Collections.Generic;
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

        Process process;

        WindowManager wm;

        AppWindow mainWindow;

        Button runButton;

        CodeEditor editor;

        TextBox problems;

        TextBox output;

        FileBrowser fileBrowser;

        private const int headersHeight = 24;
        private const int problemsHeight = 128;
        private const int outputHeight = 128;

        private string? path = null;

        private bool modified = false;

        private void TextChanged()
        {
            modified = true;

            UpdateTitle();
        }
        
        private void WindowResized()
        {
            int editorHeight = mainWindow.Height - headersHeight - problemsHeight - outputHeight - (headersHeight * 3);
            editor.Move(0, headersHeight);
            editor.Resize(mainWindow.Width, editorHeight);
            editor.MarkAllLines();
            editor.Render();
            
            problems.Move(0, headersHeight + editorHeight + headersHeight);
            problems.Resize(mainWindow.Width, problemsHeight + (headersHeight * 2));
            problems.MarkAllLines();
            problems.Render();
            
            output.Move(0, headersHeight + editorHeight + problemsHeight + (headersHeight * 2));
            output.Resize(mainWindow.Width, outputHeight + (headersHeight * 2));
            output.MarkAllLines();
            output.Render();
            
            mainWindow.Clear(Theme.Background);
            mainWindow.DrawString("Problems", Color.White, 0, headersHeight + editorHeight);
            mainWindow.DrawString("Output", Color.White, 0, headersHeight + editorHeight + problemsHeight + headersHeight);
        }

        private static class Theme
        {
            internal static Color Background = Color.FromArgb(68, 76, 84);
            internal static Color CodeBackground = Color.FromArgb(41, 46, 51);
        }

        private void UpdateTitle()
        {
            if (path == null)
            {
                mainWindow.Title = "Untitled - Lua CodeStudio";
                return;
            }

            if (modified)
            {
                mainWindow.Title = $"{Path.GetFileName(path)}* - Lua CodeStudio";
            }
            else
            {
                mainWindow.Title = $"{Path.GetFileName(path)} - Lua CodeStudio";
            }
        }

        internal void Open(string newPath, bool readFile = true)
        {
            if (newPath == null) return;

            if (readFile && !File.Exists(newPath))
            {
                MessageBox messageBox = new MessageBox(process, "Lua CodeStudio", $"No such file '{Path.GetFileName(newPath)}'.");
                messageBox.Show();
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
                        if (i > 1) result += "\t";
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
                        problems.Foreground = Color.LimeGreen;
                        problems.Text = "Execution successful";
                    }
                    else
                    {
                        string errorMsg = lua.ToString(-1);
                        problems.Foreground = Color.Pink;
                        problems.Text = $"Script execution error: {errorMsg}";
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    problems.Foreground = Color.Pink;
                    problems.Text = $"Script load error: {errorMsg}";
                }
            }
            catch (Exception e)
            {
                problems.Foreground = Color.Pink;
                problems.Text = e.Message;
            }
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
                    problems.Foreground = Color.LimeGreen;
                    problems.Text = "No syntax errors";
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        problems.Foreground = Color.Pink;
                        problems.Text = "Script load error: Unknown error";
                    }
                    else
                    {
                        problems.Foreground = Color.Pink;
                        problems.Text = $"Script load error: {errorMsg}";
                    }
                }
            }
            catch (Exception e)
            {
                problems.Foreground = Color.Pink;
                problems.Text = e.Message;
            }
        }

        internal void Start()
        {
            mainWindow = new AppWindow(process, 96, 96, 800, 600);
            mainWindow.Icon = iconBitmap;
            mainWindow.Clear(Theme.Background);
            mainWindow.Closing = process.TryStop;
            mainWindow.CanResize = true;
            mainWindow.UserResized = WindowResized;
            UpdateTitle();
            wm.AddWindow(mainWindow);

            runButton = new Button(mainWindow, 0, 0, 60, headersHeight);
            runButton.Background = Theme.Background;
            runButton.Border = Theme.Background;
            runButton.Foreground = Color.White;
            runButton.Text = "Run";
            runButton.Image = runBitmap;
            runButton.ImageLocation = Button.ButtonImageLocation.Left;
            runButton.OnClick = RunClicked;
            wm.AddWindow(runButton);

            editor = new CodeEditor(mainWindow, 0, headersHeight, mainWindow.Width, mainWindow.Height - headersHeight - problemsHeight - outputHeight - (headersHeight * 3))
            {
                Background = Theme.CodeBackground,
                Foreground = Color.White,
                Text = "print(\"Hello World!\")",
                Changed = TextChanged,
                MultiLine = true
            };
            editor.SetSyntaxHighlighting(true, ".lua");
            wm.AddWindow(editor);

            problems = new TextBox(mainWindow, 0, headersHeight + editor.Height + headersHeight, mainWindow.Width, problemsHeight + (headersHeight * 2))
            {
                Background = Theme.CodeBackground,
                Foreground = Color.Gray,
                Text = "Click Evaluate to check your program for syntax errors.",
                ReadOnly = true,
                MultiLine = true
            };
            wm.AddWindow(problems);

            mainWindow.DrawString("Problems", Color.White, 0, headersHeight + editor.Height);

            output = new TextBox(mainWindow, 0, headersHeight + editor.Height + problemsHeight + (headersHeight * 2), mainWindow.Width, outputHeight + (headersHeight * 2))
            {
                Background = Theme.CodeBackground,
                Foreground = Color.White,
                Text = "Output will appear here...",
                ReadOnly = true,
                MultiLine = true
            };
            wm.AddWindow(output);

            mainWindow.DrawString("Output", Color.White, 0, headersHeight + editor.Height + problemsHeight + headersHeight);

            var shortcutBar = new ShortcutBar(mainWindow, runButton.Width, 0, mainWindow.Width - runButton.Width, headersHeight)
            {
                Background = Theme.Background,
                Foreground = Color.White
            };
            shortcutBar.Cells.Add(new ShortcutBarCell("Open", OpenFilePrompt)); 
            shortcutBar.Cells.Add(new ShortcutBarCell("Save", Save));
            shortcutBar.Cells.Add(new ShortcutBarCell("Save As", SaveAsPrompt));
            shortcutBar.Cells.Add(new ShortcutBarCell("Evaluate", Evaluate));
            shortcutBar.Render();
            wm.AddWindow(shortcutBar);

            wm.Update(mainWindow);
        }
    }
}
