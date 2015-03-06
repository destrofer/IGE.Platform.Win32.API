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
using System.Collections;

namespace IGE.Platform.Win32 {
	public class WindowClass : IDisposable {
		private static Hashtable m_RegisteredClassNames = new Hashtable();
		
		protected WndClassInfo m_Info;
		public WndClassInfo Info { get { return m_Info; } }
		
		protected bool m_Registered;
		public bool Registered { get { return m_Registered; } }
		
		public WindowClass(WndClassInfo info) {
			m_Info = info;
			if( m_RegisteredClassNames.ContainsKey(m_Info.ClassName) ) {
				m_RegisteredClassNames[m_Info.ClassName] = (int)m_RegisteredClassNames[m_Info.ClassName] + 1;
				m_Registered = true;
			}
			else {
				m_RegisteredClassNames.Add(m_Info.ClassName, 1);
				unsafe { m_Registered = API.Externals.RegisterClass(ref info) != 0; }
			}
		}
		
		~WindowClass() {
			Dispose();
		}
		
		public virtual void Dispose() {
			if( m_Registered ) {
				int count;
				m_RegisteredClassNames[m_Info.ClassName] = count = (int)m_RegisteredClassNames[m_Info.ClassName] - 1;
				if( count <= 0 ) {
					m_RegisteredClassNames.Remove(m_Info.ClassName);
					unsafe { API.Externals.UnregisterClass(m_Info.ClassName, m_Info.Instance); }
				}
				m_Registered = false;
			}
		}
	}
}
