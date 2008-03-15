using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class NTAPI
	{
		/// <summary>
		/// Queries system parameters using the NT Native API.
		/// </summary>
		/// <param name="dwType">The type of information to retrieve.</param>
		/// <param name="dwData">The buffer to receive the information.</param>
		/// <param name="dwMaxSize">The size of the buffer.</param>
		/// <param name="dwDataSize">Receives the amount of data written to the
		/// buffer.</param>
		/// <returns>A system error code.</returns>
		[DllImport("NtDll.dll")]
		public static extern uint NtQuerySystemInformation(uint dwType, byte[] dwData,
			uint dwMaxSize, out uint dwDataSize);
	}
}
