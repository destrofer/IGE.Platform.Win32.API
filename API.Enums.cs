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
	public enum Win32Error : uint {
	}
	
	[Flags]
	public enum WindowStyleFlags : uint {
		Overlapped = 0x00000000,
		Popup = 0x80000000,
		Child = 0x40000000,
		Minimize = 0x20000000,
		Visible = 0x10000000,
		Disabled = 0x08000000,
		ClipSiblings = 0x04000000,
		ClipChildren = 0x02000000,
		Maximize = 0x01000000,
		Caption = Border | DialogFrame,
		Border = 0x00800000,
		DialogFrame = 0x00400000,
		VScroll = 0x00200000,
		HScreen = 0x00100000,
		SystemMenu = 0x00080000,
		ThickFrame = 0x00040000,
		Group = 0x00020000,
		TabStop = 0x00010000,
		
		MinimizeBox = 0x00020000,
		MaximizeBox = 0x00010000,
		
		Tiled = Overlapped,
		Iconic = Minimize,
		SizeBox = ThickFrame,
		TiledWindow = OverlappedWindow,
		
		// Common window styles:
		OverlappedWindow = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,
		PopupWindow = Popup | Border | SystemMenu,
		ChildWindow = Child
	}
	
	[Flags]
	public enum ExtendedWindowStyleFlags : uint {
		DialogModalFrame = 0x00000001,
		NoParentNotify = 0x00000004,
		Topmost = 0x00000008,
		AcceptFiles = 0x00000010,
		Transparent = 0x00000020,
		
		// #if(WINVER >= 0x0400)
		MdiChild = 0x00000040,
		ToolWindow = 0x00000080,
		WindowEdge = 0x00000100,
		ClientEdge = 0x00000200,
		ContextHelp = 0x00000400,
		// #endif
		
		// #if(WINVER >= 0x0400)
		Right = 0x00001000,
		Left = 0x00000000,
		RightToLeftReading = 0x00002000,
		LeftToRightReading = 0x00000000,
		LeftScrollbar = 0x00004000,
		RightScrollbar = 0x00000000,
		
		ControlParent = 0x00010000,
		StaticEdge = 0x00020000,
		ApplicationWindow = 0x00040000,
		
		OverlappedWindow = WindowEdge | ClientEdge,
		PaletteWindow = WindowEdge | ToolWindow | Topmost,
		// #endif
		
		// #if(_WIN32_WINNT >= 0x0500)
		Layered = 0x00080000,
		// #endif
		
		// #if(WINVER >= 0x0500)
		NoInheritLayout = 0x00100000, // Disable inheritence of mirroring by children
		RightToLeftLayout = 0x00400000, // Right to left mirroring
		// #endif /* WINVER >= 0x0500 */
		
		// #if(_WIN32_WINNT >= 0x0501)
		Composited = 0x02000000,
		// #endif /* _WIN32_WINNT >= 0x0501 */
		
		// #if(_WIN32_WINNT >= 0x0500)
		NoActivate = 0x08000000
		// #endif /* _WIN32_WINNT >= 0x0500 */
	}
	
	[Flags]
	public enum PixelFormatDescriptorFlags : int {
		// General PixelFormatDescriptor flags
		DOUBLEBUFFER = 0x01,
		STEREO = 0x02,
		DRAW_TO_WINDOW = 0x04,
		DRAW_TO_BITMAP = 0x08,
		SUPPORT_GDI = 0x10,
		SUPPORT_OPENGL = 0x20,
		GENERIC_FORMAT = 0x40,
		NEED_PALETTE = 0x80,
		NEED_SYSTEM_PALETTE = 0x100,
		SWAP_EXCHANGE = 0x200,
		SWAP_COPY = 0x400,
		SWAP_LAYER_BUFFERS = 0x800,
		GENERIC_ACCELERATED = 0x1000,
		SUPPORT_DIRECTDRAW = 0x2000,
		SUPPORT_COMPOSITION = 0x8000,
		
		// PixelFormatDescriptor flags for use in ChoosePixelFormat only
		DEPTH_DONTCARE = 0x20000000,
		DOUBLEBUFFER_DONTCARE = 0x40000000,
		STEREO_DONTCARE = unchecked((int)0x80000000)
	}	
	
	public enum ApiPixelType : byte {
		RGBA = 0,
		INDEXED = 1
	}
	
	public enum WindowMessageEnum : uint {
		NULL = 0x0000,
		CREATE = 0x0001,
		DESTROY = 0x0002,
		MOVE = 0x0003,
		SIZE = 0x0005,
		ACTIVATE = 0x0006,
		SETFOCUS = 0x0007,
		KILLFOCUS = 0x0008,
		SETVISIBLE = 0x0009,
		ENABLE = 0x000A,
		SETREDRAW = 0x000B,
		SETTEXT = 0x000C,
		GETTEXT = 0x000D,
		GETTEXTLENGTH = 0x000E,
		PAINT = 0x000F,
		CLOSE = 0x0010,
		QUERYENDSESSION = 0x0011,
		QUIT = 0x0012,
		QUERYOPEN = 0x0013,
		ERASEBKGND = 0x0014,
		SYSCOLORCHANGE = 0x0015,
		ENDSESSION = 0x0016,
		SYSTEMERROR = 0x0017,
		SHOWWINDOW = 0x0018,
		CTLCOLOR = 0x0019,
		WININICHANGE = 0x001A,
		SETTINGCHANGE = 0x001A,
		DEVMODECHANGE = 0x001B,
		ACTIVATEAPP = 0x001C,
		FONTCHANGE = 0x001D,
		TIMECHANGE = 0x001E,
		CANCELMODE = 0x001F,
		SETCURSOR = 0x0020,
		MOUSEACTIVATE = 0x0021,
		CHILDACTIVATE = 0x0022,
		QUEUESYNC = 0x0023,
		GETMINMAXINFO = 0x0024,
		PAINTICON = 0x0026,
		ICONERASEBKGND = 0x0027,
		NEXTDLGCTL = 0x0028,
		ALTTABACTIVE = 0x0029,
		SPOOLERSTATUS = 0x002A,
		DRAWITEM = 0x002B,
		MEASUREITEM = 0x002C,
		DELETEITEM = 0x002D,
		VKEYTOITEM = 0x002E,
		CHARTOITEM = 0x002F,
		SETFONT = 0x0030,
		GETFONT = 0x0031,
		SETHOTKEY = 0x0032,
		GETHOTKEY = 0x0033,
		FILESYSCHANGE = 0x0034,
		ISACTIVEICON = 0x0035,
		QUERYPARKICON = 0x0036,
		QUERYDRAGICON = 0x0037,
		COMPAREITEM = 0x0039,
		TESTING = 0x003A,
		OTHERWINDOWCREATED = 0x003C,
		GETOBJECT = 0x003D,
		ACTIVATESHELLWINDOW = 0x003E,
		COMPACTING = 0x0041,
		COMMNOTIFY = 0x0044,
		WINDOWPOSCHANGING = 0x0046,
		WINDOWPOSCHANGED = 0x0047,
		POWER = 0x0048,
		COPYDATA = 0x004A,
		CANCELJOURNAL = 0x004B,
		NOTIFY = 0x004E,
		INPUTLANGCHANGEREQUEST = 0x0050,
		INPUTLANGCHANGE = 0x0051,
		TCARD = 0x0052,
		HELP = 0x0053,
		USERCHANGED = 0x0054,
		NOTIFYFORMAT = 0x0055,
		CONTEXTMENU = 0x007B,
		STYLECHANGING = 0x007C,
		STYLECHANGED = 0x007D,
		DISPLAYCHANGE = 0x007E,
		GETICON = 0x007F,
		SETICON = 0x0080,
		NCCREATE = 0x0081,
		NCDESTROY = 0x0082,
		NCCALCSIZE = 0x0083,
		NCHITTEST = 0x0084,
		NCPAINT = 0x0085,
		NCACTIVATE = 0x0086,
		GETDLGCODE = 0x0087,
		SYNCPAINT = 0x0088,
		SYNCTASK = 0x0089,
		NCMOUSEMOVE = 0x00A0,
		NCLBUTTONDOWN = 0x00A1,
		NCLBUTTONUP = 0x00A2,
		NCLBUTTONDBLCLK = 0x00A3,
		NCRBUTTONDOWN = 0x00A4,
		NCRBUTTONUP = 0x00A5,
		NCRBUTTONDBLCLK = 0x00A6,
		NCMBUTTONDOWN = 0x00A7,
		NCMBUTTONUP = 0x00A8,
		NCMBUTTONDBLCLK = 0x00A9,
		NCXBUTTONDOWN    = 0x00ab,
		NCXBUTTONUP      = 0x00ac,
		NCXBUTTONDBLCLK  = 0x00ad,
		INPUT = 0x00FF,
		KEYDOWN = 0x0100,
		KEYFIRST = 0x0100,
		KEYUP = 0x0101,
		CHAR = 0x0102,
		DEADCHAR = 0x0103,
		SYSKEYDOWN = 0x0104,
		SYSKEYUP = 0x0105,
		SYSCHAR = 0x0106,
		SYSDEADCHAR = 0x0107,
		KEYLAST = 0x0108,
		IME_STARTCOMPOSITION = 0x010D,
		IME_ENDCOMPOSITION = 0x010E,
		IME_COMPOSITION = 0x010F,
		IME_KEYLAST = 0x010F,
		INITDIALOG = 0x0110,
		COMMAND = 0x0111,
		SYSCOMMAND = 0x0112,
		TIMER = 0x0113,
		HSCROLL = 0x0114,
		VSCROLL = 0x0115,
		INITMENU = 0x0116,
		INITMENUPOPUP = 0x0117,
		SYSTIMER = 0x0118,
		MENUSELECT = 0x011F,
		MENUCHAR = 0x0120,
		ENTERIDLE = 0x0121,
		MENURBUTTONUP = 0x0122,
		MENUDRAG = 0x0123,
		MENUGETOBJECT = 0x0124,
		UNINITMENUPOPUP = 0x0125,
		MENUCOMMAND = 0x0126,
		CHANGEUISTATE = 0x0127,
		UPDATEUISTATE = 0x0128,
		QUERYUISTATE = 0x0129,
		LBTRACKPOINT = 0x0131,
		CTLCOLORMSGBOX = 0x0132,
		CTLCOLOREDIT = 0x0133,
		CTLCOLORLISTBOX = 0x0134,
		CTLCOLORBTN = 0x0135,
		CTLCOLORDLG = 0x0136,
		CTLCOLORSCROLLBAR = 0x0137,
		CTLCOLORSTATIC = 0x0138,
		MOUSEMOVE = 0x0200,
		MOUSEFIRST = 0x0200,
		LBUTTONDOWN = 0x0201,
		LBUTTONUP = 0x0202,
		LBUTTONDBLCLK = 0x0203,
		RBUTTONDOWN = 0x0204,
		RBUTTONUP = 0x0205,
		RBUTTONDBLCLK = 0x0206,
		MBUTTONDOWN = 0x0207,
		MBUTTONUP = 0x0208,
		MBUTTONDBLCLK = 0x0209,
		MOUSEWHEEL = 0x020A,
		MOUSELAST = 0x020D,
		XBUTTONDOWN = 0x020B,
		XBUTTONUP = 0x020C,
		XBUTTONDBLCLK = 0x020D,
		PARENTNOTIFY = 0x0210,
		ENTERMENULOOP = 0x0211,
		EXITMENULOOP = 0x0212,
		NEXTMENU = 0x0213,
		SIZING = 0x0214,
		CAPTURECHANGED = 0x0215,
		MOVING = 0x0216,
		POWERBROADCAST = 0x0218,
		DEVICECHANGE = 0x0219,
		MDICREATE = 0x0220,
		MDIDESTROY = 0x0221,
		MDIACTIVATE = 0x0222,
		MDIRESTORE = 0x0223,
		MDINEXT = 0x0224,
		MDIMAXIMIZE = 0x0225,
		MDITILE = 0x0226,
		MDICASCADE = 0x0227,
		MDIICONARRANGE = 0x0228,
		MDIGETACTIVE = 0x0229,
		DROPOBJECT = 0x022A,
		QUERYDROPOBJECT = 0x022B,
		BEGINDRAG = 0x022C,
		DRAGLOOP = 0x022D,
		DRAGSELECT = 0x022E,
		DRAGMOVE = 0x022F,
		MDISETMENU = 0x0230,
		ENTERSIZEMOVE = 0x0231,
		EXITSIZEMOVE = 0x0232,
		DROPFILES = 0x0233,
		MDIREFRESHMENU = 0x0234,
		IME_SETCONTEXT = 0x0281,
		IME_NOTIFY = 0x0282,
		IME_CONTROL = 0x0283,
		IME_COMPOSITIONFULL = 0x0284,
		IME_SELECT = 0x0285,
		IME_CHAR = 0x0286,
		IME_REQUEST = 0x0288,
		IME_KEYDOWN = 0x0290,
		IME_KEYUP = 0x0291,
		NCMOUSEHOVER = 0x02A0,
		MOUSEHOVER = 0x02A1,
		NCMOUSELEAVE = 0x02A2,
		MOUSELEAVE = 0x02A3,
		CUT = 0x0300,
		COPY = 0x0301,
		PASTE = 0x0302,
		CLEAR = 0x0303,
		UNDO = 0x0304,
		RENDERFORMAT = 0x0305,
		RENDERALLFORMATS = 0x0306,
		DESTROYCLIPBOARD = 0x0307,
		DRAWCLIPBOARD = 0x0308,
		PAINTCLIPBOARD = 0x0309,
		VSCROLLCLIPBOARD = 0x030A,
		SIZECLIPBOARD = 0x030B,
		ASKCBFORMATNAME = 0x030C,
		CHANGECBCHAIN = 0x030D,
		HSCROLLCLIPBOARD = 0x030E,
		QUERYNEWPALETTE = 0x030F,
		PALETTEISCHANGING = 0x0310,
		PALETTECHANGED = 0x0311,
		HOTKEY = 0x0312,
		PRINT = 0x0317,
		PRINTCLIENT = 0x0318,
		HANDHELDFIRST = 0x0358,
		HANDHELDLAST = 0x035F,
		AFXFIRST = 0x0360,
		AFXLAST = 0x037F,
		PENWINFIRST = 0x0380,
		PENWINLAST = 0x038F,
		APP = 0x8000,
		USER = 0x0400,
	}
	
	public enum MultimediaMessageEnum : uint {
		JOY1MOVE = 0x03A0,
		JOY2MOVE,
		JOY1ZMOVE,
		JOY2ZMOVE,
		JOY1BUTTONDOWN = 0x03B5,
		JOY2BUTTONDOWN,
		JOY1BUTTONUP,
		JOY2BUTTONUP,
		MCINOTIFY = 0x03B9,
		WOM_OPEN = 0x03BB,
		WOM_CLOSE,
		WOM_DONE,
		WIM_OPEN,
		WIM_CLOSE,
		WIM_DATA,
		MIM_OPEN,
		MIM_CLOSE,
		MIM_DATA,
		MIM_LONGDATA,
		MIM_ERROR,
		MIM_LONGERROR,
		MOM_OPEN,
		MOM_CLOSE,
		MOM_DONE,
		DRVM_OPEN = 0x03D0,
		DRVM_CLOSE,
		DRVM_DATA,
		DRVM_ERROR,
		STREAM_OPEN,
		STREAM_CLOSE,
		STREAM_DONE,
		STREAM_ERROR,
		MOM_POSITIONCB = 0x03CA,
		MCISIGNAL,
		MIM_MOREDATA,
		MIXM_LINE_CHANGE = 0x03D0,
		MIXM_CONTROL_CHANGE = 0x03D1,
	}
	
	public enum ShowWindowCommandEnum : int {
		HIDE = 0,
		SHOWNORMAL = 1,
		NORMAL = 1,
		SHOWMINIMIZED = 2,
		SHOWMAXIMIZED = 3,
		MAXIMIZE = 3,
		SHOWNOACTIVATE = 4,
		SHOW = 5,
		MINIMIZE = 6,
		SHOWMINNOACTIVE = 7,
		SHOWNA = 8,
		RESTORE = 9,
		SHOWDEFAULT = 10,
		FORCEMINIMIZE = 11,
		MAX = 11,
	}
	
	[Flags]
	public enum WindowClassStyle : uint {
		NONE = 0x0000,
		VREDRAW = 0x0001,
		HREDRAW = 0x0002,
		DOUBLECLICKS = 0x0008,
		OWNDC = 0x0020,
		CLASSDC = 0x0040,
		PARENTDC = 0x0080,
		NOCLOSE = 0x0200,
		SAVEBITS = 0x0800,
		BYTEALIGNCLIENT = 0x1000,
		BYTEALIGNWINDOW = 0x2000,
		GLOBALCLASS = 0x4000,
		IME = 0x00010000,
		DROPSHADOW = 0x00020000
	}

	/// <summary>
	/// Capability of a device specified by device context.
	/// Read more about possible returned values here: http://msdn.microsoft.com/en-us/library/windows/desktop/dd144877(v=vs.85).aspx
	/// </summary>
	public enum DeviceCapability : int {
		/// <summary>
		/// Device driver version
		/// </summary>
		DriverVersion = 0,
		
		/// <summary>
		/// Device classification
		/// </summary>
		Technology = 2,
		
		/// <summary>
		/// Horizontal size in millimeters
		/// </summary>
		HorizontalSize = 4,
		
		/// <summary>
		/// Vertical size in millimeters
		/// </summary>
		VerticalSize = 6,
		
		/// <summary>
		/// Horizontal width in pixels
		/// </summary>
		HorizontalResolution = 8,
		
		/// <summary>
		/// Vertical height in pixels
		/// </summary>
		VerticalResolution = 10,
		
		/// <summary>
		/// Number of bits per pixel
		/// </summary>
		BitsPerPixel = 12,
		
		/// <summary>
		/// Number of planes
		/// </summary>
		PlaneCount = 14,
		
		/// <summary>
		/// Number of brushes the device has
		/// </summary>
		BrushCount = 16,
		
		/// <summary>
		/// Number of pens the device has
		/// </summary>
		PenCount = 18,
		
		/// <summary>
		/// Number of markers the device has
		/// </summary>
		MarkerCount = 20,
		
		/// <summary>
		/// Number of fonts the device has
		/// </summary>
		FontCount = 22,
		
		/// <summary>
		/// Number of colors the device supports
		/// </summary>
		ColorCount = 24,
		
		/// <summary>
		/// Size required for device descriptor
		/// </summary>
		PDDeviceSize = 26,
		
		/// <summary>
		/// Curve capabilities
		/// </summary>
		CurveCaps = 28,
		
		/// <summary>
		/// Line capabilities
		/// </summary>
		LineCaps = 30,
		
		/// <summary>
		/// Polygonal capabilities
		/// </summary>
		PolygonalCaps = 32,
		
		/// <summary>
		/// Text capabilities
		/// </summary>
		TextCaps = 34,
		
		/// <summary>
		/// Clipping capabilities
		/// </summary>
		ClipCaps = 36,
		
		/// <summary>
		/// Bitblt capabilities
		/// </summary>
		RasterCaps = 38,
		
		/// <summary>
		/// Length of the X leg
		/// </summary>
		AspectX = 40,
		
		/// <summary>
		/// Length of the Y leg
		/// </summary>
		AspectY = 42,
		
		/// <summary>
		/// Length of the hypotenuse
		/// </summary>
		AspectXY = 44,
		
		/// <summary>
		/// Shading and blending caps
		/// </summary>
		ShadeBlendCaps = 45,

		/// <summary>
		/// Logical pixels/inch in X
		/// </summary>
		LogicalPixelsX = 88,
		
		/// <summary>
		/// Logical pixels/inch in Y
		/// </summary>
		LogicalPixelsY = 90,
		
		/// <summary>
		/// Number of entries in physical palette
		/// </summary>
		PaletteSize = 104,
		
		/// <summary>
		/// Number of reserved entries in palette
		/// </summary>
		PaletteReservedCount = 106,
		
		/// <summary>
		/// Actual color resolution
		/// </summary>
		ColorResolution = 108,
		
		/// <summary>
		/// [Printing related] Physical Width in device units
		/// </summary>
		PhysicalWidth = 110,
		
		/// <summary>
		/// [Printing related] Physical Height in device units
		/// </summary>
		PhysicalHeight = 111,
		
		/// <summary>
		/// [Printing related] Physical Printable Area x margin
		/// </summary>
		PhysicalOffsetX = 112,
		
		/// <summary>
		/// [Printing related] Physical Printable Area y margin
		/// </summary>
		PhysicalOffsetY = 113,
		
		/// <summary>
		/// [Printing related] Scaling factor x
		/// </summary>
		ScalingFactorX = 114,
		
		/// <summary>
		/// [Printing related] Scaling factor y
		/// </summary>
		ScalingFactorY = 115,
		
		/// <summary>
		/// [Display driver specific] Current vertical refresh rate of the display device (for displays only) in Hz
		/// </summary>
		RefreshRate = 116,
		
		/// <summary>
		/// [Display driver specific] Horizontal width of entire desktop in pixels
		/// </summary>
		DesktopVerticalResolution = 117,
		
		/// <summary>
		/// [Display driver specific] Vertical height of entire desktop in pixels
		/// </summary>
		DesktopHorizontalResolution = 118,
		
		/// <summary>
		/// [Display driver specific] Preferred blt alignment
		/// </summary>
		BltAlignment = 119
	}	
	
	[Flags]
	public enum DeviceModeFields : uint {
		Orientation		= 0x00000001,
		PaperSize		= 0x00000002,
		PaperLength		= 0x00000004,
		PaperWidth		= 0x00000008,
		Scale			= 0x00000010,
		Position		= 0x00000020,
		Unknown1		= 0x00000040,
		Unknown2		= 0x00000080,
		Copies			= 0x00000100,
		DefaultSource	= 0x00000200,
		PrintQuality	= 0x00000400,
		Color			= 0x00000800,
		Duplex			= 0x00001000,
		YResolution		= 0x00002000,
		TTOption		= 0x00004000,
		Collate			= 0x00008000,
		FormName		= 0x00010000,
		LogPixels		= 0x00020000,
		BitsPerPel		= 0x00040000,
		PelsWidth		= 0x00080000,
		PelsHeight		= 0x00100000,
		DisplayFlags	= 0x00200000,
		DisplayFrequency = 0x00400000,
		
		ICMMethod		= 0x00800000,
		ICMIntent		= 0x01000000,
		MediaType		= 0x02000000,
		DitherType		= 0x04000000,
		PanningWidth	= 0x08000000,
		PanningHeight	= 0x10000000
	}
	
	public enum SystemMetric : uint {
		VirtualScreenLeft = 76, XVirtualScreen = VirtualScreenLeft,
		VirtualScreenTop = 77, YVirtualScreen = VirtualScreenTop,
		VirtualScreenWidth = 78, CXVirtualScreen = VirtualScreenWidth,
		VirtualScreenHeight = 79, CYVirtualScreen = VirtualScreenHeight,
	}
	
	[Flags]
	public enum ChangeDisplaySettingsEnum : uint {
		UpdateRegistry = 0x00000001,
		Test = 0x00000002,
		Fullscreen = 0x00000004,
	}
	
	[Flags]
	public enum DisplayDeviceStateFlags : uint {
		None              = 0x00000000,
		AttachedToDesktop = 0x00000001,
		MultiDriver       = 0x00000002,
		PrimaryDevice     = 0x00000004,
		MirroringDriver   = 0x00000008,
		VgaCompatible     = 0x00000010,
		Removable         = 0x00000020,
		ModesPruned       = 0x08000000,
		Remote            = 0x04000000,
		Disconnect        = 0x02000000,
		
		// Child device state
		Active            = 0x00000001,
		Attached          = 0x00000002,
	}
	
	public enum ConsoleCodePage : uint {
		UTF16LE = 1200,
		UTF16BE = 1201,
		UTF32LE = 12000,
		UTF32BE = 12001,
		UTF7 = 65000,
		UTF8 = 65001,
	}
}
