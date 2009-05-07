/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: Kasra Nassiri <cjax@users.sourceforge.net> @10/7/2008
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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Globalization;

namespace Eraser.Util
{
	public static class File
	{
		/// <summary>
		/// Gets the list of ADSes of the given file. 
		/// </summary>
		/// <param name="info">The FileInfo object with the file path etc.</param>
		/// <returns>A list containing the names of the ADSes of each file. The
		/// list will be empty if no ADSes exist.</returns>
		public static ICollection<string> GetADSes(FileInfo info)
		{
			List<string> result = new List<string>();
			using (FileStream stream = new FileStream(info.FullName, FileMode.Open,
				FileAccess.Read, FileShare.ReadWrite))
			{
				SafeFileHandle streamHandle = stream.SafeFileHandle;

				//Allocate the structures
				WIN32_STREAM_ID streamID = new WIN32_STREAM_ID();
				IntPtr context = IntPtr.Zero;
				uint bytesRead = 0;

				//Read the header of the WIN32_STREAM_ID
				BackupRead(streamHandle, ref streamID, (uint)Marshal.SizeOf(streamID),
					ref bytesRead, false, false, ref context);

				while (bytesRead == Marshal.SizeOf(streamID))
				{
					if (streamID.dwStreamId == BACKUP_ALTERNATE_DATA)
					{
						//Allocate memory to copy the stream name into, then copy the name
						IntPtr pName = Marshal.AllocHGlobal((int)streamID.dwStreamNameSize);
						uint nameLength = streamID.dwStreamNameSize / sizeof(char);
						char[] name = new char[nameLength];
						BackupRead(streamHandle, pName, streamID.dwStreamNameSize, ref bytesRead,
							false, false, ref context);
						Marshal.Copy(pName, name, 0, (int)nameLength);

						//Get the name of the stream. The raw value is :NAME:$DATA
						string streamName = new string(name);
						result.Add(streamName.Substring(1, streamName.LastIndexOf(':') - 1));
					}

					//Skip the file contents. Jump to the next header.
					uint seekLow = 0, seekHigh = 0;
					BackupSeek(streamHandle, (uint)(streamID.Size & uint.MaxValue),
						(uint)(streamID.Size >> (sizeof(uint) * 8)), out seekLow,
						out seekHigh, ref context);

					//And try to read the header
					BackupRead(streamHandle, ref streamID, (uint)Marshal.SizeOf(streamID),
						ref bytesRead, false, false, ref context);
				}

				//Free the context
				BackupRead(streamHandle, IntPtr.Zero, 0, ref bytesRead, true, false, ref context);
			}

			return result;
		}

		/// <summary>
		/// Uses SHGetFileInfo to retrieve the description for the given file,
		/// folder or drive.
		/// </summary>
		/// <param name="path">A string that contains the path and file name for
		/// the file in question. Both absolute and relative paths are valid.
		/// Directories and volumes must contain the trailing \</param>
		/// <returns>A string containing the description</returns>
		public static string GetFileDescription(string path)
		{
			ShellAPI.NativeMethods.SHFILEINFO shfi = new ShellAPI.NativeMethods.SHFILEINFO();
			ShellAPI.NativeMethods.SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				ShellAPI.NativeMethods.SHGetFileInfoFlags.SHGFI_DISPLAYNAME);
			return shfi.szDisplayName;
		}

		/// <summary>
		/// Uses SHGetFileInfo to retrieve the icon for the given file, folder or
		/// drive.
		/// </summary>
		/// <param name="path">A string that contains the path and file name for
		/// the file in question. Both absolute and relative paths are valid.
		/// Directories and volumes must contain the trailing \</param>
		/// <returns>An Icon object containing the bitmap</returns>
		public static Icon GetFileIcon(string path)
		{
			ShellAPI.NativeMethods.SHFILEINFO shfi = new ShellAPI.NativeMethods.SHFILEINFO();
			ShellAPI.NativeMethods.SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				ShellAPI.NativeMethods.SHGetFileInfoFlags.SHGFI_SMALLICON |
				ShellAPI.NativeMethods.SHGetFileInfoFlags.SHGFI_ICON);

			if (shfi.hIcon != IntPtr.Zero)
				return Icon.FromHandle(shfi.hIcon);
			else
				throw new IOException(string.Format("Could not load file icon from {0}", path),
					Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
		}

