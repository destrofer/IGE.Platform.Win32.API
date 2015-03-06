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

using IGE.Platform;

namespace IGE.Platform.Win32 {
	/// <summary>
	/// </summary>
	public class Win32NativeWindow : INativeWindow {
		public static WindowStyleFlags DefaultStyle = WindowStyleFlags.OverlappedWindow;
		
		public event WindowMessageEventHandler PreprocessWindowMessageEvent;
		public event WindowMessageEventHandler PostprocessWindowMessageEvent;

		public event CloseEventHandler CloseEvent;
		public event MoveEventHandler MoveEvent;
		public event ResizeEventHandler ResizeEvent;
		public event SizingAndMovingStateChangeEventHandler EnterSizeMoveEvent;
		public event SizingAndMovingStateChangeEventHandler ExitSizeMoveEvent;

		protected int m_Width = 0;
		protected int m_Height = 0;
		
		protected static object SyncRoot = new object();
		
		private IntPtr m_hWnd = IntPtr.Zero;
		private WindowClass m_WindowClass = null;
		
		
		public IntPtr Handle { get { return m_hWnd; } }
		
		public virtual bool Disposed { get { return m_hWnd == IntPtr.Zero; } }
		public virtual bool Exists { get { return m_hWnd != IntPtr.Zero; } }
		
		public Win32NativeWindow(INativeWindow parent, int x, int y, int w, int h) : this(new WindowCreateStruct {
				WindowTitle = "IGE NativeWindow",
				X = x,
				Y = y,
				Width = w,
				Height = h,
				Style = Win32NativeWindow.DefaultStyle,
				ExStyle = ExtendedWindowStyleFlags.ApplicationWindow,
				ParentWindow = (Win32NativeWindow)parent,
				Menu = IntPtr.Zero,
				Param = IntPtr.Zero,

				ClassName = "IGENativeWindow",
				ClassStyle = WindowClassStyle.NONE, // WindowClassStyle.HREDRAW | WindowClassStyle.VREDRAW | WindowClassStyle.OWNDC,
				Background = (IntPtr)5, // Application.CreateSolidBrush(0x00000000), // (IntPtr)5 = COLOR_WINDOW
				Icon = API.Externals.LoadIcon(IntPtr.Zero, (IntPtr)32512), // 32512 = IDI_APPLICATION
				Cursor = API.Externals.LoadCursor(IntPtr.Zero, (IntPtr)32512), // 32512 = IDC_ARROW
				ClassMenu = IntPtr.Zero,
				ClassExtra = 0,
				WindowExtra = 0
			})
		{
		}
		
		public Win32NativeWindow(WindowCreateStruct create_struct) {
			m_hWnd = IntPtr.Zero;
			m_WindowClass = null;
	
			WndClassInfo wc = new WndClassInfo {
				Style = create_struct.ClassStyle,
				ClassName = create_struct.ClassName,
				WndProc = Win32Application.WndProc,
				ClassExtra = create_struct.ClassExtra,
				WindowExtra = create_struct.WindowExtra,
				Instance = Win32Application.GetInstanceHandle(),
				Background = create_struct.Background,
				Icon = create_struct.Icon,
				Cursor = create_struct.Cursor,
				MenuName = create_struct.ClassMenu
			};
			
			GameDebugger.EngineLog(LogLevel.Debug, "Registering window class '{0}'", wc.ClassName);
			m_WindowClass = new WindowClass(wc);
			
			if( !m_WindowClass.Registered )
				throw new UserFriendlyException(String.Format("Failed to register window class '{0}'", wc.ClassName), "Error while trying to create a window");

			lock( SyncRoot ) {
				if( create_struct.ParentWindow != null && create_struct.ParentWindow.Disposed )
					throw new UserFriendlyException(String.Format("Child window '{0}' cannot be created since it's parent is already disposed", create_struct.WindowTitle), "Error while trying to create a window");
				
				GameDebugger.EngineLog(LogLevel.Debug, "Creating window {0}x{1} at {2},{3}", create_struct.Width, create_struct.Height, create_struct.X, create_struct.Y);
				unsafe {
					m_hWnd = API.Externals.CreateWindowEx(
						create_struct.ExStyle,
						wc.ClassName,
						create_struct.WindowTitle,
						create_struct.Style,
						create_struct.X,
						create_struct.Y,
						create_struct.Width,
						create_struct.Height,
						(create_struct.ParentWindow == null) ? IntPtr.Zero : create_struct.ParentWindow.Handle,
						create_struct.Menu,
						wc.Instance,
						create_struct.Param
					);
					m_Width = create_struct.Width;
					m_Height = create_struct.Height;
					//GameDebugger.Log("QuickAlgos window created: 0x{0:x}", (int)m_hWnd);
				}
			}
			
			if( m_hWnd == IntPtr.Zero ) {
				Dispose();
				throw new UserFriendlyException(String.Format("Failed to create window '{0}'", create_struct.WindowTitle), "Error while trying to create a window");
			}
			
			Win32Application.RegisterWindow(this);
			
			OnCreate();
		}

