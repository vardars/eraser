/* 
 * $Id$
 * Copyright 2008-2009 The Eraser Project
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
		public static IList<string> GetADSes(FileInfo info)
		{
			List<string> result = new List<string>();
			using (FileStream stream = new StreamInfo(info.FullName).Open(FileMode.Open,
				FileAccess.Read, FileShare.ReadWrite))
			using (SafeFileHandle streamHandle = stream.SafeFileHandle)
			{
				//Allocate the structures
				NativeMethods.FILE_STREAM_INFORMATION[] streams = GetADSes(streamHandle);

				foreach (NativeMethods.FILE_STREAM_INFORMATION streamInfo in streams)
				{
					//Get the name of the stream. The raw value is :NAME:$DATA
					string streamName = streamInfo.StreamName.Substring(1,
						streamInfo.StreamName.LastIndexOf(':') - 1);
					
					if (streamName.Length != 0)
						result.Add(streamName);
				}
			}

			return result.AsReadOnly();
		}

		private static NativeMethods.FILE_STREAM_INFORMATION[] GetADSes(SafeFileHandle FileHandle)
		{
			NativeMethods.IO_STATUS_BLOCK status = new NativeMethods.IO_STATUS_BLOCK();
			IntPtr fileInfoPtr = IntPtr.Zero;

			try
			{
				NativeMethods.FILE_STREAM_INFORMATION streamInfo =
					new NativeMethods.FILE_STREAM_INFORMATION();
				int fileInfoPtrLength = (Marshal.SizeOf(streamInfo) + 32768) / 2;
				uint ntStatus = 0;

				do
				{
					fileInfoPtrLength *= 2;
					if (fileInfoPtr != IntPtr.Zero)
						Marshal.FreeHGlobal(fileInfoPtr);
					fileInfoPtr = Marshal.AllocHGlobal(fileInfoPtrLength);

					ntStatus = NativeMethods.NtQueryInformationFile(FileHandle, ref status,
						fileInfoPtr, (uint)fileInfoPtrLength,
						NativeMethods.FILE_INFORMATION_CLASS.FileStreamInformation);
				}
				while (ntStatus != 0 /*STATUS_SUCCESS*/ && ntStatus == 0x80000005 /*STATUS_BUFFER_OVERFLOW*/);

				//Marshal the structure manually (argh!)
				List<NativeMethods.FILE_STREAM_INFORMATION> result =
					new List<NativeMethods.FILE_STREAM_INFORMATION>();
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
			NativeMethods.SHFILEINFO shfi = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				NativeMethods.SHGetFileInfoFlags.SHGFI_DISPLAYNAME);
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
			NativeMethods.SHFILEINFO shfi = new NativeMethods.SHFILEINFO();
			NativeMethods.SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				NativeMethods.SHGetFileInfoFlags.SHGFI_SMALLICON |
				NativeMethods.SHGetFileInfoFlags.SHGFI_ICON);

			if (shfi.hIcon != IntPtr.Zero)
				return Icon.FromHandle(shfi.hIcon);
			else
				throw new IOException(string.Format(CultureInfo.CurrentCulture,
					"Could not load file icon from {0}", path),
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
					if (!NativeMethods.PathCompactPathEx(builder, longPath,
						(uint)charCount--, 0))
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
			return NativeMethods.SfcIsFileProtected(IntPtr.Zero, filePath);
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

			using (FileStream strm = new FileStream(NativeMethods.CreateFile(path,
				NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
				0, IntPtr.Zero, NativeMethods.OPEN_EXISTING,
				NativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero), FileAccess.Read))
			{
				if (NativeMethods.DeviceIoControl(strm.SafeFileHandle,
					NativeMethods.FSCTL_GET_COMPRESSION, IntPtr.Zero, 0,
					out compressionStatus, sizeof(ushort), out bytesReturned, IntPtr.Zero))
				{
					return compressionStatus != NativeMethods.COMPRESSION_FORMAT_NONE;
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
				NativeMethods.COMPRESSION_FORMAT_DEFAULT :
				NativeMethods.COMPRESSION_FORMAT_NONE;
			uint bytesReturned = 0;

			using (FileStream strm = new FileStream(NativeMethods.CreateFile(path,
				NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
				0, IntPtr.Zero, NativeMethods.OPEN_EXISTING,
				NativeMethods.FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero), FileAccess.ReadWrite))
			{
				return NativeMethods.DeviceIoControl(strm.SafeFileHandle,
					NativeMethods.FSCTL_SET_COMPRESSION, ref compressionStatus,
					sizeof(ushort), IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
			}
		}

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
						return string.Format(CultureInfo.CurrentCulture,
							"{0} {1}", (int)dBytes, units[i]);
					else
						return string.Format(CultureInfo.CurrentCulture,
							"{0:0.00} {1}", dBytes, units[i]);
				dBytes /= 1024.0;
			}

			return string.Format(CultureInfo.CurrentCulture, "{0, 2} {1}",
				dBytes, units[units.Length - 1]);
		}

		private static DateTime FileTimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME value)
		{
			long time = (long)((((ulong)value.dwHighDateTime) << sizeof(int) * 8) |
				(uint)value.dwLowDateTime);
			return DateTime.FromFileTime(time);
		}

		private static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFileTime(DateTime value)
		{
			long time = value.ToFileTime();

			System.Runtime.InteropServices.ComTypes.FILETIME result =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			result.dwLowDateTime = (int)(time & 0xFFFFFFFFL);
			result.dwHighDateTime = (int)(time >> 32);

			return result;
		}

		public static void GetFileTime(SafeFileHandle file, out DateTime creationTime,
			out DateTime accessedTime, out DateTime modifiedTime)
		{
			System.Runtime.InteropServices.ComTypes.FILETIME accessedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME modifiedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME createdTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();

			if (!NativeMethods.GetFileTime(file, out createdTimeNative, out accessedTimeNative,
				out modifiedTimeNative))
			{
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}

			creationTime = FileTimeToDateTime(createdTimeNative);
			accessedTime = FileTimeToDateTime(accessedTimeNative);
			modifiedTime = FileTimeToDateTime(modifiedTimeNative);
		}

		public static void SetFileTime(SafeFileHandle file, DateTime creationTime,
			DateTime accessedTime, DateTime modifiedTime)
		{
			System.Runtime.InteropServices.ComTypes.FILETIME accessedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME modifiedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME createdTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();

			if (!NativeMethods.GetFileTime(file, out createdTimeNative,
				out accessedTimeNative, out modifiedTimeNative))
			{
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}

			if (creationTime != DateTime.MinValue)
				createdTimeNative = DateTimeToFileTime(creationTime);
			if (accessedTime != DateTime.MinValue)
				accessedTimeNative = DateTimeToFileTime(accessedTime);
			if (modifiedTime != DateTime.MinValue)
				modifiedTimeNative = DateTimeToFileTime(modifiedTime);

			if (!NativeMethods.SetFileTime(file, ref createdTimeNative,
				ref accessedTimeNative, ref modifiedTimeNative))
			{
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}
		}
	}
}
