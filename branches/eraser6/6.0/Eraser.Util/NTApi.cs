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
		public static uint NtQuerySystemInformation(uint type, byte[] data,
			uint maxSize, out uint dataSize)
		{
			return NativeMethods.NtQuerySystemInformation(type, data, maxSize,
				out dataSize);
		}

		/// <summary>
		/// Resolves the reparse point pointed to by the path.
		/// </summary>
		/// <param name="path">The path to the reparse point.</param>
		/// <returns>The NT Namespace Name of the reparse point.</returns>
		public static string ResolveReparsePoint(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentException(path);

			//If the path is a directory, remove the trailing \
			if (Directory.Exists(path) && path[path.Length - 1] == '\\')
				path = path.Remove(path.Length - 1);

			using (SafeFileHandle handle = KernelApi.NativeMethods.CreateFile(path,
				KernelApi.NativeMethods.FILE_READ_ATTRIBUTES | KernelApi.NativeMethods.FILE_READ_EA,
				KernelApi.NativeMethods.FILE_SHARE_READ | KernelApi.NativeMethods.FILE_SHARE_WRITE,
				IntPtr.Zero, KernelApi.NativeMethods.OPEN_EXISTING,
				KernelApi.NativeMethods.FILE_FLAG_OPEN_REPARSE_POINT |
				KernelApi.NativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero))
			{
				if (handle.IsInvalid)
					throw new System.IO.IOException(string.Format("Cannot open handle to {0}", path));

				int bufferSize = Marshal.SizeOf(typeof(NativeMethods.REPARSE_DATA_BUFFER)) + 260 * sizeof(char);
				IntPtr buffer = Marshal.AllocHGlobal(bufferSize);

				//Get all the information about the reparse point.
				try
				{
					uint returnedBytes = 0;
					while (!NativeMethods.DeviceIoControl(handle, NativeMethods.FSCTL_GET_REPARSE_POINT,
						IntPtr.Zero, 0, buffer, (uint)bufferSize, out returnedBytes, IntPtr.Zero))
					{
						if (Marshal.GetLastWin32Error() == 122) //ERROR_INSUFFICIENT_BUFFER
						{
							bufferSize *= 2;
							Marshal.FreeHGlobal(buffer);
							buffer = Marshal.AllocHGlobal(bufferSize);
						}
						else
						{
							throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
						}
					}

					string result = ResolveReparsePoint(buffer, path);

					//Is it a directory? If it is, we need to add a trailing \
					if (Directory.Exists(path) && !result.EndsWith("\\"))
						result += '\\';
					return result;
				}
				finally
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
		}

		private static string ResolveReparsePoint(IntPtr ptr, string path)
		{
			NativeMethods.REPARSE_DATA_BUFFER buffer = (NativeMethods.REPARSE_DATA_BUFFER)
				Marshal.PtrToStructure(ptr, typeof(NativeMethods.REPARSE_DATA_BUFFER));

			//Check that this Reparse Point has a Microsoft Tag
			if (((uint)buffer.ReparseTag & (1 << 31)) == 0)
			{
				//We can only handle Microsoft's reparse tags.
				throw new ArgumentException("Unknown Reparse point type.");
			}

			//Then handle the tags
			switch (buffer.ReparseTag)
			{
				case NativeMethods.REPARSE_DATA_TAG.IO_REPARSE_TAG_MOUNT_POINT:
					return ResolveReparsePointJunction((IntPtr)(ptr.ToInt64() + Marshal.SizeOf(buffer)));

				case NativeMethods.REPARSE_DATA_TAG.IO_REPARSE_TAG_SYMLINK:
					return ResolveReparsePointSymlink((IntPtr)(ptr.ToInt64() + Marshal.SizeOf(buffer)),
						path);

				default:
					throw new ArgumentException("Unsupported Reparse point type.");
			}
		}

		private static string ResolveReparsePointJunction(IntPtr ptr)
		{
			NativeMethods.REPARSE_DATA_BUFFER.MountPointReparseBuffer buffer =
				(NativeMethods.REPARSE_DATA_BUFFER.MountPointReparseBuffer)
				Marshal.PtrToStructure(ptr,
					typeof(NativeMethods.REPARSE_DATA_BUFFER.MountPointReparseBuffer));

			//Get the substitute and print names from the buffer.
			string substituteName;
			string printName;
			unsafe
			{
				char* path = (char*)(((byte*)ptr.ToInt64()) + Marshal.SizeOf(buffer));
				printName = new string(path + (buffer.PrintNameOffset / sizeof(char)), 0,
					buffer.PrintNameLength / sizeof(char));
				substituteName = new string(path + (buffer.SubstituteNameOffset / sizeof(char)), 0,
					buffer.SubstituteNameLength / sizeof(char));
			}

			return substituteName;
		}

		private static string ResolveReparsePointSymlink(IntPtr ptr, string path)
		{
			NativeMethods.REPARSE_DATA_BUFFER.SymbolicLinkReparseBuffer buffer =
				(NativeMethods.REPARSE_DATA_BUFFER.SymbolicLinkReparseBuffer)
				Marshal.PtrToStructure(ptr,
					typeof(NativeMethods.REPARSE_DATA_BUFFER.SymbolicLinkReparseBuffer));

			//Get the substitute and print names from the buffer.
			string substituteName;
			string printName;
			unsafe
			{
				char* pathBuffer = (char*)(((byte*)ptr.ToInt64()) + Marshal.SizeOf(buffer));
				printName = new string(pathBuffer + (buffer.PrintNameOffset / sizeof(char)), 0,
					buffer.PrintNameLength / sizeof(char));
				substituteName = new string(pathBuffer + (buffer.SubstituteNameOffset / sizeof(char)), 0,
					buffer.SubstituteNameLength / sizeof(char));
			}

			if ((buffer.Flags & NativeMethods.REPARSE_DATA_BUFFER.SymbolicLinkFlags.SYMLINK_FLAG_RELATIVE) != 0)
			{
				return Path.Combine(Path.GetDirectoryName(path), substituteName);
			}

			return substituteName;
		}

		internal static class NativeMethods
		{
			/// <summary>
			/// Queries system parameters using the NT Native API.
			/// </summary>
			/// <param name="dwType">The type of information to retrieve.</param>
			/// <param name="dwData">The buffer to receive the information.</param>
			/// <param name="dwMaxSize">The size of the buffer.</param>
			/// <param name="dwDataSize">Receives the amount of data written to the
			/// buffer.</param>
			/// <returns>A system error code.</returns>
			[DllImport("NtDll.dll")]
			public static extern uint NtQuerySystemInformation(uint dwType, byte[] dwData,
				uint dwMaxSize, out uint dwDataSize);

			/// <summary>
			/// The ZwQueryInformationFile routine returns various kinds of information
			/// about a file object.
			/// </summary>
			/// <param name="FileHandle">Handle to a file object. The handle is created
			/// by a successful call to ZwCreateFile or ZwOpenFile.</param>
			/// <param name="IoStatusBlock">Pointer to an IO_STATUS_BLOCK structure
			/// that receives the final completion status and information about
			/// the operation. The Information member receives the number of bytes
			/// that this routine actually writes to the FileInformation buffer.</param>
			/// <param name="FileInformation">Pointer to a caller-allocated buffer
			/// into which the routine writes the requested information about the
			/// file object. The FileInformationClass parameter specifies the type
			/// of information that the caller requests.</param>
			/// <param name="Length">The size, in bytes, of the buffer pointed to
			/// by FileInformation.</param>
			/// <param name="FileInformationClass">Specifies the type of information
			/// to be returned about the file, in the buffer that FileInformation
			/// points to. Device and intermediate drivers can specify any of the
			/// following FILE_INFORMATION_CLASS enumeration values, which are defined
			/// in header file Wdm.h.</param>
			/// <returns>ZwQueryInformationFile returns STATUS_SUCCESS or an appropriate
			/// NTSTATUS error code.</returns>
			[DllImport("NtDll.dll")]
			private static extern uint NtQueryInformationFile(SafeFileHandle FileHandle,
				ref IO_STATUS_BLOCK IoStatusBlock, IntPtr FileInformation, uint Length,
				FILE_INFORMATION_CLASS FileInformationClass);

			public static FILE_STREAM_INFORMATION[] NtQueryInformationFile(SafeFileHandle FileHandle)
			{
				IO_STATUS_BLOCK status = new IO_STATUS_BLOCK();
				IntPtr fileInfoPtr = IntPtr.Zero;

				try
				{
					FILE_STREAM_INFORMATION streamInfo = new FILE_STREAM_INFORMATION();
					int fileInfoPtrLength = (Marshal.SizeOf(streamInfo) + 32768) / 2;
					uint ntStatus = 0;

					do
					{
						fileInfoPtrLength *= 2;
						if (fileInfoPtr != IntPtr.Zero)
							Marshal.FreeHGlobal(fileInfoPtr);
						fileInfoPtr = Marshal.AllocHGlobal(fileInfoPtrLength);

						ntStatus = NtQueryInformationFile(FileHandle, ref status, fileInfoPtr,
							(uint)fileInfoPtrLength, FILE_INFORMATION_CLASS.FileStreamInformation);
					}
					while (ntStatus != 0 /*STATUS_SUCCESS*/ && ntStatus == 0x80000005 /*STATUS_BUFFER_OVERFLOW*/);

					//Marshal the structure manually (argh!)
					List<FILE_STREAM_INFORMATION> result = new List<FILE_STREAM_INFORMATION>();
					unsafe
					{
						for (byte* i = (byte*)fileInfoPtr; streamInfo.NextEntryOffset != 0;
							i += streamInfo.NextEntryOffset)
						{
							byte* currStreamPtr = i;
							streamInfo.NextEntryOffset = *(uint*)currStreamPtr;
							currStreamPtr += sizeof(uint);

							streamInfo.StreamNameLength = *(uint*)currStreamPtr;
							currStreamPtr += sizeof(uint);

							streamInfo.StreamSize = *(long*)currStreamPtr;
							currStreamPtr += sizeof(long);

							streamInfo.StreamAllocationSize = *(long*)currStreamPtr;
							currStreamPtr += sizeof(long);

							streamInfo.StreamName = Marshal.PtrToStringUni((IntPtr)currStreamPtr,
								(int)streamInfo.StreamNameLength / 2);
							result.Add(streamInfo);
						}
					}

					return result.ToArray();
				}
				finally
				{
					Marshal.FreeHGlobal(fileInfoPtr);
				}
			}

			public struct IO_STATUS_BLOCK
			{
				public IntPtr PointerStatus;
				public UIntPtr Information;
			}
			
			public struct FILE_STREAM_INFORMATION
			{
				/// <summary>
				/// The offset of the next FILE_STREAM_INFORMATION entry. This
				/// member is zero if no other entries follow this one. 
				/// </summary>
				public uint NextEntryOffset;

				/// <summary>
				/// Length, in bytes, of the StreamName string. 
				/// </summary>
				public uint StreamNameLength;

				/// <summary>
				/// Size, in bytes, of the stream. 
				/// </summary>
				public long StreamSize;

				/// <summary>
				/// File stream allocation size, in bytes. Usually this value
				/// is a multiple of the sector or cluster size of the underlying
				/// physical device.
				/// </summary>
				public long StreamAllocationSize;

				/// <summary>
				/// Unicode string that contains the name of the stream. 
				/// </summary>
				public string StreamName;
			}

			public enum FILE_INFORMATION_CLASS
			{
				FileDirectoryInformation = 1,
				FileFullDirectoryInformation,
				FileBothDirectoryInformation,
				FileBasicInformation,
				FileStandardInformation,
				FileInternalInformation,
				FileEaInformation,
				FileAccessInformation,
				FileNameInformation,
				FileRenameInformation,
				FileLinkInformation,
				FileNamesInformation,
				FileDispositionInformation,
				FilePositionInformation,
				FileFullEaInformation,
				FileModeInformation,
				FileAlignmentInformation,
				FileAllInformation,
				FileAllocationInformation,
				FileEndOfFileInformation,
				FileAlternateNameInformation,
				FileStreamInformation,
				FilePipeInformation,
				FilePipeLocalInformation,
				FilePipeRemoteInformation,
				FileMailslotQueryInformation,
				FileMailslotSetInformation,
				FileCompressionInformation,
				FileCopyOnWriteInformation,
				FileCompletionInformation,
				FileMoveClusterInformation,
				FileQuotaInformation,
				FileReparsePointInformation,
				FileNetworkOpenInformation,
				FileObjectIdInformation,
				FileTrackingInformation,
				FileOleDirectoryInformation,
				FileContentIndexInformation,
				FileInheritContentIndexInformation,
				FileOleInformation,
				FileMaximumInformation
			}

			/// <summary>
			/// Represents volume data. This structure is passed to the
			/// FSCTL_GET_NTFS_VOLUME_DATA control code.
			/// </summary>
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			public struct NTFS_VOLUME_DATA_BUFFER
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

			/// <summary>
			/// Retrieves information about the specified NTFS file system volume.
			/// </summary>
			public const int FSCTL_GET_NTFS_VOLUME_DATA = (9 << 16) | (25 << 2);

			/// <summary>
			/// Sends the FSCTL_GET_NTFS_VOLUME_DATA control code, returning the resuling
			/// NTFS_VOLUME_DATA_BUFFER.
			/// </summary>
			/// <param name="volume">The volume to query.</param>
			/// <returns>The NTFS_VOLUME_DATA_BUFFER structure representing the data
			/// file systme structures for the volume.</returns>
			public static NTFS_VOLUME_DATA_BUFFER GetNtfsVolumeData(VolumeInfo volume)
			{
				using (SafeFileHandle volumeHandle = KernelApi.NativeMethods.CreateFile(
					volume.VolumeId.Remove(volume.VolumeId.Length - 1),
					KernelApi.NativeMethods.GENERIC_READ, KernelApi.NativeMethods.FILE_SHARE_READ |
					KernelApi.NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero,
					KernelApi.NativeMethods.OPEN_EXISTING, 0, IntPtr.Zero))
				{
					uint resultSize = 0;
					NTFS_VOLUME_DATA_BUFFER volumeData = new NTFS_VOLUME_DATA_BUFFER();
					if (DeviceIoControl(volumeHandle, FSCTL_GET_NTFS_VOLUME_DATA,
						IntPtr.Zero, 0, out volumeData, (uint)Marshal.SizeOf(volumeData),
						out resultSize, IntPtr.Zero))
					{
						return volumeData;
					}

					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}

			[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool DeviceIoControl(SafeFileHandle hDevice,
				uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
				out NTFS_VOLUME_DATA_BUFFER lpOutBuffer, uint nOutBufferSize,
				out uint lpBytesReturned, IntPtr lpOverlapped);

			/// <summary>
			/// The REPARSE_DATA_BUFFER structure contains reparse point data for a
			/// Microsoft reparse point. (Third-party reparse point owners must use
			/// the REPARSE_GUID_DATA_BUFFER structure instead.) 
			/// </summary>
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			public struct REPARSE_DATA_BUFFER
			{
				/// <summary>
				/// Contains the reparse point information for a Symbolic Link.
				/// </summary>
				/// <remarks>The PathBuffer member found at the end of the C structure
				/// declaration is appended at the end of this structure.</remarks>
				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
				public struct SymbolicLinkReparseBuffer
				{
					/// <summary>
					/// Offset, in bytes, of the substitute name string in the PathBuffer
					/// array. Note that this offset must be divided by sizeof(char) to
					/// get the array index.
					/// </summary>
					public ushort SubstituteNameOffset;

					/// <summary>
					/// Length, in bytes, of the substitute name string. If this string is
					/// NULL-terminated, SubstituteNameLength does not include space for
					/// the UNICODE_NULL character.
					/// </summary>
					public ushort SubstituteNameLength;

					/// <summary>
					/// Offset, in bytes, of the print name string in the PathBuffer array.
					/// Note that this offset must be divided by sizeof(char) to get the
					/// array index.
					/// </summary>
					public ushort PrintNameOffset;

					/// <summary>
					/// Length, in bytes, of the print name string. If this string is
					/// NULL-terminated, PrintNameLength does not include space for the
					/// UNICODE_NULL character.
					/// </summary>
					public ushort PrintNameLength;

					/// <summary>
					/// Used to indicate if the given symbolic link is an absolute or relative
					/// symbolic link. If Flags contains SYMLINK_FLAG_RELATIVE, the symbolic
					/// link contained in the PathBuffer array (at offset SubstitueNameOffset)
					/// is processed as a relative symbolic link; otherwise, it is processed
					/// as an absolute symbolic link.
					/// </summary>
					public SymbolicLinkFlags Flags;
				}

				/// <summary>
				/// Contains the reparse point information for a Directory Junction.
				/// </summary>
				/// <remarks>The PathBuffer member found at the end of the C structure
				/// declaration is appended at the end of this structure.</remarks>
				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
				public struct MountPointReparseBuffer
				{
					/// <summary>
					/// Offset, in bytes, of the substitute name string in the PathBuffer
					/// array. Note that this offset must be divided by sizeof(char) to
					/// get the array index.
					/// </summary>
					public ushort SubstituteNameOffset;

					/// <summary>
					/// Length, in bytes, of the substitute name string. If this string is
					/// NULL-terminated, SubstituteNameLength does not include space for
					/// the UNICODE_NULL character.
					/// </summary>
					public ushort SubstituteNameLength;

					/// <summary>
					/// Offset, in bytes, of the print name string in the PathBuffer array.
					/// Note that this offset must be divided by sizeof(char) to get the
					/// array index.
					/// </summary>
					public ushort PrintNameOffset;

					/// <summary>
					/// Length, in bytes, of the print name string. If this string is
					/// NULL-terminated, PrintNameLength does not include space for the
					/// UNICODE_NULL character.
					/// </summary>
					public ushort PrintNameLength;
				}

				[Flags]
				public enum SymbolicLinkFlags
				{
					/// <summary>
					/// <see cref="SymbolicLinkReparseBuffer.Flags"/>
					/// </summary>
					SYMLINK_FLAG_RELATIVE = 0x00000001
				}

				/// <summary>
				/// Reparse point tag. Must be a Microsoft reparse point tag. (See the following Remarks section.)
				/// </summary>
				public REPARSE_DATA_TAG ReparseTag;

				/// <summary>
				/// Size, in bytes, of the reparse data in the DataBuffer member.
				/// </summary>
				public ushort ReparseDataLength;
				ushort Reserved;
			}

			/// <summary>
			/// See http://msdn.microsoft.com/en-us/library/windows/desktop/aa365511%28v=vs.85%29.aspx.
			/// </summary>
			public enum REPARSE_DATA_TAG : uint
			{
				IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003,
				IO_REPARSE_TAG_HSM = 0xC0000004,
				IO_REPARSE_TAG_HSM2 = 0x80000006,
				IO_REPARSE_TAG_SIS = 0x80000007,
				IO_REPARSE_TAG_WIM = 0x80000008,
				IO_REPARSE_TAG_CSV = 0x80000009,
				IO_REPARSE_TAG_DFS = 0x8000000A,
				IO_REPARSE_TAG_SYMLINK = 0xA000000C,
				IO_REPARSE_TAG_DFSR = 0x80000012
			}

			/// <summary>
			/// Retrieves the reparse point data associated with the file or
			/// directory identified by the specified handle.
			/// 
			/// To perform this operation, call the DeviceIoControl function with
			/// the following parameters.
			/// </summary>
			/// <param name="hDevice">A handle to the file or directory from which
			/// to retrieve the reparse point data. To retrieve a handle, use the
			/// CreateFile function.</param>
			/// <param name="dwIoControlCode">The control code for the operation.
			/// Use FSCTL_GET_REPARSE_POINT for this operation.</param>
			/// <param name="lpInBuffer">Not used with this operation; set to
			/// NULL.</param>
			/// <param name="nInBufferSize">Not used with this operation; set to
			/// zero.</param>
			/// <param name="lpOutBuffer">A pointer to a buffer that receives the reparse
			/// point data. If the reparse tag is a Microsoft reparse tag, the data is
			/// a REPARSE_DATA_BUFFER structure. Otherwise, the data is a
			/// REPARSE_GUID_DATA_BUFFER structure.</param>
			/// <param name="nOutBufferSize">The size of the output buffer, in bytes.
			/// This value must be at least the size indicated by
			/// REPARSE_GUID_DATA_BUFFER_HEADER_SIZE, plus the size of the expected
			/// user-defined data.</param>
			/// <param name="lpBytesReturned">A pointer to a variable that receives the
			/// size of the data stored in the output buffer, in bytes.
			/// 
			/// If the output buffer is too small, the call fails, GetLastError returns
			/// ERROR_INSUFFICIENT_BUFFER, and lpBytesReturned is zero.
			/// 
			/// If lpOverlapped is NULL, lpBytesReturned cannot be NULL. Even when an
			/// operation returns no output data and lpOutBuffer is NULL, DeviceIoControl
			/// makes use of lpBytesReturned. After such an operation, the value of
			/// lpBytesReturned is meaningless.
			/// 
			/// If lpOverlapped is not NULL, lpBytesReturned can be NULL. If this parameter
			/// is not NULL and the operation returns data, lpBytesReturned is meaningless
			/// until the overlapped operation has completed. To retrieve the number of bytes
			/// returned, call GetOverlappedResult. If hDevice is associated with an
			/// I/O completion port, you can retrieve the number of bytes returned by
			/// calling GetQueuedCompletionStatus.</param>
			/// <param name="lpOverlapped">A pointer to an OVERLAPPED structure.
			/// 
			/// If hDevice was opened without specifying FILE_FLAG_OVERLAPPED, lpOverlapped
			/// is ignored.
			/// 
			/// If hDevice was opened with the FILE_FLAG_OVERLAPPED flag, the operation is
			/// performed as an overlapped (asynchronous) operation. In this case, lpOverlapped
			/// must point to a valid OVERLAPPED structure that contains a handle to an event
			/// object. Otherwise, the function fails in unpredictable ways.
			/// 
			/// For overlapped operations, DeviceIoControl returns immediately, and the event
			/// object is signaled when the operation has been completed. Otherwise, the
			/// function does not return until the operation has been completed or an error
			/// occurs.</param>
			/// <returns>If the operation completes successfully, DeviceIoControl returns
			/// a nonzero value, and the output buffer pointed to by lpOutBuffer contains a
			/// valid REPARSE_GUID_DATA_BUFFER structure, or a portion thereof, depending
			/// on the value of nOutBufferSize.
			/// 
			/// If the operation fails or is pending, DeviceIoControl returns zero. The
			/// contents of the output buffer pointed to by lpOutBuffer are meaningless.
			/// For extended error information, call Marshal.GetLastWin32Error.</returns>
			[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool DeviceIoControl(SafeFileHandle hDevice,
				uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
				IntPtr lpOutBuffer, uint nOutBufferSize,
				out uint lpBytesReturned, IntPtr lpOverlapped);

			public const uint FSCTL_GET_REPARSE_POINT =  (9 << 16) | (42 << 2);
		}
	}
}
