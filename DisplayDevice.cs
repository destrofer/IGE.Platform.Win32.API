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

namespace IGE.Platform.Win32 {
	/// <summary>
	/// This class allows enumerating and controlling display devices.
	/// </summary>
	public sealed class DisplayDevice : IDisplayDevice {

		private bool m_Exists;
		public bool Exists { get { return m_Exists; } }

		private DisplayDeviceInfoStruct m_DeviceInfo;
		public string UniqueId { get { return m_DeviceInfo.DeviceKey; } }
		public string Id { get { return m_DeviceInfo.DeviceName; } }
		public string Name { get { return m_DeviceInfo.DeviceString; } }
		public bool Primary { get { return (m_DeviceInfo.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) == DisplayDeviceStateFlags.PrimaryDevice; } }
		
		private static object SyncRoot = new object();
		
		private static List<MonitorInfoEx> AllMonitors; // should be refreshed after resolution has changed since monitor info will be different
		private List<MonitorInfoEx> m_Monitors; // should be refreshed after resolution has changed since monitor info will be different

		public DisplayDevice() : this((string)null) {
		}

		/// <summary>
		/// Creates an instance of a display device
		/// </summary>
		/// <param name="index">Index of the display device with base on 1. If the display with selected index does not exist, this object will be created for the primary display device (same as calling static method DisplayDevice.GetPrimary()).</param>
		public DisplayDevice(string id) {
			GameDebugger.EngineLog(LogLevel.Debug, "Searching for display device '{0}'", id);
			m_DeviceInfo = new DisplayDeviceInfoStruct();
			m_Exists = false;
			List<DisplayDevice> devices = DisplayDevice.GetAvailableDevices();
			if( id != null ) {
				foreach( DisplayDevice dev in devices ) {
					if( dev.Id.Equals(id) ) {
						m_DeviceInfo = dev.m_DeviceInfo;
						m_Exists = true;
						break;
					}
				}
			}
			if( !m_Exists ) {
				GameDebugger.EngineLog(LogLevel.Notice, "Did not find a requested display device. Using primary display device instead.");
				foreach( DisplayDevice dev in devices ) {
					if( dev.Primary ) {
						GameDebugger.EngineLog(LogLevel.Debug, "Primary display device found.");
						m_DeviceInfo = dev.m_DeviceInfo;
						m_Exists = true;
						break;
					}
				}
			}
			if( m_Exists ) {
				DisplayMode.ResolutionChangeEvent += RefreshMonitorData;
				RefreshMonitorData();
			}
			else {
				GameDebugger.EngineLog(LogLevel.Error, "Primary display device not found.");
			}
		}
		
		private DisplayDevice(DisplayDeviceInfoStruct dd) {
			m_Exists = true;
			m_DeviceInfo = dd;
			RefreshMonitorData();
		}
		
		
		#region Monitor related
		
		private unsafe static bool EnumMonitorsCallback(IntPtr hMonitor, IntPtr hDC, ref Rectangle rect, IntPtr dwData) {
			MonitorInfoEx mi = new MonitorInfoEx();
			mi.Size = MonitorInfoEx.SizeInBytes;
			IGE.Platform.Win32.API.Externals.GetMonitorInfo(hMonitor, ref mi);
			mi.Monitor.FixLTRB();
			mi.Work.FixLTRB();
			AllMonitors.Add(mi);
			return true;
		}
		
		private void RefreshMonitorData() {
			lock(SyncRoot) {
				GameDebugger.EngineLog(LogLevel.Debug, "Searching monitors attached to display device '{0}'.", m_DeviceInfo.DeviceName);
				
				AllMonitors = new List<MonitorInfoEx>();
				unsafe { IGE.Platform.Win32.API.Externals.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, EnumMonitorsCallback, IntPtr.Zero); }
				
				m_Monitors = new List<MonitorInfoEx>();
				foreach( MonitorInfoEx mi in AllMonitors ) {
					if( mi.DeviceName.Equals(m_DeviceInfo.DeviceName) ) {
						GameDebugger.EngineLog(LogLevel.Debug, "Added monitor Resolution={0}; WorkingSize={1}; Flags={2}", mi.Monitor, mi.Work, mi.Flags);
						m_Monitors.Add(mi);
					}
				}
				if( m_Monitors.Count == 0 ) {
					GameDebugger.EngineLog(LogLevel.Error, "Could not find any monitors attached to display device '{0}'. Listing all monitors for debugging purposes:", m_DeviceInfo.DeviceName);
					foreach( MonitorInfoEx mi in AllMonitors ) {
						GameDebugger.EngineLog(LogLevel.Error, "Monitor '{0}': Resolution={1}; WorkingSize={2}; Flags={3}", mi.DeviceName, mi.Monitor, mi.Work, mi.Flags);
					}
				}
			}
		}

