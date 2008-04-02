/* 
 * $Id$
 * Copyright 2008 The Eraser Project
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
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY
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
using System.ComponentModel;
using System.IO;

namespace Eraser.Util
{
	public static class Drive
	{
		/// <summary>
		/// Determines the cluster size of the given volume.
		/// </summary>
		/// <param name="driveName">The path to the root of the drive, including
		/// the trailing \</param>
		/// <returns>The size of one cluster, in bytes.</returns>
		public static uint GetClusterSize(string driveName)
		{
			UInt32 clusterSize = 0, sectorSize, freeClusters, totalClusters;
			if (GetDiskFreeSpace(driveName, out clusterSize, out sectorSize,
				out freeClusters, out totalClusters))
				return clusterSize * sectorSize;

			throw new Win32Exception(Marshal.GetLastWin32Error(),
				"Eraser.Util.Drive.GetClusterSize");
		}

		/// <summary>
		/// Checks if the current user has disk quotas on the volume provided.
		/// </summary>
		/// <param name="driveName">The path to the root of the drive, including
		/// the trailing \</param>
		/// <returns>True if quotas are in effect.</returns>
		public static bool HasQuota(string driveName)
		{
			DriveInfo info = new DriveInfo(driveName);
			return info.TotalFreeSpace != info.AvailableFreeSpace;
		}

		/// <summary>
		/// Retrieves information about the specified disk, including the amount
		/// of free space on the disk.
		/// 
		/// The GetDiskFreeSpace function cannot report volume sizes that are
		/// greater than 2 gigabytes (GB). To ensure that your application works
		/// with large capacity hard drives, use the GetDiskFreeSpaceEx function.
		/// </summary>
		/// <param name="lpRootPathName">The root directory of the disk for which
		/// information is to be returned. If this parameter is NULL, the function
		/// uses the root of the current disk. If this parameter is a UNC name,
		/// it must include a trailing backslash (for example, \\MyServer\MyShare\).
		/// Furthermore, a drive specification must have a trailing backslash
		/// (for example, C:\). The calling application must have FILE_LIST_DIRECTORY
		/// access rights for this directory.</param>
		/// <param name="lpSectorsPerCluster">A pointer to a variable that receives
		/// the number of sectors per cluster.</param>
		/// <param name="lpBytesPerSector">A pointer to a variable that receives
		/// the number of bytes per sector.</param>
		/// <param name="lpNumberOfFreeClusters">A pointer to a variable that
		/// receives the total number of free clusters on the disk that are
		/// available to the user who is associated with the calling thread.
		/// 
		/// If per-user disk quotas are in use, this value may be less than the 
		/// total number of free clusters on the disk.</param>
		/// <param name="lpTotalNumberOfClusters">A pointer to a variable that
		/// receives the total number of clusters on the disk that are available
		/// to the user who is associated with the calling thread.
		/// 
		/// If per-user disk quotas are in use, this value may be less than the
		/// total number of clusters on the disk.</param>
		/// <returns>If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpace(
			[MarshalAs(UnmanagedType.LPStr)]string lpRootPathName,
			out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector,
			out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters);
	}
}
