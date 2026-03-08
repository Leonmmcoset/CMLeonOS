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

// TODO

namespace UniLua
{

	internal class  LuaIOLib
	{
		public const string LIB_NAME = "io";

		public static int OpenLib( ILuaState lua )
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair( "close", 		IO_Close ),
				new NameFuncPair( "flush", 		IO_Flush ),
				new NameFuncPair( "input", 		IO_Input ),
				new NameFuncPair( "lines", 		IO_Lines ),
				new NameFuncPair( "open", 		IO_Open ),
				new NameFuncPair( "output", 	IO_Output ),
				new NameFuncPair( "popen", 		IO_Popen ),
				new NameFuncPair( "read", 		IO_Read ),
				new NameFuncPair( "tmpfile", 	IO_Tmpfile ),
				new NameFuncPair( "type", 		IO_Type ),
				new NameFuncPair( "write", 		IO_Write ),
			};

			lua.L_NewLib( define );
			return 1;
		}

		private static int IO_Close( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Flush( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Input( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Lines( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Open( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Output( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Popen( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Read( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Tmpfile( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Type( ILuaState lua )
		{
			// TODO
			return 0;
		}

		private static int IO_Write( ILuaState lua )
		{
			// TODO
			return 0;
		}
	}

}

