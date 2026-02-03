using System;
using System.Collections.Generic;

namespace CMLeonOS
{
    // 简单的代码编辑器类
    // 支持基本的文本编辑功能，如输入、删除、移动光标等
    // 支持文件的加载和保存
    public class Editor
    {
        private string fileName;         // 要编辑的文件名
        private List<string> lines;      // 文件内容的行列表
        private int currentLine;         // 当前光标所在行
        private int currentColumn;       // 当前光标所在列
        private bool isRunning;          // 编辑器是否正在运行
        private FileSystem fileSystem;   // 文件系统实例，用于读写文件

        // 构造函数
        // 参数：fileName - 要编辑的文件名
        // 参数：fs - 文件系统实例
        public Editor(string fileName, FileSystem fs)
        {
            this.fileName = fileName;
            this.fileSystem = fs;
            this.lines = new List<string>();
            this.currentLine = 0;
            this.currentColumn = 0;
            this.isRunning = true;
            
            LoadFile();
        }

        // 运行编辑器
        // 主循环：渲染界面 -> 处理输入 -> 重复
        public void Run()
        {
            while (isRunning)
            {
                Render();
                HandleInput();
            }
        }

        // 加载文件内容
        private void LoadFile()
        {
            try
            {
                // 尝试从文件系统读取文件内容
                string content = fileSystem.ReadFile(fileName);
                if (!string.IsNullOrEmpty(content))
                {
                    // 分割内容为行
                    string[] fileLines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (string line in fileLines)
                    {
                        lines.Add(line);
                    }
                }
                else
                {
                    // 文件不存在或为空，创建新文件
                    lines.Add("");
                }
            }
            catch
            {
                // 文件不存在，创建新文件
                lines.Add("");
            }
        }

