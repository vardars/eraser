/* 
 * $Id$
 * Copyright 2009 The Eraser Project
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
using System.ComponentModel;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class AdvApi
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
		public static bool UacEnabled()
		{
			//Check whether we're on Vista
			if (Environment.OSVersion.Platform != PlatformID.Win32NT ||
				Environment.OSVersion.Version < new Version(6, 0))
			{
				//UAC doesn't exist on these platforms.
				return false;
			}

			//Get the process token.
			SafeTokenHandle hToken = new SafeTokenHandle();
			bool result = NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(),
				NativeMethods.TOKEN_QUERY, out hToken);
			if (!result || hToken.IsInvalid)
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());

			IntPtr pElevationType = Marshal.AllocHGlobal(Marshal.SizeOf(
				typeof(NativeMethods.TOKEN_ELEVATION_TYPE)));
			try
			{
				//Get the token information for our current process.
				uint returnSize = 0;
				result = NativeMethods.GetTokenInformation(hToken,
					NativeMethods.TOKEN_INFORMATION_CLASS.TokenElevationType,
					pElevationType, sizeof(NativeMethods.TOKEN_ELEVATION_TYPE),
					out returnSize);

				//Check the return code
				if (!result)
					throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());

				NativeMethods.TOKEN_ELEVATION_TYPE elevationType =
					(NativeMethods.TOKEN_ELEVATION_TYPE)Marshal.PtrToStructure(
						pElevationType, typeof(NativeMethods.TOKEN_ELEVATION_TYPE));
				return elevationType != NativeMethods.TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
			}
			finally
			{
				Marshal.FreeHGlobal(pElevationType);
			}
		}
	}

	public sealed class CryptApi : IDisposable
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private CryptApi()
		{
			/* Intel i8xx (82802 Firmware Hub Device) hardware random number generator */
			const string IntelDefaultProvider = "Intel Hardware Cryptographic Service Provider";

			handle = new SafeCryptHandle();
			if (NativeMethods.CryptAcquireContext(out handle, string.Empty,
				IntelDefaultProvider, NativeMethods.PROV_INTEL_SEC, 0))
			{
				return;
			}
			else if (NativeMethods.CryptAcquireContext(out handle, string.Empty,
				string.Empty, NativeMethods.PROV_RSA_FULL, 0))
			{
				return;
			}
			else if (Marshal.GetLastWin32Error() == NativeMethods.NTE_BAD_KEYSET)
			{
				//Default keyset doesn't exist, attempt to create a new one
				if (NativeMethods.CryptAcquireContext(out handle, string.Empty, string.Empty,
					NativeMethods.PROV_RSA_FULL, NativeMethods.CRYPT_NEWKEYSET))
				{
					return;
				}
			}

			throw new NotSupportedException("Unable to acquire a cryptographic service provider.");
		}

		#region IDisposable Members
		~CryptApi()
		{
			Dispose(false);
		}

		public void Dispose(bool disposing)
		{
			if (disposing)
				handle.Close();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		/// <summary>
		/// The GenRandom function fills a buffer with cryptographically random bytes.
		/// </summary>
		/// <param name="buffer">Buffer to receive the returned data. This buffer
		/// must be at least dwLen bytes in length.
		/// 
		/// Optionally, the application can fill this buffer with data to use as
		/// an auxiliary random seed.</param>
		public static bool CryptGenRandom(byte[] buffer)
		{
			return NativeMethods.CryptGenRandom(instance.handle, (uint)buffer.Length, buffer);
		}

		/// <summary>
		/// The HCRYPTPROV handle.
		/// </summary>
		private SafeCryptHandle handle;

		/// <summary>
		/// The global CryptAPI instance.
		/// </summary>
		private static CryptApi instance = new CryptApi();
	}

	internal class SafeCryptHandle : SafeHandle
	{
		public SafeCryptHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			get { return handle == IntPtr.Zero; }
		}

		protected override bool ReleaseHandle()
		{
			NativeMethods.CryptReleaseContext(handle, 0u);
			handle = IntPtr.Zero;
			return true;
		}
	}

	internal class SafeTokenHandle : SafeHandle
	{
		public SafeTokenHandle()
			: base(IntPtr.Zero, true)
		{
		}

		public override bool IsInvalid
		{
			get { return handle == IntPtr.Zero; }
		}

		protected override bool ReleaseHandle()
		{
			NativeMethods.CloseHandle(handle);
			handle = IntPtr.Zero;
			return true;
		}
	}
}