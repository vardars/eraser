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

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public class StreamInfo
	{
		/// <summary>
		/// Initializes a new instance of the Eraser.Util.FileInfo class, which
		//  acts as a wrapper for a file path.
		/// </summary>
		/// <param name="path">The fully qualified name (with :ADSName for ADSes)
		/// of the new file, or the relative file name.</param>
		public StreamInfo(string path)
		{
			//Separate the path into the ADS and the file.
			if (path.IndexOf(':') != path.LastIndexOf(':'))
			{
				int streamNameColon = path.IndexOf(':', path.IndexOf(':') + 1);
				fileName = path.Substring(0, streamNameColon);
				streamName = path.Substring(streamNameColon + 1);
			}
			else
				fileName = path;
		}

		/// <summary>
		/// Gets an instance of the parent directory.
		/// </summary>
		public DirectoryInfo Directory
		{
			get
			{
				return new DirectoryInfo(DirectoryName);
			}
		}

		/// <summary>
		/// Gets a string representing the containing directory's full path.
		/// </summary>
		public string DirectoryName
		{
			get
			{
				return fileName.Substring(0, fileName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			}
		}

		public string FullName
		{
			get
			{
				if (streamName != null)
					return fileName + ':' + streamName;
				return fileName;
			}
		}

		/// <summary>
		/// Gets an instance of the main file. If this object refers to an ADS, the
		/// result is null.
		/// </summary>
		public FileInfo File
		{
			get
			{
				if (streamName == null)
					return new FileInfo(fileName);
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the file attributes on this stream.
		/// </summary>
		public FileAttributes Attributes
		{
			get
			{
				uint attributes = GetFileAttributes(fileName);
				FileAttributes result = new FileAttributes();

				if ((attributes & FILE_ATTRIBUTE_ARCHIVE) != 0)
					result |= FileAttributes.Archive;
				if ((attributes & FILE_ATTRIBUTE_COMPRESSED) != 0)
					result |= FileAttributes.Compressed;
				if ((attributes & FILE_ATTRIBUTE_DEVICE) != 0)
					result |= FileAttributes.Device;
				if ((attributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
					result |= FileAttributes.Directory;
				if ((attributes & FILE_ATTRIBUTE_ENCRYPTED) != 0)
					result |= FileAttributes.Encrypted;
				if ((attributes & FILE_ATTRIBUTE_HIDDEN) != 0)
					result |= FileAttributes.Hidden;
				if ((attributes & FILE_ATTRIBUTE_NOT_CONTENT_INDEXED) != 0)
					result |= FileAttributes.NotContentIndexed;
				if ((attributes & FILE_ATTRIBUTE_OFFLINE) != 0)
					result |= FileAttributes.Offline;
				if ((attributes & FILE_ATTRIBUTE_READONLY) != 0)
					result |= FileAttributes.ReadOnly;
				if ((attributes & FILE_ATTRIBUTE_REPARSE_POINT) != 0)
					result |= FileAttributes.ReparsePoint;
				if ((attributes & FILE_ATTRIBUTE_SPARSE_FILE) != 0)
					result |= FileAttributes.SparseFile;
				if ((attributes & FILE_ATTRIBUTE_SYSTEM) != 0)
					result |= FileAttributes.System;
				if ((attributes & FILE_ATTRIBUTE_TEMPORARY) != 0)
					result |= FileAttributes.Temporary;

				if ((attributes & FILE_ATTRIBUTE_NORMAL) != 0)
					result = FileAttributes.Normal;
				return result;
			}
			set
			{
				uint result = 0;
				if ((value & (~FileAttributes.Normal)) == 0)
					result = FILE_ATTRIBUTE_NORMAL;

				if ((value & FileAttributes.Archive) != 0)
					result |= FILE_ATTRIBUTE_ARCHIVE;
				if ((value & FileAttributes.Compressed) != 0)
					result |= FILE_ATTRIBUTE_COMPRESSED;
				if ((value & FileAttributes.Device) != 0)
					result |= FILE_ATTRIBUTE_DEVICE;
				if ((value & FileAttributes.Directory) != 0)
					result |= FILE_ATTRIBUTE_DIRECTORY;
				if ((value & FileAttributes.Encrypted) != 0)
					result |= FILE_ATTRIBUTE_ENCRYPTED;
				if ((value & FileAttributes.Hidden) != 0)
					result |= FILE_ATTRIBUTE_HIDDEN;
				if ((value & FileAttributes.NotContentIndexed) != 0)
					result |= FILE_ATTRIBUTE_NOT_CONTENT_INDEXED;
				if ((value & FileAttributes.Offline) != 0)
					result |= FILE_ATTRIBUTE_OFFLINE;
				if ((value & FileAttributes.ReadOnly) != 0)
					result |= FILE_ATTRIBUTE_READONLY;
				if ((value & FileAttributes.ReparsePoint) != 0)
					result |= FILE_ATTRIBUTE_REPARSE_POINT;
				if ((value & FileAttributes.SparseFile) != 0)
					result |= FILE_ATTRIBUTE_SPARSE_FILE;
				if ((value & FileAttributes.System) != 0)
					result |= FILE_ATTRIBUTE_SYSTEM;
				if ((value & FileAttributes.Temporary) != 0)
					result |= FILE_ATTRIBUTE_TEMPORARY;

				SetFileAttributes(fileName, result);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the stream exists.
		/// </summary>
		public bool Exists
		{
			get
			{
				using (SafeFileHandle handle = fileHandle)
					return !(handle.IsInvalid &&
						Marshal.GetLastWin32Error() == 2 /*ERROR_FILE_NOT_FOUND*/);
			}
		}

		/// <summary>
		/// Gets or sets a value that determines if the current file is read only.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				uint attributes = GetFileAttributes(fileName);
				return (attributes & FILE_ATTRIBUTE_READONLY) != 0;
			}

			set
			{
				if (value)
					SetFileAttributes(fileName, GetFileAttributes(fileName) | FILE_ATTRIBUTE_READONLY);
				else
					SetFileAttributes(fileName, GetFileAttributes(fileName) & ~FILE_ATTRIBUTE_READONLY);
			}
		}

		/// <summary>
		/// Gets the size of the current stream.
		/// </summary>
		public long Length
		{
			get
			{
				long fileSize;
				using (SafeFileHandle handle = fileHandle)
					if (GetFileSizeEx(handle, out fileSize))
						return fileSize;

				throw new IOException("The size of the stream could not be retrieved",
					Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public string Name
		{
			get { return fileName; }
		}

		/// <summary>
		/// Permanently deletes a file.
		/// </summary>
		public void Delete()
		{
			throw new InvalidOperationException("Deleting streams are not implemented");
		}

		/// <summary>
		/// Opens a file in the specified mode.
		/// </summary>
		/// <param name="mode">A System.IO.FileMode constant specifying the mode
		/// (for example, Open or Append) in which to open the file.</param>
		/// <returns>A file opened in the specified mode, with read/write access,
		/// unshared, and no special file options.</returns>
		public FileStream Open(FileMode mode)
		{
			return Open(mode, FileAccess.ReadWrite, FileShare.None, FileOptions.None);
		}

		/// <summary>
		/// Opens a file in the specified mode with read, write, or read/write access.
		/// </summary>
		/// <param name="mode">A System.IO.FileMode constant specifying the mode
		/// (for example, Open or Append) in which to open the file.</param>
		/// <param name="access">A System.IO.FileAccess constant specifying whether
		/// to open the file with Read, Write, or ReadWrite file access.</param>
		/// <returns>A System.IO.FileStream object opened in the specified mode
		/// and access, unshared, and no special file options.</returns>
		public FileStream Open(FileMode mode, FileAccess access)
		{
			return Open(mode, access, FileShare.None, FileOptions.None);
		}

		/// <summary>
		/// Opens a file in the specified mode with read, write, or read/write access
		/// and the specified sharing option.
		/// </summary>
		/// <param name="mode">A System.IO.FileMode constant specifying the mode
		/// (for example, Open or Append) in which to open the file.</param>
		/// <param name="access">A System.IO.FileAccess constant specifying whether
		/// to open the file with Read, Write, or ReadWrite file access.</param>
		/// <param name="share">A System.IO.FileShare constant specifying the type
		/// of access other FileStream objects have to this file.</param>
		/// <returns>A System.IO.FileStream object opened with the specified mode,
		/// access, sharing options, and no special file options.</returns>
		public FileStream Open(FileMode mode, FileAccess access, FileShare share)
		{
			return Open(mode, access, share, FileOptions.None);
		}

		/// <summary>
		/// Opens a file in the specified mode with read, write, or read/write access,
		/// the specified sharing option, and other advanced options.
		/// </summary>
		/// <param name="mode">A System.IO.FileMode constant specifying the mode
		/// (for example, Open or Append) in which to open the file.</param>
		/// <param name="access">A System.IO.FileAccess constant specifying whether
		/// to open the file with Read, Write, or ReadWrite file access.</param>
		/// <param name="share">A System.IO.FileShare constant specifying the type
		/// of access other FileStream objects have to this file.</param>
		/// <param name="options">The System.IO.FileOptions constant specifying
		/// the advanced file options to use when opening the file.</param>
		/// <returns>A System.IO.FileStream object opened with the specified mode,
		/// access, sharing options, and special file options.</returns>
		public FileStream Open(FileMode mode, FileAccess access, FileShare share,
			FileOptions options)
		{
			//Access mode
			uint iAccess = 0;
			switch (access)
			{
				case FileAccess.Read:
					iAccess = Util.File.GENERIC_READ;
					break;
				case FileAccess.ReadWrite:
					iAccess = Util.File.GENERIC_READ | Util.File.GENERIC_WRITE;
					break;
				case FileAccess.Write:
					iAccess = Util.File.GENERIC_WRITE;
					break;
			}

			//File mode
			uint iMode = 0;
			switch (mode)
			{
				case FileMode.Append:
					iMode = Util.File.OPEN_EXISTING;
					break;
				case FileMode.Create:
					iMode = Util.File.CREATE_ALWAYS;
					break;
				case FileMode.CreateNew:
					iMode = Util.File.CREATE_NEW;
					break;
				case FileMode.Open:
					iMode = Util.File.OPEN_EXISTING;
					break;
				case FileMode.OpenOrCreate:
					iMode = Util.File.OPEN_ALWAYS;
					break;
				case FileMode.Truncate:
					iMode = Util.File.TRUNCATE_EXISTING;
					break;
			}

			//Sharing mode
			uint iShare = 0;
			switch (share)
			{
				case FileShare.Delete:
					iShare = Util.File.FILE_SHARE_DELETE;
					break;
				case FileShare.Inheritable:
					throw new NotImplementedException("Inheritable handles are not implemented.");
				case FileShare.None:
					iShare = 0;
					break;
				case FileShare.Read:
					iShare = Util.File.FILE_SHARE_READ;
					break;
				case FileShare.ReadWrite:
					iShare = Util.File.FILE_SHARE_READ | Util.File.FILE_SHARE_WRITE;
					break;
				case FileShare.Write:
					iShare = Util.File.FILE_SHARE_WRITE;
					break;
			}

			//Advanced options
			uint iOptions = 0;
			if ((options & FileOptions.Asynchronous) != 0)
				throw new NotImplementedException("Asynchronous handles are not implemented.");
			if ((options & FileOptions.Encrypted) != 0)
				iOptions |= FILE_ATTRIBUTE_ENCRYPTED;
			if ((options & FileOptions.DeleteOnClose) != 0)
				iOptions |= Util.File.FILE_FLAG_DELETE_ON_CLOSE;
			if ((options & FileOptions.WriteThrough) != 0)
				iOptions |= Util.File.FILE_FLAG_WRITE_THROUGH;
			if ((options & FileOptions.RandomAccess) != 0)
				iOptions |= Util.File.FILE_FLAG_RANDOM_ACCESS;
			if ((options & FileOptions.SequentialScan) != 0)
				iOptions |= Util.File.FILE_FLAG_SEQUENTIAL_SCAN;

			//Create the handle
			SafeFileHandle handle = Util.File.CreateFile(FullName, iAccess, iShare,
				IntPtr.Zero, iMode, iOptions, IntPtr.Zero);

			//Check that the handle is valid
			if (handle.IsInvalid)
				throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());

			//Return the FileStream
			return new FileStream(handle, access);
		}

		/// <summary>
		/// Returns the path as a string.
		/// </summary>
		/// <returns>A string representing the path.</returns>
		public override string ToString()
		{
			return FullName;
		}

		private SafeFileHandle fileHandle
		{
			get
			{
				//Create the handle
				return Util.File.CreateFile(FullName, 0,
					Util.File.FILE_SHARE_READ | Util.File.FILE_SHARE_WRITE | Util.File.FILE_SHARE_DELETE,
					IntPtr.Zero, Util.File.OPEN_EXISTING, 0, IntPtr.Zero);
			}
		}

		private string fileName;
		private string streamName;

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
		private static uint GetFileAttributes(string lpFileName)
		{
			return GetFileAttributesInternal(lpFileName);
		}

		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode,
			EntryPoint = "GetFileAttributes")]
		private static extern uint GetFileAttributesInternal(string lpFileName);

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
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetFileAttributes(string lpFileName,
			uint dwFileAttributes);

		/// <summary>
		/// A file or directory that is an archive file or directory. Applications
		/// use this attribute to mark files for backup or removal.
		/// </summary>
		const uint FILE_ATTRIBUTE_ARCHIVE = 0x20;

		/// <summary>
		/// A file or directory that is compressed.
		/// 
		/// For a file, all of the data in the file is compressed. For a directory,
		/// compression is the default for newly created files and subdirectories.
		/// </summary>
		const uint FILE_ATTRIBUTE_COMPRESSED = 0x800;

		/// <summary>
		/// Reserved; do not use.
		/// </summary>
		const int FILE_ATTRIBUTE_DEVICE = 0x40;

		/// <summary>
		/// The handle that identifies a directory.
		/// </summary>
		const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

		/// <summary>
		/// A file or directory that is encrypted.
		/// 
		/// For a file, all data streams in the file are encrypted.
		/// For a directory, encryption is the default for newly created files
		/// and subdirectories.
		/// </summary>
		const uint FILE_ATTRIBUTE_ENCRYPTED = 0x4000;

		/// <summary>
		/// The file or directory is hidden. It is not included in an ordinary
		/// directory listing.
		/// </summary>
		const uint FILE_ATTRIBUTE_HIDDEN = 0x2;

		/// <summary>
		/// A file or directory that does not have other attributes set. This attribute
		/// is valid only when used alone.
		/// </summary>
		const uint FILE_ATTRIBUTE_NORMAL = 0x80;

		/// <summary>
		/// The file is not to be indexed by the content indexing service.
		/// </summary>
		const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000;

		/// <summary>
		/// The data of a file is not available immediately. This attribute indicates
		/// that the file data is physically moved to offline storage. This attribute
		/// is used by Remote Storage, which is the hierarchical storage management
		/// software. Applications should not arbitrarily change this attribute.
		/// </summary>
		const uint FILE_ATTRIBUTE_OFFLINE = 0x1000;

		/// <summary>
		/// A file or directory that is read-only.
		/// 
		/// For a file, applications can read the file, but cannot write to it or
		/// delete it.
		/// 
		/// For a directory, applications cannot delete it.
		/// </summary>
		const uint FILE_ATTRIBUTE_READONLY = 0x1;

		/// <summary>
		/// A file or directory that has an associated reparse point, or a file
		/// that is a symbolic link.
		/// </summary>
		const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x400;

		/// <summary>
		/// A file that is a sparse file.
		/// </summary>
		const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x200;

		/// <summary>
		/// A file or directory that the operating system uses a part of, or uses
		/// exclusively.
		/// </summary>
		const uint FILE_ATTRIBUTE_SYSTEM = 0x4;

		/// <summary>
		/// A file that is being used for temporary storage.
		/// 
		/// File systems avoid writing data back to mass storage if sufficient cache
		/// memory is available, because typically, an application deletes a temporary
		/// file after the handle is closed. In that scenario, the system can entirely
		/// avoid writing the data. Otherwise, the data is written after the handle
		/// is closed.
		/// </summary>
		const uint FILE_ATTRIBUTE_TEMPORARY = 0x100;

		/// <summary>
		/// A file is a virtual file.
		/// </summary>
		const uint FILE_ATTRIBUTE_VIRTUAL = 0x10000;

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
		private static extern bool GetFileSizeEx(SafeFileHandle hFile, out long lpFileSize);
	}
}
