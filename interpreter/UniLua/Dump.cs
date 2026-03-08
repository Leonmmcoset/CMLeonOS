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
using System.Collections.Generic;

namespace UniLua
{
	public enum DumpStatus
	{
		OK,
		ERROR,
	}

	public delegate DumpStatus LuaWriter( byte[] bytes, int start, int length );
	internal class DumpState
	{
		public static DumpStatus Dump(
			LuaProto proto, LuaWriter writer, bool strip )
		{
			var d = new DumpState();
			d.Writer 	= writer;
			d.Strip 	= strip;
			d.Status	= DumpStatus.OK;

			d.DumpHeader();
			d.DumpFunction( proto );

			return d.Status;
		}

		private LuaWriter 	Writer;
		private bool		Strip;
		private DumpStatus	Status;

		private const string LUAC_TAIL = "\u0019\u0093\r\n\u001a\n";
		private static int VERSION = (LuaDef.LUA_VERSION_MAJOR[0]-'0') * 16 + 
			(LuaDef.LUA_VERSION_MINOR[0]-'0');
		private static int LUAC_HEADERSIZE = LuaConf.LUA_SIGNATURE.Length +
			2 + 6 + LUAC_TAIL.Length;
		private const int FORMAT = 0;
		private const int ENDIAN = 1;

		private DumpState()
		{
		}

		private byte[] BuildHeader()
		{
			var bytes = new byte[LUAC_HEADERSIZE];
			int i = 0;

			for(var j=0; j<LuaConf.LUA_SIGNATURE.Length; ++j)
				bytes[i++] = (byte)LuaConf.LUA_SIGNATURE[j];

			bytes[i++] = (byte)VERSION;
			bytes[i++] = (byte)FORMAT;
			bytes[i++] = (byte)ENDIAN;
			bytes[i++] = (byte)4; // sizeof(int)
			bytes[i++] = (byte)4; // sizeof(size_t)
			bytes[i++] = (byte)4; // sizeof(Instruction)
			bytes[i++] = (byte)sizeof(double); // sizeof(lua_Number)
			bytes[i++] = (byte)0; // is lua_Number integral?

			for(var j=0; j<LUAC_TAIL.Length; ++j)
				bytes[i++] = (byte)LUAC_TAIL[j];

			return bytes;
		}

		private void DumpHeader()
		{
			var bytes = BuildHeader();
			DumpBlock( bytes );
		}

		private void DumpBool( bool value )
		{
			DumpByte( value ? (byte)1 : (byte) 0 );
		}

		private void DumpInt( int value )
		{
			DumpBlock( BitConverter.GetBytes( value ) );
		}

		private void DumpUInt( uint value )
		{
			DumpBlock( BitConverter.GetBytes( value ) );
		}

		private void DumpString( string value )
		{
			if( value == null )
			{
				DumpUInt(0);
			}
			else
			{
				DumpUInt( (uint)(value.Length + 1) );
				for(var i=0; i<value.Length; ++i)
					DumpByte( (byte)value[i] );
				DumpByte( (byte)'\0' );
			}
		}

		private void DumpByte( byte value )
		{
			var bytes = new byte[] { value };
			DumpBlock( bytes );
		}

		private void DumpCode( LuaProto proto )
		{
			DumpVector( proto.Code, (ins) => {
				DumpBlock( BitConverter.GetBytes( (uint)ins ) );
			});
		}

		private void DumpConstants( LuaProto proto )
		{
			DumpVector( proto.K, (k) => {
				var t = k.V.Tt;
				DumpByte( (byte)t );
				switch( t )
				{
					case (int)LuaType.LUA_TNIL:
						break;
					case (int)LuaType.LUA_TBOOLEAN:
						DumpBool(k.V.BValue());
						break;
					case (int)LuaType.LUA_TNUMBER:
						DumpBlock( BitConverter.GetBytes(k.V.NValue) );
						break;
					case (int)LuaType.LUA_TSTRING:
						DumpString(k.V.SValue());
						break;
					default:
						Utl.Assert(false);
						break;
				}
			});

			DumpVector( proto.P, (p) => {
				DumpFunction( p );
			});
		}

		private void DumpUpvalues( LuaProto proto )
		{
			DumpVector( proto.Upvalues, (upval) => {
				DumpByte( upval.InStack ? (byte)1 : (byte)0 );
				DumpByte( (byte)upval.Index );
			});
		}

		private void DumpDebug( LuaProto proto )
		{
			DumpString( Strip ? null : proto.Source );

			DumpVector( (Strip ? null : proto.LineInfo), (line) => {
				DumpInt(line);
			});

			DumpVector( (Strip ? null : proto.LocVars), (locvar) => {
				DumpString( locvar.VarName );
				DumpInt( locvar.StartPc );
				DumpInt( locvar.EndPc );
			});

			DumpVector( (Strip ? null : proto.Upvalues), (upval) => {
				DumpString( upval.Name );
			});
		}

		private void DumpFunction( LuaProto proto )
		{
			DumpInt( proto.LineDefined );
			DumpInt( proto.LastLineDefined );
			DumpByte( (byte)proto.NumParams );
			DumpByte( proto.IsVarArg ? (byte)1 : (byte)0 );
			DumpByte( (byte)proto.MaxStackSize );
			DumpCode( proto );
			DumpConstants( proto );
			DumpUpvalues( proto );
			DumpDebug( proto );
		}

		private delegate void DumpItemDelegate<T>( T item );
		private void DumpVector<T>( IList<T> list, DumpItemDelegate<T> dumpItem )
		{
			if( list == null )
			{
				DumpInt( 0 );
			}
			else
			{
				DumpInt( list.Count );
				for( var i=0; i<list.Count; ++i )
				{
					dumpItem( list[i] );
				}
			}
		}

		private void DumpBlock( byte[] bytes )
		{
			DumpBlock( bytes, 0, bytes.Length );
		}

		private void DumpBlock( byte[] bytes, int start, int length )
		{
			if( Status == DumpStatus.OK )
			{
				Status = Writer(bytes, start, length);
			}
		}
	}
}

