 
namespace UniLua
{
	using System.Diagnostics;

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
#endif
	}
}

