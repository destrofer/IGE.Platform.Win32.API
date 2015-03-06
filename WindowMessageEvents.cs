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

namespace IGE.Platform.Win32 {
	public delegate void WindowMessageEventHandler(WindowMessageEventArgs args);
	
	public class WindowMessageEventArgs : NativeWindowEventArgs {
		public IntPtr hWnd;
		public WindowMessageEnum uMsg;
		public IntPtr wParam;
		public IntPtr lParam;
		public IntPtr ReturnValue = IntPtr.Zero;
		
		private bool m_PreventsDefault = false;
		public bool PreventsDefault { get { return m_PreventsDefault; } }
		
		public WindowMessageEventArgs(Win32NativeWindow window, WindowMessageEnum _uMsg, IntPtr _wParam, IntPtr _lParam) : base(window) {
			hWnd = (window == null) ? IntPtr.Zero : window.Handle;
			uMsg = _uMsg;
			wParam = _wParam;
			lParam = _lParam;
		}
		
		public WindowMessageEventArgs(IntPtr _hWnd, WindowMessageEnum _uMsg, IntPtr _wParam, IntPtr _lParam) : base(null) {
			hWnd = _hWnd;
			uMsg = _uMsg;
			wParam = _wParam;
			lParam = _lParam;
		}
		
		public void PreventDefault() {
			m_PreventsDefault = true;
		}
	}
}
