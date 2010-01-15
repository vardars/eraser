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

using System.IO;

namespace Eraser.Util.ExtensionMethods
{
	/// <summary>
	/// Implements extension methods for IO-bound operations.
	/// </summary>
	public static class IO
	{
		/// <summary>
		/// Gets the parent directory of the current <see cref="System.IO.FileSystemInfo"/>
		/// object.
		/// </summary>
		/// <param name="info">The <see cref="System.IO.FileSystemInfo"/>object
		/// to query its parent.</param>
		/// <returns>The parent directory of the current
		/// <see cref="System.IO.FileSystemInfo"/> object, or null if info is
		/// aleady the root</returns>
		public static DirectoryInfo GetParent(this FileSystemInfo info)
		{
			FileInfo file = info as FileInfo;
			DirectoryInfo directory = info as DirectoryInfo;

			if (file != null)
				return file.Directory;
			else if (directory != null)
				return directory.Parent;
			else
				throw new ArgumentException("Unknown FileSystemInfo type.");
		}

		/// <summary>
		/// Moves the provided <see cref="System.IO.FileSystemInfo"/> object
		/// to the provided path.
		/// </summary>
		/// <param name="info">The <see cref="System.IO.FileSystemInfo"/> object
		/// to move.</param>
		/// <param name="path">The path to move the object to.</param>
		public static void MoveTo(this FileSystemInfo info, string path)
		{
			FileInfo file = info as FileInfo;
			DirectoryInfo directory = info as DirectoryInfo;

			if (file != null)
				file.MoveTo(path);
			else if (directory != null)
				directory.MoveTo(path);
			else
				throw new ArgumentException("Unknown FileSystemInfo type.");
		}
	}
}
