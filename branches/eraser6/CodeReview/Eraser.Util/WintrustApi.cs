/* 
 * $Id$
 * Copyright 2008-2009 The Eraser Project
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
using System.Windows.Forms;

namespace Eraser.Util
{
	public static class WintrustApi
	{
		/// <summary>
		/// Verifies the Authenticode signature in a file.
		/// </summary>
		/// <param name="pathToFile">The file to verify.</param>
		/// <returns>True if the file contains a valid Authenticode certificate.</returns>
		public static bool VerifyAuthenticode(string pathToFile)
		{
			IntPtr unionPointer = IntPtr.Zero;

			try
			{
				NativeMethods.WINTRUST_FILE_INFO fileinfo = new NativeMethods.WINTRUST_FILE_INFO();
				fileinfo.cbStruct = (uint)Marshal.SizeOf(typeof(NativeMethods.WINTRUST_FILE_INFO));
				fileinfo.pcwszFilePath = pathToFile;

				NativeMethods.WINTRUST_DATA data = new NativeMethods.WINTRUST_DATA();
				data.cbStruct = (uint)Marshal.SizeOf(typeof(NativeMethods.WINTRUST_DATA));
				data.dwUIChoice = NativeMethods.WINTRUST_DATA.UIChoices.WTD_UI_NONE;
				data.fdwRevocationChecks = NativeMethods.WINTRUST_DATA.RevocationChecks.WTD_REVOKE_NONE;
				data.dwUnionChoice = NativeMethods.WINTRUST_DATA.UnionChoices.WTD_CHOICE_FILE;
				unionPointer = data.pUnion = Marshal.AllocHGlobal((int)fileinfo.cbStruct);
				Marshal.StructureToPtr(fileinfo, data.pUnion, false);

				Guid guid = NativeMethods.WINTRUST_ACTION_GENERIC_VERIFY_V2;
				return NativeMethods.WinVerifyTrust(IntPtr.Zero, ref guid, ref data) == 0;
			}
			finally
			{
				if (unionPointer != IntPtr.Zero)
					Marshal.FreeHGlobal(unionPointer);
			}
		}
	}
}
