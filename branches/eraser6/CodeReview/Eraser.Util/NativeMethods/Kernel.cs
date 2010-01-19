﻿/* 
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
using Microsoft.Win32.SafeHandles;

namespace Eraser.Util
{
	/// <summary>
	/// Stores Kernel32.dll functions, structs and constants.
	/// </summary>
	internal static partial class NativeMethods
	{
		/// <summary>
		/// Closes an open object handle.
		/// </summary>
		/// <param name="hObject">A valid handle to an open object.</param>
		/// <returns>If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().
		/// 
		/// If the application is running under a debugger, the function will throw
		/// an exception if it receives either a handle value that is not valid
		/// or a pseudo-handle value. This can happen if you close a handle twice,
		/// or if you call CloseHandle on a handle returned by the FindFirstFile
		/// function.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);

		/// <summary>
		/// Deletes an existing file.
		/// </summary>
		/// <param name="lpFileName">The name of the file to be deleted.
		/// 
		/// In the ANSI version of this function, the name is limited to MAX_PATH
		/// characters. To extend this limit to 32,767 wide characters, call
		/// the Unicode version of the function and prepend "\\?\" to the path.
		/// For more information, see Naming a File.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero (0). To get extended
		/// error information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		[Obsolete]
		public static extern bool DeleteFile(string lpFileName);

		/// <summary>
		/// Retrieves information about the current system.
		/// </summary>
		/// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that
		/// receives the information.</param>
		[DllImport("Kernel32.dll")]
		public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

		/// <summary>
		/// The QueryPerformanceCounter function retrieves the current value of
		/// the high-resolution performance counter.
		/// </summary>
		/// <param name="lpPerformanceCount">[out] Pointer to a variable that receives
		/// the current performance-counter value, in counts.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error. </returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		/// <summary>
		/// Contains information about the current computer system. This includes
		/// the architecture and type of the processor, the number of processors
		/// in the system, the page size, and other such information.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		internal struct SYSTEM_INFO
		{
			/// <summary>
			/// Represents a list of processor architectures.
			/// </summary>
			public enum ProcessorArchitecture : ushort
			{
				/// <summary>
				/// x64 (AMD or Intel).
				/// </summary>
				PROCESSOR_ARCHITECTURE_AMD64 = 9,

				/// <summary>
				/// Intel Itanium Processor Family (IPF).
				/// </summary>
				PROCESSOR_ARCHITECTURE_IA64 = 6,

				/// <summary>
				/// x86.
				/// </summary>
				PROCESSOR_ARCHITECTURE_INTEL = 0,

				/// <summary>
				/// Unknown architecture.
				/// </summary>
				PROCESSOR_ARCHITECTURE_UNKNOWN = 0xffff
			}

			/// <summary>
			/// The processor architecture of the installed operating system.
			/// This member can be one of the ProcessorArchitecture values.
			/// </summary>
			public ProcessorArchitecture processorArchitecture;

			/// <summary>
			/// This member is reserved for future use.
			/// </summary>
			private const ushort reserved = 0;

			/// <summary>
			/// The page size and the granularity of page protection and commitment.
			/// This is the page size used by the VirtualAlloc function.
			/// </summary>
			public uint pageSize;

			/// <summary>
			/// A pointer to the lowest memory address accessible to applications
			/// and dynamic-link libraries (DLLs).
			/// </summary>
			public IntPtr minimumApplicationAddress;

			/// <summary>
			/// A pointer to the highest memory address accessible to applications
			/// and DLLs.
			/// </summary>
			public IntPtr maximumApplicationAddress;

			/// <summary>
			/// A mask representing the set of processors configured into the system.
			/// Bit 0 is processor 0; bit 31 is processor 31.
			/// </summary>
			public IntPtr activeProcessorMask;

			/// <summary>
			/// The number of processors in the system.
			/// </summary>
			public uint numberOfProcessors;

			/// <summary>
			/// An obsolete member that is retained for compatibility. Use the
			/// wProcessorArchitecture, wProcessorLevel, and wProcessorRevision
			/// members to determine the type of processor.
			/// Name						Value
			/// PROCESSOR_INTEL_386			386
			/// PROCESSOR_INTEL_486			486
			/// PROCESSOR_INTEL_PENTIUM		586
			/// PROCESSOR_INTEL_IA64		2200
			/// PROCESSOR_AMD_X8664			8664
			/// </summary>
			public uint processorType;

			/// <summary>
			/// The granularity for the starting address at which virtual memory
			/// can be allocated. For more information, see VirtualAlloc.
			/// </summary>
			public uint allocationGranularity;

			/// <summary>
			/// The architecture-dependent processor level. It should be used only
			/// for display purposes. To determine the feature set of a processor,
			/// use the IsProcessorFeaturePresent function.
			/// 
			/// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_INTEL, wProcessorLevel
			/// is defined by the CPU vendor.
			/// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_IA64, wProcessorLevel
			/// is set to 1.
			/// </summary>
			public ushort processorLevel;

			/// <summary>
			/// The architecture-dependent processor revision. The following table
			/// shows how the revision value is assembled for each type of
			/// processor architecture.
			/// 
			/// Processor					Value
			/// Intel Pentium, Cyrix		The high byte is the model and the
			/// or NextGen 586				low byte is the stepping. For example,
			///								if the value is xxyy, the model number
			///								and stepping can be displayed as follows:
			///								Model xx, Stepping yy
			///	Intel 80386 or 80486		A value of the form xxyz.
			///								If xx is equal to 0xFF, y - 0xA is the model
			///								number, and z is the stepping identifier.
			/// 
			///								If xx is not equal to 0xFF, xx + 'A'
			///								is the stepping letter and yz is the minor stepping.
			/// </summary>
			public ushort processorRevision;
		}

		[DllImport("Kernel32.dll")]
		public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

		[Flags]
		public enum EXECUTION_STATE : uint
		{
			ES_AWAYMODE_REQUIRED = 0x00000040,
			ES_CONTINUOUS = 0x80000000,
			ES_DISPLAY_REQUIRED = 0x00000002,
			ES_SYSTEM_REQUIRED = 0x00000001,
			ES_USER_PRESENT = 0x00000004
		}

		/// <summary>
		/// Allocates a new console for the calling process.
		/// </summary>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		/// <remarks>A process can be associated with only one console, so the AllocConsole
		/// function fails if the calling process already has a console. A process can
		/// use the FreeConsole function to detach itself from its current console, then
		/// it can call AllocConsole to create a new console or AttachConsole to attach
		/// to another console.
		/// 
		/// If the calling process creates a child process, the child inherits the
		/// new console.
		/// 
		/// AllocConsole initializes standard input, standard output, and standard error
		/// handles for the new console. The standard input handle is a handle to the
		/// console's input buffer, and the standard output and standard error handles
		/// are handles to the console's screen buffer. To retrieve these handles, use
		/// the GetStdHandle function.
		/// 
		/// This function is primarily used by graphical user interface (GUI) application
		/// to create a console window. GUI applications are initialized without a
		/// console. Console applications are initialized with a console, unless they
		/// are created as detached processes (by calling the CreateProcess function
		/// with the DETACHED_PROCESS flag).</remarks>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AllocConsole();

		/// <summary>
		/// Detaches the calling process from its console.
		/// </summary>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		/// <remarks>A process can be attached to at most one console. If the calling
		/// process is not already attached to a console, the error code returned is
		/// ERROR_INVALID_PARAMETER (87).
		/// 
		/// A process can use the FreeConsole function to detach itself from its
		/// console. If other processes share the console, the console is not destroyed,
		/// but the process that called FreeConsole cannot refer to it. A console is
		/// closed when the last process attached to it terminates or calls FreeConsole.
		/// After a process calls FreeConsole, it can call the AllocConsole function to
		/// create a new console or AttachConsole to attach to another console.</remarks>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FreeConsole();

		/// <summary>
		/// The CreateFile function creates or opens a file, file stream, directory,
		/// physical disk, volume, console buffer, tape drive, communications resource,
		/// mailslot, or named pipe. The function returns a handle that can be used
		/// to access an object.
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="DesiredAccess"> access to the object, which can be read,
		/// write, or both</param>
		/// <param name="ShareMode">The sharing mode of an object, which can be
		/// read, write, both, or none</param>
		/// <param name="SecurityAttributes">A pointer to a SECURITY_ATTRIBUTES
		/// structure that determines whether or not the returned handle can be
		/// inherited by child processes. Can be null</param>
		/// <param name="CreationDisposition">An action to take on files that exist
		/// and do not exist</param>
		/// <param name="FlagsAndAttributes">The file attributes and flags.</param>
		/// <param name="hTemplateFile">A handle to a template file with the
		/// GENERIC_READ access right. The template file supplies file attributes
		/// and extended attributes for the file that is being created. This
		/// parameter can be null</param>
		/// <returns>If the function succeeds, the return value is an open handle
		/// to a specified file. If a specified file exists before the function
		/// all and dwCreationDisposition is CREATE_ALWAYS or OPEN_ALWAYS, a call
		/// to GetLastError returns ERROR_ALREADY_EXISTS, even when the function
		/// succeeds. If a file does not exist before the call, GetLastError
		/// returns 0.
		/// 
		/// If the function fails, the return value is INVALID_HANDLE_VALUE.
		/// To get extended error information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
			uint dwShareMode, IntPtr SecurityAttributes, uint dwCreationDisposition,
			uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		public const uint GENERIC_READ = 0x80000000;
		public const uint GENERIC_WRITE = 0x40000000;
		public const uint GENERIC_EXECUTE = 0x20000000;
		public const uint GENERIC_ALL = 0x10000000;

		public const uint FILE_SHARE_READ = 0x00000001;
		public const uint FILE_SHARE_WRITE = 0x00000002;
		public const uint FILE_SHARE_DELETE = 0x00000004;

		public const uint CREATE_NEW = 1;
		public const uint CREATE_ALWAYS = 2;
		public const uint OPEN_EXISTING = 3;
		public const uint OPEN_ALWAYS = 4;
		public const uint TRUNCATE_EXISTING = 5;

		public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
		public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
		public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
		public const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
		public const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
		public const uint FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
		public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
		public const uint FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
		public const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
		public const uint FILE_FLAG_OPEN_NO_RECALL = 0x00100000;
		public const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			out ushort lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, ref ushort lpInBuffer, uint nInBufferSize,
			IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		public const uint FSCTL_GET_COMPRESSION = 0x9003C;
		public const uint FSCTL_SET_COMPRESSION = 0x9C040;
		public const ushort COMPRESSION_FORMAT_NONE = 0x0000;
		public const ushort COMPRESSION_FORMAT_DEFAULT = 0x0001;

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		public const uint FSCTL_LOCK_VOLUME = 0x90018;
		public const uint FSCTL_UNLOCK_VOLUME = 0x9001C;

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			out DiskPerformanceInfoInternal lpOutBuffer, uint nOutBufferSize,
			out uint lpBytesReturned, IntPtr lpOverlapped);

		public const uint IOCTL_DISK_PERFORMANCE = ((0x00000007) << 16) | ((0x0008) << 2);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DiskPerformanceInfoInternal
		{
			public long BytesRead;
			public long BytesWritten;
			public long ReadTime;
			public long WriteTime;
			public long IdleTime;
			public uint ReadCount;
			public uint WriteCount;
			public uint QueueDepth;
			public uint SplitCount;
			public long QueryTime;
			public uint StorageDeviceNumber;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string StorageManagerName;
		}

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeviceIoControl(SafeFileHandle hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			out NTFS_VOLUME_DATA_BUFFER lpOutBuffer, uint nOutBufferSize,
			out uint lpBytesReturned, IntPtr lpOverlapped);

		/// <summary>
		/// Retrieves information about the specified NTFS file system volume.
		/// </summary>
		public const int FSCTL_GET_NTFS_VOLUME_DATA = (9 << 16) | (25 << 2);

		/// <summary>
		/// Retrieves a set of FAT file system attributes for a specified file or
		/// directory.
		/// </summary>
		/// <param name="lpFileName">The name of the file or directory.</param>
		/// <returns>If the function succeeds, the return value contains the attributes
		/// of the specified file or directory.
		/// 
		/// If the function fails, the return value is INVALID_FILE_ATTRIBUTES.
		/// To get extended error information, call Marshal.GetLastWin32Error.
		/// 
		/// The attributes can be one or more of the FILE_ATTRIBUTE_* values.</returns>
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern uint GetFileAttributes(string lpFileName);

		/// <summary>
		/// Sets the attributes for a file or directory.
		/// </summary>
		/// <param name="lpFileName">The name of the file whose attributes are
		/// to be set.</param>
		/// <param name="dwFileAttributes">The file attributes to set for the file.
		/// This parameter can be one or more of the FILE_ATTRIBUTE_* values.
		/// However, all other values override FILE_ATTRIBUTE_NORMAL.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api")]
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileAttributes(string lpFileName,
			uint dwFileAttributes);

		/// <summary>
		/// Retrieves the size of the specified file.
		/// </summary>
		/// <param name="hFile">A handle to the file. The handle must have been
		/// created with either the GENERIC_READ or GENERIC_WRITE access right.
		/// For more information, see File Security and Access Rights.</param>
		/// <param name="lpFileSize">A reference to a long that receives the file
		/// size, in bytes.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileSizeEx(SafeFileHandle hFile, out long lpFileSize);

		/// <summary>
		/// Retrieves the date and time that a file or directory was created, last
		/// accessed, and last modified.
		/// </summary>
		/// <param name="hFile">A handle to the file or directory for which dates
		/// and times are to be retrieved. The handle must have been created using
		/// the CreateFile function with the GENERIC_READ access right. For more
		/// information, see File Security and Access Rights.</param>
		/// <param name="lpCreationTime">A pointer to a FILETIME structure to
		/// receive the date and time the file or directory was created. This
		/// parameter can be NULL if the application does not require this
		/// information.</param>
		/// <param name="lpLastAccessTime">A pointer to a FILETIME structure to
		/// receive the date and time the file or directory was last accessed. The
		/// last access time includes the last time the file or directory was
		/// written to, read from, or, in the case of executable files, run. This
		/// parameter can be NULL if the application does not require this
		/// information.</param>
		/// <param name="lpLastWriteTime">A pointer to a FILETIME structure to
		/// receive the date and time the file or directory was last written to,
		/// truncated, or overwritten (for example, with WriteFile or SetEndOfFile).
		/// This date and time is not updated when file attributes or security
		/// descriptors are changed. This parameter can be NULL if the application
		/// does not require this information.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetFileTime(SafeFileHandle hFile,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpLastAccessTime,
			out System.Runtime.InteropServices.ComTypes.FILETIME lpLastWriteTime);

		/// <summary>
		/// Sets the date and time that the specified file or directory was created,
		/// last accessed, or last modified.
		/// </summary>
		/// <param name="hFile">A handle to the file or directory. The handle must
		/// have been created using the CreateFile function with the
		/// FILE_WRITE_ATTRIBUTES access right. For more information, see File
		/// Security and Access Rights.</param>
		/// <param name="lpCreationTime">A pointer to a FILETIME structure that
		/// contains the new creation date and time for the file or directory.
		/// This parameter can be NULL if the application does not need to change
		/// this information.</param>
		/// <param name="lpLastAccessTime">A pointer to a FILETIME structure that
		/// contains the new last access date and time for the file or directory.
		/// The last access time includes the last time the file or directory was
		/// written to, read from, or (in the case of executable files) run. This
		/// parameter can be NULL if the application does not need to change this
		/// information.
		/// 
		/// To preserve the existing last access time for a file even after accessing
		/// a file, call SetFileTime immediately after opening the file handle
		/// with this parameter's FILETIME structure members initialized to
		/// 0xFFFFFFFF.</param>
		/// <param name="lpLastWriteTime">A pointer to a FILETIME structure that
		/// contains the new last modified date and time for the file or directory.
		/// This parameter can be NULL if the application does not need to change
		/// this information.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call GetLastError.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetFileTime(SafeFileHandle hFile,
			ref System.Runtime.InteropServices.ComTypes.FILETIME lpCreationTime,
			ref System.Runtime.InteropServices.ComTypes.FILETIME lpLastAccessTime,
			ref System.Runtime.InteropServices.ComTypes.FILETIME lpLastWriteTime);

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
		public static extern SafeFileHandle FindFirstVolume(StringBuilder lpszVolumeName,
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
		public static extern bool FindNextVolume(SafeHandle hFindVolume,
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
		public static extern bool FindVolumeClose(SafeHandle hFindVolume);

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
		public static extern SafeFileHandle FindFirstVolumeMountPoint(
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
		public static extern bool FindNextVolumeMountPoint(
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
		public static extern bool FindVolumeMountPointClose(SafeHandle hFindVolumeMountPoint);

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
		public static extern bool GetDiskFreeSpace(
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
		public static extern bool GetDiskFreeSpaceEx(
			string lpDirectoryName,
			out UInt64 lpFreeBytesAvailable,
			out UInt64 lpTotalNumberOfBytes,
			out UInt64 lpTotalNumberOfFreeBytes);

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
		public static extern uint GetDriveType(string lpRootPathName);

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
		public static extern bool GetVolumeInformation(
			string lpRootPathName,
			StringBuilder lpVolumeNameBuffer,
			uint nVolumeNameSize,
			out uint lpVolumeSerialNumber,
			out uint lpMaximumComponentLength,
			out uint lpFileSystemFlags,
			StringBuilder lpFileSystemNameBuffer,
			uint nFileSystemNameSize);

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
		public static extern bool GetVolumeNameForVolumeMountPoint(
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
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetVolumePathNamesForVolumeName(
			string lpszVolumeName, StringBuilder lpszVolumePathNames, uint cchBufferLength,
			out uint lpcchReturnLength);

		public const int MaxPath = 260;
		public const int LongPath = 32768;
	}
}
