using System;

namespace CMLeonOS.Commands
{
    public static class HelpCommand
    {
        private static readonly string[] allCommands = {
            "  echo <text>      - Display text (supports \\n for newline)",
            "  clear/cls        - Clear the screen",
            "  restart          - Restart the system",
            "  shutdown         - Shutdown the system",
            "  time             - Display current time",
            "  date             - Display current date",
            "  prompt <text>    - Change command prompt",
            "  calc <expr>      - Simple calculator",
            "  history          - Show command history",
            "  background <hex> - Change background color",
            "  cuitest          - Test CUI framework",
            "  edit <file>      - Simple code editor",
            "                     Tab key inserts 4 spaces",
            "  ls <dir>         - List files and directories",
            "  cd <dir>         - Change directory",
            "                     cd ..              - Go to parent directory",
            "                     cd dir1/dir2/dir3  - Go to numbered directory",
            "  pwd              - Show current directory",
            "  mkdir <dir>      - Create directory",
            "  rm <file>        - Remove file",
            "                     Use -norisk to delete files in sys folder",
            "  rmdir <dir>      - Remove directory",
            "  cat <file>       - Display file content",
            "  echo <text> > <file> - Write text to file",
            "  head <file>      - Display first lines of file",
            "                     Usage: head <file> <lines>",
            "  tail <file>      - Display last lines of file",
            "                     Usage: tail <file> <lines>",
            "  wc <file>        - Count lines, words, characters",
            "  cp <src> <dst>   - Copy file",
            "  mv <src> <dst>   - Move/rename file",
            "  touch <file>     - Create empty file",
            "  find <name>      - Find file",
            "  getdisk          - Show disk information",
            "  user <cmd>       - User management",
            "                     user add admin <username> <password>    - Add admin user",
            "                     user add user <username> <password>     - Add regular user",
            "                     user delete <username>                  - Delete user",
            "                     user list                               - List all users",
            "  cpass            - Change password",
            "  env <cmd>       - Environment variables",
            "                     env see <varname>                    - Show variable value",
            "                     env change <varname> <value>           - Set variable value",
            "                     env delete <varname>                  - Delete variable",
            "  beep             - Play beep sound",
            "  uptime           - Show system uptime",
            "  branswe <filename> - Execute Branswe code file",
            "  backup <name>    - Backup system files",
            "  restore <name>  - Restore system files",
            "  grep <pattern> <file> - Search text in file",
            "  ping <ip>        - Ping IP address (5 times)",
            "  tcpserver <port> - Start TCP server on specified port",
            "  tcpclient <ip> <port> - Connect to TCP server",
            "  wget <url>       - Download file from URL",
            "  setdns <ip>     - Set DNS server",
            "  setgateway <ip>  - Set gateway",
            "  ipconfig         - Show network configuration",
            "  nslookup <domain> - DNS lookup",
            "  whoami          - Show current username",
            "  base64 encrypt <text> - Encode text to Base64",
            "  base64 decrypt <text> - Decode Base64 to text",
            "  lua <file>       - Execute Lua script",
            "  version          - Show OS version",
            "  about            - Show about information",
            "  help <page>      - Show help page (1-3)",
            "  help all          - Show all help pages",
            "",
            "Startup Script: sys\\startup.cm",
            "  Commands in this file will be executed on startup",
            "  Each line should contain one command",
            "  Lines starting with # are treated as comments"
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
            
            int totalPages = (int)Math.Ceiling((double)allCommands.Length / CommandsPerPage);
            
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
                Console.WriteLine(cmd);
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
            int endIndex = Math.Min(startIndex + CommandsPerPage, allCommands.Length);
            
            Console.WriteLine("====================================");
            Console.WriteLine($"        Help - Page {pageNumber}/{totalPages}");
            Console.WriteLine("====================================");
            Console.WriteLine();
            
            for (int i = startIndex; i < endIndex; i++)
            {
                Console.WriteLine(allCommands[i]);
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
    }
}
