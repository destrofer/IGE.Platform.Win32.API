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
using System.Runtime.InteropServices;
using System.Collections.Generic;

using IGE;
using IGE.Platform;

namespace IGE.Platform.Win32 {
	public sealed class DisplayMode : IDisplayMode {
		public static event ResolutionChangeEventHandler ResolutionChangeEvent;

		private DeviceModeInfoStruct m_DeviceMode;
		
		public Point2 Position { get { return m_DeviceMode.Position; } }
		public int BitsPerPixel { get { return m_DeviceMode.BitsPerPel; } }
		public int Width { get { return  m_DeviceMode.PelsWidth; } }
		public int Height { get { return  m_DeviceMode.PelsHeight; } }
		public int RefreshRate { get { return  m_DeviceMode.DisplayFrequency; } }
		public DeviceModeFields Fields { get { return  m_DeviceMode.Fields; } }

		private static bool m_ResolutionChanged = false;
		
		internal DisplayMode(DeviceModeInfoStruct dm) {
			m_DeviceMode = dm;
		}
		
		public bool Set(bool bFullScreen) {
			bool ret = IGE.Platform.Win32.API.Externals.ChangeDisplaySettingsEx(m_DeviceMode.DeviceName, ref m_DeviceMode, IntPtr.Zero, bFullScreen ? ChangeDisplaySettingsEnum.Fullscreen : (ChangeDisplaySettingsEnum)0, IntPtr.Zero) == 0;
			if( ret ) {
				if( !m_ResolutionChanged )
					AppDomain.CurrentDomain.ProcessExit += OnAppTerminate;					
				m_ResolutionChanged = true;
				if( ResolutionChangeEvent != null )
					ResolutionChangeEvent();
			}
			return ret;
		}
		
		private static void Reset(bool manual) {
			bool ret = IGE.Platform.Win32.API.Externals.ChangeDisplaySettings(IntPtr.Zero, 0) == 0;
			if( ret ) {
				if( manual )
					AppDomain.CurrentDomain.ProcessExit -= OnAppTerminate;
				m_ResolutionChanged = false;
				if( ResolutionChangeEvent != null )
					ResolutionChangeEvent();
			}
		}
		
		public static void Reset() {
			Reset(true);
		}
		
		private static void OnAppTerminate(object sender, EventArgs e) {
			Reset(false);
		}
		
	    public override string ToString() {
	    	return String.Format("{2}bit {0}x{1} @{3} [{4}]", Width, Height, BitsPerPixel, RefreshRate, m_DeviceMode.DeviceName );
		}
	}

	public delegate void ResolutionChangeEventHandler();
}
