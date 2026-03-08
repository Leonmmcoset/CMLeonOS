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
using System.IO;
using System.Collections.Generic;

namespace UniLua
{
	public delegate string PathHook(string filename);
	public class LuaFile
	{
		public static Dictionary<string, byte[]> VirtualFiles;

        public static ILoadInfo OpenFile( string filename )
		{
            foreach (var file in VirtualFiles.Keys)
            {
                if (file == filename)
                {
                    return new BytesLoadInfo(VirtualFiles[filename]); ;
                }
            }

            return new FileLoadInfo( File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) );
		}

		public static bool Readable( string filename )
		{
            foreach (var file in VirtualFiles.Keys)
            {
                if (file == filename)
                {
                    return true;
                }
            }

            try {
				using( var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) ) {
					return true;
				}
			}
			catch( Exception ) {
				return false;
			}
		}
	}

	public class FileLoadInfo : ILoadInfo, IDisposable
	{
		public FileLoadInfo( FileStream stream )
		{
			Stream = stream;
      Reader = new StreamReader(Stream, System.Text.Encoding.ASCII);
			Buf = new Queue<char>();
		}

		public int ReadByte()
		{
			if( Buf.Count > 0 )
				return (int)Buf.Dequeue();
			else
				return Reader.Read();
		}

		public int PeekByte()
		{
			if( Buf.Count > 0 )
				return (int)Buf.Peek();
			else
			{
				var c = Reader.Read();
				if( c == -1 )
					return c;
				Save( (char)c );
				return c;
			}
		}

		public void Dispose()
		{
      Reader.Dispose();
			Stream.Dispose();
		}

		private const string UTF8_BOM = "\u00EF\u00BB\u00BF";
		private FileStream 	Stream;
		private StreamReader 	Reader;
		private Queue<char>	Buf;

		private void Save( char b )
		{
			Buf.Enqueue( b );
		}

		private void Clear()
		{
			Buf.Clear();
		}

#if false
		private int SkipBOM()
		{
			for( var i=0; i<UTF8_BOM.Length; ++i )
			{
				var c = Stream.ReadByte();
				if(c == -1 || c != (byte)UTF8_BOM[i])
					return c;
				Save( (char)c );
			}
			// perfix matched; discard it
			Clear();
			return Stream.ReadByte();
		}
#endif

		public void SkipComment()
		{
			var c = Reader.Read();//SkipBOM();

			// first line is a comment (Unix exec. file)?
			if( c == '#' )
			{
				do {
					c = Reader.Read();
				} while( c != -1 && c != '\n' );
				Save( (char)'\n' ); // fix line number
			}
			else if( c != -1 )
			{
				Save( (char)c );
			}
		}
	}

}

