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
using System.ComponentModel;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Eraser.Util
{
	public class Volume
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumeID">The ID of the volume.</param>
		public Volume(string volumeID)
		{
			//Set the volume Id
			this.volumeID = volumeID;

			//Get the paths of the said volume
			IntPtr pathNamesBuffer = IntPtr.Zero;
			string pathNames = string.Empty;
			try
			{
				uint currentBufferSize = MaxPath;
				uint returnLength = 0;
				pathNamesBuffer = Marshal.AllocHGlobal((int)(currentBufferSize * sizeof(char)));
				while (!GetVolumePathNamesForVolumeName(volumeID, pathNamesBuffer, currentBufferSize,
					out returnLength))
				{
					if (Marshal.GetLastWin32Error() == 234/*ERROR_MORE_DATA*/)
					{
						currentBufferSize *= 2;
						pathNamesBuffer = Marshal.AllocHGlobal((int)(currentBufferSize * sizeof(char)));
					}
				}

				pathNames = Marshal.PtrToStringAuto(pathNamesBuffer, (int)returnLength);
			}
			finally
			{
				if (pathNamesBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(pathNamesBuffer);
			}

			//OK, the marshalling is complete. Convert the pathNames string into a list
			//of strings containing all of the volumes mountpoints; because the
			//GetVolumePathNamesForVolumeName function returns a convoluted structure
			//containing the path names.
			for (int lastIndex = 0, i = 0; i != pathNames.Length; ++i)
			{
				if (pathNames[i] == '\0')
				{
					mountPoints.Add(pathNames.Substring(lastIndex, i - lastIndex));

					lastIndex = i + 1;
					if (pathNames[lastIndex] == '\0')
						break;
				}
			}
		}

		/// <summary>
		/// Lists all the volumes in the system.
		/// </summary>
		/// <returns>Returns a list of volumes representing all volumes present in
		/// the system.</returns>
		public static List<Volume> GetVolumes()
		{
			List<Volume> result = new List<Volume>();
			IntPtr nextVolume = Marshal.AllocHGlobal(MaxPath * sizeof(char));
			try
			{
				SafeHandle handle = FindFirstVolume(nextVolume, MaxPath);
				if (handle.IsInvalid)
					return result;

				//Iterate over the volume mountpoints
				do
					result.Add(new Volume(Marshal.PtrToStringAuto(nextVolume)));
				while (FindNextVolume(handle, nextVolume, MaxPath));

				//Close the handle
				if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
					FindVolumeClose(handle);
				
				return result;
			}
			finally
			{
				Marshal.FreeHGlobal(nextVolume);
			}
		}

		/// <summary>
		/// Determines the cluster size of the current volume.
		/// </summary>
		/// <returns>The size of one cluster, in bytes.</returns>
		public uint GetClusterSize()
		{
			UInt32 clusterSize, sectorSize, freeClusters, totalClusters;
			if (GetDiskFreeSpace(mountPoints[0], out clusterSize, out sectorSize,
				out freeClusters, out totalClusters))
				return clusterSize * sectorSize;

			throw new Win32Exception(Marshal.GetLastWin32Error(),
				"Eraser.Util.Drive.GetClusterSize");
		}

		/// <summary>
		/// Checks if the current user has disk quotas on the current volume.
		/// </summary>
		/// <returns>True if quotas are in effect.</returns>
		public bool HasQuota()
		{
			UInt64 freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;
			if (GetDiskFreeSpaceEx(VolumeID, out freeBytesAvailable, out totalNumberOfBytes,
				out totalNumberOfFreeBytes))
				return totalNumberOfFreeBytes != freeBytesAvailable;

			throw new Win32Exception(Marshal.GetLastWin32Error(),
				"Eraser.Util.Drive.HasQuota");
		}

		/// <summary>
		/// Retrieves all mountpoints in the current volume, if the current volume
		/// contains volume mountpoints.
		/// </summary>
		/// <returns>A list containing Volume objects, representing each of the
		/// volumes at the mountpoints.</returns>
		public List<Volume> GetMountpoints()
		{
			List<Volume> result = new List<Volume>();
			IntPtr nextMountpoint = Marshal.AllocHGlobal(MaxPath * sizeof(char));
			try
			{
				SafeHandle handle = FindFirstVolumeMountPoint(VolumeID,
					nextMountpoint, MaxPath);
				if (handle.IsInvalid)
					return result;

				//Iterate over the volume mountpoints
				while (FindNextVolumeMountPoint(handle, nextMountpoint, MaxPath))
					result.Add(new Volume(Marshal.PtrToStringAuto(nextMountpoint)));

				//Close the handle
				if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
					FindVolumeMountPointClose(handle);

				return result;
			}
			finally
			{
				Marshal.FreeHGlobal(nextMountpoint);
			}
		}

		/// <summary>
		/// Returns the volume identifier as would be returned from FindFirstVolume.
		/// </summary>
		public string VolumeID
		{
			get { return volumeID; }
		}
		private string volumeID;

		/// <summary>
		/// The various mountpoints to the root of the volume. This list contains
		/// paths which may be a drive or a mountpoint. Every string includes the
		/// trailing backslash.
		/// </summary>
		public List<string> MountPoints
		{
			get { return mountPoints; }
			set { mountPoints = value; }
		}
		private List<string> mountPoints = new List<string>();

		/// <summary>
		/// Gets whether the current volume is mounted at any place.
		/// </summary>
		public bool IsMounted
		{
			get { return mountPoints.Count != 0; }
		}

		/// <summary>
		/// Gets the drive type; returns one of the System.IO.DriveType values.
		/// </summary>
		public DriveType VolumeType
		{
			get
			{
				return (DriveType)GetDriveType(VolumeID);
			}
		}

		internal const int MaxPath = 32768;

		#region Windows API Functions
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
			[MarshalAs(UnmanagedType.LPStr)] string lpRootPathName,
			out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector,
			out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx(
			[MarshalAs(UnmanagedType.LPStr)] string lpDirectoryName,
			out UInt64 lpFreeBytesAvailable,
			out UInt64 lpTotalNumberOfBytes,
			out UInt64 lpTotalNumberOfFreeBytes);

		/// <summary>
		/// Retrieves the name of a volume on a computer. FindFirstVolume is used
		/// to begin scanning the volumes of a computer.
		/// </summary>
		/// <param name="lpszVolumeName">A pointer to a buffer that receives a
		/// null-terminated string that specifies the unique volume name of the
		/// first volume found.</param>
		/// <param name="cchBufferLength">The length of the buffer to receive the
		/// name, in TCHARs.</param>
		/// <returns>If the function succeeds, the return value is a search handle
		/// used in a subsequent call to the FindNextVolume and FindVolumeClose
		/// functions.
		/// 
		/// If the function fails to find any volumes, the return value is the
		/// INVALID_HANDLE_VALUE error code. To get extended error information,
		/// call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "FindFirstVolumeW")]
		internal static extern SafeFileHandle FindFirstVolume(
			IntPtr lpszVolumeName,
			uint cchBufferLength);

		/// <summary>
		/// Continues a volume search started by a call to the FindFirstVolume
		/// function. FindNextVolume finds one volume per call.
		/// </summary>
		/// <param name="hFindVolume">The volume search handle returned by a previous
		/// call to the FindFirstVolume function.</param>
		/// <param name="lpszVolumeName">A pointer to a string that receives the
		/// unique volume name found.</param>
		/// <param name="cchBufferLength">The length of the buffer that receives
		/// the name, in TCHARs.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call GetLastError. If no matching files can be found, the
		/// GetLastError function returns the ERROR_NO_MORE_FILES error code. In
		/// that case, close the search with the FindVolumeClose function.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "FindNextVolumeW")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolume(SafeHandle hFindVolume,
			IntPtr lpszVolumeName, uint cchBufferLength);

		/// <summary>
		/// Closes the specified volume search handle. The FindFirstVolume and
		/// FindNextVolume functions use this search handle to locate volumes.
		/// </summary>
		/// <param name="hFindVolume">The volume search handle to be closed. This
		/// handle must have been previously opened by the FindFirstVolume function.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindVolumeClose(SafeHandle hFindVolume);

		/// <summary>
		/// Retrieves the name of a volume mount point on the specified volume.
		/// FindFirstVolumeMountPoint is used to begin scanning the volume mount
		/// points on a volume.
		/// </summary>
		/// <param name="lpszRootPathName">The unique volume name of the volume
		/// to scan for volume mount points. A trailing backslash is required.</param>
		/// <param name="lpszVolumeMountPoint">A pointer to a buffer that receives
		/// the name of the first volume mount point found.</param>
		/// <param name="cchBufferLength">The length of the buffer that receives
		/// the volume mount point name, in TCHARs.</param>
		/// <returns>If the function succeeds, the return value is a search handle
		/// used in a subsequent call to the FindNextVolumeMountPoint and
		/// FindVolumeMountPointClose functions.
		/// 
		/// If the function fails to find a volume mount point on the volume, the
		/// return value is the INVALID_HANDLE_VALUE error code. To get extended
		/// error information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "FindFirstVolumeMountPointW")]
		internal static extern SafeFileHandle FindFirstVolumeMountPoint(
			[MarshalAs(UnmanagedType.LPWStr)] string lpszRootPathName,
			IntPtr lpszVolumeMountPoint,
			uint cchBufferLength);

		/// <summary>
		/// Continues a volume mount point search started by a call to the
		/// FindFirstVolumeMountPoint function. FindNextVolumeMountPoint finds one
		/// volume mount point per call.
		/// </summary>
		/// <param name="hFindVolumeMountPoint">A mount-point search handle returned
		/// by a previous call to the FindFirstVolumeMountPoint function.</param>
		/// <param name="lpszVolumeMountPoint">A pointer to a buffer that receives
		/// the name of the volume mount point found.</param>
		/// <param name="cchBufferLength">The length of the buffer that receives
		/// the names, in TCHARs.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call GetLastError. If no matching files can be found, the
		/// GetLastError function returns the ERROR_NO_MORE_FILES error code. In
		/// that case, close the search with the FindVolumeMountPointClose function.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "FindNextVolumeMountPointW")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolumeMountPoint(SafeHandle hFindVolumeMountPoint,
			IntPtr lpszVolumeMountPoint,
			uint cchBufferLength);

		/// <summary>
		/// Closes the specified mount-point search handle. The FindFirstVolumeMountPoint
		/// and FindNextVolumeMountPoint  functions use this search handle to locate
		/// volume mount points on a specified volume.
		/// </summary>
		/// <param name="hFindVolumeMountPoint">The mount-point search handle to
		/// be closed. This handle must have been previously opened by the
		/// FindFirstVolumeMountPoint function.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		///
		/// If the function fails, the return value is zero. To get extended error
		/// information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindVolumeMountPointClose(SafeHandle hFindVolumeMountPoint);

		/// <summary>
		/// Retrieves a list of path names for the specified volume name.
		/// </summary>
		/// <param name="lpszVolumeName">The volume name.</param>
		/// <param name="lpszVolumePathNames">A pointer to a buffer that receives
		/// the list of volume path names. The list is an array of null-terminated
		/// strings terminated by an additional NULL character. If the buffer is
		/// not large enough to hold the complete list, the buffer holds as much
		/// of the list as possible.</param>
		/// <param name="cchBufferLength">The length of the lpszVolumePathNames
		/// buffer, in TCHARs.</param>
		/// <param name="lpcchReturnLength">If the call is successful, this parameter
		/// is the number of TCHARs copied to the lpszVolumePathNames buffer. Otherwise,
		/// this parameter is the size of the buffer required to hold the complete
		/// list, in TCHARs.</param>
		/// <returns></returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "GetVolumePathNamesForVolumeNameW")]
		internal static extern bool GetVolumePathNamesForVolumeName(
			[MarshalAs(UnmanagedType.LPWStr)] string lpszVolumeName,
			IntPtr lpszVolumePathNames,
			uint cchBufferLength,
			out uint lpcchReturnLength);

		/// <summary>
		/// Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk,
		/// or network drive.
		/// </summary>
		/// <param name="lpRootPathName">The root directory for the drive.
		/// 
		/// A trailing backslash is required. If this parameter is NULL, the function
		/// uses the root of the current directory.</param>
		/// <returns>The return value specifies the type of drive, which can be
		/// one of the DriveInfo.DriveType values.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, EntryPoint = "GetDriveTypeW")]
		internal static extern uint GetDriveType(
			[MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName);
		#endregion
	}
}