		~Win32NativeWindow() {
			Dispose();
		}
		
		public virtual void Dispose() {
			lock(SyncRoot) {
				if( m_hWnd != IntPtr.Zero ) {
					//GameDebugger.Log("Destroying window");
					CloseEvent = null;
					MoveEvent = null;
					ResizeEvent = null;
					EnterSizeMoveEvent = null;
					ExitSizeMoveEvent = null;
					PreprocessWindowMessageEvent = null;
					PostprocessWindowMessageEvent = null;

					API.Externals.DestroyWindow(m_hWnd);
					Win32Application.UnregisterWindow(this);
					m_hWnd = IntPtr.Zero;
					//GameDebugger.Log("QuickAlgos window destroyed: 0x{0:x}", (int)m_hWnd);
				}
			}
			
			if( m_WindowClass != null ) {
				m_WindowClass.Dispose();
				m_WindowClass = null;
			}
		}
		
		public virtual Rectangle GetRect() {
			Rectangle rect = new Rectangle();
			if( m_hWnd != IntPtr.Zero ) {
				API.Externals.GetWindowRect(m_hWnd, ref rect);
				rect.FixLTRB();
			}
			return rect;
		}
		
		public virtual Rectangle GetClientRect() {
			Rectangle rect = new Rectangle();
			if( m_hWnd != IntPtr.Zero ) {
				API.Externals.GetClientRect(m_hWnd, ref rect);
				rect.FixLTRB();
			}
			return rect;
		}
		
		public virtual void Close() {
			if( m_hWnd != IntPtr.Zero )
				API.Externals.PostMessage(m_hWnd, WindowMessageEnum.CLOSE, (IntPtr)0, (IntPtr)0);
		}
		
		public void Show(ShowWindowCommandEnum nCmd) {
			if( m_hWnd != IntPtr.Zero )
				API.Externals.ShowWindow(m_hWnd, nCmd);
		}
		
		public virtual void Show() {
			Show(ShowWindowCommandEnum.SHOW);
		}
		
		public virtual void Hide() {
			Show(ShowWindowCommandEnum.HIDE);
		}
		
		internal IntPtr WndProcInternal(WindowMessageEnum uMsg, IntPtr wParam, IntPtr lParam) {
			if( PreprocessWindowMessageEvent != null ) {
				WindowMessageEventArgs args = new WindowMessageEventArgs(this, uMsg, wParam, lParam);
				PreprocessWindowMessageEvent.Invoke(args);
				if( args.PreventsDefault )
					return args.ReturnValue;
			}
			return WndProc(uMsg, wParam, lParam);
		}

		/// <summary>
		/// Override this method if you need to process messages that were not processed by the WndProc() method.
		/// </summary>
		/// <param name="uMsg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		protected virtual IntPtr DefWindowProc(WindowMessageEnum uMsg, IntPtr wParam, IntPtr lParam) {
			if( PostprocessWindowMessageEvent != null ) {
				WindowMessageEventArgs args = new WindowMessageEventArgs(this, uMsg, wParam, lParam);
				PostprocessWindowMessageEvent.Invoke(args);
				if( args.PreventsDefault )
					return args.ReturnValue;
			}
			return API.Externals.DefWindowProc(m_hWnd, uMsg, wParam, lParam);
		}
		
