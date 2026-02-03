using System;
using System.Collections.Generic;
using UniLua;

namespace CMLeonOS
{
    public class Nano
    {
        private string path;
        private bool quit = false;
        private bool modified = false;
        private string clipboard = string.Empty;
        private List<string> lines = new List<string>();
        private int currentLine = 0;
        private int linePos = 0;
        private int scrollX = 0;
        private int scrollY = 0;
        private int? updatedLinesStart;
        private int? updatedLinesEnd;
        private const int TITLEBAR_HEIGHT = 1;
        private const int SHORTCUT_BAR_HEIGHT = 1;
        private FileSystem fileSystem;
        private UserSystem userSystem;
        private Shell shell;

        private readonly (string, string)[] SHORTCUTS = new (string, string)[]
        {
            ("Ctrl+X", "Quit"),
            ("Ctrl+S", "Save"),
            ("Ctrl+I", "Info"),
            ("Ctrl+K", "Cut Line"),
            ("Ctrl+V", "Paste"),
            ("Ctrl+R", "Run")
        };

        string? pendingNotification;

        public Nano(string value, bool isPath = true, FileSystem fs = null, UserSystem us = null, Shell sh = null)
        {
            this.fileSystem = fs;
            this.userSystem = us;
            this.shell = sh;

            if (isPath)
            {
                this.path = value;

                if (value != null && fileSystem != null)
                {
                    string fullPath = fileSystem.GetFullPath(value);
                    
                    if (System.IO.File.Exists(fullPath))
                    {
                        string text = fileSystem.ReadFile(fullPath);
                        text = text.Replace("\r\n", "\n");
                        lines.AddRange(text.Split('\n'));
                    }
                    else
                    {
                        lines.Add(string.Empty);
                    }
                }
                else
                {
                    lines.Add(string.Empty);
                }
            }
            else
            {
                string text = value;
                text = text.Replace("\r\n", "\n");
                lines.AddRange(text.Split('\n'));
            }

            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
        }

        private void Render()
        {
            if (updatedLinesStart != null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                int consoleWidth = 80;
                int consoleHeight = 25;

                for (int i = (int)updatedLinesStart; i <= updatedLinesEnd; i++)
                {
                    int y = i - scrollY + TITLEBAR_HEIGHT;
                    if (y < TITLEBAR_HEIGHT || y >= consoleHeight - SHORTCUT_BAR_HEIGHT) continue;

                    Console.SetCursorPosition(0, y);

                    if (i >= lines.Count || scrollX >= lines[i].Length)
                    {
                        Console.Write(new string(' ', consoleWidth));
                    }
                    else
                    {
                        string line = lines[i].Substring(scrollX, Math.Min(consoleWidth, lines[i].Length - scrollX));
                        Console.Write(line + new string(' ', Math.Max(0, consoleWidth - line.Length)));
                    }
                }

                updatedLinesStart = null;
                updatedLinesEnd = null;
            }

            Console.SetCursorPosition(linePos - scrollX, currentLine + TITLEBAR_HEIGHT - scrollY);
        }

        // Insert a new line at the cursor.
        private void InsertLine()
        {
            string line = lines[currentLine];
            if (linePos == line.Length)
            {
                lines.Insert(currentLine + 1, string.Empty);
            }
            else
            {
                lines.Insert(currentLine + 1, line.Substring(linePos, line.Length - linePos));
                lines[currentLine] = line.Remove(linePos, line.Length - linePos);
            }
            updatedLinesStart = currentLine;
            updatedLinesEnd = lines.Count - 1;

            currentLine += 1;
            linePos = 0;

            modified = true;
        }

        // Insert text at the cursor.
        private void Insert(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                lines[currentLine] = lines[currentLine].Insert(linePos, text[i].ToString());
                linePos++;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        // Backspace at the cursor.
        private void Backspace()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                linePos = lines[currentLine - 1].Length;
                lines[currentLine - 1] += lines[currentLine];

                updatedLinesStart = currentLine - 1;
                updatedLinesEnd = lines.Count - 1;

                lines.RemoveAt(currentLine);
                currentLine -= 1;
            }
            else
            {
                lines[currentLine] = lines[currentLine].Remove(linePos - 1, 1);
                linePos--;

                updatedLinesStart = currentLine;
                updatedLinesEnd = currentLine;
            }
            modified = true;
        }

