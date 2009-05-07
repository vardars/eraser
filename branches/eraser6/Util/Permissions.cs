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

using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Eraser.Util
{
	/// <summary>
	/// Deals with permissions in Windows.
	/// </summary>
	public static class Permissions
	{
		/// <summary>
		/// Checks whether the current process is running with administrative
		/// privileges.
		/// </summary>
		/// <returns>True if the user is an administrator. This only returns
		/// true under Vista when UAC is enabled and the process is elevated.</returns>
		public static bool IsAdministrator()
		{
			WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		/// <summary>
		/// Checks whether the current process is running with administrative privileges.
		/// </summary>
		/// <returns>Returns true if UAC is enabled under Vista. Will return false
		/// under pre-Vista OSes</returns>
		public static bool UACEnabled()
		{
			//Check whether we're on Vista
			if (Environment.OSVersion.Platform != PlatformID.Win32NT ||
				Environment.OSVersion.Version < new Version(6, 0))
			{
				//UAC doesn't exist on these platforms.
				return false;
			}

			//Get the process token.
			IntPtr hToken = IntPtr.Zero;
			bool result = OpenProcessToken(KernelAPI.NativeMethods.GetCurrentProcess(),
				TOKEN_QUERY, out hToken);
			if (!result || hToken == IntPtr.Zero)
				throw new Exception("Could not open process token.");

			IntPtr pElevationType = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(TOKEN_ELEVATION_TYPE)));
			try
			{
				//Get the token information for our current process.
				uint returnSize = 0;
				result = GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevationType,
					pElevationType, sizeof(TOKEN_ELEVATION_TYPE), out returnSize);

				//Check the return code
				if (!result)
					throw new Exception("Could not retrieve token information.");

				TOKEN_ELEVATION_TYPE elevationType = (TOKEN_ELEVATION_TYPE)Marshal.PtrToStructure(
					pElevationType, typeof(TOKEN_ELEVATION_TYPE));
				return elevationType != TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
			}
			finally
			{
				KernelAPI.NativeMethods.CloseHandle(hToken);
				Marshal.FreeHGlobal(pElevationType);
			}
		}

		public const uint STANDARD_RIGHTS_REQUIRED = 0xF0000;
		public const uint TOKEN_ASSIGN_PRIMARY     = 0x00001;
		public const uint TOKEN_DUPLICATE          = 0x00002;
		public const uint TOKEN_IMPERSONATE        = 0x00004;
		public const uint TOKEN_QUERY              = 0x00008;
		public const uint TOKEN_QUERY_SOURCE       = 0x00010;
		public const uint TOKEN_ADJUST_PRIVILEGES  = 0x00020;
		public const uint TOKEN_ADJUST_GROUPS      = 0x00040;
		public const uint TOKEN_ADJUST_DEFAULT     = 0x00080;
		public const uint TOKEN_ADJUST_SESSIONID   = 0x00100;
		public const uint TOKEN_ALL_ACCESS_P = (STANDARD_RIGHTS_REQUIRED |
			TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY |
			TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS |
			TOKEN_ADJUST_DEFAULT);

		public enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups = 2,
			TokenPrivileges = 3,
			TokenOwner = 4,
			TokenPrimaryGroup = 5,
			TokenDefaultDacl = 6,
			TokenSource = 7,
			TokenType = 8,
			TokenImpersonationLevel = 9,
			TokenStatistics = 10,
			TokenRestrictedSids = 11,
			TokenSessionId = 12,
			TokenGroupsAndPrivileges = 13,
			TokenSessionReference = 14,
			TokenSandBoxInert = 15,
			TokenAuditPolicy = 16,
			TokenOrigin = 17,
			TokenElevationType = 18,
			TokenLinkedToken = 19,
			TokenElevation = 20,
			TokenHasRestrictions = 21,
			TokenAccessInformation = 22,
			TokenVirtualizationAllowed = 23,
			TokenVirtualizationEnabled = 24,
			TokenIntegrityLevel = 25,
			TokenUIAccess = 26,
			TokenMandatoryPolicy = 27,
			TokenLogonSid = 28,
			MaxTokenInfoClass = 29  // MaxTokenInfoClass should always be the last enum
		}

		enum TOKEN_ELEVATION_TYPE
		{
			TokenElevationTypeDefault = 1,
			TokenElevationTypeFull = 2,
			TokenElevationTypeLimited = 3,
		}

		/// <summary>
		/// The OpenProcessToken function opens the access token associated with a process.
		/// </summary>
		/// <param name="ProcessHandle">A handle to the process whose access token
		/// is opened. The process must have the PROCESS_QUERY_INFORMATION access
		/// permission.</param>
		/// <param name="DesiredAccess">Specifies an access mask that specifies
		/// the requested types of access to the access token. These requested
		/// access types are compared with the discretionary access control
		/// list (DACL) of the token to determine which accesses are granted or
		/// denied.</param>
		/// <param name="TokenHandle">A pointer to a handle that identifies the
		/// newly opened access token when the function returns.</param>
		/// <returns> If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool OpenProcessToken(IntPtr ProcessHandle,
			UInt32 DesiredAccess, out IntPtr TokenHandle);

		/// <summary>
		/// The GetTokenInformation function retrieves a specified type of information
		/// about an access token. The calling process must have appropriate access
		/// rights to obtain the information.
		/// </summary>
		/// <param name="TokenHandle">A handle to an access token from which
		/// information is retrieved. If TokenInformationClass specifies TokenSource,
		/// the handle must have TOKEN_QUERY_SOURCE access. For all other
		/// TokenInformationClass values, the handle must have TOKEN_QUERY access.</param>
		/// <param name="TokenInformationClass">Specifies a value from the
		/// TOKEN_INFORMATION_CLASS enumerated type to identify the type of
		/// information the function retrieves.</param>
		/// <param name="TokenInformation">A pointer to a buffer the function
		/// fills with the requested information. The structure put into this
		/// buffer depends upon the type of information specified by the
		/// TokenInformationClass parameter.</param>
		/// <param name="TokenInformationLength">Specifies the size, in bytes,
		/// of the buffer pointed to by the TokenInformation parameter.
		/// If TokenInformation is NULL, this parameter must be zero.</param>
		/// <param name="ReturnLength">A pointer to a variable that receives the
		/// number of bytes needed for the buffer pointed to by the TokenInformation
		/// parameter. If this value is larger than the value specified in the
		/// TokenInformationLength parameter, the function fails and stores no
		/// data in the buffer.
		/// 
		/// If the value of the TokenInformationClass parameter is TokenDefaultDacl
		/// and the token has no default DACL, the function sets the variable pointed
		/// to by ReturnLength to sizeof(TOKEN_DEFAULT_DACL) and sets the
		/// DefaultDacl member of the TOKEN_DEFAULT_DACL structure to NULL.</param>
		/// <returns> If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().</returns>
		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetTokenInformation(IntPtr TokenHandle,
			TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation,
			uint TokenInformationLength, out uint ReturnLength);
	}
}
