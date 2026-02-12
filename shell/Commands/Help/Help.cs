using System;
using System.Collections.Generic;

namespace CMLeonOS.Commands
{
    public static class HelpCommand
    {
        private class CommandInfo
        {
            public string Command { get; set; }
            public string Parameters { get; set; }
            public string Description { get; set; }
            public SubCommandInfo[] SubCommands { get; set; }
        }

        private class SubCommandInfo
        {
            public string Command { get; set; }
            public string Description { get; set; }
        }

        private static readonly List<CommandInfo> allCommands = new List<CommandInfo>
        {
            new CommandInfo
            {
                Command = "echo",
                Parameters = "<text>",
                Description = "Display text (supports \\n for newline)"
            },
            new CommandInfo
            {
                Command = "clear/cls",
                Parameters = "",
                Description = "Clear screen"
            },
            new CommandInfo
            {
                Command = "restart",
                Parameters = "",
                Description = "Restart system"
            },
            new CommandInfo
            {
                Command = "shutdown",
                Parameters = "",
                Description = "Shutdown system"
            },
            new CommandInfo
            {
                Command = "time",
                Parameters = "",
                Description = "Display current time"
            },
            new CommandInfo
            {
                Command = "date",
                Parameters = "",
                Description = "Display current date"
            },
            new CommandInfo
            {
                Command = "prompt",
                Parameters = "<text>",
                Description = "Change command prompt"
            },
            new CommandInfo
            {
                Command = "calc",
                Parameters = "<expr>",
                Description = "Simple calculator"
            },
            new CommandInfo
            {
                Command = "calcgui",
                Parameters = "",
                Description = "TUI calculator"
            },
            new CommandInfo
            {
                Command = "history",
                Parameters = "",
                Description = "Show command history"
            },
            new CommandInfo
            {
                Command = "background",
                Parameters = "<hex>",
                Description = "Change background color"
            },
            new CommandInfo
            {
                Command = "cuitest",
                Parameters = "",
                Description = "Test CUI framework"
            },
            new CommandInfo
            {
                Command = "labyrinth",
                Parameters = "",
                Description = "Play maze escape game"
            },
            new CommandInfo
            {
                Command = "snake",
                Parameters = "",
                Description = "Play snake game"
            },
            new CommandInfo
            {
                Command = "edit",
                Parameters = "<file>",
                Description = "Simple code editor",
                SubCommands = new[] { new SubCommandInfo { Command = "", Description = "Tab key inserts 4 spaces" } }
            },
            new CommandInfo
            {
                Command = "ls",
                Parameters = "<dir>",
                Description = "List files and directories"
            },
            new CommandInfo
            {
                Command = "cd",
                Parameters = "<dir>",
                Description = "Change directory",
                SubCommands = new[] 
                { 
                    new SubCommandInfo { Command = "cd ..", Description = "Go to parent directory" },
                    new SubCommandInfo { Command = "cd dir1/dir2/dir3", Description = "Go to numbered directory" }
                }
            },
            new CommandInfo
            {
                Command = "pwd",
                Parameters = "",
                Description = "Show current directory"
            },
            new CommandInfo
            {
                Command = "mkdir",
                Parameters = "<dir>",
                Description = "Create directory"
            },
            new CommandInfo
            {
                Command = "rm",
                Parameters = "<file>",
                Description = "Remove file",
                SubCommands = new[] { new SubCommandInfo { Command = "", Description = "Use -norisk to delete files in sys folder" } }
            },
            new CommandInfo
            {
                Command = "rmdir",
                Parameters = "<dir>",
                Description = "Remove directory"
            },
            new CommandInfo
            {
                Command = "cat",
                Parameters = "<file>",
                Description = "Display file content"
            },
            new CommandInfo
            {
                Command = "echo",
                Parameters = "<text> > <file>",
                Description = "Write text to file"
            },
            new CommandInfo
            {
                Command = "head",
                Parameters = "<file>",
                Description = "Display first lines of file",
                SubCommands = new[] { new SubCommandInfo { Command = "head <file> <lines>", Description = "Usage" } }
            },
            new CommandInfo
            {
                Command = "tail",
                Parameters = "<file>",
                Description = "Display last lines of file",
                SubCommands = new[] { new SubCommandInfo { Command = "tail <file> <lines>", Description = "Usage" } }
            },
            new CommandInfo
            {
                Command = "wc",
                Parameters = "<file>",
                Description = "Count lines, words, characters"
            },
            new CommandInfo
            {
                Command = "cp",
                Parameters = "<src> <dst>",
                Description = "Copy file"
            },
            new CommandInfo
            {
                Command = "mv",
                Parameters = "<src> <dst>",
                Description = "Move/rename file"
            },
            new CommandInfo
            {
                Command = "touch",
                Parameters = "<file>",
                Description = "Create empty file"
            },
            new CommandInfo
            {
                Command = "find",
                Parameters = "<name>",
                Description = "Find file"
            },
            new CommandInfo
            {
                Command = "getdisk",
                Parameters = "",
                Description = "Show disk information"
            },
            new CommandInfo
            {
                Command = "user",
                Parameters = "<cmd>",
                Description = "User management",
                SubCommands = new[]
                {
                    new SubCommandInfo { Command = "user add admin <username> <password>", Description = "Add admin user" },
                    new SubCommandInfo { Command = "user add user <username> <password>", Description = "Add regular user" },
                    new SubCommandInfo { Command = "user delete <username>", Description = "Delete user" },
                    new SubCommandInfo { Command = "user list", Description = "List all users" }
                }
            },
            new CommandInfo
            {
                Command = "cpass",
                Parameters = "",
                Description = "Change password"
            },
            new CommandInfo
            {
                Command = "env",
                Parameters = "<cmd>",
                Description = "Environment variables",
                SubCommands = new[]
                {
                    new SubCommandInfo { Command = "env see <varname>", Description = "Show variable value" },
                    new SubCommandInfo { Command = "env change <varname> <value>", Description = "Set variable value" },
                    new SubCommandInfo { Command = "env delete <varname>", Description = "Delete variable" }
                }
            },
            new CommandInfo
            {
                Command = "beep",
                Parameters = "",
                Description = "Play beep sound"
            },
            new CommandInfo
            {
                Command = "uptime",
                Parameters = "",
                Description = "Show system uptime"
            },
            new CommandInfo
            {
                Command = "matrix",
                Parameters = "",
                Description = "Show Matrix effect (The Matrix movie)"
            },
            new CommandInfo
            {
                Command = "app",
                Parameters = "<command>",
                Description = "Application manager"
            },
            new CommandInfo
            {
                Command = "branswe",
                Parameters = "<filename>",
                Description = "Execute Branswe code file"
            },
            new CommandInfo
            {
                Command = "grep",
                Parameters = "<pattern> <file>",
                Description = "Search text in file"
            },
            new CommandInfo
            {
                Command = "ping",
                Parameters = "<ip>",
                Description = "Ping IP address (5 times)"
            },
            new CommandInfo
            {
                Command = "tcpserver",
                Parameters = "<port>",
                Description = "Start TCP server on specified port"
            },
            new CommandInfo
            {
                Command = "tcpclient",
                Parameters = "<ip> <port>",
                Description = "Connect to TCP server"
            },
            new CommandInfo
            {
                Command = "wget",
                Parameters = "<url>",
                Description = "Download file from URL"
            },
            new CommandInfo
            {
                Command = "setdns",
                Parameters = "<ip>",
                Description = "Set DNS server"
            },
            new CommandInfo
            {
                Command = "setgateway",
                Parameters = "<ip>",
                Description = "Set gateway"
            },
            new CommandInfo
            {
                Command = "ipconfig",
                Parameters = "",
                Description = "Show network configuration"
            },
            new CommandInfo
            {
                Command = "nslookup",
                Parameters = "<domain>",
                Description = "DNS lookup"
            },
            new CommandInfo
            {
                Command = "whoami",
                Parameters = "",
                Description = "Show current username"
            },
            new CommandInfo
            {
                Command = "base64",
                Parameters = "encrypt <text>",
                Description = "Encode text to Base64"
            },
            new CommandInfo
            {
                Command = "base64",
                Parameters = "decrypt <text>",
                Description = "Decode Base64 to text"
            },
            new CommandInfo
            {
                Command = "alias",
                Parameters = "<name> <cmd>",
                Description = "Create command alias",
                SubCommands = new[] { new SubCommandInfo { Command = "alias", Description = "List all aliases" } }
            },
            new CommandInfo
            {
                Command = "unalias",
                Parameters = "<name>",
                Description = "Remove command alias"
            },
            new CommandInfo
            {
                Command = "lua",
                Parameters = "<file>",
                Description = "Execute Lua script"
            },
            new CommandInfo
            {
                Command = "version",
                Parameters = "",
                Description = "Show OS version"
            },
            new CommandInfo
            {
                Command = "about",
                Parameters = "",
                Description = "Show about information"
            },
            new CommandInfo
            {
                Command = "settings",
                Parameters = "<key> [value]",
                Description = "View or modify system settings",
                SubCommands = new[] { new SubCommandInfo { Command = "settings", Description = "List all settings" } }
            },
            new CommandInfo
            {
                Command = "help",
                Parameters = "<page>",
                Description = "Show help page (1-3)",
                SubCommands = new[] { new SubCommandInfo { Command = "help all", Description = "Show all help pages" } }
            }
        };

