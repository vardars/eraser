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
	public class VolumeInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="volumeID">The ID of the volume, in the form "\\?\Volume{GUID}\"</param>
		public VolumeInfo(string volumeID)
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
					else
						throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				pathNames = Marshal.PtrToStringUni(pathNamesBuffer, (int)returnLength);
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
					//If there are no mount points for this volume, the string will only
					//have one NULL
					if (i - lastIndex == 0)
						break;

					mountPoints.Add(pathNames.Substring(lastIndex, i - lastIndex));

					lastIndex = i + 1;
					if (pathNames[lastIndex] == '\0')
						break;
				}
			}

			//Fill up the remaining members of the structure: file system, label, etc.
			StringBuilder volumeName = new StringBuilder(MaxPath * sizeof(char)),
				   fileSystemName = new StringBuilder(MaxPath * sizeof(char));
			uint serialNumber, maxComponentLength, filesystemFlags;
			if (!GetVolumeInformation(volumeID, volumeName, MaxPath, out serialNumber,
				out maxComponentLength, out filesystemFlags, fileSystemName, MaxPath))
			{
				int lastError = Marshal.GetLastWin32Error();
				switch (lastError)
				{
					case 0:		//ERROR_NO_ERROR
					case 21:	//ERROR_NOT_READY
					case 1005:	//ERROR_UNRECOGNIZED_VOLUME
						break;

					default:
						throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}
			else
			{
				isReady = true;
				volumeLabel = volumeName.ToString();
				volumeFormat = fileSystemName.ToString();
			}
		}

		/// <summary>
		/// Lists all the volumes in the system.
		/// </summary>
		/// <returns>Returns a list of volumes representing all volumes present in
		/// the system.</returns>
		public static List<VolumeInfo> GetVolumes()
		{
			List<VolumeInfo> result = new List<VolumeInfo>();
			StringBuilder nextVolume = new StringBuilder(LongPath * sizeof(char));
			SafeHandle handle = FindFirstVolume(nextVolume, LongPath);
			if (handle.IsInvalid)
				return result;

			//Iterate over the volume mountpoints
			do
				result.Add(new VolumeInfo(nextVolume.ToString()));
			while (FindNextVolume(handle, nextVolume, LongPath));

			//Close the handle
			if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
				FindVolumeClose(handle);

			return result;
		}

		/// <summary>
		/// Creates a Volume object from its mountpoint.
		/// </summary>
		/// <param name="mountpoint">The path to the mountpoint.</param>
		/// <returns>The volume object if such a volume exists, or an exception
		/// is thrown.</returns>
		public static VolumeInfo FromMountpoint(string mountpoint)
		{
			DirectoryInfo mountpointDir = new DirectoryInfo(mountpoint);
			StringBuilder volumeID = new StringBuilder(50 * sizeof(char));

			do
			{
				string currentDir = mountpointDir.FullName;
				if (currentDir.Length > 0 && currentDir[currentDir.Length - 1] != '\\')
					currentDir += '\\';
				if (GetVolumeNameForVolumeMountPoint(currentDir, volumeID, 50))
					return new VolumeInfo(volumeID.ToString());
				else
					switch (Marshal.GetLastWin32Error())
					{
						case 1: //ERROR_INVALID_FUNCTION
						case 2: //ERROR_FILE_NOT_FOUND
						case 3: //ERROR_PATH_NOT_FOUND
						case 4390: //ERROR_NOT_A_REPARSE_POINT
							break;
						default:
							throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
					}
				mountpointDir = mountpointDir.Parent;
			}
			while (mountpointDir != null);

			throw new Win32Exception(4390 /*ERROR_NOT_A_REPARSE_POINT*/);
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
		/// Gets or sets the volume label of a drive.
		/// </summary>
		public string VolumeLabel
		{
			get { return VolumeLabel; }
			set { throw new NotImplementedException();  }
		}
		private string volumeLabel;

		/// <summary>
		/// Gets the name of the file system, such as NTFS or FAT32.
		/// </summary>
		public string VolumeFormat
		{
			get
			{
				return volumeFormat;
			}
		}
		private string volumeFormat;

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

		/// <summary>
		/// Determines the cluster size of the current volume.
		/// </summary>
		public int ClusterSize
		{
			get
			{
				uint clusterSize, sectorSize, freeClusters, totalClusters;
				if (GetDiskFreeSpace(volumeID, out clusterSize, out sectorSize,
					out freeClusters, out totalClusters))
					return (int)(clusterSize * sectorSize);

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Checks if the current user has disk quotas on the current volume.
		/// </summary>
		public bool HasQuota
		{
			get
			{
				ulong freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;
				if (GetDiskFreeSpaceEx(volumeID, out freeBytesAvailable, out totalNumberOfBytes,
					out totalNumberOfFreeBytes))
					return totalNumberOfFreeBytes != freeBytesAvailable;
				else if (Marshal.GetLastWin32Error() == 21 /*ERROR_NOT_READY*/)
					//For the lack of more appropriate responses.
					return false;

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Gets a value indicating whether a drive is ready.
		/// </summary>
		public bool IsReady
		{
			get { return isReady; }
		}
		private bool isReady = false;

		/// <summary>
		/// Gets the total amount of free space available on a drive.
		/// </summary>
		public long TotalFreeSpace
		{
			get
			{
				ulong result, dummy;
				if (GetDiskFreeSpaceEx(volumeID, out dummy, out dummy, out result))
					return (long)result;

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}
		
		/// <summary>
		/// Gets the total size of storage space on a drive.
		/// </summary>
		public long TotalSize
		{
			get
			{
				UInt64 result, dummy;
				if (GetDiskFreeSpaceEx(volumeID, out dummy, out result, out dummy))
					return (long)result;

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Indicates the amount of available free space on a drive.
		/// </summary>
		public long AvailableFreeSpace
		{
			get
			{
				UInt64 result, dummy;
				if (GetDiskFreeSpaceEx(volumeID, out result, out dummy, out dummy))
					return (long)result;

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Retrieves all mountpoints in the current volume, if the current volume
		/// contains volume mountpoints.
		/// </summary>
		public List<VolumeInfo> MountedVolumes
		{
			get
			{
				List<VolumeInfo> result = new List<VolumeInfo>();
				StringBuilder nextMountpoint = new StringBuilder(LongPath * sizeof(char));

				SafeHandle handle = FindFirstVolumeMountPoint(VolumeID,
					nextMountpoint, LongPath);
				if (handle.IsInvalid)
					return result;

				//Iterate over the volume mountpoints
				while (FindNextVolumeMountPoint(handle, nextMountpoint, LongPath))
					result.Add(new VolumeInfo(nextMountpoint.ToString()));

				//Close the handle
				if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
					FindVolumeMountPointClose(handle);

				return result;
			}
		}

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

		#region Windows API Functions
		internal const int MaxPath = 260;
		internal const int LongPath = 32768;

		/// <summary>
		/// Retrieves information about the file system and volume associated with
		/// the specified root directory.
		/// 
		/// To specify a handle when retrieving this information, use the
		/// GetVolumeInformationByHandleW function.
		/// 
		/// To retrieve the current compression state of a file or directory, use
		/// FSCTL_GET_COMPRESSION.
		/// </summary>
		/// <param name="lpRootPathName">    A pointer to a string that contains
		/// the root directory of the volume to be described.
		/// 
		/// If this parameter is NULL, the root of the current directory is used.
		/// A trailing backslash is required. For example, you specify
		/// \\MyServer\MyShare as "\\MyServer\MyShare\", or the C drive as "C:\".</param>
		/// <param name="lpVolumeNameBuffer">A pointer to a buffer that receives
		/// the name of a specified volume. The maximum buffer size is MAX_PATH+1.</param>
		/// <param name="nVolumeNameSize">The length of a volume name buffer, in
		/// TCHARs. The maximum buffer size is MAX_PATH+1.
		/// 
		/// This parameter is ignored if the volume name buffer is not supplied.</param>
		/// <param name="lpVolumeSerialNumber">A pointer to a variable that receives
		/// the volume serial number.
		/// 
		/// This parameter can be NULL if the serial number is not required.
		/// 
		/// This function returns the volume serial number that the operating system
		/// assigns when a hard disk is formatted. To programmatically obtain the
		/// hard disk's serial number that the manufacturer assigns, use the
		/// Windows Management Instrumentation (WMI) Win32_PhysicalMedia property
		/// SerialNumber.</param>
		/// <param name="lpMaximumComponentLength">A pointer to a variable that
		/// receives the maximum length, in TCHARs, of a file name component that
		/// a specified file system supports.
		/// 
		/// A file name component is the portion of a file name between backslashes.
		/// 
		/// The value that is stored in the variable that *lpMaximumComponentLength
		/// points to is used to indicate that a specified file system supports
		/// long names. For example, for a FAT file system that supports long names,
		/// the function stores the value 255, rather than the previous 8.3 indicator.
		/// Long names can also be supported on systems that use the NTFS file system.</param>
		/// <param name="lpFileSystemFlags">A pointer to a variable that receives
		/// flags associated with the specified file system.
		/// 
		/// This parameter can be one or more of the FS_FILE* flags. However,
		/// FS_FILE_COMPRESSION and FS_VOL_IS_COMPRESSED are mutually exclusive.</param>
		/// <param name="lpFileSystemNameBuffer">A pointer to a buffer that receives
		/// the name of the file system, for example, the FAT file system or the
		/// NTFS file system. The maximum buffer size is MAX_PATH+1.</param>
		/// <param name="nFileSystemNameSize">The length of the file system name
		/// buffer, in TCHARs. The maximum buffer size is MAX_PATH+1.
		/// 
		/// This parameter is ignored if the file system name buffer is not supplied.</param>
		/// <returns>If all the requested information is retrieved, the return value
		/// is nonzero.
		/// 
		/// 
		/// If not all the requested information is retrieved, the return value is
		/// zero (0). To get extended error information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetVolumeInformation(
			string lpRootPathName,
			StringBuilder lpVolumeNameBuffer,
			uint nVolumeNameSize,
			out uint lpVolumeSerialNumber,
			out uint lpMaximumComponentLength,
			out uint lpFileSystemFlags,
			StringBuilder lpFileSystemNameBuffer,
			uint nFileSystemNameSize);

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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpace(
			string lpRootPathName, out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector,
			out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters);

		/// <summary>
		/// Retrieves information about the amount of space that is available on
		/// a disk volume, which is the total amount of space, the total amount
		/// of free space, and the total amount of free space available to the
		/// user that is associated with the calling thread.
		/// </summary>
		/// <param name="lpDirectoryName">A directory on the disk.
		/// 
		/// If this parameter is NULL, the function uses the root of the current
		/// disk.
		/// 
		/// If this parameter is a UNC name, it must include a trailing backslash,
		/// for example, "\\MyServer\MyShare\".
		/// 
		/// This parameter does not have to specify the root directory on a disk.
		/// The function accepts any directory on a disk.
		/// 
		/// The calling application must have FILE_LIST_DIRECTORY access rights
		/// for this directory.</param>
		/// <param name="lpFreeBytesAvailable">A pointer to a variable that receives
		/// the total number of free bytes on a disk that are available to the
		/// user who is associated with the calling thread.
		/// 
		/// This parameter can be NULL.
		/// 
		/// If per-user quotas are being used, this value may be less than the
		/// total number of free bytes on a disk.</param>
		/// <param name="lpTotalNumberOfBytes">A pointer to a variable that receives
		/// the total number of bytes on a disk that are available to the user who
		/// is associated with the calling thread.
		/// 
		/// This parameter can be NULL.
		/// 
		/// If per-user quotas are being used, this value may be less than the
		/// total number of bytes on a disk.
		/// 
		/// To determine the total number of bytes on a disk or volume, use
		/// IOCTL_DISK_GET_LENGTH_INFO.</param>
		/// <param name="lpTotalNumberOfFreeBytes">A pointer to a variable that
		/// receives the total number of free bytes on a disk.
		/// 
		/// This parameter can be NULL.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero (0). To get extended
		/// error information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx(
			string lpDirectoryName,
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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SafeFileHandle FindFirstVolume(StringBuilder lpszVolumeName,
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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolume(SafeHandle hFindVolume,
			StringBuilder lpszVolumeName, uint cchBufferLength);

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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern SafeFileHandle FindFirstVolumeMountPoint(
			string lpszRootPathName, StringBuilder lpszVolumeMountPoint,
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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextVolumeMountPoint(
			SafeHandle hFindVolumeMountPoint, StringBuilder lpszVolumeMountPoint,
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
		/// Retrieves the unique volume name for the specified volume mount point or root directory.
		/// </summary>
		/// <param name="lpszVolumeMountPoint">The path of a volume mount point (with a trailing
		/// backslash, "\") or a drive letter indicating a root directory (in the
		/// form "D:\").</param>
		/// <param name="lpszVolumeName">A pointer to a string that receives the
		/// volume name. This name is a unique volume name of the form
		/// "\\?\Volume{GUID}\" where GUID is the GUID that identifies the volume.</param>
		/// <param name="cchBufferLength">The length of the output buffer, in TCHARs.
		/// A reasonable size for the buffer to accommodate the largest possible
		/// volume name is 50 characters.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended
		/// error information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetVolumeNameForVolumeMountPoint(
			string lpszVolumeMountPoint, StringBuilder lpszVolumeName,
			uint cchBufferLength);

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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern bool GetVolumePathNamesForVolumeName(
			string lpszVolumeName, IntPtr lpszVolumePathNames, uint cchBufferLength,
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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern uint GetDriveType(string lpRootPathName);
		#endregion
	}
}
