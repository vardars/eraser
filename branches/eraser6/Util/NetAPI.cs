using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class NetAPI
	{
		/// <summary>
		/// The NetStatisticsGet function retrieves operating statistics for a service.
		/// Currently, only the workstation and server services are supported.
		/// </summary>
		/// <param name="server">Pointer to a string that specifies the DNS or
		/// NetBIOS name of the server on which the function is to execute. If
		/// this parameter is NULL, the local computer is used.</param>
		/// <param name="service">Pointer to a string that specifies the name of
		/// the service about which to get the statistics. Only the values
		/// SERVICE_SERVER and SERVICE_WORKSTATION are currently allowed.</param>
		/// <param name="level">Specifies the information level of the data.
		/// This parameter must be 0.</param>
		/// <param name="options">This parameter must be zero.</param>
		/// <param name="bufptr">Pointer to the buffer that receives the data. The
		/// format of this data depends on the value of the level parameter.
		/// This buffer is allocated by the system and must be freed using the
		/// NetApiBufferFree function. For more information, see Network Management
		/// Function Buffers and Network Management Function Buffer Lengths.</param>
		/// <returns>If the function succeeds, the return value is NERR_Success.
		/// 
		/// If the function fails, the return value is a system error code. For
		/// a list of error codes, see System Error Codes.</returns>
		[DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
		public static unsafe extern uint NetStatisticsGet(string server, string service,
			uint level, uint options, out IntPtr bufptr);

		/// <summary>
		/// The NetApiBufferSize function returns the size, in bytes, of a buffer
		/// allocated by a call to the NetApiBufferAllocate function.
		/// </summary>
		/// <param name="Buffer">Pointer to a buffer returned by the NetApiBufferAllocate
		/// function.</param>
		/// <param name="ByteCount">Receives the size of the buffer, in bytes.</param>
		/// <returns>If the function succeeds, the return value is NERR_Success.
		/// 
		/// If the function fails, the return value is a system error code. For
		/// a list of error codes, see System Error Codes.</returns>
		[DllImport("Netapi32.dll")]
		public static extern uint NetApiBufferSize(IntPtr Buffer, out uint ByteCount);

		/// <summary>
		/// The NetApiBufferFree function frees the memory that the NetApiBufferAllocate
		/// function allocates. Call NetApiBufferFree to free the memory that other
		/// network management functions return.
		/// </summary>
		/// <param name="Buffer">Pointer to a buffer returned previously by another
		/// network management function.</param>
		/// <returns>If the function succeeds, the return value is NERR_Success.
		/// 
		/// If the function fails, the return value is a system error code. For
		/// a list of error codes, see System Error Codes.</returns>
		[DllImport("Netapi32.dll")]
		public static unsafe extern uint NetApiBufferFree(IntPtr Buffer);

		private const uint NERR_Success = 0;
		public const string SERVICE_WORKSTATION = "LanmanWorkstation";
		public const string SERVICE_SERVER      = "LanmanServer";
	}
}