        private const int CommandsPerPage = 15;

        private static int GetCommandLinesCount(CommandInfo cmd)
        {
            int lines = 1;
            if (cmd.SubCommands != null && cmd.SubCommands.Length > 0)
            {
                lines += cmd.SubCommands.Length;
            }
            return lines;
        }

        public static void ProcessHelp(string args)
        {
            string[] helpArgs = args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int pageNumber = 1;
            bool showAll = false;
            
            if (helpArgs.Length > 0)
            {
                if (helpArgs[0].ToLower() == "all")
                {
                    showAll = true;
                }
                else if (int.TryParse(helpArgs[0], out int page))
                {
                    pageNumber = page;
                }
            }
            
            int totalLines = 0;
            foreach (var cmd in allCommands)
            {
                totalLines += GetCommandLinesCount(cmd);
            }
            
            int totalPages = (int)Math.Ceiling((double)totalLines / CommandsPerPage);
            
            if (showAll)
            {
                ShowAllPages();
            }
            else
            {
                ShowPage(pageNumber, totalPages);
            }
        }

        private static void ShowAllPages()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("        Help - All Pages");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            foreach (var cmd in allCommands)
            {
                DisplayCommand(cmd);
            }
            
            Console.WriteLine();
            Console.WriteLine("-- End of help --");
        }

