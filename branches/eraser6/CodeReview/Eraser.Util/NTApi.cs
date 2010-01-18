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
using Microsoft.Win32.SafeHandles;

namespace Eraser.Util
{
	public static class NTApi
	{
		/// <summary>
		/// Queries system parameters using the NT Native API.
		/// </summary>
		/// <param name="type">The type of information to retrieve.</param>
		/// <param name="data">The buffer to receive the information.</param>
		/// <param name="maxSize">The size of the buffer.</param>
		/// <param name="dataSize">Receives the amount of data written to the
		/// buffer.</param>
		/// <returns>A system error code.</returns>
		[Obsolete]
		public static uint NtQuerySystemInformation(uint type, byte[] data,
			uint maxSize, out uint dataSize)
		{
			return NativeMethods.NtQuerySystemInformation(type, data, maxSize,
				out dataSize);
		}
	}
}
