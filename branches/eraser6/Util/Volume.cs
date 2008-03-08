using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class Drives
	{
		/// <summary>
		/// Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk,
		/// or network drive.
		/// 
		/// To determine whether a drive is a USB-type drive, call
		/// SetupDiGetDeviceRegistryProperty and specify the SPDRP_REMOVAL_POLICY property.
		/// </summary>
		/// <param name="rootPathName">The root directory for the drive.</param>
		/// <remarks>A trailing backslash is required. If this parameter is NULL, the
		/// function uses the root of the current directory.</remarks>
		/// <returns>The return value specifies the type of drive, which can be one
		/// of the values in the DriveTypes enumeration</returns>
		[DllImport("Kernel32.dll")]
		public static extern DriveTypes GetDriveType(string rootPathName);
	}

	/// <summary>
	/// The types returned by the GetDriveType function.
	/// </summary>
	public enum DriveTypes
	{
		/// <summary>
		/// The drive type cannot be determined.
		/// </summary>
		DRIVE_UNKNOWN = 0,

		/// <summary>
		/// The root path is invalid; for example, there is no volume is mounted at the path.
		/// </summary>
		DRIVE_NO_ROOT_DIR = 1,

		/// <summary>
		/// The drive has removable media; for example, a floppy drive, thumb drive, or flash card reader.
		/// </summary>
		DRIVE_REMOVABLE = 2,

		/// <summary>
		/// The drive has fixed media; for example, a hard drive or flash drive.
		/// </summary>
		DRIVE_FIXED = 3,

		/// <summary>
		/// The drive is a remote (network) drive.
		/// </summary>
		DRIVE_REMOTE = 4,

		/// <summary>
		/// The drive is a CD-ROM drive.
		/// </summary>
		DRIVE_CDROM = 5,

		/// <summary>
		/// The drive is a RAM disk.
		/// </summary>
		DRIVE_RAMDISK = 6
	}
}
