 
namespace UniLua
{
	using System.Diagnostics;
	using System;
	using CMLeonOS;
	using CMLeonOS.UI;
	using Sys = Cosmos.System;
	using System.Threading;
	
	internal class LuaOSLib
	{
		public const string LIB_NAME = "os";

		private static DateTime? _timerStartTime;

		public static int OpenLib( ILuaState lua )
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
#if !UNITY_WEBPLAYER
				new NameFuncPair("clock",	OS_Clock),
				new NameFuncPair("gethostname", OS_Gethostname),
				new NameFuncPair("getenv", OS_Getenv),
				new NameFuncPair("setenv", OS_Setenv),
				new NameFuncPair("delenv", OS_Delenv),
				new NameFuncPair("addenv", OS_Addenv),
				new NameFuncPair("execute", OS_Execute),
				new NameFuncPair("executefile", OS_Executefile),
				new NameFuncPair("reboot", OS_Reboot),
				new NameFuncPair("shutdown", OS_Shutdown),
				new NameFuncPair("sleep", OS_Sleep),
				new NameFuncPair("beep", OS_Beep),
				new NameFuncPair("clear", OS_Clear),
				new NameFuncPair("getusername", OS_Getusername),
				new NameFuncPair("isadmin", OS_Isadmin),
				new NameFuncPair("sha256", OS_Sha256),
				new NameFuncPair("base64encrypt", OS_Base64Encrypt),
				new NameFuncPair("base64decrypt", OS_Base64Decrypt),
				new NameFuncPair("timerstart", OS_TimerStart),
				new NameFuncPair("timerstop", OS_TimerStop),
				new NameFuncPair("tui_drawbox", OS_TUIDrawBox),
				new NameFuncPair("tui_drawtext", OS_TUIDrawText),
				new NameFuncPair("tui_setcolor", OS_TUISetColor),
				new NameFuncPair("tui_setcursor", OS_TUISetCursor),
				new NameFuncPair("tui_clear", OS_TUIClear),
				new NameFuncPair("tui_drawline", OS_TUIDrawLine),
#endif
			};

			lua.L_NewLib( define );
 
			return 1;
		}

