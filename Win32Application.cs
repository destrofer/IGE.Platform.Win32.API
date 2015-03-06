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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IGE.Platform.Win32 {
	/// <summary>
	/// </summary>
	public sealed class Win32Application : IApplication {
		private static Win32Application Instance;

		public event IdleEventHandler PreIdleEvent;
		public event IdleEventHandler IdleEvent;
		public event IdleEventHandler PostIdleEvent;
		
		public event ActivateAppEventHandler ActivateAppEvent;
		public event ActivateAppEventHandler DeactivateAppEvent;
		
		public static event WindowMessageEventHandler ProcessWindowMessageEvent;

		private bool m_KeepAliveWithoutWindows = false;
		private bool m_Exits = false;
		private bool m_Active = false;
		private static INativeWindow m_MainWindow = null;
		
		private Dictionary<int, Win32NativeWindow> Windows = new Dictionary<int, Win32NativeWindow>();
		
		public bool KeepAliveWithoutWindows { get { return m_KeepAliveWithoutWindows; } set { m_KeepAliveWithoutWindows = value; } }
		public bool Exits { get { return m_Exits; } }
		public bool Active { get { return m_Active; } }
		public INativeWindow MainWindow { get { return m_MainWindow; } }
		
		static Win32Application() {
			Instance = new Win32Application();
		}
		
		private Win32Application() {
		}

		public static Win32Application GetInstance() {
			return Instance;
		}
		
		public static IntPtr GetInstanceHandle(string module_name) {
			return API.Externals.GetModuleHandle(module_name);
		}

		public static IntPtr GetInstanceHandle() {
			return GetInstanceHandle(null);
		}
		
		public bool DoLoop() {
			WindowMessage msg = new WindowMessage();
			bool res = API.Externals.PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0x0001); // PM_REMOVE = 0x0001 
			if( res ) {
				// GameDebugger.Log(msg);
				if( msg.Message == WindowMessageEnum.QUIT ) {
					//GameDebugger.Log("Quit message received");
					m_Exits = AskWindowsIfCanQuit();
				}
				
				API.Externals.TranslateMessage(ref msg);
				API.Externals.DispatchMessage(ref msg);
			}
			
			if( m_Exits ) {
				// run last messages left in the queue
				//GameDebugger.Log("Finalizing message queue");
				while( API.Externals.PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0x0001) ) {
					API.Externals.TranslateMessage(ref msg);
					API.Externals.DispatchMessage(ref msg);
				}
			}
			return res;
		}
		
		public void Run() {
			while( !m_Exits ) {
				if( !DoLoop() ) {
					if( !m_KeepAliveWithoutWindows && Windows.Count == 0 )
						Application.Exit();
					if( PreIdleEvent != null )
						PreIdleEvent();
					if( IdleEvent != null )
						IdleEvent();
					if( PostIdleEvent != null )
						PostIdleEvent();
				}
			}
		}
		
		public void Exit(int exitCode) {
			if( !m_Exits )
				API.Externals.PostQuitMessage(exitCode);
		}
		
		private void OnActivate(bool active) {
			m_Active = active;
			if( m_Active ) {
				if( ActivateAppEvent != null )
					ActivateAppEvent();
			}
			else {
				if( DeactivateAppEvent != null )
					DeactivateAppEvent();
			}
		}
		
		public static IntPtr WndProc(IntPtr hWnd, WindowMessageEnum uMsg, IntPtr wParam, IntPtr lParam) {
			Win32NativeWindow window;
			
			switch( uMsg ) {
				case WindowMessageEnum.ACTIVATEAPP: {
					Instance.OnActivate(!((int)wParam == 0));
					break;
				}
			}
			
			if( Instance.Windows.TryGetValue((int)hWnd, out window) ) {
				if( ProcessWindowMessageEvent != null ) {
					WindowMessageEventArgs args = new WindowMessageEventArgs(window, uMsg, wParam, lParam);
					ProcessWindowMessageEvent(args);
					if( args.PreventsDefault )
						return args.ReturnValue;
				}
				return window.WndProcInternal(uMsg, wParam, lParam);
			}
			
			if( ProcessWindowMessageEvent != null ) {
				WindowMessageEventArgs args = new WindowMessageEventArgs(hWnd, uMsg, wParam, lParam);
				ProcessWindowMessageEvent(args);
				if( args.PreventsDefault )
					return args.ReturnValue;
			}
			return API.Externals.DefWindowProc(hWnd, uMsg, wParam, lParam);
		}
		
		public bool AskWindowsIfCanQuit() {
			lock(Windows) {
				foreach(Win32NativeWindow win in Windows.Values)
					if( !win.CanClose() )
						return false;
			}
			return true;
		}
		
		public static void RegisterWindow(Win32NativeWindow window) {
			lock(Instance.Windows) {
				if( m_MainWindow == null )
					m_MainWindow = window;
				Instance.Windows.Add((int)window.Handle, window);
			}
		}
		
		public static void UnregisterWindow(Win32NativeWindow window) {
			lock(Instance.Windows) {
				Instance.Windows.Remove((int)window.Handle);
				if( window == m_MainWindow ) {
					IEnumerator<Win32NativeWindow> windows = Instance.Windows.Values.GetEnumerator();
					windows.Reset();
					if( windows.MoveNext() )
						m_MainWindow = windows.Current;
					else
						m_MainWindow = null;
				}
			}
		}
	}
}
