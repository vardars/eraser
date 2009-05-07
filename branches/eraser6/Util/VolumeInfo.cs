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
			VolumeID = volumeID;

			//Get the paths of the said volume
			IntPtr pathNamesBuffer = IntPtr.Zero;
			string pathNames = string.Empty;
			try
			{
				uint currentBufferSize = KernelAPI.NativeMethods.MaxPath;
				uint returnLength = 0;
				pathNamesBuffer = Marshal.AllocHGlobal((int)(currentBufferSize * sizeof(char)));
				while (!KernelAPI.NativeMethods.GetVolumePathNamesForVolumeName(VolumeID,
					pathNamesBuffer, currentBufferSize, out returnLength))
				{
					if (Marshal.GetLastWin32Error() == 234/*ERROR_MORE_DATA*/)
					{
						Marshal.FreeHGlobal(pathNamesBuffer);
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
			MountPoints = new List<string>();
			for (int lastIndex = 0, i = 0; i != pathNames.Length; ++i)
			{
				if (pathNames[i] == '\0')
				{
					//If there are no mount points for this volume, the string will only
					//have one NULL
					if (i - lastIndex == 0)
						break;

					MountPoints.Add(pathNames.Substring(lastIndex, i - lastIndex));

					lastIndex = i + 1;
					if (pathNames[lastIndex] == '\0')
						break;
				}
			}

			//Fill up the remaining members of the structure: file system, label, etc.
			StringBuilder volumeName = new StringBuilder(KernelAPI.NativeMethods.MaxPath * sizeof(char)),
				fileSystemName = new StringBuilder(KernelAPI.NativeMethods.MaxPath * sizeof(char));
			uint serialNumber, maxComponentLength, filesystemFlags;
			if (!KernelAPI.NativeMethods.GetVolumeInformation(volumeID, volumeName,
				KernelAPI.NativeMethods.MaxPath, out serialNumber, out maxComponentLength,
				out filesystemFlags, fileSystemName, KernelAPI.NativeMethods.MaxPath))
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
				IsReady = true;
				VolumeLabel = volumeName.ToString();
				VolumeFormat = fileSystemName.ToString();
			}
		}

		/// <summary>
		/// Lists all the volumes in the system.
		/// </summary>
		/// <returns>Returns a list of volumes representing all volumes present in
		/// the system.</returns>
		public static ICollection<VolumeInfo> Volumes
		{
			get
			{
				List<VolumeInfo> result = new List<VolumeInfo>();
				StringBuilder nextVolume = new StringBuilder(
					KernelAPI.NativeMethods.LongPath * sizeof(char));
				SafeHandle handle = KernelAPI.NativeMethods.FindFirstVolume(nextVolume,
					KernelAPI.NativeMethods.LongPath);
				if (handle.IsInvalid)
					return result;

				//Iterate over the volume mountpoints
				do
					result.Add(new VolumeInfo(nextVolume.ToString()));
				while (KernelAPI.NativeMethods.FindNextVolume(handle, nextVolume,
					KernelAPI.NativeMethods.LongPath));

				//Close the handle
				if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
					KernelAPI.NativeMethods.FindVolumeClose(handle);

				return result.AsReadOnly();
			}
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
				if (KernelAPI.NativeMethods.GetVolumeNameForVolumeMountPoint(currentDir,
					volumeID, 50))
				{
					return new VolumeInfo(volumeID.ToString());
				}
				else
				{
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
				}

				mountpointDir = mountpointDir.Parent;
			}
			while (mountpointDir != null);

			throw new Win32Exception(4390 /*ERROR_NOT_A_REPARSE_POINT*/);
		}

		/// <summary>
		/// Returns the volume identifier as would be returned from FindFirstVolume.
		/// </summary>
		public string VolumeID { get; private set; }

		/// <summary>
		/// Gets or sets the volume label of a drive.
		/// </summary>
		public string VolumeLabel { get; private set; }

		/// <summary>
		/// Gets the name of the file system, such as NTFS or FAT32.
		/// </summary>
		public string VolumeFormat { get; private set; }

		/// <summary>
		/// Gets the drive type; returns one of the System.IO.DriveType values.
		/// </summary>
		public DriveType VolumeType
		{
			get
			{
				return (DriveType)KernelAPI.NativeMethods.GetDriveType(VolumeID);
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
				if (KernelAPI.NativeMethods.GetDiskFreeSpace(VolumeID, out clusterSize,
					out sectorSize, out freeClusters, out totalClusters))
				{
					return (int)(clusterSize * sectorSize);
				}

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
				if (KernelAPI.NativeMethods.GetDiskFreeSpaceEx(VolumeID, out freeBytesAvailable,
					out totalNumberOfBytes, out totalNumberOfFreeBytes))
				{
					return totalNumberOfFreeBytes != freeBytesAvailable;
				}
				else if (Marshal.GetLastWin32Error() == 21 /*ERROR_NOT_READY*/)
				{
					//For the lack of more appropriate responses.
					return false;
				}

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Gets a value indicating whether a drive is ready.
		/// </summary>
		public bool IsReady { get; private set; }

		/// <summary>
		/// Gets the total amount of free space available on a drive.
		/// </summary>
		public long TotalFreeSpace
		{
			get
			{
				ulong result, dummy;
				if (KernelAPI.NativeMethods.GetDiskFreeSpaceEx(VolumeID, out dummy,
					out dummy, out result))
				{
					return (long)result;
				}

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
				ulong result, dummy;
				if (KernelAPI.NativeMethods.GetDiskFreeSpaceEx(VolumeID, out dummy,
					out result, out dummy))
				{
					return (long)result;
				}

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
				ulong result, dummy;
				if (KernelAPI.NativeMethods.GetDiskFreeSpaceEx(VolumeID, out result,
					out dummy, out dummy))
				{
					return (long)result;
				}

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Retrieves all mountpoints in the current volume, if the current volume
		/// contains volume mountpoints.
		/// </summary>
		public ICollection<VolumeInfo> MountedVolumes
		{
			get
			{
				List<VolumeInfo> result = new List<VolumeInfo>();
				StringBuilder nextMountpoint = new StringBuilder(
					KernelAPI.NativeMethods.LongPath * sizeof(char));

				SafeHandle handle = KernelAPI.NativeMethods.FindFirstVolumeMountPoint(VolumeID,
					nextMountpoint, KernelAPI.NativeMethods.LongPath);
				if (handle.IsInvalid)
					return result;

				//Iterate over the volume mountpoints
				while (KernelAPI.NativeMethods.FindNextVolumeMountPoint(handle,
					nextMountpoint, KernelAPI.NativeMethods.LongPath))
				{
					result.Add(new VolumeInfo(nextMountpoint.ToString()));
				}

				//Close the handle
				if (Marshal.GetLastWin32Error() == 18 /*ERROR_NO_MORE_FILES*/)
					KernelAPI.NativeMethods.FindVolumeMountPointClose(handle);

				return result.AsReadOnly();
			}
		}

		/// <summary>
		/// The various mountpoints to the root of the volume. This list contains
		/// paths which may be a drive or a mountpoint. Every string includes the
		/// trailing backslash.
		/// </summary>
		public List<string> MountPoints { get; private set; }

		/// <summary>
		/// Gets whether the current volume is mounted at any place.
		/// </summary>
		public bool IsMounted
		{
			get { return MountPoints.Count != 0; }
		}
	}
}
