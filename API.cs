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
using System.Text;
using System.Reflection;

using IGE;
using IGE.Platform;

namespace IGE.Platform.Win32 {
	/// <summary>
	/// This is a singleton class. Making several instance of this class is useless.
	/// </summary>
	public sealed partial class API : IApiDriver {
		public string DriverName { get { return "Native Win32 API"; } }
		public Version DriverVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
		public bool IsSupported { get { return true; } }
		
		/// <summary>
		/// Contains the singleton instance of this class.
		/// </summary>
		internal static IGE.Platform.Win32.API Instance = null;
		public static IApiDriver GetInstance() {
			if( Instance != null )
				return Instance;
			return Instance = new IGE.Platform.Win32.API();
		}
		
		private API() {
		}
		
		public bool Initialize() {
			return true;
		}
		
		public bool Test() {
			return true;
		}
		
		
		public void RuntimeImport(Type type, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			DinamicLibrary.RuntimeImport(type, proc_address_get_func, proc_address_get_func_param);
		}
		
		public void RuntimeImport(object instance, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			DinamicLibrary.RuntimeImport(instance, proc_address_get_func, proc_address_get_func_param);
		}
		
		
		
		public INativeWindow CreateWindow(INativeWindow parent, int x, int y, int width, int height) {
			return new Win32NativeWindow(parent, x, y, width, height);
		}
		
		
		public IDisplayDevice GetDisplayDevice(string id) {
			return new DisplayDevice(id);
		}
		
		public void ResetDisplayMode() {
			DisplayMode.Reset();
		}
		
		public IApplication GetApplication() {
			return Win32Application.GetInstance();
		}
		
		
		
		
		
		public bool QueryPerformanceCounter(ref long PerformanceCount) {
			return Externals.QueryPerformanceCounter(ref PerformanceCount);
		}

		public bool QueryPerformanceFrequency(ref long PerformanceFrequency) {
			return Externals.QueryPerformanceFrequency(ref PerformanceFrequency);
		}
		
		public static string GetShortPathName(string longPathName) {
			StringBuilder output = new StringBuilder(1024);
			Externals.GetShortPathName(String.Concat(@"\\?\", longPathName), output, 1024);
			output.Remove(0, 4);
			return output.ToString();
		}
		
		public static string GetLongPathName(string shortPathName) {
			StringBuilder output = new StringBuilder(1024);
			Externals.GetLongPathName(String.Concat(@"\\?\", shortPathName), output, 1024);
			output.Remove(0, 4);
			return output.ToString();
		}
		
		public Size2 AdjustWindowSize(int clientAreaWidth, int clientAreaHeight) {
			Rectangle rect = new Rectangle(0, 0, clientAreaWidth, clientAreaHeight);
			IGE.Platform.Win32.API.Externals.AdjustWindowRectEx(ref rect, Win32NativeWindow.DefaultStyle, false, ExtendedWindowStyleFlags.ApplicationWindow);
			rect.FixLTRB();
			return rect.Size;
		}
	}
}