		/// <summary>
		/// Compacts the file path, fitting in the given width.
		/// </summary>
		/// <param name="longPath">The long file path.</param>
		/// <param name="newWidth">The target width of the text.</param>
		/// <param name="drawFont">The font used for drawing the text.</param>
		/// <returns>The compacted file path.</returns>
		public static string GetCompactPath(string longPath, int newWidth, Font drawFont)
		{
			using (Control ctrl = new Control())
			{
				//First check if the source string is too long.
				Graphics g = ctrl.CreateGraphics();
				int width = g.MeasureString(longPath, drawFont).ToSize().Width;
				if (width <= newWidth)
					return longPath;

				//It is, shorten it.
				int aveCharWidth = width / longPath.Length;
				int charCount = newWidth / aveCharWidth;
				StringBuilder builder = new StringBuilder();
				builder.Append(longPath);
				builder.EnsureCapacity(charCount);

				while (g.MeasureString(builder.ToString(), drawFont).Width > newWidth)
				{
					if (!PathCompactPathEx(builder, longPath, (uint)charCount--, 0))
					{
						return string.Empty;
					}
				}

				return builder.ToString();
			}
		}

		/// <summary>
		/// Determines if a given file is protected by SFC.
		/// </summary>
		/// <param name="filePath">The path to check</param>
		/// <returns>True if the file is protected.</returns>
		public static bool IsProtectedSystemFile(string filePath)
		{
			if (SfcIsFileProtected(IntPtr.Zero, filePath))
				return true;

			switch (Marshal.GetLastWin32Error())
			{
				case 0: //ERROR_SUCCESS
				case 2: //ERROR_FILE_NOT_FOUND
					return false;

				default:
					throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}

		/// <summary>
		/// Checks whether the path given is compressed.
		/// </summary>
		/// <param name="path">The path to the file or folder</param>
		/// <returns>True if the file or folder is compressed.</returns>
		public static bool IsCompressed(string path)
		{
			ushort compressionStatus = 0;
			uint bytesReturned = 0;

			using (FileStream strm = new FileStream(CreateFile(path,
				GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING,
				FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero), FileAccess.Read))
			{
				if (DeviceIoControl(strm.SafeFileHandle.DangerousGetHandle(),
					FSCTL_GET_COMPRESSION, IntPtr.Zero, 0, out compressionStatus,
					sizeof(ushort), out bytesReturned, IntPtr.Zero))
				{
					const ushort COMPRESSION_FORMAT_NONE = 0x0000;
					return compressionStatus != COMPRESSION_FORMAT_NONE;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets whether the file system object pointed to by path is compressed.
		/// </summary>
		/// <param name="path">The path to the file or folder.</param>
		/// <returns>True if the file or folder has its compression value set.</returns>
		public static bool SetCompression(string path, bool compressed)
		{
			ushort compressionStatus = compressed ?
				COMPRESSION_FORMAT_DEFAULT : COMPRESSION_FORMAT_NONE;
			uint bytesReturned = 0;

			using (FileStream strm = new FileStream(CreateFile(path,
				GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING,
				FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero), FileAccess.ReadWrite))
			{
				return DeviceIoControl(strm.SafeFileHandle.DangerousGetHandle(),
					FSCTL_SET_COMPRESSION, ref compressionStatus,
					sizeof(ushort), IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
			}
		}

		/// <summary>
		/// Truncates a path to fit within a certain number of characters by
		/// replacing path components with ellipses.
		/// </summary>
		/// <param name="pszOut">[out] The address of the string that has been altered.</param>
		/// <param name="pszSrc">[in] A pointer to a null-terminated string of maximum
		/// length MAX_PATH that contains the path to be altered.</param>
		/// <param name="cchMax">[in] The maximum number of characters to be
		/// contained in the new string, including the terminating NULL character.
		/// For example, if cchMax = 8, the resulting string can contain a maximum
		/// of 7 characters plus the terminating NULL character.</param>
		/// <param name="dwFlags">Reserved.</param>
		/// <returns>Returns TRUE if successful, or FALSE otherwise.</returns>
		[DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PathCompactPathEx(StringBuilder pszOut,
			string pszSrc, uint cchMax, uint dwFlags);

		/// <summary>
		/// Determines whether the specified file is protected. Applications
		/// should avoid replacing protected system files.
		/// </summary>
		/// <param name="RpcHandle">This parameter must be NULL.</param>
		/// <param name="ProtFileName">The name of the file.</param>
		/// <returns>If the file is protected, the return value is true.
		/// 
		/// If the file is not protected, the return value is false and
		/// Marshal.GetLastWin32Error() returns ERROR_FILE_NOT_FOUND. If the
		/// function fails, Marshal.GetLastWin32Error() will return a different
		/// error code.</returns>
		[DllImport("Sfc.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SfcIsFileProtected(IntPtr RpcHandle,
			string ProtFileName);

		/// <summary>
		/// The BackupRead function can be used to back up a file or directory,
		/// including the security information. The function reads data associated
		/// with a specified file or directory into a buffer, which can then be
		/// written to the backup medium using the WriteFile function.
		/// </summary>
		/// <param name="hFile">Handle to the file or directory to be backed up.
		/// To obtain the handle, call the CreateFile function. The SACLs are not
		/// read unless the file handle was created with the ACCESS_SYSTEM_SECURITY
		/// access right. For more information, see File Security and Access Rights.
		/// 
		/// The BackupRead function may fail if CreateFile was called with the flag
		/// FILE_FLAG_NO_BUFFERING. In this case, the GetLastError function
		/// returns the value ERROR_INVALID_PARAMETER.</param>
		/// <param name="lpBuffer">Pointer to a buffer that receives the data.</param>
		/// <param name="nNumberOfBytesToRead">Length of the buffer, in bytes. The
		/// buffer size must be greater than the size of a WIN32_STREAM_ID structure.</param>
		/// <param name="lpNumberOfBytesRead">Pointer to a variable that receives
		/// the number of bytes read.
		/// 
		/// If the function returns a nonzero value, and the variable pointed to
		/// by lpNumberOfBytesRead is zero, then all the data associated with the
		/// file handle has been read.</param>
		/// <param name="bAbort">Indicates whether you have finished using BackupRead
		/// on the handle. While you are backing up the file, specify this parameter
		/// as FALSE. Once you are done using BackupRead, you must call BackupRead
		/// one more time specifying TRUE for this parameter and passing the appropriate
		/// lpContext. lpContext must be passed when bAbort is TRUE; all other
		/// parameters are ignored.</param>
		/// <param name="bProcessSecurity">Indicates whether the function will
		/// restore the access-control list (ACL) data for the file or directory.
		/// 
		/// If bProcessSecurity is TRUE, the ACL data will be backed up.</param>
		/// <param name="lpContext">Pointer to a variable that receives a pointer
		/// to an internal data structure used by BackupRead to maintain context
		/// information during a backup operation.
		/// 
		/// You must set the variable pointed to by lpContext to NULL before the
		/// first call to BackupRead for the specified file or directory. The
		/// function allocates memory for the data structure, and then sets the
		/// variable to point to that structure. You must not change lpContext or
		/// the variable that it points to between calls to BackupRead.
		/// 
		/// To release the memory used by the data structure, call BackupRead with
		/// the bAbort parameter set to TRUE when the backup operation is complete.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero, indicating that an
		/// I/O error occurred. To get extended error information, call
		/// Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BackupRead(SafeFileHandle hFile,
			IntPtr lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead,
			[MarshalAs(UnmanagedType.Bool)] bool bAbort,
			[MarshalAs(UnmanagedType.Bool)] bool bProcessSecurity,
			ref IntPtr lpContext);
		
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BackupRead(SafeFileHandle hFile,
			ref WIN32_STREAM_ID lpBuffer, uint nNumberOfBytesToRead,
			ref uint lpNumberOfBytesRead, [MarshalAs(UnmanagedType.Bool)] bool bAbort,
			[MarshalAs(UnmanagedType.Bool)] bool bProcessSecurity,
			ref IntPtr lpContext);

		/// <summary>
		/// The BackupSeek function seeks forward in a data stream initially
		/// accessed by using the BackupRead or BackupWrite function.
		/// </summary>
		/// <param name="hFile">Handle to the file or directory. This handle is
		/// created by using the CreateFile function.</param>
		/// <param name="dwLowBytesToSeek">Low-order part of the number of bytes
		/// to seek.</param>
		/// <param name="dwHighBytesToSeek">High-order part of the number of bytes
		/// to seek.</param>
		/// <param name="lpdwLowByteSeeked">Pointer to a variable that receives
		/// the low-order bits of the number of bytes the function actually seeks.</param>
		/// <param name="lpdwHighByteSeeked">Pointer to a variable that receives
		/// the high-order bits of the number of bytes the function actually seeks.</param>
		/// <param name="lpContext">Pointer to an internal data structure used by
		/// the function. This structure must be the same structure that was
		/// initialized by the BackupRead function. An application must not touch
		/// the contents of this structure.</param>
		/// <returns>If the function could seek the requested amount, the function
		/// returns a nonzero value.
		/// 
		/// If the function could not seek the requested amount, the function 
		/// returns zero. To get extended error information, call
		/// Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool BackupSeek(SafeFileHandle hFile, uint dwLowBytesToSeek,
			uint dwHighBytesToSeek, out uint lpdwLowByteSeeked, out uint lpdwHighByteSeeked,
			ref IntPtr lpContext);

		/// <summary>
		/// The WIN32_STREAM_ID structure contains stream data.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct WIN32_STREAM_ID
		{
			/// <summary>
			/// Type of data. This member can be one of the BACKUP_* values.
			/// </summary>
			public uint dwStreamId;

			/// <summary>
			/// Attributes of data to facilitate cross-operating system transfer.
			/// This member can be one or more of the following values.
			/// Value						Meaning
			/// STREAM_MODIFIED_WHEN_READ	Attribute set if the stream contains
			///								data that is modified when read. Allows
			///								the backup application to know that
			///								verification of data will fail.
			///	STREAM_CONTAINS_SECURITY	Stream contains security data
			///								(general attributes). Allows the stream
			///								to be ignored on cross-operations restore.
			/// </summary>
			public uint dwStreamAttributes;

			/// <summary>
			/// Size of data, in bytes.
			/// </summary>
			public long Size;

			/// <summary>
			/// Length of the name of the alternative data stream, in bytes.
			/// </summary>
			public uint dwStreamNameSize;
		}

		/// <summary>
		/// Alternative data streams.
		/// </summary>
		public const uint BACKUP_ALTERNATE_DATA = 0x00000004;

		/// <summary>
		/// Standard data.
		/// </summary>
		public const uint BACKUP_DATA = 0x00000001;

		/// <summary>
		/// Extended attribute data.
		/// </summary>
		public const uint BACKUP_EA_DATA = 0x00000002;

		/// <summary>
		/// Hard link information.
		/// </summary>
		public const uint BACKUP_LINK = 0x00000005;

		/// <summary>
		/// Objects identifiers.
		/// </summary>
		public const uint BACKUP_OBJECT_ID = 0x00000007;

		/// <summary>
		/// Property data.
		/// </summary>
		public const uint BACKUP_PROPERTY_DATA = 0x00000006;

		/// <summary>
		/// Reparse points.
		/// </summary>
		public const uint BACKUP_REPARSE_DATA = 0x00000008;

		/// <summary>
		/// Security descriptor data.
		/// </summary>
		public const uint BACKUP_SECURITY_DATA = 0x00000003;

		/// <summary>
		/// Sparse file.
		/// </summary>
		public const uint BACKUP_SPARSE_BLOCK = 0x00000009;

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
		private extern static bool DeviceIoControl(IntPtr hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private extern static bool DeviceIoControl(IntPtr hDevice,
			uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize,
			out ushort lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private extern static bool DeviceIoControl(IntPtr hDevice,
			uint dwIoControlCode, ref ushort lpInBuffer, uint nInBufferSize,
			IntPtr lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned,
			IntPtr lpOverlapped);

		private const uint FSCTL_GET_COMPRESSION = 0x9003C;
		private const uint FSCTL_SET_COMPRESSION = 0x9C040;
		private const ushort COMPRESSION_FORMAT_NONE = 0x0000;
		private const ushort COMPRESSION_FORMAT_DEFAULT = 0x0001;

		/// <summary>
		/// Gets the human-readable representation of a file size from the byte-wise
		/// length of a file. This returns a KB = 1024 bytes (Windows convention.)
		/// </summary>
		/// <param name="bytes">The file size to scale.</param>
		/// <returns>A string containing the file size and the associated unit.
		/// Files larger than 1MB will be accurate to 2 decimal places.</returns>
		public static string GetHumanReadableFilesize(long bytes)
		{
			//List of units, in ascending scale
			string[] units = new string[] {
				"bytes",
				"KB",
				"MB",
				"GB",
				"TB",
				"PB",
				"EB"
			};

			double dBytes = (double)bytes;
			for (int i = 0; i != units.Length; ++i)
			{
				if (dBytes < 1020.0)
					if (i <= 1)
						return string.Format("{0} {1}", (int)dBytes, units[i]);
					else
						return string.Format(CultureInfo.CurrentCulture, "{0:0.00} {1}",
							dBytes, units[i]);
				dBytes /= 1024.0;
			}

			return string.Format("{0, 2} {1}", dBytes, units[units.Length - 1]);
		}
	}
}
