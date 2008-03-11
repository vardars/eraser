using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Eraser.Util
{
	using HICON = IntPtr;
	using HIMAGELIST = IntPtr;

	public class File
	{
		/// <summary>
		/// Uses SHGetFileInfo to retrieve the description for the given file,
		/// folder or drive.
		/// </summary>
		/// <param name="path">A string that contains the path and file name for
		/// the file in question. Both absolute and relative paths are valid.</param>
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
		/// the file in question. Both absolute and relative paths are valid.</param>
		/// <returns>An Icon object containing the bitmap</returns>
		public static Icon GetFileIcon(string path)
		{
			SHFILEINFO shfi = new SHFILEINFO();
			SHGetFileInfo(path, 0, ref shfi, Marshal.SizeOf(shfi),
				SHGetFileInfoFlags.SHGFI_SMALLICON | SHGetFileInfoFlags.SHGFI_ICON);
			return Icon.FromHandle(shfi.hIcon);
		}

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
					if (PathCompactPathEx(builder, longPath, (UIntPtr)(--charCount),
						(UIntPtr)0) == UIntPtr.Zero)
					{
						return string.Empty;
					}
				}

				return builder.ToString();
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
		[DllImport("Shlwapi.dll")]
		private static extern UIntPtr PathCompactPathEx(
			StringBuilder pszOut, string pszSrc, UIntPtr cchMax, UIntPtr dwFlags);

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
	}
}
