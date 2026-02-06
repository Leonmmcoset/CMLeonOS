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
            public string[] SubCommands { get; set; }
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
                Command = "edit",
                Parameters = "<file>",
                Description = "Simple code editor",
                SubCommands = new[] { "Tab key inserts 4 spaces" }
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
                    "cd ..              - Go to parent directory",
                    "cd dir1/dir2/dir3  - Go to numbered directory"
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
                SubCommands = new[] { "Use -norisk to delete files in sys folder" }
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
                SubCommands = new[] { "Usage: head <file> <lines>" }
            },
            new CommandInfo
            {
                Command = "tail",
                Parameters = "<file>",
                Description = "Display last lines of file",
                SubCommands = new[] { "Usage: tail <file> <lines>" }
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
                    "user add admin <username> <password>    - Add admin user",
                    "user add user <username> <password>     - Add regular user",
                    "user delete <username>                  - Delete user",
                    "user list                               - List all users"
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
                    "env see <varname>                    - Show variable value",
                    "env change <varname> <value>           - Set variable value",
                    "env delete <varname>                  - Delete variable"
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
                Command = "branswe",
                Parameters = "<filename>",
                Description = "Execute Branswe code file"
            },
            new CommandInfo
            {
                Command = "backup",
                Parameters = "<name>",
                Description = "Backup system files"
            },
            new CommandInfo
            {
                Command = "restore",
                Parameters = "<name>",
                Description = "Restore system files"
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
                SubCommands = new[] { "alias                  - List all aliases" }
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
                SubCommands = new[] { "settings              - List all settings" }
            },
            new CommandInfo
            {
                Command = "help",
                Parameters = "<page>",
                Description = "Show help page (1-3)",
                SubCommands = new[] { "help all          - Show all help pages" }
            }
        };

        private const int CommandsPerPage = 15;

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
            
            int totalPages = (int)Math.Ceiling((double)allCommands.Count / CommandsPerPage);
            
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
            
            for (int i = startIndex; i < endIndex; i++)
            {
                DisplayCommand(allCommands[i]);
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
            
            string commandPart = PadRight(cmd.Command, maxCommandWidth);
            string paramPart = PadRight(cmd.Parameters, maxParamWidth);
            
            Console.WriteLine($"  {commandPart} {paramPart} - {cmd.Description}");
            
            if (cmd.SubCommands != null)
            {
                foreach (var subCmd in cmd.SubCommands)
                {
                    Console.WriteLine($"                     {subCmd}");
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
