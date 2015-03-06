/*
 * Author: Viacheslav Soroka
 * 
 * This file is part of IGE <https://github.com/destrofer/IGE>.
 * 
 * IGE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * IGE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with IGE.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

using IGE.Platform.Win32;

namespace IGE.Platform {
	public class VirtualConsole : CharBlock {
		private static SafeFileHandle m_OutputHandle = null;
		
		protected int m_CursorX;
		protected int m_CursorY;

		protected bool m_CursorVisible;
		public bool CursorVisible { get { return m_CursorVisible; } set { m_CursorVisible = value; } }
		
		public VirtualConsole(short width, short height) : base((int)width, (int)height) {
			m_CursorX = 0;
			m_CursorY = 0;
			m_CursorVisible = false;
			if( m_OutputHandle == null )
				Initialize(width, height);
			Clear();
		}
		
		public VirtualConsole() : this(80, 25) {
		}

		private static void Initialize(short width, short height) {
			if( m_OutputHandle != null ) return;
			
			System.Console.SetWindowSize(width, height);
			System.Console.BufferWidth = width;
			System.Console.BufferHeight = height;
			System.Console.ForegroundColor = ConsoleColor.Gray;
			System.Console.BackgroundColor = ConsoleColor.Black;
			
			System.Console.CursorVisible = false;
			System.Console.Clear();
			
			IGE.Platform.Win32.API.Externals.SetConsoleCP(ConsoleCodePage.UTF16LE); 
			IGE.Platform.Win32.API.Externals.SetConsoleOutputCP(ConsoleCodePage.UTF16LE);
			
			m_OutputHandle = IGE.Platform.Win32.API.Externals.CreateFile( "CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero );
			
			AppDomain.CurrentDomain.ProcessExit += OnTerminate;
		}

		private static void Deinitialize() {
			if( m_OutputHandle == null ) return;
			
			System.Console.ForegroundColor = ConsoleColor.Gray;
			System.Console.BackgroundColor = ConsoleColor.Black;
			System.Console.Clear();
			System.Console.CursorVisible = true;
			
			m_OutputHandle = null;
		}
		
		private static void OnTerminate(object caller, EventArgs e) {
			Deinitialize();
		}
		
		protected virtual void OnBeforeApply() {
		}
		
		public void Apply() {
			OnBeforeApply();			
			SmallRect2D rect = new SmallRect2D() { Left = 0, Top = 0, Right = (short)m_Width, Bottom = (short)m_Height };
			IGE.Platform.Win32.API.Externals.WriteConsoleOutput(m_OutputHandle, m_Chars,
			              new SmallPoint2D() { X = (short)m_Width, Y = (short)m_Height },
			              new SmallPoint2D() { X = 0, Y = 0 },
			              ref rect);

			if( m_CursorVisible && m_CursorX >= 0 && m_CursorY >= 0 && m_CursorX < m_Width && m_CursorY < m_Height ) {
				System.Console.CursorVisible = true;
				System.Console.SetCursorPosition(m_CursorX, m_CursorY);
			}
			else
				System.Console.CursorVisible = false;
		}

		public virtual void SetCursorPosition(int x, int y) {
			m_CursorX = x;
			m_CursorY = y;
		}

		public virtual void ShowCursor(bool sw) {
			m_CursorVisible = sw;
		}

		public virtual string InputEx(string defval, int x, int y, int width, int maxlen, bool can_cancel) {
			string value;
			ConsoleKeyInfo key;
			int str_pos, cur_pos, str_shift, delta, i, l;

			value = (defval == null)?"":defval;
			str_shift = 0;
			str_pos = value.Length;
			cur_pos = str_pos;

			if( cur_pos >= width ) {
				str_shift += cur_pos - width + 1;
				cur_pos = width - 1;
			}
			
			do {
				l = value.Length;
				if( str_shift > l - width )
				{
					delta = (l - width) - str_shift;
					str_shift += delta;
					cur_pos -= delta;
					if( cur_pos >= width )
					{
						delta = (width - 1) - cur_pos;
						cur_pos += delta;
						str_pos += delta;
					}
				}
				l = (l < width)?l:width;
				
				for( i = 0; i < l; i++ )
					PrintAt(x + i, y, value[i+str_shift]);
					
				key = System.Console.ReadKey(false);
				if( key.Key == ConsoleKey.Escape && can_cancel )
					return null;
				else if( key.KeyChar >= ' ' )
				{
					value.Insert(str_pos, "" + key.KeyChar);
					str_pos++;
					if( cur_pos >= width - 1 )
						str_shift++;
					else
						cur_pos++;
				}
			} while( key.Key != ConsoleKey.Enter );
			return value;
		}
	}
	
	public class CharBlock {
		protected const int TabWidth = 4;
		
		protected int m_Width;
		public int Width { get { return m_Width; } }
		
		protected int m_Height;
		public int Height { get { return m_Height; } }

		protected int m_TextColor;
		public int TextColor { get { return m_TextColor; } set { m_TextColor = value & 0x0F; } }

		protected int m_BackgroundColor;
		public int BackgroundColor { get { return m_BackgroundColor; } set { m_BackgroundColor = value & 0x0F; } }
		
		protected CharInfo[] m_Chars;
		public CharInfo[] Chars { get { return m_Chars; } }
		
		public CharInfo ClearSymbol;
		public CharInfo OffblockSymbol;
		
		public CharBlock(int w, int h) {
			m_Width = w;
			m_Height = h;
			m_Chars = new CharInfo[w * h];
			m_TextColor = 7;
			m_BackgroundColor = 0;
			
			ClearSymbol = CharInfo.Space;
			OffblockSymbol = CharInfo.Zero;
		}
		
		public virtual CharInfo this[int x, int y] {
			get {
				if( x < 0 || x >= m_Width || y < 0 || y >= m_Height )
					return OffblockSymbol;
				return m_Chars[x + y*m_Width];
			}
			set {
				if( x < 0 || x >= m_Width || y < 0 || y >= m_Height )
					return;
				m_Chars[x + y*m_Width] = value;
			}
		}
		
		/*
		public virtual char this[int x, int y] {
			get {
				if( x < 0 || x >= m_Width || y < 0 || y >= m_Height )
					return OffblockSymbol;
				return m_Chars[x + y*m_Width].Char;
			}
			set {
				if( x < 0 || x >= m_Width || y < 0 || y >= m_Height )
					return;
				m_Chars[x + y*m_Width] = new CharInfo(value, (short)((m_BackgroundColor << 4) | m_TextColor));
			}
		}
		*/

		// string printing
		public virtual void PrintAt(int x, int y, string str, int text_color, int bg_color) {
			int ln = str.Length;
			int cx = x, dx;
			char chr;
			short color = (short)((bg_color << 4) | text_color);
			int idx = cx + y * m_Width;
			
			for( int i = 0; i < ln; i++ ) {
				chr = str[i];
				if( chr == '\r' ) continue;
				if( chr == '\n' ) {
					cx = x; y++;
					idx = cx + y * m_Width;
					continue;
				}
				if( chr == '\t' ) {
					dx = TabWidth - (cx - x) % TabWidth;
					cx += dx; idx += dx;
					continue;
				}

				if( y >= m_Height ) return;
				if( y >= 0 && cx >= 0 && cx < m_Width ) {
					m_Chars[idx].Char = chr;
					m_Chars[idx].Attributes = color;
				}
				idx++; cx++;
			}
		}

		public virtual void PrintAt(int x, int y, string str, int text_color) {
			PrintAt(x, y, str, text_color, m_BackgroundColor);
		}

		public virtual void PrintAt(int x, int y, string str) {
			PrintAt(x, y, str, m_TextColor, m_BackgroundColor);
		}

		// char printing
		public virtual void PrintAt(int x, int y, char chr, int text_color, int bg_color) {
			if( y < 0 || x < 0 || y >= m_Height || x >= m_Width )
				return;

			int idx = x + y * m_Width;
			m_Chars[idx].Char = chr;
			m_Chars[idx].Attributes = (short)((bg_color << 4) | text_color);
		}

		public virtual void PrintAt(int x, int y, char chr, ConsoleColor text_color, ConsoleColor bg_color) {
			PrintAt(x, y, chr, (int)text_color, (int)bg_color);
		}

		public virtual void PrintAt(int x, int y, char chr, ConsoleColor text_color) {
			PrintAt(x, y, chr, (int)text_color, m_BackgroundColor);
		}

		public virtual void PrintAt(int x, int y, char chr, int text_color) {
			PrintAt(x, y, chr, text_color, m_BackgroundColor);
		}

		public virtual void PrintAt(int x, int y, char chr) {
			PrintAt(x, y, chr, m_TextColor, m_BackgroundColor);
		}
		
		// another block printing over
		public virtual void PrintAt(int x, int y, CharBlock block) {
			int
			scan_width, scan_lines, delta,
			src_x, src_y, src_idx,
			dst_x, dst_y, dst_idx
			;
			
			dst_x = x;
			dst_y = y;
			src_x = 0;
			src_y = 0;
			scan_width = block.Width;
			scan_lines = block.Height;
			if( dst_y < 0 ) {
				delta = dst_y;
				dst_y = 0;
				src_y -= delta;
				scan_lines += delta;
			}
			if( dst_y >= m_Height || src_y >= block.Height ) return; // nothing to copy
			
			if( dst_x < 0 ) {
				delta = dst_x;
				dst_x = 0;
				src_x -= delta;
				scan_width += delta;
			}
			if( dst_x >= m_Width || src_x >= block.Width ) return; // nothing to copy

			delta = m_Height - dst_y;
			if( scan_lines > delta ) scan_lines = delta;
			
			delta = m_Width - dst_x;
			if( scan_width > delta ) scan_width = delta;
			
			src_idx = src_x + src_y * block.Width;
			dst_idx = dst_x + dst_y * m_Width;
			
			for( int i = 0; i < scan_lines; i++ ) {
				Array.Copy(block.Chars, src_idx, m_Chars, dst_idx, scan_width);
				src_idx += block.Width;
				dst_idx += m_Width;
			}
		}
		
		public virtual void Clear() {
			for(int idx = 0; idx < m_Width * m_Height; idx++) {
				m_Chars[idx] = ClearSymbol;
			}
		}				
	}
}