/* 
 * $Id$
 * Copyright 2008-2012 The Eraser Project
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

namespace Eraser.Util
{
	internal static partial class NativeMethods
	{
		/// <summary>
		/// Parses a Unicode command line string and returns an array of pointers to the command
		/// line arguments, along with a count of such arguments, in a way that is similar to
		/// the standard C run-time argv and argc values.
		/// </summary>
		/// <param name="lpCmdLine">Pointer to a null-terminated Unicode string that contains
		/// the full command line. If this parameter is an empty string the function returns the
		/// path to the current executable file.</param>
		/// <param name="pNumArgs">Pointer to an int that receives the number of array elements
		/// returned, similar to argc.</param>
		/// <returns>A pointer to an array of strings, similar to argv.</returns>
		/// <remarks>The address returned by CommandLineToArgvW is the address of the first
		/// element in an array of LPWSTR values; the number of pointers in this array is
		/// indicated by pNumArgs. Each pointer to a null-terminated Unicode string represents
		/// an individual argument found on the command line.
		/// 
		/// CommandLineToArgvW allocates a block of contiguous memory for pointers to the
		/// argument strings, and for the argument strings themselves; the calling application
		/// must free the memory used by the argument list when it is no longer needed. To free
		/// the memory, use a single call to the LocalFree function.
		/// 
		/// For more information about the argv and argc argument convention, see Argument
		/// Definitions and Parsing C++ Command-Line Arguments.
		/// 
		/// The GetCommandLineW function can be used to get a command line string that is
		/// suitable for use as the lpCmdLine parameter.
		/// 
		/// This function accepts command lines that contain a program name; the program name
		/// can be enclosed in quotation marks or not.
		/// 
		/// CommandLineToArgvW has a special interpretation of backslash characters when they
		/// are followed by a quotation mark character ("), as follows:
		/// 
		///     * 2n backslashes followed by a quotation mark produce n backslashes
		///       followed by a quotation mark.
		///     * (2n) + 1 backslashes followed by a quotation mark again produce n
		///       backslashes followed by a quotation mark.
		///     * n backslashes not followed by a quotation mark simply produce n
		///       backslashes.
		/// </remarks>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CommandLineToArgvW(string lpCmdLine, out int pNumArgs);

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
		public static extern bool PathCompactPathEx(StringBuilder pszOut,
			string pszSrc, uint cchMax, uint dwFlags);

		/// <summary>
		/// Empties the Recycle Bin on the specified drive.
		/// </summary>
		/// <param name="hwnd">A handle to the parent window of any dialog boxes
		/// that might be displayed during the operation. This parameter can be
		/// NULL.</param>
		/// <param name="pszRootPath">The address of a null-terminated string of
		/// maximum length MAX_PATH that contains the path of the root drive on
		/// which the Recycle Bin is located. This parameter can contain the address
		/// of a string formatted with the drive, folder, and subfolder names, for
		/// example c:\windows\system\. It can also contain an empty string or NULL.
		/// If this value is an empty string or NULL, all Recycle Bins on all drives
		/// will be emptied.</param>
		/// <param name="dwFlags">One or more of the SHEmptyRecycleBinFlags</param>
		/// <returns>Returns S_OK if successful, or a COM-defined error value
		/// otherwise.</returns>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
		public static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath,
			SHEmptyRecycleBinFlags dwFlags);

		public enum SHEmptyRecycleBinFlags : uint
		{
			SHERB_NOCONFIRMATION = 0x00000001,
			SHERB_NOPROGRESSUI = 0x00000002,
			SHERB_NOSOUND = 0x00000004
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
		/// <param name="uFlags">[in] The flags that specify the file information
		/// to retrieve.
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
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SHGetFileInfo(string path, uint fileAttributes,
			ref SHFILEINFO psfi, int cbFileInfo, SHGetFileInfoFlags uFlags);

		public enum SHGetFileInfoFlags
		{
			/// <summary>
			/// Retrieve the handle to the icon that represents the file and the
			///	index of the icon within the system image list. The handle is
			///	copied to the hIcon member of the structure specified by psfi,
			///	and the index is copied to the iIcon member.
			/// </summary>
			SHGFI_ICON = 0x000000100,

			/// <summary>
			/// Retrieve the display name for the file. The name is copied to the
			///	szDisplayName member of the structure specified in psfi. The returned
			/// display name uses the long file name, if there is one, rather than
			/// the 8.3 form of the file name.
			/// </summary>
			SHGFI_DISPLAYNAME = 0x000000200,

			/// <summary>
			/// Retrieve the string that describes the file's type. The string
			///	is copied to the szTypeName member of the structure specified in
			///	psfi.
			/// </summary>
			SHGFI_TYPENAME = 0x000000400,

			/// <summary>
			/// Retrieve the item attributes. The attributes are copied to the
			///	dwAttributes member of the structure specified in the psfi parameter.
			///	These are the same attributes that are obtained from
			///	IShellFolder::GetAttributesOf.
			/// </summary>
			SHGFI_ATTRIBUTES = 0x000000800,

			/// <summary>
			/// Retrieve the name of the file that contains the icon representing
			///	the file specified by pszPath, as returned by the
			/// IExtractIcon::GetIconLocation method of the file's icon handler.
			///	Also retrieve the icon index within that file. The name of the
			///	file containing the icon is copied to the szDisplayName member
			///	of the structure specified by psfi. The icon's index is copied to
			///	that structure's iIcon member.
			/// </summary>
			SHGFI_ICONLOCATION = 0x000001000,

			/// <summary>
			/// Retrieve the type of the executable file if pszPath identifies an
			///	executable file. The information is packed into the return value.
			///	This flag cannot be specified with any other flags.
			/// </summary>
			SHGFI_EXETYPE = 0x000002000,

			/// <summary>
			/// Retrieve the index of a system image list icon. If successful,
			///	the index is copied to the iIcon member of psfi. The return value
			///	is a handle to the system image list. Only those images whose
			///	indices are successfully copied to iIcon are valid. Attempting
			///	to access other images in the system image list will result in
			///	undefined behavior.
			/// </summary>
			SHGFI_SYSICONINDEX = 0x000004000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to add the link overlay
			///	to the file's icon. The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_LINKOVERLAY = 0x000008000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to blend the file's icon
			///	with the system highlight color. The SHGFI_ICON flag must also
			/// be set.
			/// </summary>
			SHGFI_SELECTED = 0x000010000,

			/// <summary>
			/// Modify SHGFI_ATTRIBUTES to indicate that the dwAttributes member
			///	of the SHFILEINFO structure at psfi contains the specific attributes
			///	that are desired. These attributes are passed to IShellFolder::GetAttributesOf.
			///	If this flag is not specified, 0xFFFFFFFF is passed to
			///	IShellFolder::GetAttributesOf, requesting all attributes. This flag
			///	cannot be specified with the SHGFI_ICON flag.
			/// </summary>
			SHGFI_ATTR_SPECIFIED = 0x000020000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	large icon. The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_LARGEICON = 0x000000000,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	small icon. Also used to modify SHGFI_SYSICONINDEX, causing the
			///	function to return the handle to the system image list that
			///	contains small icon images. The SHGFI_ICON and/or
			///	SHGFI_SYSICONINDEX flag must also be set.
			/// </summary>
			SHGFI_SMALLICON = 0x000000001,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve the file's
			///	open icon. Also used to modify SHGFI_SYSICONINDEX, causing the
			///	function to return the handle to the system image list that
			///	contains the file's small open icon. A container object displays
			///	an open icon to indicate that the container is open. The SHGFI_ICON
			///	and/or SHGFI_SYSICONINDEX flag must also be set.
			/// </summary>
			SHGFI_OPENICON = 0x000000002,

			/// <summary>
			/// Modify SHGFI_ICON, causing the function to retrieve a Shell-sized
			///	icon. If this flag is not specified the function sizes the icon
			///	according to the system metric values. The SHGFI_ICON flag must
			///	also be set.
			/// </summary>
			SHGFI_SHELLICONSIZE = 0x000000004,

			/// <summary>
			/// Indicate that pszPath is the address of an ITEMIDLIST structure
			///	rather than a path name.
			/// </summary>
			SHGFI_PIDL = 0x000000008,

			/// <summary>
			/// Indicates that the function should not attempt to access the file
			///	specified by pszPath. Rather, it should act as if the file specified
			///	by pszPath exists with the file attributes passed in dwFileAttributes.
			///	This flag cannot be combined with the SHGFI_ATTRIBUTES, SHGFI_EXETYPE,
			///	or SHGFI_PIDL flags.
			/// </summary>
			SHGFI_USEFILEATTRIBUTES = 0x000000010,

			/// <summary>
			/// Version 5.0. Apply the appropriate overlays to the file's icon.
			///	The SHGFI_ICON flag must also be set.
			/// </summary>
			SHGFI_ADDOVERLAYS = 0x000000020,

			/// <summary>
			/// Version 5.0. Return the index of the overlay icon. The value of
			///	the overlay index is returned in the upper eight bits of the iIcon
			///	member of the structure specified by psfi. This flag requires that
			///	the SHGFI_ICON be set as well.
			/// </summary>
			SHGFI_OVERLAYINDEX = 0x000000040
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHFILEINFO
		{
			/// <summary>
			/// A handle to the icon that represents the file. You are responsible
			/// for destroying this handle with DestroyIcon when you no longer need it.
			/// </summary>
			public IntPtr hIcon;

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
		/// Retrieves the path of a known folder as an ITEMIDLIST structure.
		/// </summary>
		/// <param name="rfid">A reference to the KNOWNFOLDERID that identifies the
		/// folder. The folders associated with the known folder IDs might not exist
		/// on a particular system.</param>
		/// <param name="dwFlags">Flags that specify special retrieval options. This
		/// value can be 0; otherwise, it is one or more of the KNOWN_FOLDER_FLAG values.</param>
		/// <param name="hToken">An access token used to represent a particular user. This
		/// parameter is usually set to NULL, in which case the function tries to access
		/// the current user's instance of the folder. However, you may need to assign
		/// a value to hToken for those folders that can have multiple users but are
		/// treated as belonging to a single user. The most commonly used folder of this
		/// type is Documents.
		/// 
		/// The calling application is responsible for correct impersonation when hToken
		/// is non-null. It must have appropriate security privileges for the particular
		/// user, including TOKEN_QUERY and TOKEN_IMPERSONATE, and the user's registry
		/// hive must be currently mounted. See Access Control for further discussion
		/// of access control issues.
		/// 
		/// Assigning the hToken parameter a value of -1 indicates the Default User.
		/// This allows clients of SHGetKnownFolderIDList to find folder locations
		/// (such as the Desktop folder) for the Default User. The Default User user
		/// profile is duplicated when any new user account is created, and includes
		/// special folders such as Documents and Desktop. Any items added to the
		/// Default User folder also appear in any new user account. Note that access
		/// to the Default User folders requires administrator privileges.</param>
		/// <param name="ppidl">When this method returns, contains a pointer to the PIDL
		/// of the folder. This parameter is passed uninitialized. The caller is
		/// responsible for freeing the returned PIDL when it is no longer needed
		/// by calling ILFree.</param>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
		public static extern void SHGetKnownFolderIDList(ref Guid rfid, int dwFlags,
			IntPtr hToken, out IntPtr ppidl);

		/// <summary>
		/// Frees an ITEMIDLIST structure allocated by the Shell.
		/// </summary>
		/// <param name="pidl">A pointer to the ITEMIDLIST structure to be freed.
		/// This parameter can be NULL.</param>
		[DllImport("Shell32.dll")]
		public static extern void ILFree(IntPtr pidl);

		/// <summary>
		/// Converts an item identifier list to a file system path.
		/// </summary>
		/// <param name="pidl">The address of an item identifier list that specifies
		/// a file or directory location relative to the root of the namespace'
		/// (the desktop).</param>
		/// <param name="pszPath">The address of a buffer to receive the file system path.
		/// This buffer must be at least MAX_PATH characters in size.</param>
		/// <returns>Returns TRUE if successful; otherwise, FALSE.</returns>
		/// <remarks>If the location specified by the pidl parameter is not part of the
		/// file system, this function will fail.
		/// 
		/// If the pidl parameter specifies a shortcut, the pszPath will contain the
		/// path to the shortcut, not to the shortcut's target.</remarks>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SHGetPathFromIDList(IntPtr pidl,
			StringBuilder pszPath);

		/// <summary>
		/// Retrieves the full path of a known folder identified by the folder's KNOWNFOLDERID.
		/// </summary>
		/// <param name="rfid">A reference to the KNOWNFOLDERID that identifies the
		/// folder.</param>
		/// <param name="dwFlags">Flags that specify special retrieval options. This value
		/// can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
		/// <param name="hToken">An access token that represents a particular user. If
		/// this parameter is NULL, which is the most common usage, the function requests
		/// the known folder for the current user.
		/// 
		/// Request a specific user's folder by passing the hToken of that user. This is
		/// typically done in the context of a service that has sufficient privileges to
		/// retrieve the token of a given user. That token must be opened with TOKEN_QUERY
		/// and TOKEN_IMPERSONATE rights. In addition to passing the user's hToken, the
		/// registry hive of that specific user must be mounted. See Access Control for
		/// further discussion of access control issues.
		/// 
		/// Assigning the hToken parameter a value of -1 indicates the Default User. This
		/// allows clients of SHGetKnownFolderPath to find folder locations (such as the
		/// Desktop folder) for the Default User. The Default User user profile is duplicated
		/// when any new user account is created, and includes special folders such as
		/// Documents and Desktop. Any items added to the Default User folder also appear in
		/// any new user account. Note that access to the Default User folders requires
		/// administrator privileges.</param>
		/// <param name="ppszPath">When this method returns, contains the address of a
		/// pointer to a null-terminated Unicode string that specifies the path of the
		/// known folder. The calling process is responsible for freeing this resource
		/// once it is no longer needed by calling CoTaskMemFree. The returned path does
		/// not include a trailing backslash. For example, "C:\Users" is returned rather
		/// than "C:\Users\".</param>
		/// <returns>Returns S_OK if successful, or an error value otherwise</returns>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Error)]
		internal static extern uint SHGetKnownFolderPath(
			ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

		/// <summary>
		/// Retrieves information about system-defined Shell icons.
		/// </summary>
		/// <param name="siid">One of the values from the SHGetStockIcons enumeration that
		/// specifies which icon should be retrieved.</param>
		/// <param name="uFlags">A combination of zero or more of the SHGetStockIconInfo
		/// flags that specify which information is requested.</param>
		/// <param name="psii">A pointer to a SHSTOCKICONINFO structure. When this function
		/// is called, the cbSize member of this structure needs to be set to the size of
		/// the SHSTOCKICONINFO structure. When this function returns, contains a pointer
		/// to a SHSTOCKICONINFO structure that contains the requested information.</param>
		/// <returns></returns>
		[DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Error)]
		internal static extern uint SHGetStockIconInfo(SHStockIcons siid,
			ShGetStockIconInfoFlags uFlags, ref SHSTOCKICONINFO psii);

		public enum SHStockIcons
		{
			SIID_DOCNOASSOC = 0,          // document (blank page), no associated program
			SIID_DOCASSOC = 1,            // document with an associated program
			SIID_APPLICATION = 2,         // generic application with no custom icon
			SIID_FOLDER = 3,              // folder (closed)
			SIID_FOLDEROPEN = 4,          // folder (open)
			SIID_DRIVE525 = 5,            // 5.25" floppy disk drive
			SIID_DRIVE35 = 6,             // 3.5" floppy disk drive
			SIID_DRIVEREMOVE = 7,         // removable drive
			SIID_DRIVEFIXED = 8,          // fixed (hard disk) drive
			SIID_DRIVENET = 9,            // network drive
			SIID_DRIVENETDISABLED = 10,   // disconnected network drive
			SIID_DRIVECD = 11,            // CD drive
			SIID_DRIVERAM = 12,           // RAM disk drive
			SIID_WORLD = 13,              // entire network
			SIID_SERVER = 15,             // a computer on the network
			SIID_PRINTER = 16,            // printer
			SIID_MYNETWORK = 17,          // My network places
			SIID_FIND = 22,               // Find
			SIID_HELP = 23,               // Help
			SIID_SHARE = 28,              // overlay for shared items
			SIID_LINK = 29,               // overlay for shortcuts to items
			SIID_SLOWFILE = 30,           // overlay for slow items
			SIID_RECYCLER = 31,           // empty recycle bin
			SIID_RECYCLERFULL = 32,       // full recycle bin
			SIID_MEDIACDAUDIO = 40,       // Audio CD Media
			SIID_LOCK = 47,               // Security lock
			SIID_AUTOLIST = 49,           // AutoList
			SIID_PRINTERNET = 50,         // Network printer
			SIID_SERVERSHARE = 51,        // Server share
			SIID_PRINTERFAX = 52,         // Fax printer
			SIID_PRINTERFAXNET = 53,      // Networked Fax Printer
			SIID_PRINTERFILE = 54,        // Print to File
			SIID_STACK = 55,              // Stack
			SIID_MEDIASVCD = 56,          // SVCD Media
			SIID_STUFFEDFOLDER = 57,      // Folder containing other items
			SIID_DRIVEUNKNOWN = 58,       // Unknown drive
			SIID_DRIVEDVD = 59,           // DVD Drive
			SIID_MEDIADVD = 60,           // DVD Media
			SIID_MEDIADVDRAM = 61,        // DVD-RAM Media
			SIID_MEDIADVDRW = 62,         // DVD-RW Media
			SIID_MEDIADVDR = 63,          // DVD-R Media
			SIID_MEDIADVDROM = 64,        // DVD-ROM Media
			SIID_MEDIACDAUDIOPLUS = 65,   // CD+ (Enhanced CD) Media
			SIID_MEDIACDRW = 66,          // CD-RW Media
			SIID_MEDIACDR = 67,           // CD-R Media
			SIID_MEDIACDBURN = 68,        // Burning CD
			SIID_MEDIABLANKCD = 69,       // Blank CD Media
			SIID_MEDIACDROM = 70,         // CD-ROM Media
			SIID_AUDIOFILES = 71,         // Audio files
			SIID_IMAGEFILES = 72,         // Image files
			SIID_VIDEOFILES = 73,         // Video files
			SIID_MIXEDFILES = 74,         // Mixed files
			SIID_FOLDERBACK = 75,         // Folder back
			SIID_FOLDERFRONT = 76,        // Folder front
			SIID_SHIELD = 77,             // Security shield. Use for UAC prompts only.
			SIID_WARNING = 78,            // Warning
			SIID_INFO = 79,               // Informational
			SIID_ERROR = 80,              // Error
			SIID_KEY = 81,                // Key / Secure
			SIID_SOFTWARE = 82,           // Software
			SIID_RENAME = 83,             // Rename
			SIID_DELETE = 84,             // Delete
			SIID_MEDIAAUDIODVD = 85,      // Audio DVD Media
			SIID_MEDIAMOVIEDVD = 86,      // Movie DVD Media
			SIID_MEDIAENHANCEDCD = 87,    // Enhanced CD Media
			SIID_MEDIAENHANCEDDVD = 88,   // Enhanced DVD Media
			SIID_MEDIAHDDVD = 89,         // HD-DVD Media
			SIID_MEDIABLURAY = 90,        // BluRay Media
			SIID_MEDIAVCD = 91,           // VCD Media
			SIID_MEDIADVDPLUSR = 92,      // DVD+R Media
			SIID_MEDIADVDPLUSRW = 93,     // DVD+RW Media
			SIID_DESKTOPPC = 94,          // desktop computer
			SIID_MOBILEPC = 95,           // mobile computer (laptop/notebook)
			SIID_USERS = 96,              // users
			SIID_MEDIASMARTMEDIA = 97,    // Smart Media
			SIID_MEDIACOMPACTFLASH = 98,  // Compact Flash
			SIID_DEVICECELLPHONE = 99,    // Cell phone
			SIID_DEVICECAMERA = 100,      // Camera
			SIID_DEVICEVIDEOCAMERA = 101, // Video camera
			SIID_DEVICEAUDIOPLAYER = 102, // Audio player
			SIID_NETWORKCONNECT = 103,    // Connect to network
			SIID_INTERNET = 104,          // Internet
			SIID_ZIPFILE = 105,           // ZIP file
			SIID_SETTINGS = 106,          // Settings
			// 107-131 are internal Vista RTM icons
			// 132-159 for SP1 icons
			SIID_DRIVEHDDVD = 132,        // HDDVD Drive (all types)
			SIID_DRIVEBD = 133,           // BluRay Drive (all types)
			SIID_MEDIAHDDVDROM = 134,     // HDDVD-ROM Media
			SIID_MEDIAHDDVDR = 135,       // HDDVD-R Media
			SIID_MEDIAHDDVDRAM = 136,     // HDDVD-RAM Media
			SIID_MEDIABDROM = 137,        // BluRay ROM Media
			SIID_MEDIABDR = 138,          // BluRay R Media
			SIID_MEDIABDRE = 139,         // BluRay RE Media (Rewriable and RAM)
			SIID_CLUSTEREDDRIVE = 140,    // Clustered disk
			// 160+ are for Windows 7 icons
			SIID_MAX_ICONS = 174,
		}

		[Flags]
		public enum ShGetStockIconInfoFlags
		{
			/// <summary>
			/// The szPath and iIcon members of the SHSTOCKICONINFO structure receive the path
			/// and icon index of the requested icon, in a format suitable for passing to the
			/// ExtractIcon function. The numerical value of this flag is zero, so you always
			/// get the icon location regardless of other flags.
			/// </summary>
			SHGSI_ICONLOCATION = 0,

			/// <summary>
			/// The hIcon member of the SHSTOCKICONINFO structure receives a handle to the
			/// specified icon.
			/// </summary>
			SHGSI_ICON = 0x000000100,
			
			/// <summary>
			/// The iSysImageImage member of the SHSTOCKICONINFO structure receives the index
			/// of the specified icon in the system imagelist.
			/// </summary>
			SHGSI_SYSICONINDEX = 0x000004000,

			/// <summary>
			/// Modifies the SHGSI_ICON value by causing the function to add the link overlay
			/// to the file's icon.
			/// </summary>
			SHGSI_LINKOVERLAY = 0x000008000,
		
			/// <summary>
			/// Modifies the SHGSI_ICON value by causing the function to blend the icon with
			/// the system highlight color.
			/// </summary>
			SHGSI_SELECTED = 0x000010000,

			/// <summary>
			/// Modifies the SHGSI_ICON value by causing the function to retrieve the large
			/// version of the icon, as specified by the SM_CXICON and SM_CYICON system metrics.
			/// </summary>
			SHGSI_LARGEICON = 0x000000000,

			/// <summary>
			/// Modifies the SHGSI_ICON value by causing the function to retrieve the small
			/// version of the icon, as specified by the SM_CXSMICON and SM_CYSMICON system
			/// metrics.
			/// </summary>
			SHGSI_SMALLICON = 0x000000001,

			/// <summary>
			/// Modifies the SHGSI_LARGEICON or SHGSI_SMALLICON values by causing the function
			/// to retrieve the Shell-sized icons rather than the sizes specified by the
			/// system metrics.
			/// </summary>
			SHGSI_SHELLICONSIZE = 0x000000004
		}

		/// <summary>
		/// Receives information used to retrieve a stock Shell icon. This structure is used
		/// in a call SHGetStockIconInfo.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHSTOCKICONINFO
		{
			/// <summary>
			/// The size of this structure, in bytes.
			/// </summary>
			public uint cbSize;

			/// <summary>
			/// When SHGetStockIconInfo is called with the SHGSI_ICON flag, this member
			/// receives a handle to the icon.
			/// </summary>
			public IntPtr hIcon;

			/// <summary>
			/// When SHGetStockIconInfo is called with the SHGSI_SYSICONINDEX flag, this member
			/// receives the index of the image in the system icon cache.
			/// </summary>
			public int iSysImageIndex;

			/// <summary>
			/// When SHGetStockIconInfo is called with the SHGSI_ICONLOCATION flag, this member
			/// receives the index of the icon in the resource whose path is received in szPath.
			/// </summary>
			public int iIcon;

			/// <summary>
			/// When SHGetStockIconInfo is called with the SHGSI_ICONLOCATION flag, this member
			/// receives the path of the resource that contains the icon. The index of the icon
			/// within the resource is received in iIcon.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxPath)]
			public char[] szPath;
		}
	}
}
