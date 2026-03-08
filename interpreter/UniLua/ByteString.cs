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
using System.Collections.Generic;

namespace UniLua
{
	internal class ByteStringBuilder
	{
		public ByteStringBuilder()
		{
			BufList = new LinkedList<byte[]>();
			TotalLength = 0;
		}

		public override string ToString()
		{
			if( TotalLength <= 0 )
				return String.Empty;

			var result = new char[TotalLength];
			var i = 0;
			var node = BufList.First;
			while(node != null)
			{
				var buf = node.Value;
				for(var j=0; j<buf.Length; ++j)
				{
					result[i++] = (char)buf[j];
				}
				node = node.Next;
			}
			return new string(result);
		}

		public ByteStringBuilder Append(byte[] bytes, int start, int length)
		{
			var buf = new byte[length];
			Array.Copy(bytes, start, buf, 0, length);
			BufList.AddLast( buf );
			TotalLength += length;
			return this;
		}

		private LinkedList<byte[]> 	BufList;
		private int					TotalLength;
	}
}

