using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

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

			throw new Exception("Unknown error calling GetDiskFreeSpace()");
		}

		/// <summary>
		/// Determines the free space size of the given volume.
		/// </summary>
		/// <param name="driveName">The path to the root of the drive, including
		/// the trailing \</param>
		/// <returns>The free space of the drive, in bytes.</returns>
		/// <remarks>This value is affected by disk quotas.</remarks>
		public static long GetFreeSpace(string driveName)
		{
			ulong userAccessibleBytes, totalUserBytes, totalFreeBytes;
			if (GetDiskFreeSpaceEx(driveName, out userAccessibleBytes,
				out totalUserBytes, out totalFreeBytes))
			{
				return (long)userAccessibleBytes;
			}

			throw new Exception("Unknown error while determining free drive space.");
		}

		/// <summary>
		/// Checks if the current user has disk quotas on the volume provided.
		/// </summary>
		/// <param name="driveName">The path to the root of the drive, including
		/// the trailing \</param>
		/// <returns>True if quotas are in effect.</returns>
		public static bool HasQuota(string driveName)
		{
			ulong userAccessibleBytes, totalUserBytes, totalFreeBytes;
			if (GetDiskFreeSpaceEx(driveName, out userAccessibleBytes,
				out totalUserBytes, out totalFreeBytes))
			{
				return userAccessibleBytes != totalFreeBytes;
			}

			throw new Exception("Unknown error while determining free drive space.");
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

		/// <summary>
		/// Retrieves information about the amount of space that is available on
		/// a disk volume, which is the total amount of space, the total amount
		/// of free space, and the total amount of free space available to the
		/// user that is associated with the calling thread.
		/// </summary>
		/// <param name="lpDirectoryName">A directory on the disk.</param>
		/// <param name="lpFreeBytesAvailable">A pointer to a variable that receives
		/// the total number of free bytes on a disk that are available to the
		/// user who is associated with the calling thread.
		/// 
		/// This parameter can be NULL.
		/// 
		/// If per-user quotas are being used, this value may be less than the
		/// total number of free bytes on a disk.</param>
		/// <param name="lpTotalNumberOfBytes">A pointer to a variable that receives
		/// the total number of bytes on a disk that are available to the user
		/// who is associated with the calling thread.
		/// 
		/// This parameter can be NULL.
		/// 
		/// If per-user quotas are being used, this value may be less than the
		/// total number of bytes on a disk.</param>
		/// <param name="lpTotalNumberOfFreeBytes">A pointer to a variable that
		/// receives the total number of free bytes on a disk.
		/// 
		/// This parameter can be NULL.</param>
		/// <returns>If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().</returns>
		/// <remarks>The values obtained by this function are of the type ULARGE_INTEGER.
		/// Do not truncate these values to 32 bits.
		/// 
		/// The GetDiskFreeSpaceEx function returns zero (0) for lpTotalNumberOfFreeBytes
		/// and lpFreeBytesAvailable for all CD requests unless the disk is an
		/// unwritten CD in a CD-RW drive.
		/// 
		/// Symbolic link behavior—If the path points to a symbolic link, the
		/// operation is performed on the target.</remarks>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx(
			[MarshalAs(UnmanagedType.LPStr)]string lpDirectoryName,
			out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes,
			out ulong lpTotalNumberOfFreeBytes);

	}
}
