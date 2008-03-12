using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class Kernel
	{
		/// <summary>
		/// Retrieves the calling thread's last-error code value. The last-error
		/// code is maintained on a per-thread basis. Multiple threads do not
		/// overwrite each other's last-error code.
		/// </summary>
		/// <returns>The return value is the calling thread's last-error code.</returns>
		[DllImport("Kernel32.dll")]
		public static extern uint GetLastError();
	}
}
