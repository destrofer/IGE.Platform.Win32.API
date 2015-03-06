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
	public class Monitor : IMonitor {
		protected MonitorInfoEx m_MonitorInfo;

		protected bool m_ModeDetected = false;
		protected int m_BitsPerPixel = -1;
		protected int m_RefreshRate = -1;
		
		public int X { get { return m_MonitorInfo.Monitor.X; } }
		public int Y { get { return m_MonitorInfo.Monitor.Y; } }
		public int Width { get { return m_MonitorInfo.Monitor.Width; } }
		public int Height { get { return m_MonitorInfo.Monitor.Height; } }
		
		public int BitsPerPixel { get { DetectMode(); return m_BitsPerPixel; } }
		public int RefreshRate { get { DetectMode(); return m_RefreshRate; } }
		
		public Monitor(MonitorInfoEx mi) {
			m_MonitorInfo = mi;
		}
		
		protected void DetectMode() {
			Application.KeepAliveWithoutWindows = true;
			// could use full monitor width, but it may be problematic if window touches another monitor
			using(Win32NativeWindow temp_win = new Win32NativeWindow(null, X, Y, 1, 1)) {
				using(DeviceContext temp_dc = new DeviceContext(temp_win)) {
					m_BitsPerPixel = temp_dc.GetCaps(DeviceCapability.BitsPerPixel);
					m_RefreshRate = temp_dc.GetCaps(DeviceCapability.RefreshRate);
				}
			}
			Application.KeepAliveWithoutWindows = false;
		}
	}
}
