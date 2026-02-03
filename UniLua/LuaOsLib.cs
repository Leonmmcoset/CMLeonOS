 
namespace UniLua
{
	using System.Diagnostics;
	using CMLeonOS;
	
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
#endif
	}
}