#if !UNITY_WEBPLAYER
		private static int OS_Clock( ILuaState lua )
		{
			lua.PushNumber(0);
			return 1;
		}

		private static int OS_Gethostname( ILuaState lua )
		{
			string hostname = CMLeonOS.Kernel.userSystem?.GetHostname() ?? "Not set";
			lua.PushString(hostname);
			return 1;
		}

		private static int OS_Getenv( ILuaState lua )
		{
			string varName = lua.L_CheckString(1);
			string varValue = EnvironmentVariableManager.Instance.GetVariable(varName);
			if (varValue == null)
			{
				lua.PushNil();
			}
			else
			{
				lua.PushString(varValue);
			}
			return 1;
		}

		private static int OS_Setenv( ILuaState lua )
		{
			string varName = lua.L_CheckString(1);
			string varValue = lua.L_CheckString(2);
			EnvironmentVariableManager.Instance.SetVariable(varName, varValue);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Delenv( ILuaState lua )
		{
			string varName = lua.L_CheckString(1);
			bool success = EnvironmentVariableManager.Instance.DeleteVariable(varName);
			lua.PushBoolean(success);
			return 1;
		}

		private static int OS_Addenv( ILuaState lua )
		{
			string varName = lua.L_CheckString(1);
			string varValue = lua.L_CheckString(2);
			EnvironmentVariableManager.Instance.SetVariable(varName, varValue);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Execute( ILuaState lua )
		{
			string command = lua.L_CheckString(1);
			if (string.IsNullOrWhiteSpace(command))
			{
				lua.PushNil();
				return 1;
			}
			
			CMLeonOS.Kernel.shell?.ExecuteCommand(command);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Executefile( ILuaState lua )
		{
			string filePath = lua.L_CheckString(1);
			if (string.IsNullOrWhiteSpace(filePath))
			{
				lua.PushNil();
				return 1;
			}
			
			CMLeonOS.Kernel.shell?.ExecuteCommand($"com {filePath}");
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Reboot( ILuaState lua )
		{
			Sys.Power.Reboot();
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Shutdown( ILuaState lua )
		{
			Sys.Power.Shutdown();
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Sleep( ILuaState lua )
		{
			double seconds = lua.L_CheckNumber(1);
			if (seconds < 0)
			{
				lua.PushNil();
				return 1;
			}
			
			int milliseconds = (int)(seconds * 1000);
			Thread.Sleep(milliseconds);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Beep( ILuaState lua )
		{
			Console.Beep();
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Clear( ILuaState lua )
		{
			Console.Clear();
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_Getusername( ILuaState lua )
		{
			string username = CMLeonOS.Kernel.userSystem?.CurrentUsername ?? "Not logged in";
			lua.PushString(username);
			return 1;
		}

		private static int OS_Isadmin( ILuaState lua )
		{
			bool isAdmin = CMLeonOS.Kernel.userSystem?.CurrentUserIsAdmin ?? false;
			lua.PushBoolean(isAdmin);
			return 1;
		}

		private static int OS_Sha256( ILuaState lua )
		{
			string input = lua.L_CheckString(1);
			string hash = CMLeonOS.UserSystem.HashPasswordSha256(input) ?? "";
			lua.PushString(hash);
			return 1;
		}

		private static int OS_Base64Encrypt( ILuaState lua )
		{
			string input = lua.L_CheckString(1);
			string encoded = CMLeonOS.Base64Helper.Encode(input);
			lua.PushString(encoded);
			return 1;
		}

		private static int OS_Base64Decrypt( ILuaState lua )
		{
			string input = lua.L_CheckString(1);
			string decoded = CMLeonOS.Base64Helper.Decode(input);
			lua.PushString(decoded);
			return 1;
		}

		private static int OS_TimerStart( ILuaState lua )
		{
			_timerStartTime = DateTime.Now;
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TimerStop( ILuaState lua )
		{
			if (!_timerStartTime.HasValue)
			{
				lua.PushNil();
				return 1;
			}
			
			TimeSpan elapsed = DateTime.Now - _timerStartTime.Value;
			double elapsedSeconds = elapsed.TotalSeconds;
			
			lua.PushNumber(elapsedSeconds);
			_timerStartTime = null;
			return 1;
		}

		private static int OS_TUIDrawBox(ILuaState lua)
		{
			int x = (int)lua.L_CheckNumber(1);
			int y = (int)lua.L_CheckNumber(2);
			int width = (int)lua.L_CheckNumber(3);
			int height = (int)lua.L_CheckNumber(4);
			string title = lua.L_CheckString(5);
			string borderColor = lua.L_CheckString(6);
			string backgroundColor = lua.L_CheckString(7);

			ConsoleColor borderColorEnum = ParseColor(borderColor);
			ConsoleColor backgroundColorEnum = ParseColor(backgroundColor);

			var rect = new Rect(x, y, width, height);
			TUIHelper.SetColors(borderColorEnum, backgroundColorEnum);
			TUIHelper.DrawBox(rect, title, borderColorEnum);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TUIDrawText(ILuaState lua)
		{
			int x = (int)lua.L_CheckNumber(1);
			int y = (int)lua.L_CheckNumber(2);
			string text = lua.L_CheckString(3);
			string foregroundColor = lua.L_CheckString(4);
			string backgroundColor = lua.L_CheckString(5);

			ConsoleColor foregroundColorEnum = ParseColor(foregroundColor);
			ConsoleColor backgroundColorEnum = ParseColor(backgroundColor);

			TUIHelper.SetColors(foregroundColorEnum, backgroundColorEnum);
			Console.SetCursorPosition(x, y);
			Console.Write(text);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TUISetColor(ILuaState lua)
		{
			string foregroundColor = lua.L_CheckString(1);
			string backgroundColor = lua.L_CheckString(2);

			ConsoleColor foregroundColorEnum = ParseColor(foregroundColor);
			ConsoleColor backgroundColorEnum = ParseColor(backgroundColor);

			TUIHelper.SetColors(foregroundColorEnum, backgroundColorEnum);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TUISetCursor(ILuaState lua)
		{
			int x = (int)lua.L_CheckNumber(1);
			int y = (int)lua.L_CheckNumber(2);

			Console.SetCursorPosition(x, y);
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TUIClear(ILuaState lua)
		{
			Console.Clear();
			lua.PushBoolean(true);
			return 1;
		}

		private static int OS_TUIDrawLine(ILuaState lua)
		{
			int x = (int)lua.L_CheckNumber(1);
			int y = (int)lua.L_CheckNumber(2);
			int length = (int)lua.L_CheckNumber(3);
			char character = lua.L_CheckString(4)[0];
			string color = lua.L_CheckString(5);

			ConsoleColor colorEnum = ParseColor(color);

			TUIHelper.SetColors(colorEnum, ConsoleColor.Black);
			Console.SetCursorPosition(x, y);
			for (int i = 0; i < length; i++)
			{
				Console.Write(character);
			}
			lua.PushBoolean(true);
			return 1;
		}

		private static ConsoleColor ParseColor(string colorName)
		{
			switch (colorName.ToLower())
			{
				case "black": return ConsoleColor.Black;
				case "blue": return ConsoleColor.Blue;
				case "cyan": return ConsoleColor.Cyan;
				case "darkblue": return ConsoleColor.DarkBlue;
				case "darkcyan": return ConsoleColor.DarkCyan;
				case "darkgray": return ConsoleColor.DarkGray;
				case "darkgreen": return ConsoleColor.DarkGreen;
				case "darkmagenta": return ConsoleColor.DarkMagenta;
				case "darkred": return ConsoleColor.DarkRed;
				case "darkyellow": return ConsoleColor.DarkYellow;
				case "gray": return ConsoleColor.Gray;
				case "green": return ConsoleColor.Green;
				case "magenta": return ConsoleColor.Magenta;
				case "red": return ConsoleColor.Red;
				case "white": return ConsoleColor.White;
				case "yellow": return ConsoleColor.Yellow;
				default: return ConsoleColor.White;
			}
		}
#endif
	}
}