        // Move the cursor left.
        private void MoveLeft()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine--;
                linePos = lines[currentLine].Length;
            }
            else
            {
                linePos--;
            }
        }

        // Move the cursor right.
        private void MoveRight()
        {
            if (linePos == lines[currentLine].Length)
            {
                if (currentLine + 1 == lines.Count) return;
                currentLine++;
                linePos = 0;
            }
            else
            {
                linePos++;
            }
        }

        // Move the cursor up.
        private void MoveUp()
        {
            if (currentLine == 0) return;
            currentLine--;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Move the cursor down.
        private void MoveDown()
        {
            if (currentLine + 1 == lines.Count) return;
            currentLine++;
            linePos = Math.Min(linePos, lines[currentLine].Length);
        }

        // Jump back to the previous word.
        private void JumpToPreviousWord()
        {
            if (linePos == 0)
            {
                if (currentLine == 0) return;
                currentLine -= 1;
                linePos = lines[currentLine].Length;
                JumpToPreviousWord();
            }
            else
            {
                int res = lines[currentLine].Substring(0, linePos - 1).LastIndexOf(' ');
                linePos = res == -1 ? 0 : res + 1;
            }
        }

        // Jump forward to the next word.
        private void JumpToNextWord()
        {
            int res = lines[currentLine].IndexOf(' ', linePos + 1);
            if (res == -1)
            {
                if (currentLine == lines.Count - 1)
                {
                    linePos = lines[currentLine].Length;
                    return;
                }

                linePos = 0;
                currentLine++;
                while (lines[currentLine] == string.Empty)
                {
                    if (currentLine == lines.Count - 1) break;
                    currentLine++;
                }
            }
            else
            {
                linePos = res + 1;
            }
        }

        private void ShowNotification(string text)
        {
            int consoleWidth = 80;
            int consoleHeight = 25;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition((consoleWidth - text.Length) / 2, consoleHeight - SHORTCUT_BAR_HEIGHT - 1);
            Console.Write($" {text} ");
        }

        private void RenderPrompt(string question, (string, string)[] shortcuts)
        {
            RenderShortcuts(shortcuts);

            int consoleHeight = 25;
            int y = consoleHeight - SHORTCUT_BAR_HEIGHT - 1;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            int consoleWidth = 80;
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', consoleWidth));

            Console.SetCursorPosition(1, y);
            Console.Write(question);

            Console.SetCursorPosition(question.Length + 1, y);
        }

        // Get the entire document as a string.
        private string GetAllText()
        {
            string text = string.Empty;
            foreach (string line in lines)
            {
                text += line + "\n";
            }
            // Strip the trailing newline.
            text = text.Remove(text.Length - 1);

            return text;
        }

        private void Save(bool showFeedback)
        {
            if (path == null)
            {
                RenderPrompt("Path to save to: ", new (string, string)[] { ("Enter", "Save"), ("Esc", "Cancel") });

                string input = "";
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        input = null;
                        break;
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                        Console.SetCursorPosition(1 + input.Length, Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1);
                        Console.Write(" ");
                        Console.SetCursorPosition(1 + input.Length, Console.WindowHeight - SHORTCUT_BAR_HEIGHT - 1);
                    }
                    else if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }
                }

                path = input;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                RenderUI();
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
                Render();

                if (path == null)
                {
                    return;
                }
            }
            else
            {
                if (!modified) return;
            }

            modified = false;
            string text = GetAllText();
            try
            {
                fileSystem.CreateFile(path, text);

                if (showFeedback)
                {
                    RenderUI();
                    ShowNotification($"Saved to {path}");
                }
            }
            catch
            {
                ShowNotification("Failed to save");
                return;
            }
        }

        // Quit, and if the file is modified, prompt to save it.
        private void Quit()
        {
            quit = true;
            if (modified)
            {
                RenderPrompt("Save your changes?", new (string, string)[] { ("Y", "Yes"), ("N", "No"), ("Esc", "Cancel") });
                bool choiceMade = false;
                while (!choiceMade)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            Save(false);
                            choiceMade = true;
                            break;
                        case ConsoleKey.N:
                            choiceMade = true;
                            break;
                        case ConsoleKey.Escape:
                            choiceMade = true;
                            quit = false;

                            // Hide the prompt.
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();
                            RenderUI();
                            updatedLinesStart = 0;
                            updatedLinesEnd = lines.Count - 1;
                            Render();
                            break;
                    }
                }
            }
        }

        // Show information about the document.
        private void ShowInfo()
        {
            ShowNotification($"Ln {currentLine + 1}, Col {linePos + 1}");
        }

        // Cut the current line.
        private void CutLine()
        {
            if (lines[currentLine] != string.Empty)
            {
                clipboard = lines[currentLine];
                if (lines.Count == 1)
                {
                    lines[currentLine] = string.Empty;
                    linePos = 0;

                    updatedLinesStart = 0;
                    updatedLinesEnd = 0;
                }
                else
                {
                    lines.RemoveAt(currentLine);

                    if (currentLine >= lines.Count)
                    {
                        currentLine--;
                    }

                    if (linePos >= lines[currentLine].Length)
                    {
                        linePos = lines[currentLine].Length - 1;
                    }

                    updatedLinesStart = currentLine;
                    updatedLinesEnd = lines.Count;
                }
                modified = true;
            }
            else
            {
                ShowNotification("Nothing was cut");
            }
        }

        // Paste from the clipboard.
        private void Paste()
        {
            if (clipboard != string.Empty)
            {
                Insert(clipboard);
            }
            else
            {
                ShowNotification("Nothing to paste");
            }
        }

        private void RunLua()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            try
            {
                string source = GetAllText();
                
                ILuaState lua = LuaAPI.NewState();
                lua.L_OpenLibs();
                
                UniLua.ThreadStatus loadResult = lua.L_LoadString(source);
                
                if (loadResult == UniLua.ThreadStatus.LUA_OK)
                {
                    UniLua.ThreadStatus callResult = lua.PCall(0, 0, 0);
                    
                    if (callResult != UniLua.ThreadStatus.LUA_OK)
                    {
                        string errorMsg = lua.ToString(-1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Script execution error: {errorMsg}");
                    }
                }
                else
                {
                    string errorMsg = lua.ToString(-1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Script load error: {errorMsg}");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error occurred while running script: {e.Message}");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            RenderUI();
            updatedLinesStart = 0;
            updatedLinesEnd = lines.Count - 1;
            Render();
        }

        private bool HandleInput()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Modifiers)
            {
                case ConsoleModifiers.Control:
                    switch (key.Key)
                    {
                        case ConsoleKey.X:
                            Quit();
                            break;
                        case ConsoleKey.S:
                            Save(true);
                            break;
                        case ConsoleKey.I:
                            ShowInfo();
                            break;
                        case ConsoleKey.K:
                            CutLine();
                            break;
                        case ConsoleKey.V:
                            Paste();
                            break;
                        case ConsoleKey.R:
                            RunLua();
                            break;
                        case ConsoleKey.LeftArrow:
                            JumpToPreviousWord();
                            break;
                        case ConsoleKey.RightArrow:
                            JumpToNextWord();
                            break;
                        case ConsoleKey.UpArrow:
                            currentLine = 0;
                            linePos = 0;
                            break;
                        case ConsoleKey.DownArrow:
                            currentLine = lines.Count - 1;
                            linePos = lines[currentLine].Length;
                            break;
                    }
                    return false;
            }
            switch (key.Key)
            {
                case ConsoleKey.Backspace:
                    Backspace();
                    break;
                case ConsoleKey.Enter:
                    InsertLine();
                    break;
                case ConsoleKey.LeftArrow:
                    MoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight();
                    break;
                case ConsoleKey.UpArrow:
                    MoveUp();
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                default:
                    if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        Insert(key.KeyChar.ToString());
                    }
                    break;
            }
            return false;
        }

        private void UpdateScrolling()
        {
            bool scrollChanged = false;
            int consoleWidth = 80;
            int consoleHeight = 25;

            if (currentLine < scrollY)
            {
                scrollY = currentLine;
                scrollChanged = true;
            }
            else if (currentLine >= scrollY + consoleHeight - TITLEBAR_HEIGHT - SHORTCUT_BAR_HEIGHT)
            {
                scrollY = currentLine - consoleHeight + TITLEBAR_HEIGHT + SHORTCUT_BAR_HEIGHT + 1;
                scrollChanged = true;
            }

            if (linePos < scrollX)
            {
                scrollX = linePos;
                scrollChanged = true;
            }
            else if (linePos > scrollX + consoleWidth - 1)
            {
                scrollX = linePos - consoleWidth + 1;
                scrollChanged = true;
            }

            if (scrollChanged)
            {
                updatedLinesStart = 0;
                updatedLinesEnd = lines.Count - 1;
            }
        }

        private void RenderShortcuts((string, string)[] shortcuts)
        {
            int consoleWidth = 80;
            int y = 24;

            Console.SetCursorPosition(0, y);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', consoleWidth - 1));

            Console.SetCursorPosition(0, y);
            foreach (var shortcut in shortcuts)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{shortcut.Item1}");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {shortcut.Item2} ");
            }
        }

        private void RenderUI()
        {
            int consoleWidth = 80;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
            string text = "  Nano Editor 1.2";
            Console.WriteLine(text + new string(' ', consoleWidth - text.Length));

            string displayName = path == null ? "New File" : System.IO.Path.GetFileName(path);
            Console.SetCursorPosition((consoleWidth - displayName.Length) / 2, 0);
            Console.Write(displayName);

            RenderShortcuts(SHORTCUTS);
        }

        public void Start()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            RenderUI();
            while (!quit)
            {
                Render();
                if (pendingNotification != null)
                {
                    ShowNotification(pendingNotification);
                    pendingNotification = null;
                }

                HandleInput();
                UpdateScrolling();
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    }
}