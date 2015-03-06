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

namespace IGE.Platform.Win32 {
	[StructLayout(LayoutKind.Sequential)]
	public struct SmallPoint2D {
		public short X;
		public short Y;
		
		public SmallPoint2D(short X, short Y) {
			this.X = X;
			this.Y = Y;
		}
	};
	
	[StructLayout(LayoutKind.Sequential)]
	public struct SmallRect2D {
		public short Left;
		public short Top;
		public short Right;
		public short Bottom;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT {
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct PixelFormatDescriptor {
		public short Size;
		public short Version;
		public PixelFormatDescriptorFlags Flags;
		public ApiPixelType PixelType;
		public byte ColorBits;
		public byte RedBits;
		public byte RedShift;
		public byte GreenBits;
		public byte GreenShift;
		public byte BlueBits;
		public byte BlueShift;
		public byte AlphaBits;
		public byte AlphaShift;
		public byte AccumBits;
		public byte AccumRedBits;
		public byte AccumGreenBits;
		public byte AccumBlueBits;
		public byte AccumAlphaBits;
		public byte DepthBits;
		public byte StencilBits;
		public byte AuxBuffers;
		public byte LayerType;
		private byte Reserved;
		public int LayerMask;
		public int VisibleMask;
		public int DamageMask;
		
		readonly public static short StructSize = (short)Marshal.SizeOf(typeof(PixelFormatDescriptor));
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct WndClassInfo {
		public WindowClassStyle Style;
		[MarshalAs(UnmanagedType.FunctionPtr)]
		public WndProc WndProc;
		public Int32 ClassExtra;
		public Int32 WindowExtra;
		public IntPtr Instance;
		public IntPtr Icon;
		public IntPtr Cursor;
		public IntPtr Background;
		public IntPtr MenuName;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string ClassName;
		
		readonly public static int StructSize = Marshal.SizeOf(typeof(WndClassInfo));
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct WindowMessage {
		public IntPtr hWnd;
		public WindowMessageEnum Message;
		public IntPtr wParam;
		public IntPtr lParam;
		public UInt32 Time;
		public Point2 Point;
		//internal object RefObject;
		
		public override string ToString() { return String.Format("WindowMessage hWnd=0x{0:x} Message={1}=0x{2:x} wParam=0x{3:x} lParam=0x{4:x} Point={5}", (int)hWnd, Message.ToString(), (int)Message, (int)wParam, (int)lParam, Point.ToString()); }
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct DeviceModeInfoStruct {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;
		
		public ushort SpecVersion;
		public ushort DriverVersion;
		public short Size;
		public ushort DriverExtra;
		public DeviceModeFields Fields;
		public Point2 Position;
		public uint DisplayOrientation;
		public uint DisplayFixedOutput;
		public short Color;
		public short Duplex;
		public short YResolution;
		public short TTOption;
		public short Collate;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string FormName;

		public ushort LogPixels;
		public int BitsPerPel;
		public int PelsWidth;
		public int PelsHeight;
		public uint DisplayFlags;
		public int DisplayFrequency;
		public uint ICMMethod;
		public uint ICMIntent;
		public uint MediaType;
		public uint DitherType;
		public uint Reserved1;
		public uint Reserved2;
		public uint PanningWidth;
		public uint PanningHeight;
		
		readonly public static short StructSize = (short)Marshal.SizeOf(typeof(DeviceModeInfoStruct));
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class DisplayDeviceInfoStruct {
		public readonly int StructSize;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceString;
		
		public DisplayDeviceStateFlags StateFlags;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceID;
		
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string DeviceKey;
		
		public DisplayDeviceInfoStruct() {
			StructSize = Marshal.SizeOf(typeof(DisplayDeviceInfoStruct));
		}
	}
	
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MonitorInfoEx {
		public int Size;
		public Rectangle Monitor;
		public Rectangle Work;
		public uint Flags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;
		
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(MonitorInfoEx));
		public override string ToString() {
			return String.Format("Monitor={0} Work={1} DeviceName={2}", Monitor.ToString(), Work.ToString(), DeviceName);
		}
	}
	
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct CharInfo {
		public static CharInfo Zero = new CharInfo('\0', 0);
		public static CharInfo Space = new CharInfo(' ', 7);
		
		[FieldOffset(0)] public char Char;
		[FieldOffset(2)] public short Attributes;

		public CharInfo (char chr, short attr) {
			Char = chr;
			Attributes = attr;
		}

		public CharInfo (char chr) : this(chr, 7) {
		}
		
		public static short ColorAttribute(int text_color, int bg_color) {
			return (short)((bg_color << 4) | text_color);
		}
	}
}
