using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace Eraser.Util
{
	using HICON = IntPtr;
	using HIMAGELIST = IntPtr;

	public static class File
	{
		/// <summary>
		/// Gets the list of ADSes of the given file. 
		/// </summary>
		/// <param name="info">The FileInfo object with the file path etc.</param>
		/// <returns>A list containing the names of the ADSes of each file. The
		/// list will be empty if no ADSes exist.</returns>
		public static List<string> GetADSes(FileInfo info)
		{
			List<string> result = new List<string>();
			using (FileStream stream = info.OpenRead())
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
						IntPtr pName = Marshal.AllocHGlobal((IntPtr)streamID.dwStreamNameSize);
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
			SHFILEINFO shfi = new SHFILEINFO();
			SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				SHGetFileInfoFlags.SHGFI_DISPLAYNAME);
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
			SHFILEINFO shfi = new SHFILEINFO();
			SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				SHGetFileInfoFlags.SHGFI_SMALLICON | SHGetFileInfoFlags.SHGFI_ICON);
			return Icon.FromHandle(shfi.hIcon);
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
			else if (Marshal.GetLastWin32Error() == 2) //ERROR_FILE_NOT_FOUND
				return false;

			throw new Exception("Unknown SfcIsFileProtected error.");
		}

		/// <summary>
		/// Checks whether the path given is compressed.
		/// </summary>
		/// <param name="filePath">The path to the file or folder</param>
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

			throw new Exception("Unknown DeviceIoControl error.");
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
		/// Retrieves information about an object in the file system, such as a
		/// file, folder, directory, or drive root.
		/// </summary>
		/// <param name="path">[in] A pointer to a null-terminated string of maximum
		/// length MAX_PATH that contains the path and file name. Both absolute
		/// and relative paths are valid.
		/// 
		/// If the uFlags parameter includes the SHGFI_PIDL flag, this parameter
		/// must be the address of an ITEMIDLIST (PIDL) structure that contains
		/// the list of item identifiers that uniquely identifies the file within
		/// the Shell's namespace. The pointer to an item identifier list (PIDL)
		/// must be a fully qualified PIDL. Relative PIDLs are not allowed.
		/// 
		/// If the uFlags parameter includes the SHGFI_USEFILEATTRIBUTES flag,
		/// this parameter does not have to be a valid file name. The function
		/// will proceed as if the file exists with the specified name and with
		/// the file attributes passed in the dwFileAttributes parameter. This
		/// allows you to obtain information about a file type by passing just
		/// the extension for pszPath and passing FILE_ATTRIBUTE_NORMAL in
		/// dwFileAttributes.
		/// 
		/// This string can use either short (the 8.3 form) or long file names.</param>
		/// <param name="fileAttributes">[in] A combination of one or more file 
		/// attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If
		/// uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this
		/// parameter is ignored.</param>
		/// <param name="psfi">[out] The address of a SHFILEINFO structure to
		/// receive the file information.</param>
		/// <param name="cbFileInfo">[in] The size, in bytes, of the SHFILEINFO
		/// structure pointed to by the psfi parameter.</param>
		/// <param name="uFlags">[in] The flags that specify the file information to retrieve.
		/// This parameter can be a combination of the values in SHGetFileInfoFlags</param>
		/// <returns>Returns a value whose meaning depends on the uFlags parameter.
		/// 
		/// If uFlags does not contain SHGFI_EXETYPE or SHGFI_SYSICONINDEX, the return
		/// value is nonzero if successful, or zero otherwise.
		/// 
		/// If uFlags contains the SHGFI_EXETYPE flag, the return value specifies
		/// the type of the executable file. It will be one of the following values.
		///		0												Nonexecutable file or an error condition.
		///		LOWORD = NE or PE and HIWORD = Windows version	Microsoft Windows application.
		///		LOWORD = MZ and HIWORD = 0						Windows 95, Windows 98: Microsoft MS-DOS .exe, .com, or .bat file
		///														Microsoft Windows NT, Windows 2000, Windows XP: MS-DOS .exe or .com file
		///		LOWORD = PE and HIWORD = 0						Windows 95, Windows 98: Microsoft Win32 console application
		///														Windows NT, Windows 2000, Windows XP: Win32 console application or .bat file
		/// </returns>
		[DllImport("Shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string path, uint fileAttributes,
			ref SHFILEINFO psfi, int fileInfo, SHGetFileInfoFlags flags);

		enum SHGetFileInfoFlags
		{
			/// <summary>
			/// Retrieve the handle to the icon that represents the file and the
			///	index of the icon within the system image list. The handle is
			///	copied to the hIcon member of the structure specified by psfi,
			///	and the index is copied to the iIcon member.
			/// </summary>
			SHGFI_ICON				= 0x000000100,

			/// <summary>
			/// Retrieve the display name for the file. The name is copied to the
			///	szDisplayName member of the structure specified in psfi. The returned
			/// display name uses the long file name, if there is one, rather than
			/// the 8.3 form of the file name.
			/// </summary>
			SHGFI_DISPLAYNAME		= 0x000000200,

			/// <summary>
			/// Retrieve the string that describes the file's type. The string
			///	is copied to the szTypeName member of the structure specified in
			///	psfi.
			/// </summary>
			SHGFI_TYPENAME			= 0x000000400,

			/// <summary>
			/// Retrieve the item attributes. The attributes are copied to the
			///	dwAttributes member of the structure specified in the psfi parameter.
			///	These are the same attributes that are obtained from
			///	IShellFolder::GetAttributesOf.
			/// </summary>
			SHGFI_ATTRIBUTES		= 0x000000800,

			/// <summary>
			/// Retrieve the name of the file that contains the icon representing
			///	the file specified by pszPath, as returned by the
			/// IExtractIcon::GetIconLocation method of the file's icon handler.
			///	Also retrieve the icon index within that file. The name of the
			///	file containing the icon is copied to the szDisplayName member
			///	of the structure specified by psfi. The icon's index is copied to
			///	that structure's iIcon member.
			/// </summary>
			SHGFI_ICONLOCATION		= 0x000001000,

			/// <summary>
			/// Retrieve the type of the executable file if pszPath identifies an
			///	executable file. The information is packed into the return value.
			///	This flag cannot be specified with any other flags.
			/// </summary>
			SHGFI_EXETYPE			= 0x000002000,

			/// <summary>
			/// Retrieve the index of a system image list icon. If successful,
			///	the index is copied to the iIcon member of psfi. The return value
			///	is a handle to the system image list. Only those images whose
			///	indices are successfully copied to iIcon are valid. Attempting
			///	to access other images in the system image list will result in
			///	undefined behavior.
			/// </summary>
			SHGFI_SYSICONINDEX		= 0x000004000,
			
			/// <summary>
			/// Modify SHGFI_ICON, causing the function to add the link overlay
			///	to the file's icon. The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_LINKOVERLAY		= 0x000008000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to blend the file's icon
			///	with the system highlight color. The SHGFI_ICON flag must also
			/// be set.
			/// </summary>
			SHGFI_SELECTED			= 0x000010000,

			/// <summary>
			/// Modify SHGFI_ATTRIBUTES to indicate that the dwAttributes member
			///	of the SHFILEINFO structure at psfi contains the specific attributes
			///	that are desired. These attributes are passed to IShellFolder::GetAttributesOf.
			///	If this flag is not specified, 0xFFFFFFFF is passed to
			///	IShellFolder::GetAttributesOf, requesting all attributes. This flag
			///	cannot be specified with the SHGFI_ICON flag.
			/// </summary>
			SHGFI_ATTR_SPECIFIED	= 0x000020000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	large icon. The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_LARGEICON			= 0x000000000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	small icon. Also used to modify SHGFI_SYSICONINDEX, causing the
			///	function to return the handle to the system image list that
			///	contains small icon images. The SHGFI_ICON and/or
			///	SHGFI_SYSICONINDEX flag must also be set.
			/// </summary>
			SHGFI_SMALLICON			= 0x000000001,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	open icon. Also used to modify SHGFI_SYSICONINDEX, causing the
			///	function to return the handle to the system image list that
			///	contains the file's small open icon. A container object displays
			///	an open icon to indicate that the container is open. The SHGFI_ICON
			///	and/or SHGFI_SYSICONINDEX flag must also be set.
			/// </summary>
			SHGFI_OPENICON			= 0x000000002,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve a Shell-sized
			///	icon. If this flag is not specified the function sizes the icon
			///	according to the system metric values. The SHGFI_ICON flag must
			///	also be set.
			/// </summary>
			SHGFI_SHELLICONSIZE		= 0x000000004,
			
			/// <summary>
			/// Indicate that pszPath is the address of an ITEMIDLIST structure
			///	rather than a path name.
			/// </summary>
			SHGFI_PIDL				= 0x000000008,

			/// <summary>
			/// Indicates that the function should not attempt to access the file
			///	specified by pszPath. Rather, it should act as if the file specified
			///	by pszPath exists with the file attributes passed in dwFileAttributes.
			///	This flag cannot be combined with the SHGFI_ATTRIBUTES, SHGFI_EXETYPE,
			///	or SHGFI_PIDL flags.
			/// </summary>
			SHGFI_USEFILEATTRIBUTES	= 0x000000010,

			/// <summary>
			/// Version 5.0. Apply the appropriate overlays to the file's icon.
			///	The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_ADDOVERLAYS		= 0x000000020,
			
			/// <summary>
			/// Version 5.0. Return the index of the overlay icon. The value of
			///	the overlay index is returned in the upper eight bits of the iIcon
			///	member of the structure specified by psfi. This flag requires that
			///	the SHGFI_ICON be set as well.
			/// </summary>
			SHGFI_OVERLAYINDEX		= 0x000000040
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			/// <summary>
			/// A handle to the icon that represents the file. You are responsible
			/// for destroying this handle with DestroyIcon when you no longer need it.
			/// </summary>
			public HICON hIcon;

			/// <summary>
			/// The index of the icon image within the system image list.
			/// </summary>
			public int iIcon;

			/// <summary>
			/// An array of values that indicates the attributes of the file object.
			/// For information about these values, see the IShellFolder::GetAttributesOf
			/// method.
			/// </summary>
			public uint dwAttributes;

			/// <summary>
			/// A string that contains the name of the file as it appears in the
			/// Microsoft Windows Shell, or the path and file name of the file
			/// that contains the icon representing the file.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;

			/// <summary>
			/// A string that describes the type of file.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
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
		[DllImport("Shlwapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PathCompactPathEx(
			StringBuilder pszOut, string pszSrc, uint cchMax, uint dwFlags);

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
		[DllImport("Sfc.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SfcIsFileProtected(IntPtr RpcHandle,
			[MarshalAs(UnmanagedType.LPWStr)]string ProtFileName);

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
		[DllImport("Kernel32.dll", SetLastError = true)]
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
	}
}
