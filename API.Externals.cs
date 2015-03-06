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
using System.Runtime.InteropServices; // needed to import from dll
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace IGE.Platform.Win32 {
	/// <summary>
	/// </summary>
	public sealed partial class API {
		public static class Externals {
			/* USER32 */
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern ushort RegisterClass(ref WndClassInfo window_class);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern short UnregisterClass([MarshalAs(UnmanagedType.LPTStr)]string className, IntPtr instance);
			
			[DllImport("user32.dll")]
			public static extern IntPtr GetDesktopWindow();
			
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowDC(IntPtr hWnd);
			
			[DllImport("user32.dll")]
			public extern static IntPtr GetDC(IntPtr hWnd);
			
			[DllImport("user32.dll")]
			public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDC);
			
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
			
			[DllImport("user32.dll")]
			public static extern int GetSystemMetrics(SystemMetric metric);
			
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public extern static IntPtr DefWindowProc(IntPtr hWnd, WindowMessageEnum uMsg, IntPtr wParam, IntPtr lParam);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean PostMessage(IntPtr hWnd, WindowMessageEnum Msg, IntPtr wParam, IntPtr lParam);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern IntPtr CreateWindowEx(
				ExtendedWindowStyleFlags exStyle,
				[MarshalAs(UnmanagedType.LPTStr)] string className,
				[MarshalAs(UnmanagedType.LPTStr)] string windowTitle,
				WindowStyleFlags style,
				int x, int y,
				int width, int height,
				IntPtr hParentWnd,
				IntPtr hMenu,
				IntPtr hInstance,
				IntPtr hParam);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean DestroyWindow(IntPtr hWnd);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ShowWindow(IntPtr hWnd, ShowWindowCommandEnum nCmdShow);
			
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetWindowRect(IntPtr hWnd, ref Rectangle rect);
			
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetClientRect(IntPtr hWnd, ref Rectangle rect);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern int ChangeDisplaySettings([In,Out] ref DeviceModeInfoStruct device_mode, ChangeDisplaySettingsEnum flags);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern int ChangeDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName,
			[In,Out] ref DeviceModeInfoStruct lpDevMode, IntPtr hWnd, ChangeDisplaySettingsEnum dwflags, IntPtr lParam);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			public static extern int ChangeDisplaySettings(IntPtr null_device_mode, ChangeDisplaySettingsEnum flags);
			
			[DllImport("user32.dll", SetLastError = true, CharSet=CharSet.Auto)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean EnumDisplayDevices([MarshalAs(UnmanagedType.LPTStr)] string device_name, int iDevNum, [In, Out] DisplayDeviceInfoStruct lpDisplayDevice, int dwFlags);
			
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean EnumDisplaySettings([MarshalAs(UnmanagedType.LPTStr)] string device_name, int graphics_mode, [In, Out] ref DeviceModeInfoStruct device_mode);
			
			[DllImport("user32.dll", SetLastError = true)]
			public unsafe static extern Boolean EnumDisplayMonitors(IntPtr hDC, IntPtr lpClipRect, [MarshalAs(UnmanagedType.FunctionPtr)] EnumDisplayMonitorsProc enumProc, IntPtr dwData);
			
			[DllImport("user32.dll", SetLastError = true, CharSet=CharSet.Auto)]
			public static extern Boolean GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);
			
			[DllImport("user32.dll")]
			public static extern IntPtr LoadIcon(IntPtr hInstance, [MarshalAs(UnmanagedType.LPTStr)]string lpIconName);
			
			[DllImport("user32.dll")]
			public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);
			
			[DllImport("user32.dll")]
			public static extern IntPtr LoadCursor(IntPtr hInstance, [MarshalAs(UnmanagedType.LPTStr)]string lpCursorName);
			
			[DllImport("user32.dll")]
			public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean PeekMessage(ref WindowMessage msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, int flags);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetMessage(ref WindowMessage msg, IntPtr windowHandle, int messageFilterMin, int messageFilterMax);
			
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern void PostQuitMessage(int exitCode);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			public static extern Int32 DispatchMessage(ref WindowMessage msg);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean TranslateMessage(ref WindowMessage lpMsg);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetCursorPos(ref Point2 point);
			
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SetCursorPos(int X, int Y);
			
			[DllImport("user32.dll")]
			public static extern int ShowCursor(Boolean bShow);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ClipCursor(ref RECT clipRect);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ClipCursor(IntPtr nullPtr);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ScreenToClient(IntPtr hWnd, ref Point2 coords);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ClientToScreen(IntPtr hWnd, ref Point2 coords);
			
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean AdjustWindowRectEx( ref Rectangle coords, WindowStyleFlags dwStyle, [MarshalAs(UnmanagedType.Bool)] bool hasMenu, ExtendedWindowStyleFlags dwExStyle);
			
			/* GDI32 */
			
			[DllImport("gdi32.dll", SetLastError=true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SwapBuffers(IntPtr hDC);
			
			[DllImport("gdi32.dll")]
			public static extern int GetDeviceCaps(IntPtr hDC, DeviceCapability cap);
			
			[DllImport("gdi32.dll")]
			public static extern bool BitBlt(IntPtr hObject,int nXDest,int nYDest, int nWidth,int nHeight,IntPtr hObjectSource, int nXSrc,int nYSrc,int dwRop);
			
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC,int nWidth, int nHeight);
			
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
			
			[DllImport("gdi32.dll")]
			public static extern IntPtr CreateSolidBrush(UInt32 BrushColor);
			
			[DllImport("gdi32.dll")]
			public static extern bool DeleteDC(IntPtr hDC);
			
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);
			
			[DllImport("gdi32.dll")]
			public static extern IntPtr SelectObject(IntPtr hDC,IntPtr hObject);
			
			[DllImport("gdi32.dll")]
			unsafe public static extern int DescribePixelFormat(IntPtr hDC, int index, int pfdStructSize, PixelFormatDescriptor* pfd);
			public static int DescribePixelFormat(IntPtr hDC, int index, int size, ref PixelFormatDescriptor pfd) {
				unsafe { fixed (PixelFormatDescriptor* ppfd = &pfd) { return DescribePixelFormat(hDC, index, size, ppfd); } }
			}
			
			[DllImport("gdi32.dll")]
			public static extern int DescribePixelFormat(IntPtr hDC, int format, int pfdStructSize, IntPtr ptr);
			
			[DllImport("gdi32.dll")]
			unsafe public static extern int ChoosePixelFormat(IntPtr hDC, PixelFormatDescriptor* pfd);
			public static int ChoosePixelFormat(IntPtr hDC, ref PixelFormatDescriptor pfd) {
				unsafe { fixed (PixelFormatDescriptor* ppfd = &pfd) { return ChoosePixelFormat(hDC, ppfd); } }
			}
			
			[DllImport("gdi32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			unsafe public static extern Boolean SetPixelFormat(IntPtr hDC, int format, PixelFormatDescriptor* pfd);
			public static Boolean SetPixelFormat(IntPtr hDC, int index, ref PixelFormatDescriptor pfd) {
				unsafe { fixed (PixelFormatDescriptor* ppfd = &pfd) { return SetPixelFormat(hDC, index, ppfd); } }
			}
			
			/* KERNEL32 */
			
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetProcAddress(IntPtr hDLL, string function_name);
			
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern IntPtr LoadLibrary(string dll_name);
			
			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean FreeLibrary(IntPtr hDLL);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);
			
			[System.Security.SuppressUnmanagedCodeSecurity]
			[DllImport("kernel32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool QueryPerformanceCounter(ref long PerformanceCount);
			
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPTStr)]string module_name);
			
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern SafeFileHandle CreateFile(
				string fileName,
				uint fileAccess,
				uint fileShare,
				IntPtr securityAttributes,
				[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
				int flags,
				IntPtr template);
			
			// code page id list: http://msdn.microsoft.com/en-us/library/windows/desktop/dd317756%28v=vs.85%29.aspx
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern bool SetConsoleCP(ConsoleCodePage dwCodePage);
			
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern bool SetConsoleOutputCP(ConsoleCodePage dwCodePage);
			
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern SafeFileHandle CreateConsoleScreenBuffer(
				uint dwDesiredAccess,
				uint dwShareMode,
				IntPtr securityAttributes,
				int flags,
				IntPtr lpScreenBufferData_Reserved);
			
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern bool WriteConsoleOutput(
				SafeFileHandle hConsoleOutput, 
				CharInfo[] lpBuffer, 
				SmallPoint2D dwBufferSize, 
				SmallPoint2D dwBufferCoord, 
				ref SmallRect2D lpWriteRegion);
			
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool SetConsoleActiveScreenBuffer(SafeFileHandle hConsoleOutput);
			
			/// <summary>
		    /// WARNING: use IGE.Platform.Win32.API.GetShortPathName() method instead!
			/// </summary>
			/// <param name="longPath"></param>
			/// <param name="shortPath"></param>
			/// <param name="shortPathLength"></param>
			/// <returns></returns>
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		    internal static extern int GetShortPathName(string longPath, StringBuilder shortPath, int shortPathLength);
			
		    /// <summary>
		    /// WARNING: use IGE.Platform.Win32.API.GetLongPathName() method instead!
		    /// </summary>
		    /// <param name="shortPath"></param>
		    /// <param name="longPath"></param>
		    /// <param name="longPathLength"></param>
		    /// <returns></returns>
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		    internal static extern int GetLongPathName(string shortPath, StringBuilder longPath, int longPathLength);
		    
			[DllImport("kernel32.dll")]
			public static extern Win32Error GetLastError();
		}
	}
}