        // 保存文件内容
        private void SaveFile()
        {
            try
            {
                // 简化实现，避免可能导致栈损坏的操作
                string content = "";
                for (int i = 0; i < lines.Count; i++)
                {
                    content += lines[i];
                    if (i < lines.Count - 1)
                    {
                        content += "\n";
                    }
                }
                
                // 直接使用CreateFile，因为它会覆盖现有文件
                try
                {
                    fileSystem.CreateFile(fileName, content);
                    Console.WriteLine($"File saved: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveFile: {ex.Message}");
            }
        }

        // 渲染编辑器界面
        private void Render()
        {
            Console.Clear();
            
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            
            int consoleWidth = 80;
            
            string titleLine = $"CMLeonOS Editor - {fileName}".PadRight(consoleWidth);
            Console.WriteLine(titleLine);
            
            string separatorLine = new string('-', consoleWidth);
            Console.WriteLine(separatorLine);
            
            string commandLine = "Commands: Esc = Exit".PadRight(consoleWidth);
            Console.WriteLine(commandLine);
            
            Console.ResetColor();
            
            int maxVisibleLines = 15;
            int startLine = Math.Max(0, currentLine - maxVisibleLines / 2);
            int endLine = Math.Min(lines.Count, startLine + maxVisibleLines);
            
            for (int i = startLine; i < endLine; i++)
            {
                string line = lines[i];
                Console.WriteLine(line);
            }
            
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            
            string cursorLine = $"Cursor: Line {currentLine + 1}, Column {currentColumn + 1}".PadRight(consoleWidth);
            Console.WriteLine(cursorLine);
            
            string linesLine = $"Lines: {lines.Count}".PadRight(consoleWidth);
            Console.WriteLine(linesLine);
            
            Console.ResetColor();
            
            int consoleHeight = 25;
            int titleLines = 3;
            int visibleLines = endLine - startLine;
            int cursorY = titleLines + (currentLine - startLine);
            
            cursorY = Math.Max(titleLines, Math.Min(cursorY, consoleHeight - 5));
            
            try
            {
                Console.SetCursorPosition(currentColumn, cursorY);
            }
            catch
            {
            }
        }

        // 处理用户输入
        private void HandleInput()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    // 显示保存确认弹窗
                    ShowSaveConfirmation();
                    break;
                case ConsoleKey.Backspace:
                    // 删除字符
                    if (currentColumn > 0)
                    {
                        string backspaceLineText = lines[currentLine];
                        string backspaceModifiedLine = backspaceLineText.Remove(currentColumn - 1, 1);
                        lines[currentLine] = backspaceModifiedLine;
                        currentColumn--;
                    }
                    else if (currentLine > 0)
                    {
                        // 如果在行首，合并到上一行
                        string previousLine = lines[currentLine - 1];
                        string lineText = lines[currentLine];
                        lines.RemoveAt(currentLine);
                        currentLine--;
                        currentColumn = previousLine.Length;
                        lines[currentLine] = previousLine + lineText;
                    }
                    break;
                case ConsoleKey.Enter:
                    // 插入换行
                    string enterLineText = lines[currentLine];
                    string enterFirstPart = enterLineText.Substring(0, currentColumn);
                    string enterSecondPart = enterLineText.Substring(currentColumn);
                    lines[currentLine] = enterFirstPart;
                    lines.Insert(currentLine + 1, enterSecondPart);
                    currentLine++;
                    currentColumn = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    // 左移光标
                    if (currentColumn > 0)
                    {
                        currentColumn--;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    // 右移光标
                    if (currentColumn < lines[currentLine].Length)
                    {
                        currentColumn++;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    // 上移光标
                    if (currentLine > 0)
                    {
                        currentLine--;
                        currentColumn = Math.Min(currentColumn, lines[currentLine].Length);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    // 下移光标
                    if (currentLine < lines.Count - 1)
                    {
                        currentLine++;
                        currentColumn = Math.Min(currentColumn, lines[currentLine].Length);
                    }
                    break;
                case ConsoleKey.Tab:
                    // Tab键插入四个空格
                    string tabLineText = lines[currentLine];
                    string tabSpaces = "    ";
                    string tabModifiedLine = tabLineText.Insert(currentColumn, tabSpaces);
                    lines[currentLine] = tabModifiedLine;
                    currentColumn += 4;
                    break;
                default:
                    // 输入字符
                    if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar) || char.IsSymbol(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
                    {
                        string defaultLineText = lines[currentLine];
                        string defaultModifiedLine = defaultLineText.Insert(currentColumn, keyInfo.KeyChar.ToString());
                        lines[currentLine] = defaultModifiedLine;
                        currentColumn++;
                    }
                    break;
            }
        }

        // 显示保存确认弹窗
        private void ShowSaveConfirmation()
        {
            Console.Clear();
            
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            
            int consoleWidth = 80;
            int consoleHeight = 25;
            
            string[] popupLines = {
                "+----------------------------+",
                "|          Save File?        |",
                "+----------------------------+",
                "| Do you want to save the    |",
                "| changes?                   |",
                "+----------------------------+",
                "| Y = Yes, N = No            |",
                "+----------------------------+"
            };
            
            int popupWidth = 30;
            int popupHeight = popupLines.Length;
            
            int popupX = (consoleWidth - popupWidth) / 2;
            int popupY = (consoleHeight - popupHeight) / 2;
            
            for (int i = 0; i < popupLines.Length; i++)
            {
                Console.SetCursorPosition(popupX, popupY + i);
                Console.WriteLine(popupLines[i]);
            }
            
            while (true)
            {
                ConsoleKeyInfo keyInfo;
                try
                {
                    keyInfo = Console.ReadKey(true);
                }
                catch
                {
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    return;
                }
                
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    try
                    {
                        SaveFile();
                    }
                    catch
                    {
                        Console.WriteLine("Error saving file");
                    }
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.N)
                {
                    Console.ResetColor();
                    Console.Clear();
                    isRunning = false;
                    break;
                }
            }
        }
    }
}
