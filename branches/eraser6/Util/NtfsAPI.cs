﻿/* 
 * $Id: NTAPI.cs 348 2008-04-02 13:05:06Z lowjoel $
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
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

namespace Eraser.Util
{
	public static class NtfsAPI
	{
		/// <summary>
		/// Represents volume data. This structure is passed to the
		/// FSCTL_GET_NTFS_VOLUME_DATA control code.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct NTFS_VOLUME_DATA_BUFFER
		{
			/// <summary>
			/// The serial number of the volume. This is a unique number assigned
			/// to the volume media by the operating system.
			/// </summary>
			public long VolumeSerialNumber;

			/// <summary>
			/// The number of sectors in the specified volume.
			/// </summary>
			public long NumberSectors;

			/// <summary>
			/// The number of used and free clusters in the specified volume.
			/// </summary>
			public long TotalClusters;

			/// <summary>
			/// The number of free clusters in the specified volume.
			/// </summary>
			public long FreeClusters;

			/// <summary>
			/// The number of reserved clusters in the specified volume.
			/// </summary>
			public long TotalReserved;

			/// <summary>
			/// The number of bytes in a sector on the specified volume.
			/// </summary>
			public uint BytesPerSector;

			/// <summary>
			/// The number of bytes in a cluster on the specified volume. This
			/// value is also known as the cluster factor.
			/// </summary>
			public uint BytesPerCluster;

			/// <summary>
			/// The number of bytes in a file record segment.
			/// </summary>
			public uint BytesPerFileRecordSegment;

			/// <summary>
			/// The number of clusters in a file record segment.
			/// </summary>
			public uint ClustersPerFileRecordSegment;

			/// <summary>
			/// The length of the master file table, in bytes.
			/// </summary>
			public long MftValidDataLength;

			/// <summary>
			/// The starting logical cluster number of the master file table.
			/// </summary>
			public long MftStartLcn;

			/// <summary>
			/// The starting logical cluster number of the master file table mirror.
			/// </summary>
			public long Mft2StartLcn;

			/// <summary>
			/// The starting logical cluster number of the master file table zone.
			/// </summary>
			public long MftZoneStart;

			/// <summary>
			/// The ending logical cluster number of the master file table zone.
			/// </summary>
			public long MftZoneEnd;

			public uint ByteCount;
			public ushort MajorVersion;
			public ushort MinorVersion;
		}

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, out byte[] lpInBuffer, uint nInBufferSize,
			out byte[] lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		[DllImport("Kernel32.dll", EntryPoint = "DeviceIoControl", CharSet = CharSet.Unicode)]
		private static extern bool DeviceIoControlNtfsVolumeData(SafeFileHandle hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			out NTFS_VOLUME_DATA_BUFFER lpOutBuffer, uint nOutBufferSize,
			out uint lpBytesReturned, IntPtr lpOverlapped);

		/// <summary>
		/// Retrieves information about the specified NTFS file system volume.
		/// </summary>
		const int FSCTL_GET_NTFS_VOLUME_DATA = (9 << 16) | (25 << 2);

		/// <summary>
		/// Sends the FSCTL_GET_NTFS_VOLUME_DATA control code, returning the resuling
		/// NTFS_VOLUME_DATA_BUFFER.
		/// </summary>
		/// <param name="volume">The volume to query.</param>
		/// <returns>The NTFS_VOLUME_DATA_BUFFER structure representing the data
		/// file systme structures for the volume.</returns>
		private static NTFS_VOLUME_DATA_BUFFER GetNtfsVolumeData(VolumeInfo volume)
		{
			using (SafeFileHandle volumeHandle = File.CreateFile(
				volume.VolumeID.Remove(volume.VolumeID.Length - 1),
				File.GENERIC_READ, File.FILE_SHARE_READ | File.FILE_SHARE_WRITE,
				IntPtr.Zero, File.OPEN_EXISTING, 0, IntPtr.Zero))
			{
				uint resultSize = 0;
				NTFS_VOLUME_DATA_BUFFER volumeData = new NTFS_VOLUME_DATA_BUFFER();
				if (DeviceIoControlNtfsVolumeData(volumeHandle, FSCTL_GET_NTFS_VOLUME_DATA,
					IntPtr.Zero, 0, out volumeData, (uint)Marshal.SizeOf(volumeData),
					out resultSize, IntPtr.Zero))
				{
					return volumeData;
				}

				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Gets the actual size of the MFT.
		/// </summary>
		/// <param name="volume">The volume to query.</param>
		/// <returns>The size of the MFT.</returns>
		public static long GetMftValidSize(VolumeInfo volume)
		{
			NTFS_VOLUME_DATA_BUFFER data = GetNtfsVolumeData(volume);
			return data.MftValidDataLength;
		}

		/// <summary>
		/// Gets the size of one MFT record segment.
		/// </summary>
		/// <param name="volume">The volume to query.</param>
		/// <returns>The size of one MFT record segment.</returns>
		public static long GetMftRecordSegmentSize(VolumeInfo volume)
		{
			NTFS_VOLUME_DATA_BUFFER data = GetNtfsVolumeData(volume);
			return data.BytesPerFileRecordSegment;
		}
	}
}
