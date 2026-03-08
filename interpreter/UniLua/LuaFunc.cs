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

// #define DEBUG_FIND_UPVALUE

using System.Collections.Generic;

using ULDebug = UniLua.Tools.ULDebug;

namespace UniLua
{
	public partial class LuaState
	{

		private LuaUpvalue F_FindUpval( StkId level )
		{
#if DEBUG_FIND_UPVALUE
			Console.WriteLine( "[F_FindUpval] >>>>>>>>>>>>>>>>>>>> level:" + level );
#endif

			var node = OpenUpval.First;
			LinkedListNode<LuaUpvalue> prev = null;
			while( node != null )
			{
				var upval = node.Value;
#if DEBUG_FIND_UPVALUE
				Console.WriteLine("[F_FindUpval] >>>>>>>>>>>>>>>>>>>> upval.V:" + upval.V );
#endif
				if(upval.V.Index < level.Index)
					break;

				var next = node.Next;
				if(upval.V == level)
					return upval;

				prev = node;
				node = next;
			}

			// not found: create a new one
			var ret = new LuaUpvalue();
			ret.V   = level;
			// ret.Prev = G.UpvalHead;
			// ret.Next = G.UpvalHead.Next;
			// ret.Next.Prev = ret;
			// G.UpvalHead.Next = ret;

			if( prev == null )
				OpenUpval.AddFirst( ret );
			else
				OpenUpval.AddAfter( prev, ret );

#if DEBUG_FIND_UPVALUE
			Console.WriteLine("[F_FindUpval] >>>>>>>>>>>>>>>>>>>> create new one:" + ret.V );
#endif

			return ret;
		}

		private void F_Close( StkId level )
		{
			var node = OpenUpval.First;
			while( node != null )
			{
				var upval = node.Value;
				if( upval.V.Index < level.Index )
					break;

				var next = node.Next;
				OpenUpval.Remove( node );
				node = next;

				upval.Value.V.SetObj(ref upval.V.V);
				upval.V = upval.Value;
			}
		}

		private string F_GetLocalName( LuaProto proto, int localNumber, int pc )
		{
			for( int i=0;
				i<proto.LocVars.Count && proto.LocVars[i].StartPc <= pc;
				++i )
			{
				if( pc < proto.LocVars[i].EndPc ) { // is variable active?
					--localNumber;
					if( localNumber == 0 )
						return proto.LocVars[i].VarName;
				}
			}
			return null;
		}

	}

}

