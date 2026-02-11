# Diff Details

Date : 2026-02-11 18:21:42

Directory c:\\Users\\leon\\source\\repos\\CMLeonOS\\CMLeonOS

Total : 50 files,  3842 codes, 27 comments, 588 blanks, all 4457 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Build.bat](/Build.bat) | Batch | 1 | 1 | 0 | 2 |
| [CMLeonOS.csproj](/CMLeonOS.csproj) | XML | 35 | 0 | 8 | 43 |
| [GenerateGitCommit.ps1](/GenerateGitCommit.ps1) | PowerShell | 22 | 0 | 3 | 25 |
| [Interpreter/UniLua/LuaOsLib.cs](/Interpreter/UniLua/LuaOsLib.cs) | C# | 103 | 0 | 17 | 120 |
| [Kernel.cs](/Kernel.cs) | C# | 121 | 30 | 24 | 175 |
| [LuaApps.cs](/LuaApps.cs) | C# | 13 | 0 | 3 | 16 |
| [LuaApps/calculator.lua](/LuaApps/calculator.lua) | Lua | 87 | 0 | 11 | 98 |
| [LuaApps/helloworld.lua](/LuaApps/helloworld.lua) | Lua | 3 | 0 | 0 | 3 |
| [LuaApps/testspeed.lua](/LuaApps/testspeed.lua) | Lua | 6 | 1 | 0 | 7 |
| [Shell/CommandList.cs](/Shell/CommandList.cs) | C# | 15 | 0 | 0 | 15 |
| [Shell/Commands/Environment/EnvCommand.cs](/Shell/Commands/Environment/EnvCommand.cs) | C# | 68 | 0 | 2 | 70 |
| [Shell/Commands/FileSystem/RmCommand.cs](/Shell/Commands/FileSystem/RmCommand.cs) | C# | 18 | 0 | 1 | 19 |
| [Shell/Commands/Help/Help.cs](/Shell/Commands/Help/Help.cs) | C# | 24 | 0 | 0 | 24 |
| [Shell/Commands/Info/Version.cs](/Shell/Commands/Info/Version.cs) | C# | 1 | 0 | 0 | 1 |
| [Shell/Commands/Script/BransweCommand.cs](/Shell/Commands/Script/BransweCommand.cs) | C# | 10 | 0 | 1 | 11 |
| [Shell/Commands/Script/ComCommand.cs](/Shell/Commands/Script/ComCommand.cs) | C# | 11 | 0 | 1 | 12 |
| [Shell/Commands/Script/LuaCommand.cs](/Shell/Commands/Script/LuaCommand.cs) | C# | 16 | 0 | 1 | 17 |
| [Shell/Commands/TestTUICommand.cs](/Shell/Commands/TestTUICommand.cs) | C# | 583 | 0 | 102 | 685 |
| [Shell/Commands/User/HostnameCommand.cs](/Shell/Commands/User/HostnameCommand.cs) | C# | 11 | 0 | 1 | 12 |
| [Shell/Commands/User/UserCommand.cs](/Shell/Commands/User/UserCommand.cs) | C# | 23 | 0 | 1 | 24 |
| [Shell/Commands/Utility/AppManagerCommand.cs](/Shell/Commands/Utility/AppManagerCommand.cs) | C# | 165 | 0 | 30 | 195 |
| [Shell/Commands/Utility/Base64Command.cs](/Shell/Commands/Utility/Base64Command.cs) | C# | 31 | 0 | 2 | 33 |
| [Shell/Commands/Utility/CalcGuiCommand.cs](/Shell/Commands/Utility/CalcGuiCommand.cs) | C# | 307 | 0 | 28 | 335 |
| [Shell/Commands/Utility/MatrixCommand.cs](/Shell/Commands/Utility/MatrixCommand.cs) | C# | 62 | 0 | 13 | 75 |
| [Shell/Commands/Utility/SnakeCommand.cs](/Shell/Commands/Utility/SnakeCommand.cs) | C# | 164 | 0 | 27 | 191 |
| [Shell/Shell.cs](/Shell/Shell.cs) | C# | 79 | 0 | 13 | 92 |
| [Shell/UsageGenerator.cs](/Shell/UsageGenerator.cs) | C# | 211 | 0 | 31 | 242 |
| [System/FileSystem.cs](/System/FileSystem.cs) | C# | 4 | 4 | 0 | 8 |
| [System/UserSystem.cs](/System/UserSystem.cs) | C# | 8 | -11 | -1 | -4 |
| [UI/Components.cs](/UI/Components.cs) | C# | 960 | 0 | 144 | 1,104 |
| [UI/TUI.cs](/UI/TUI.cs) | C# | 260 | 2 | 42 | 304 |
| [UI/TUIDemo.cs](/UI/TUIDemo.cs) | C# | 279 | 0 | 54 | 333 |
| [Utils/Version.cs](/Utils/Version.cs) | C# | 5 | 0 | 1 | 6 |
| [docs/cmleonos/docs/.vuepress/dist/assets/404.html-B-2P5V78.js](/docs/cmleonos/docs/.vuepress/dist/assets/404.html-B-2P5V78.js) | JavaScript | -1 | 0 | -1 | -2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/404.html-ByTdGTSm.js](/docs/cmleonos/docs/.vuepress/dist/assets/404.html-ByTdGTSm.js) | JavaScript | 1 | 0 | 1 | 2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/app-BITa06va.js](/docs/cmleonos/docs/.vuepress/dist/assets/app-BITa06va.js) | JavaScript | -28 | -4 | -1 | -33 |
| [docs/cmleonos/docs/.vuepress/dist/assets/app-C1vKFkGc.js](/docs/cmleonos/docs/.vuepress/dist/assets/app-C1vKFkGc.js) | JavaScript | 28 | 4 | 1 | 33 |
| [docs/cmleonos/docs/.vuepress/dist/assets/commands.html-CJdMGWM7.js](/docs/cmleonos/docs/.vuepress/dist/assets/commands.html-CJdMGWM7.js) | JavaScript | -143 | 0 | -1 | -144 |
| [docs/cmleonos/docs/.vuepress/dist/assets/commands.html-DsR2axk8.js](/docs/cmleonos/docs/.vuepress/dist/assets/commands.html-DsR2axk8.js) | JavaScript | 153 | 0 | 1 | 154 |
| [docs/cmleonos/docs/.vuepress/dist/assets/get-started.html-CfHiKCXZ.js](/docs/cmleonos/docs/.vuepress/dist/assets/get-started.html-CfHiKCXZ.js) | JavaScript | 1 | 0 | 1 | 2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/get-started.html-D\_vM18Nz.js](/docs/cmleonos/docs/.vuepress/dist/assets/get-started.html-D_vM18Nz.js) | JavaScript | -1 | 0 | -1 | -2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/index.html-BMbDjf\_W.js](/docs/cmleonos/docs/.vuepress/dist/assets/index.html-BMbDjf_W.js) | JavaScript | 1 | 0 | 1 | 2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/index.html-DS7rXIvH.js](/docs/cmleonos/docs/.vuepress/dist/assets/index.html-DS7rXIvH.js) | JavaScript | -1 | 0 | -1 | -2 |
| [docs/cmleonos/docs/.vuepress/dist/assets/lua.html-3\_L4TJjQ.js](/docs/cmleonos/docs/.vuepress/dist/assets/lua.html-3_L4TJjQ.js) | JavaScript | 303 | 0 | 1 | 304 |
| [docs/cmleonos/docs/.vuepress/dist/assets/lua.html-DxDsLuB0.js](/docs/cmleonos/docs/.vuepress/dist/assets/lua.html-DxDsLuB0.js) | JavaScript | -297 | 0 | -1 | -298 |
| [docs/cmleonos/docs/.vuepress/dist/commands.html](/docs/cmleonos/docs/.vuepress/dist/commands.html) | HTML | 10 | 0 | 0 | 10 |
| [docs/cmleonos/docs/.vuepress/dist/lua.html](/docs/cmleonos/docs/.vuepress/dist/lua.html) | HTML | 6 | 0 | 0 | 6 |
| [docs/cmleonos/docs/commands.md](/docs/cmleonos/docs/commands.md) | Markdown | 46 | 0 | 10 | 56 |
| [docs/cmleonos/docs/get-started.md](/docs/cmleonos/docs/get-started.md) | Markdown | 2 | 0 | 1 | 3 |
| [docs/cmleonos/docs/lua.md](/docs/cmleonos/docs/lua.md) | Markdown | 56 | 0 | 17 | 73 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details