        private static void ShowPage(int pageNumber, int totalPages)
        {
            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            
            int startIndex = (pageNumber - 1) * CommandsPerPage;
            int endIndex = Math.Min(startIndex + CommandsPerPage, allCommands.Count);
            
            Console.WriteLine("====================================");
            Console.WriteLine($"        Help - Page {pageNumber}/{totalPages}");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            int linesOnPage = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                int cmdLines = GetCommandLinesCount(allCommands[i]);
                if (linesOnPage + cmdLines > CommandsPerPage)
                {
                    break;
                }
                DisplayCommand(allCommands[i]);
                linesOnPage += cmdLines;
            }
            
            if (pageNumber < totalPages)
            {
                Console.WriteLine();
                Console.WriteLine($"-- More -- Type 'help {pageNumber + 1}' for next page or 'help all' for all pages --");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("-- End of help --");
            }
        }

        private static void DisplayCommand(CommandInfo cmd)
        {
            int maxCommandWidth = 18;
            int maxParamWidth = 18;
            int subCommandIndent = 21;
            int maxSubCommandWidth = 17;
            
            string commandPart = PadRight(cmd.Command, maxCommandWidth);
            string paramPart = PadRight(cmd.Parameters, maxParamWidth);
            
            Console.WriteLine($"  {commandPart} {paramPart} - {cmd.Description}");
            
            if (cmd.SubCommands != null && cmd.SubCommands.Length > 0)
            {
                string indent = new string(' ', subCommandIndent);
                foreach (var subCmd in cmd.SubCommands)
                {
                    string subCmdPart = PadRight(subCmd.Command, maxSubCommandWidth);
                    Console.WriteLine($"{indent}{subCmdPart} - {subCmd.Description}");
                }
            }
        }

        private static string PadRight(string str, int totalWidth)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new string(' ', totalWidth);
            }
            
            if (str.Length >= totalWidth)
            {
                return str;
            }
            
            return str + new string(' ', totalWidth - str.Length);
        }
    }
}