		public List<IMonitor> GetMonitors() {
			if( m_Monitors == null )
				return null;
			List<IMonitor> list = new List<IMonitor>();
			foreach(MonitorInfoEx mi in m_Monitors)
				list.Add(new Monitor(mi));
			return list;
		}

		#endregion
		
		#region Display device related
		
		public static List<DisplayDevice> GetAvailableDevices() {
			return GetAvailableDevices(false);
		}
		
		public static List<DisplayDevice> GetAvailableDevices(bool list_all) {
			int index = 0;
			List<DisplayDevice> list = new List<DisplayDevice>();
			DisplayDeviceInfoStruct dd = new DisplayDeviceInfoStruct();
			while( IGE.Platform.Win32.API.Externals.EnumDisplayDevices(null, index++, dd, 1) ) {
				if( !list_all ) {
					if( (dd.StateFlags & DisplayDeviceStateFlags.Active) != DisplayDeviceStateFlags.Active )
						continue;
					if( (dd.StateFlags & DisplayDeviceStateFlags.MirroringDriver) == DisplayDeviceStateFlags.MirroringDriver )
						continue;
				}
				GameDebugger.EngineLog(LogLevel.Debug, "Found display device: Id={0}; Key={1}; Name={2}; String={3}; Flags={4}", dd.DeviceID, dd.DeviceKey, dd.DeviceName, dd.DeviceString, dd.StateFlags);
				list.Add(new DisplayDevice(dd));
				dd = new DisplayDeviceInfoStruct();
			}
			return list;
		}

		#endregion

		#region Display mode related

		public List<IDisplayMode> GetModes() {
			return GetModes(false);
		}
		
		public List<IDisplayMode> GetModes(bool list_all) {
			if( !m_Exists )
				return null;
			List<IDisplayMode> list = new List<IDisplayMode>();
			int index = 0;
			DeviceModeInfoStruct dm = new DeviceModeInfoStruct();
			dm.Size = DeviceModeInfoStruct.StructSize;
			while ( IGE.Platform.Win32.API.Externals.EnumDisplaySettings(m_DeviceInfo.DeviceName, index++, ref dm) ) {
				if( !list_all ) {
					if( dm.DisplayOrientation != 0 ) continue; // list only normal orientation modes
					if( dm.DisplayFixedOutput != 0 ) continue; // list only default output modes
					if( dm.DisplayFlags != 0 ) continue; // list only noninterlaced and nongrayscale modes
				}
				dm.DeviceName = Id;
				list.Add(new DisplayMode(dm));
				dm = new DeviceModeInfoStruct();
				dm.Size = DeviceModeInfoStruct.StructSize;
			}
			return list;
		}
		
		#endregion
		
		public void Dispose() {
			if( m_Exists )
				DisplayMode.ResolutionChangeEvent -= RefreshMonitorData;
			m_Exists = false;			
		}
		
		#region Structures and Enums
		
		public override string ToString() {
			return String.Format("{0} | {1} | {2} | {3} | {4}", m_DeviceInfo.DeviceName, m_DeviceInfo.DeviceString, m_DeviceInfo.StateFlags, m_DeviceInfo.DeviceID, m_DeviceInfo.DeviceKey );
		}
 
	    /*internal enum MonitorFrom {
	        Null = 0,
	        Primary = 1,
	        Nearest = 2,
	    }*/
	    
		#endregion
	}
}
