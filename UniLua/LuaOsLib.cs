 
namespace UniLua
{
	using System.Diagnostics;
	using System;
	using CMLeonOS;
	using Sys = Cosmos.System;
	using System.Threading;
	
	internal class LuaOSLib
	{
		public const string LIB_NAME = "os";

		public static int OpenLib( ILuaState lua )
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
#if !UNITY_WEBPLAYER
				new NameFuncPair("clock", 	OS_Clock),
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
#endif
	}
}

