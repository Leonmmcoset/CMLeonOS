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
using System.Text;

namespace UniLua
{
	class LuaEncLib
	{
		public const string LIB_NAME = "enc";

		private const string ENC_UTF8 = "utf8";

		public static int OpenLib( ILuaState lua )
		{
			var define = new NameFuncPair[]
			{
				new NameFuncPair( "encode", ENC_Encode ),
				new NameFuncPair( "decode", ENC_Decode ),
			};

			lua.L_NewLib( define );

			lua.PushString( ENC_UTF8 );
			lua.SetField( -2, "utf8" );

			return 1;
		}

		private static int ENC_Encode( ILuaState lua )
		{
			var s = lua.ToString(1);
			var e = lua.ToString(2);
			if( e != ENC_UTF8 )
				throw new Exception("unsupported encoding:" + e);

			var bytes = Encoding.ASCII.GetBytes(s);
			var sb = new StringBuilder();
			for( var i=0; i<bytes.Length; ++i )
			{
				sb.Append( (char)bytes[i] );
			}
			lua.PushString( sb.ToString() );
			return 1;
		}

		private static int ENC_Decode( ILuaState lua )
		{
			var s = lua.ToString(1);
			var e = lua.ToString(2);
			if( e != ENC_UTF8 )
				throw new Exception("unsupported encoding:" + e);

			var bytes = new Byte[s.Length];
			for( int i=0; i<s.Length; ++i )
			{
				bytes[i] = (byte)s[i];
			}

			lua.PushString( Encoding.ASCII.GetString( bytes ) );
			return 1;
		}

	}
}