		/// <summary>
		/// Override this method if you need to process messages before they get processed by the default handler. Useful to handle input and MCI notifications.
		/// </summary>
		/// <param name="uMsg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		protected virtual IntPtr WndProc(WindowMessageEnum uMsg, IntPtr wParam, IntPtr lParam) {
			bool bDispose = false;
			
			// GameDebugger.Log("WndProc Message={0}=0x{1:x} wParam=0x{2:x} lParam=0x{3:x}", uMsg.ToString(), (int)uMsg, (int)wParam, (int)lParam);
			// GameDebugger.Add(String.Format("WndProc Message={0}=0x{1:x} wParam=0x{2:x} lParam=0x{3:x}", uMsg.ToString(), uMsg, (int)wParam, (int)lParam));
			switch(uMsg) {
				case WindowMessageEnum.CLOSE: {
					if( !CanClose() )
						return (IntPtr)1;
					OnClose();
					bDispose = true;
					break;
				}
				case WindowMessageEnum.SIZE: {
					Size2 size = new Size2((int)lParam & 0xFFFF, (int)((int)lParam >> 16) & 0xFFFF);
					OnResize(ref size);
					break;
				}
				
				case WindowMessageEnum.MOVE: {
					Point2 point;
					unchecked {
						point = new Point2((int)(short)((uint)lParam & 0xFFFF), (int)(short)((int)lParam >> 16) & 0xFFFF);
					}
					OnMove(ref point);
					break;
				}
				
				case WindowMessageEnum.ENTERSIZEMOVE: {
					OnEnterSizeMove();
					break;
				}
				
				case WindowMessageEnum.EXITSIZEMOVE: {
					OnExitSizeMove();
					break;
				}
			}
			
			IntPtr ret = DefWindowProc(uMsg, wParam, lParam);
			
			if( bDispose )
				Dispose();
			return ret;
		}
		
		public virtual int Width { get { return m_Width; } }

		public virtual int Height { get { return m_Height; } }
		
		/// <summary>
		/// Gets requested by Application.AskWindowsIfCanQuit() when application tries to close.
		/// </summary>
		/// <returns>Should return true if window has nothing against closing the application</returns>
		public virtual bool CanQuit() {
			return true;
		}
		
		/// <summary>
		/// Called when user is trying to close the window.
		/// </summary>
		public virtual bool CanClose() {
			return true;
		}
		
		
		protected virtual void OnCreate() {
		}
		
		protected virtual void OnResize(ref Size2 size) {
			if( ResizeEvent != null )
				ResizeEvent(new ResizeEventArgs(this, size));
		}
		
		protected virtual void OnMove(ref Point2 position) {
			if( MoveEvent != null )
				MoveEvent(new MoveEventArgs(this, position));
		}
		
		protected virtual void OnEnterSizeMove() {
			if( EnterSizeMoveEvent != null )
				EnterSizeMoveEvent(new SizingAndMovingStateChangeEventArgs(this));
		}
		
		protected virtual void OnExitSizeMove() {
			if( ExitSizeMoveEvent != null )
				ExitSizeMoveEvent(new SizingAndMovingStateChangeEventArgs(this));
		}
		
		protected virtual void OnClose() {
			if( CloseEvent != null )
				CloseEvent(new CloseEventArgs(this));
		}
		
		public struct WindowCreateStruct {
	        public string WindowTitle;
	        public int X;
	        public int Y;
	        public int Width;
	        public int Height;
	        public WindowStyleFlags Style;
	        public ExtendedWindowStyleFlags ExStyle;
	        public Win32NativeWindow ParentWindow;
	        public IntPtr Menu;
	        public IntPtr Param;
	        
	        public WindowClassStyle ClassStyle;
	        public string ClassName;
	        public IntPtr Background;
	        public int ClassExtra;
	        public int WindowExtra;
	        public IntPtr ClassMenu;
	        public IntPtr Icon;
	        public IntPtr Cursor;
		}
	}
}
