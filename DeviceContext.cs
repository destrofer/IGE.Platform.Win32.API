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
using System.Runtime.InteropServices; // needed to import from dll
using System.Collections;
using System.Collections.Generic;

using IGE.Platform.Win32;

namespace IGE.Platform {
	public class DeviceContext : IDisposable {
		
		#region Members and properties

		public event BeforeReleaseEventHandler BeforeReleaseEvent;
		
		protected Win32NativeWindow m_Window;
		protected IntPtr m_hDC;
		public IntPtr Handle { get { return m_hDC; } }
		public bool Disposed { get { return m_hDC == IntPtr.Zero; } }
		public bool IsOpen { get { return m_hDC != IntPtr.Zero; } }
		
		#endregion

		#region Constructors
		
		public DeviceContext() : this((Win32NativeWindow)null) {
		}
		
		public DeviceContext(Win32NativeWindow window) {
			m_hDC = IntPtr.Zero;
			if( window != null ) {
				if( window.Disposed )
					throw new Exception("Device context cannot be created because window does not exist anymore");
				m_Window = window;
				GameDebugger.EngineLog(LogLevel.Debug, "Getting device context for the window");
				m_hDC = IGE.Platform.Win32.API.Externals.GetDC(m_Window.Handle);
				if( m_hDC != IntPtr.Zero )
					m_Window.CloseEvent += OnBeforeWindowDestroy;
			}
			else {
				GameDebugger.EngineLog(LogLevel.Debug, "Getting device context of the desktop");
				m_hDC = IGE.Platform.Win32.API.Externals.GetDC(IntPtr.Zero); // try getting device context of desktop ... whatever you might neeed it for :)
			}
		}
		
		#endregion
		
		#region Destructors
		
		~DeviceContext() {
			Dispose();
		}
		
		public void Dispose() {
			if( m_hDC != IntPtr.Zero ) {
				if( BeforeReleaseEvent != null )
					BeforeReleaseEvent(this);
				BeforeReleaseEvent = null;

				//GameDebugger.Log("Disposing Device Context");
				IGE.Platform.Win32.API.Externals.ReleaseDC(((m_Window != null && !m_Window.Disposed) ? m_Window.Handle : IntPtr.Zero), m_hDC);
				m_hDC = IntPtr.Zero;
			}
			
			if( m_Window != null ) {
				m_Window.CloseEvent -= OnBeforeWindowDestroy;
				m_Window = null;
			}
		}
		
		#endregion

		#region Event handlers
		
		private void OnBeforeWindowDestroy(CloseEventArgs args) {
			if( m_hDC != IntPtr.Zero && args.Window == m_Window )
				Dispose();
		}
		
		#endregion
		
		#region Enumerators
		
		public IEnumerable<PixelFormat> PixelFormats {
			get {
				if( m_hDC == IntPtr.Zero )
					yield break;
				
				int max_index = PixelFormat.GetCount(this);
				for( int i = 1; i <= max_index; i++ ) {
					PixelFormat info = new PixelFormat(this, i);
					if( info.Exists )
						yield return info;
				}
			}
		}
		
		#endregion
		
		#region Control methods
		
		public void SwapBuffers() {
			if( m_hDC != IntPtr.Zero )
				IGE.Platform.Win32.API.Externals.SwapBuffers(m_hDC);
		}
		
		public int GetCaps(DeviceCapability cap) {
			return IGE.Platform.Win32.API.Externals.GetDeviceCaps(m_hDC, cap);
		}
		
		#endregion
	}
	
	public delegate void BeforeReleaseEventHandler(DeviceContext dc);
}
