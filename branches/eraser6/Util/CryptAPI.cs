using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public class CryptAPI
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private CryptAPI()
		{
			/* Intel i8xx (82802 Firmware Hub Device) hardware random number generator */
			const string IntelDefaultProvider = "Intel Hardware Cryptographic Service Provider";

			handle = new SafeCryptHandle();
			if (CryptAcquireContext(out handle, string.Empty, IntelDefaultProvider, PROV_INTEL_SEC, 0))
				return;
			else if (CryptAcquireContext(out handle, string.Empty, string.Empty, PROV_RSA_FULL, 0))
				return;
			else if (Marshal.GetLastWin32Error() == NTE_BAD_KEYSET)
			{
				//Default keyset doesn't exist, attempt to create a new one
				if (CryptAcquireContext(out handle, string.Empty, string.Empty, PROV_RSA_FULL, CRYPT_NEWKEYSET))
					return;
			}

			throw new Exception("Unable to acquire a cryptographic service provider.");
		}

		/// <summary>
		/// The GenRandom function fills a buffer with cryptographically random bytes.
		/// </summary>
		/// <param name="pbBuffer">Buffer to receive the returned data. This buffer
		/// must be at least dwLen bytes in length.
		/// 
		/// Optionally, the application can fill this buffer with data to use as
		/// an auxiliary random seed.</param>
		public static bool CryptGenRandom(byte[] pbBuffer)
		{
			return CryptGenRandomInternal(instance.handle, (uint)pbBuffer.Length, pbBuffer);
		}

		/// <summary>
		/// The CryptAcquireContext function is used to acquire a handle to a
		/// particular key container within a particular cryptographic service
		/// provider (CSP). This returned handle is used in calls to CryptoAPI
		/// functions that use the selected CSP.
		/// 
		/// This function first attempts to find a CSP with the characteristics
		/// described in the dwProvType and pszProvider parameters. If the CSP
		/// is found, the function attempts to find a key container within the
		/// CSP that matches the name specified by the pszContainer parameter.
		/// To acquire the context and the key container of a private key
		/// associated with the public key of a certificate, use
		/// CryptAcquireCertificatePrivateKey.
		/// 
		/// With the appropriate setting of dwFlags, this function can also create
		/// and destroy key containers and can provide access to a CSP with a
		/// temporary key container if access to a private key is not required.
		/// </summary>
		/// <param name="phProv">A pointer to a handle of a CSP. When you have
		/// finished using the CSP, release the handle by calling the
		/// CryptReleaseContext function.</param>
		/// <param name="pszContainer">The key container name. This is a
		/// null-terminated string that identifies the key container to the CSP.
		/// This name is independent of the method used to store the keys.
		/// Some CSPs store their key containers internally (in hardware),
		/// some use the system registry, and others use the file system. When
		/// dwFlags is set to CRYPT_VERIFYCONTEXT, pszContainer must be set to NULL.
		/// 
		/// When pszContainer is NULL, a default key container name is used. For
		/// example, the Microsoft Base Cryptographic Provider uses the logon name
		/// of the currently logged on user as the key container name. Other CSPs
		/// can also have default key containers that can be acquired in this way.
		/// 
		/// Applications must not use the default key container to store private
		/// keys. When multiple applications use the same container, one application
		/// can change or destroy the keys that another application needs to have
		/// available. If applications use key containers linked to the application,
		/// the risk is reduced of other applications tampering with keys necessary
		/// for proper function.
		/// 
		/// An application can obtain the name of the key container in use by
		/// reading the PP_CONTAINER value with the CryptGetProvParam function.</param>
		/// <param name="pszProvider">A null-terminated string that specifies the
		/// name of the CSP to be used.
		/// 
		/// If this parameter is NULL, the user default provider is used. For more
		/// information, see Cryptographic Service Provider Contexts. For a list
		/// of available cryptographic providers, see Cryptographic Provider Names.
		/// 
		/// An application can obtain the name of the CSP in use by using the
		/// CryptGetProvParam function to read the PP_NAME CSP value in the dwParam
		/// parameter.
		/// 
		/// Due to changing export control restrictions, the default CSP can change
		/// between operating system releases. To ensure interoperability on
		/// different operating system platforms, the CSP should be explicitly
		/// set by using this parameter instead of using the default CSP.</param>
		/// <param name="dwProvType">Specifies the type of provider to acquire.
		/// Defined provider types are discussed in Cryptographic Provider Types.</param>
		/// <param name="dwFlags">Flag values. This parameter is usually set to zero,
		/// but some applications set one or more flags.</param>
		/// <returns> If the function succeeds, the function returns nonzero (TRUE).
		/// If the function fails, it returns zero (FALSE). For extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CryptAcquireContext(out SafeCryptHandle phProv,
			string pszContainer, string pszProvider, uint dwProvType, uint dwFlags);

		/// <summary>
		/// The CryptGenRandom function fills a buffer with cryptographically random bytes.
		/// </summary>
		/// <param name="hProv">Handle of a cryptographic service provider (CSP)
		/// created by a call to CryptAcquireContext.</param>
		/// <param name="dwLen">Number of bytes of random data to be generated.</param>
		/// <param name="pbBuffer">Buffer to receive the returned data. This buffer
		/// must be at least dwLen bytes in length.
		/// 
		/// Optionally, the application can fill this buffer with data to use as
		/// an auxiliary random seed.</param>
		[DllImport("Advapi32.dll", SetLastError = true, EntryPoint = "CryptGenRandom")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptGenRandomInternal(SafeCryptHandle hProv,
			uint dwLen, byte[] pbBuffer);

		/// <summary>
		/// The CryptReleaseContext function releases the handle of a cryptographic
		/// service provider (CSP) and a key container. At each call to this function,
		/// the reference count on the CSP is reduced by one. When the reference
		/// count reaches zero, the context is fully released and it can no longer
		/// be used by any function in the application.
		/// 
		/// An application calls this function after finishing the use of the CSP.
		/// After this function is called, the released CSP handle is no longer
		/// valid. This function does not destroy key containers or key pairs</summary>
		/// <param name="hProv">Handle of a cryptographic service provider (CSP)
		/// created by a call to CryptAcquireContext.</param>
		/// <param name="dwFlags">Reserved for future use and must be zero. If
		/// dwFlags is not set to zero, this function returns FALSE but the CSP
		/// is released.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).
		/// 
		/// If the function fails, the return value is zero (FALSE). For extended
		/// error information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CryptReleaseContext(IntPtr hProv, uint dwFlags);

		/// <summary>
		/// The HCRYPTPROV handle.
		/// </summary>
		private SafeCryptHandle handle;

		/// <summary>
		/// The global CryptAPI instance.
		/// </summary>
		private static CryptAPI instance = new CryptAPI();

		private const uint PROV_RSA_FULL        = 1;
		private const uint PROV_RSA_SIG         = 2;
		private const uint PROV_DSS             = 3;
		private const uint PROV_FORTEZZA        = 4;
		private const uint PROV_MS_EXCHANGE     = 5;
		private const uint PROV_SSL             = 6;
		private const uint PROV_RSA_SCHANNEL    = 12;
		private const uint PROV_DSS_DH          = 13;
		private const uint PROV_EC_ECDSA_SIG    = 14;
		private const uint PROV_EC_ECNRA_SIG    = 15;
		private const uint PROV_EC_ECDSA_FULL   = 16;
		private const uint PROV_EC_ECNRA_FULL   = 17;
		private const uint PROV_DH_SCHANNEL     = 18;
		private const uint PROV_SPYRUS_LYNKS    = 20;
		private const uint PROV_RNG             = 21;
		private const uint PROV_INTEL_SEC       = 22;

		private const int NTE_BAD_KEYSET        = unchecked ((int)0x80090016);

		private const uint CRYPT_VERIFYCONTEXT  = 0xF0000000;
		private const uint CRYPT_NEWKEYSET      = 0x00000008;
		private const uint CRYPT_DELETEKEYSET   = 0x00000010;
		private const uint CRYPT_MACHINE_KEYSET = 0x00000020;
		private const uint CRYPT_SILENT         = 0x00000040;
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
			CryptAPI.CryptReleaseContext(handle, 0u);
			handle = IntPtr.Zero;
			return true;
		}
	}
}
