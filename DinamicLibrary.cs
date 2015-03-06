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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace IGE.Platform {
	public sealed partial class DinamicLibrary : IDisposable {

		#region Automatic runtime importing into static delegates of provided type
		
		internal static Hashtable m_RuntimeImportedLibraries = new Hashtable();
		
		public static void RuntimeImport(Type type, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			if( type == null )
				return;
			RuntimeImportInternal(null, type, proc_address_get_func, proc_address_get_func_param);
		}
		
		public static void RuntimeImport(object instance, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			if( instance == null )
				return;
			// inline checking is done just for safety reasons ... in case other overloaded method does not get called when passing a type
			RuntimeImportInternal((instance is Type) ? null : instance, (instance is Type) ? (Type)instance : instance.GetType(), proc_address_get_func, proc_address_get_func_param);
		}
		
		private static void RuntimeImportInternal(object instance, Type target, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			FieldInfo[] fields = target.GetFields(BindingFlags.NonPublic | BindingFlags.Public | (instance == null ? BindingFlags.Static : BindingFlags.Instance));
			// GameDebugger.Log("RuntimeImport({0})", target.FullName);
			foreach( FieldInfo fi in fields ) {
				if( !(fi.FieldType.IsSubclassOf(typeof(Delegate))) ) {
					//GameDebugger.Log("Skipping {0} since it's type is {1}", fi.Name, fi.FieldType.FullName);
					continue; // only delegates may be loaded
				}
				
				// get RuntimeImportAttribute attributes assigned to delegate's type
				object[] attrs = fi.FieldType.GetCustomAttributes(typeof(RuntimeImportAttribute), false);
				//GameDebugger.Log("Trying {0}", fi.Name);
				if( attrs != null ) {
					foreach(object attr in attrs) {
						RuntimeImportAttribute rtattr = attr as RuntimeImportAttribute;
						string fname = (rtattr.FunctionName == null) ? fi.FieldType.Name : rtattr.FunctionName;
						if( rtattr.LibraryName == null )
							throw new Exception(String.Format("Null library specified for runtime loading the delegate {0}", fi.FieldType.FullName));
						
						DinamicLibrary dll = null;
						if(  m_RuntimeImportedLibraries.ContainsKey(rtattr.LibraryName) ) {
							dll = (DinamicLibrary)m_RuntimeImportedLibraries[rtattr.LibraryName];
						}
						else {
							dll = new DinamicLibrary(rtattr.LibraryName);
							m_RuntimeImportedLibraries.Add(rtattr.LibraryName, dll);
						}
						
						Delegate func = dll.Loaded ? dll.GetFunction(fname, fi.FieldType, proc_address_get_func, proc_address_get_func_param) : null;
						if( func == null ) {
							if( rtattr.Important ) {
								if( dll.Loaded )
									throw new Exception(String.Format("Could not load at runtime an important function {0} into delegate {1} from library {2} : function does not exist", fname, fi.Name, rtattr.LibraryName));
								else
									throw new Exception(String.Format("Could not load at runtime an important function {0} into delegate {1} from library {2} : library could not be loaded", fname, fi.Name, rtattr.LibraryName));
							}
						}
						fi.SetValue(instance, func);
					}
				}
			}
		}
		
		#endregion

		public static Dictionary<string, string> LibraryMap = new Dictionary<string, string>();

		static DinamicLibrary() {
			LibraryMap.Add("openal32", "openal32.dll");
			LibraryMap.Add("opengl32", "opengl32.dll");
			LibraryMap.Add("user32", "user32.dll");
		}
		
		private string m_LibraryName;
		private IntPtr m_hDLL;
		
		public bool Disposed { get { return m_hDLL == IntPtr.Zero; } }
		public bool Loaded { get { return m_hDLL != IntPtr.Zero; } }
		
		private static object SyncRoot = new object();
		
		internal DinamicLibrary(string dll_name) {
			lock(SyncRoot) {
				m_LibraryName = dll_name;
				if( !LibraryMap.TryGetValue(m_LibraryName, out dll_name) )
					dll_name = m_LibraryName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) ? m_LibraryName : String.Concat(m_LibraryName, ".dll");
				m_hDLL = IGE.Platform.Win32.API.Externals.LoadLibrary(dll_name);
				// GameDebugger.Log("Loading DLL: {0} 0x{1:x}", m_LibraryName, m_hDLL);
				if( m_hDLL == IntPtr.Zero )
					GameDebugger.EngineLog(LogLevel.Error, "Failed loading library '{0}'. Error code: {1}", dll_name, Marshal.GetLastWin32Error());
			}
		}
		
		~DinamicLibrary() {
			Dispose();
		}
		
		public void Dispose() {
			lock(SyncRoot) {
				if( m_hDLL != IntPtr.Zero ) {
					// GameDebugger.Log("Freeing DLL: {0}", m_LibraryName);
					IGE.Platform.Win32.API.Externals.FreeLibrary(m_hDLL);
					m_hDLL = IntPtr.Zero;
				}
			}
		}
		
		internal Delegate GetFunction(string function_name, Type signature, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			lock(SyncRoot) {
				if( m_hDLL == IntPtr.Zero || function_name == null )
					return null;
				IntPtr addr = IGE.Platform.Win32.API.Externals.GetProcAddress(m_hDLL, function_name);
				if( addr == IntPtr.Zero || addr == (IntPtr)1 || addr == (IntPtr)2 ) {
					if( proc_address_get_func != null ) {
						addr = proc_address_get_func(function_name, proc_address_get_func_param);
						if( addr == IntPtr.Zero )
							return null;
					}
					else
						return null;
				}
				return Marshal.GetDelegateForFunctionPointer(addr, signature);
			}
		}
	}
}