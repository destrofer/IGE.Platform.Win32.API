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
using System.Runtime.InteropServices; // needed to import from dll

using IGE.Platform.Win32;

namespace IGE.Platform {
	public sealed class PixelFormat {
		private IntPtr m_hDC;
		
		private int m_Index;
		public int Index { get { return m_Index; } }
		public bool Exists { get { return m_Index != 0; } }
		
		private PixelFormatDescriptor m_Descriptor;
		public PixelFormatDescriptor Descriptor { get { return m_Descriptor; } }
	
		public PixelFormat(DeviceContext dc, int index) {
			m_Index = 0;
			m_Descriptor = new PixelFormatDescriptor();
			if( dc.Disposed || 0 == IGE.Platform.Win32.API.Externals.DescribePixelFormat(dc.Handle, index, PixelFormatDescriptor.StructSize, ref m_Descriptor) )
				return;
			m_hDC = dc.Handle;
			m_Index = index;
		}

		public PixelFormat(DeviceContext dc, PixelFormatDescriptor pfd) : this(dc, ref pfd) {}
		public PixelFormat(DeviceContext dc, ref PixelFormatDescriptor pfd) {
			m_Index = 0;
			m_Descriptor = pfd;
			m_Descriptor.Size = PixelFormatDescriptor.StructSize;
			m_Descriptor.Version = 1;
			m_Index = IGE.Platform.Win32.API.Externals.ChoosePixelFormat(dc.Handle, ref m_Descriptor);
		}
		
		public PixelFormat(DeviceContext dc, PixelFormat pf) : this(dc, pf.Descriptor) {}
		
		public bool Apply() {
			if( m_Index != 0 )
				return IGE.Platform.Win32.API.Externals.SetPixelFormat(m_hDC, m_Index, ref m_Descriptor);
			return false;
		}
		
		public static int GetCount(DeviceContext dc) {
			return dc.Disposed ? 0 : IGE.Platform.Win32.API.Externals.DescribePixelFormat(dc.Handle, 0, 0, IntPtr.Zero);
		}
		
		public override string ToString() {
			return String.Format("PixelFormat Index={0}, Flags={3}, PixelType={4}, ColorBits={5} ({6}{7}{8}{9}<<{10}{11}{12}{13}), AccumBits={14} ({15}{16}{17}{18}), DepthBits={19}, StencilBits={20}, AuxBuffers={21}, LayerType={22}, LayerMask={23}, VisibleMask={24}, DamageMask={25}", Index,
				Descriptor.Size,
				Descriptor.Version,
				Descriptor.Flags,
				Descriptor.PixelType,
				Descriptor.ColorBits,
				Descriptor.RedBits,
				Descriptor.GreenBits,
				Descriptor.BlueBits,
				Descriptor.AlphaBits,
				Descriptor.RedShift,
				Descriptor.GreenShift,
				Descriptor.BlueShift,
				Descriptor.AlphaShift,
				Descriptor.AccumBits,
				Descriptor.AccumRedBits,
				Descriptor.AccumGreenBits,
				Descriptor.AccumBlueBits,
				Descriptor.AccumAlphaBits,
				Descriptor.DepthBits,
				Descriptor.StencilBits,
				Descriptor.AuxBuffers,
				Descriptor.LayerType,
				Descriptor.LayerMask,
				Descriptor.VisibleMask,
				Descriptor.DamageMask
			);
		}
	}
}
