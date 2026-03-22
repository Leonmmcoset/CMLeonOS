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

using System;
using System.Collections.Generic;

namespace CMLeonOS.Commands.FileSystem
{
    public static class TUITreeCommand
    {
        public static void ShowTUITree(CMLeonOS.FileSystem fileSystem, string args, Action<string> showError)
        {
            string startPath = string.IsNullOrEmpty(args) ? "." : args;
            string fullPath = fileSystem.GetFullPath(startPath);
            
            if (!global::System.IO.Directory.Exists(fullPath))
            {
                showError($"Directory not found: {startPath}");
                return;
            }
            
            try
            {
                var rootNode = BuildTree(fileSystem, fullPath, "");
                if (rootNode == null)
                {
                    showError("No files or directories found.");
                    return;
                }

                Console.Clear();
                Console.WriteLine($"Tree View: {fullPath}");
                Console.WriteLine("Press ESC to exit");
                Console.WriteLine();

                var treeView = new CMLeonOS.UI.TreeView(new CMLeonOS.UI.Rect(5, 4, Console.WindowWidth - 10, Console.WindowHeight - 8));
                treeView.Root = rootNode;
                treeView.BackgroundColor = ConsoleColor.Black;
                treeView.NormalColor = ConsoleColor.White;
                treeView.SelectedColor = ConsoleColor.Yellow;
                treeView.BorderColor = ConsoleColor.Gray;

                if (rootNode.Children.Count > 0)
                {
                    treeView.SelectedNodes.Add(rootNode.Children[0]);
                }

                treeView.Render();

                bool running = true;
                while (running)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                    {
                        running = false;
                    }
                    else if (treeView.HandleKey(key))
                    {
                        treeView.Render();
                    }
                }

                Console.Clear();
            }
            catch (Exception ex)
            {
                showError($"Error displaying tree: {ex.Message}");
            }
        }

        private static CMLeonOS.UI.TreeViewNode BuildTree(CMLeonOS.FileSystem fileSystem, string path, string prefix)
        {
            var node = new CMLeonOS.UI.TreeViewNode(prefix + global::System.IO.Path.GetFileName(path));

            try
            {
                var dirs = fileSystem.GetFullPathDirectoryList(path);
                foreach (var dir in dirs)
                {
                    var childNode = BuildTree(fileSystem, dir, prefix + "  ");
                    if (childNode != null)
                    {
                        node.Children.Add(childNode);
                    }
                }

                var files = fileSystem.GetFullPathFileList(path);
                foreach (var file in files)
                {
                    var fileNode = new CMLeonOS.UI.TreeViewNode(prefix + "  " + global::System.IO.Path.GetFileName(file));
                    node.Children.Add(fileNode);
                }
            }
            catch
            {
            }

            if (node.Children.Count == 0)
            {
                return null;
            }

            return node;
        }
    }
}
