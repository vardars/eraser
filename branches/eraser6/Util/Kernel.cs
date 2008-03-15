using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Eraser.Util
{
	public static class Kernel
	{
		/// <summary>
		/// Retrieves a pseudo handle for the current process.
		/// </summary>
		/// <returns>A pseudo handle to the current process.</returns>
		/// <remarks>A pseudo handle is a special constant, currently (HANDLE)-1,
		/// that is interpreted as the current process handle. For compatibility
		/// with future operating systems, it is best to call GetCurrentProcess
		/// instead of hard-coding this constant value. The calling process can
		/// use a pseudo handle to specify its own process whenever a process
		/// handle is required. Pseudo handles are not inherited by child processes.
		/// 
		/// This handle has the maximum possible access to the process object.
		/// For systems that support security descriptors, this is the maximum
		/// access allowed by the security descriptor for the calling process.
		/// For systems that do not support security descriptors, this is
		/// PROCESS_ALL_ACCESS. For more information, see Process Security and
		/// Access Rights.
		/// 
		/// A process can create a "real" handle to itself that is valid in the
		/// context of other processes, or that can be inherited by other processes,
		/// by specifying the pseudo handle as the source handle in a call to the
		/// DuplicateHandle function. A process can also use the OpenProcess
		/// function to open a real handle to itself.
		/// 
		/// The pseudo handle need not be closed when it is no longer needed.
		/// Calling the CloseHandle function with a pseudo handle has no effect.
		/// If the pseudo handle is duplicated by DuplicateHandle, the duplicate
		/// handle must be closed.</remarks>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentProcess();

		/// <summary>
		/// Closes an open object handle.
		/// </summary>
		/// <param name="hObject">A valid handle to an open object.</param>
		/// <returns>If the function succeeds, the return value is true. To get
		/// extended error information, call Marshal.GetLastWin32Error().
		/// 
		/// If the application is running under a debugger, the function will throw
		/// an exception if it receives either a handle value that is not valid
		/// or a pseudo-handle value. This can happen if you close a handle twice,
		/// or if you call CloseHandle on a handle returned by the FindFirstFile
		/// function.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);
	}
}
