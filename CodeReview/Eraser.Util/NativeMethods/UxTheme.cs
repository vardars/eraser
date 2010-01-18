/* 
 * $Id$
 * Copyright 2008-2010 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Eraser.Util
{
	internal static partial class NativeMethods
	{
		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsThemeActive();

		public static bool ThemesActive
		{
			get
			{
				try
				{
					return IsThemeActive();
				}
				catch (FileLoadException)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Causes a window to use a different set of visual style information
		/// than its class normally uses.
		/// </summary>
		/// <param name="hwnd">Handle to the window whose visual style information
		/// is to be changed.</param>
		/// <param name="pszSubAppName">Pointer to a string that contains the
		/// application name to use in place of the calling application's name.
		/// If this parameter is NULL, the calling application's name is used.</param>
		/// <param name="pszSubIdList">Pointer to a string that contains a
		/// semicolon-separated list of class identifier (CLSID) names to use
		/// in place of the actual list passed by the window's class. If this
		/// parameter is NULL, the ID list from the calling class is used.</param>
		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		public static extern void SetWindowTheme(IntPtr hwnd, string pszSubAppName,
			string pszSubIdList);
	}
}
