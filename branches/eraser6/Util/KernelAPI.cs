/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: Garrett Trant <gtrant@users.sourceforge.net>
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
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Eraser.Util
{
	public static class KernelAPI
	{
		/// <summary>
		/// Allocates a new console for the calling process.
		/// </summary>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		/// <remarks>A process can be associated with only one console, so the AllocConsole
		/// function fails if the calling process already has a console. A process can
		/// use the FreeConsole function to detach itself from its current console, then
		/// it can call AllocConsole to create a new console or AttachConsole to attach
		/// to another console.
		/// 
		/// If the calling process creates a child process, the child inherits the
		/// new console.
		/// 
		/// AllocConsole initializes standard input, standard output, and standard error
		/// handles for the new console. The standard input handle is a handle to the
		/// console's input buffer, and the standard output and standard error handles
		/// are handles to the console's screen buffer. To retrieve these handles, use
		/// the GetStdHandle function.
		/// 
		/// This function is primarily used by graphical user interface (GUI) application
		/// to create a console window. GUI applications are initialized without a
		/// console. Console applications are initialized with a console, unless they
		/// are created as detached processes (by calling the CreateProcess function
		/// with the DETACHED_PROCESS flag).</remarks>
		public static bool AllocConsole()
		{
			return NativeMethods.AllocConsole();
		}

		/// <summary>
		/// Detaches the calling process from its console.
		/// </summary>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		/// <remarks>A process can be attached to at most one console. If the calling
		/// process is not already attached to a console, the error code returned is
		/// ERROR_INVALID_PARAMETER (87).
		/// 
		/// A process can use the FreeConsole function to detach itself from its
		/// console. If other processes share the console, the console is not destroyed,
		/// but the process that called FreeConsole cannot refer to it. A console is
		/// closed when the last process attached to it terminates or calls FreeConsole.
		/// After a process calls FreeConsole, it can call the AllocConsole function to
		/// create a new console or AttachConsole to attach to another console.</remarks>
		public static bool FreeConsole()
		{
			return NativeMethods.FreeConsole();
		}

		/// <summary>
		/// Retrieves the current value of the high-resolution performance counter.
		/// </summary>
		public static long PerformanceCounter
		{
			get
			{
				long result = 0;
				if (NativeMethods.QueryPerformanceCounter(out result))
					return result;
				return 0;
			}
		}

		/// <summary>
		/// Gets the current CPU type of the system.
		/// </summary>
		/// <returns>One of the <see cref="ProcessorTypes"/> enumeration values.</returns>
		public static ProcessorArchitecture ProcessorArchitecture
		{
			get
			{
				NativeMethods.SYSTEM_INFO info = new NativeMethods.SYSTEM_INFO();
				NativeMethods.GetSystemInfo(out info);

				switch (info.processorArchitecture)
				{
					case NativeMethods.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64:
						return ProcessorArchitecture.Amd64;
					case NativeMethods.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_IA64:
						return ProcessorArchitecture.IA64;
					case NativeMethods.SYSTEM_INFO.ProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL:
						return ProcessorArchitecture.X86;
					default:
						return ProcessorArchitecture.None;
				}
			}
		}

		/// <summary>
		/// Enables an application to inform the system that it is in use, thereby
		/// preventing the system from entering sleep or turning off the display
		/// while the application is running.
		/// </summary>
		/// <param name="executionState">The thread's execution requirements. This
		/// parameter can be one or more of the EXECUTION_STATE values.</param>
		/// <returns>If the function succeeds, the return value is the previous
		/// thread execution state.
		/// 
		/// If the function fails, the return value is NULL.</returns>
		/// <remarks>The system automatically detects activities such as local keyboard
		/// or mouse input, server activity, and changing window focus. Activities
		/// that are not automatically detected include disk or CPU activity and
		/// video display.
		/// 
		/// Calling SetThreadExecutionState without ES_CONTINUOUS simply resets
		/// the idle timer; to keep the display or system in the working state,
		/// the thread must call SetThreadExecutionState periodically.
		/// 
		/// To run properly on a power-managed computer, applications such as fax
		/// servers, answering machines, backup agents, and network management
		/// applications must use both ES_SYSTEM_REQUIRED and ES_CONTINUOUS when
		/// they process events. Multimedia applications, such as video players
		/// and presentation applications, must use ES_DISPLAY_REQUIRED when they
		/// display video for long periods of time without user input. Applications
		/// such as word processors, spreadsheets, browsers, and games do not need
		/// to call SetThreadExecutionState.
		/// 
		/// The ES_AWAYMODE_REQUIRED value should be used only when absolutely
		/// necessary by media applications that require the system to perform
		/// background tasks such as recording television content or streaming media
		/// to other devices while the system appears to be sleeping. Applications
		/// that do not require critical background processing or that run on
		/// portable computers should not enable away mode because it prevents
		/// the system from conserving power by entering true sleep.
		/// 
		/// To enable away mode, an application uses both ES_AWAYMODE_REQUIRED and
		/// ES_CONTINUOUS; to disable away mode, an application calls
		/// SetThreadExecutionState with ES_CONTINUOUS and clears
		/// ES_AWAYMODE_REQUIRED. When away mode is enabled, any operation that
		/// would put the computer to sleep puts it in away mode instead. The computer
		/// appears to be sleeping while the system continues to perform tasks that
		/// do not require user input. Away mode does not affect the sleep idle
		/// timer; to prevent the system from entering sleep when the timer expires,
		/// an application must also set the ES_SYSTEM_REQUIRED value.
		/// 
		/// The SetThreadExecutionState function cannot be used to prevent the user
		/// from putting the computer to sleep. Applications should respect that
		/// the user expects a certain behavior when they close the lid on their
		/// laptop or press the power button.
		/// 
		/// This function does not stop the screen saver from executing. 
		/// </remarks>
		public static ThreadExecutionState SetThreadExecutionState(
			ThreadExecutionState executionState)
		{
			return (ThreadExecutionState)NativeMethods.SetThreadExecutionState(
				(NativeMethods.EXECUTION_STATE)executionState);
		}

		/// <summary>
		/// Stores Kernel32.dll functions, structs and constants.
		/// </summary>
		internal static class NativeMethods
		{
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
			/// Retrieves information about the current system.
			/// </summary>
			/// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that
			/// receives the information.</param>
			[DllImport("Kernel32.dll")]
			public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

			/// <summary>
			/// The QueryPerformanceCounter function retrieves the current value of
			/// the high-resolution performance counter.
			/// </summary>
			/// <param name="lpPerformanceCount">[out] Pointer to a variable that receives
			/// the current performance-counter value, in counts.</param>
			/// <returns>If the function succeeds, the return value is nonzero.
			/// 
			/// If the function fails, the return value is zero. To get extended error
			/// information, call Marshal.GetLastWin32Error. </returns>
			[DllImport("Kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

			/// <summary>
			/// Contains information about the current computer system. This includes
			/// the architecture and type of the processor, the number of processors
			/// in the system, the page size, and other such information.
			/// </summary>
			[StructLayout(LayoutKind.Sequential)]
			internal struct SYSTEM_INFO
			{
				/// <summary>
				/// Represents a list of processor architectures.
				/// </summary>
				public enum ProcessorArchitecture : ushort
				{
					/// <summary>
					/// x64 (AMD or Intel).
					/// </summary>
					PROCESSOR_ARCHITECTURE_AMD64 = 9,

					/// <summary>
					/// Intel Itanium Processor Family (IPF).
					/// </summary>
					PROCESSOR_ARCHITECTURE_IA64 = 6,

					/// <summary>
					/// x86.
					/// </summary>
					PROCESSOR_ARCHITECTURE_INTEL = 0,

					/// <summary>
					/// Unknown architecture.
					/// </summary>
					PROCESSOR_ARCHITECTURE_UNKNOWN = 0xffff
				}

				/// <summary>
				/// The processor architecture of the installed operating system.
				/// This member can be one of the ProcessorArchitecture values.
				/// </summary>
				public ProcessorArchitecture processorArchitecture;

				/// <summary>
				/// This member is reserved for future use.
				/// </summary>
				private const ushort reserved = 0;

				/// <summary>
				/// The page size and the granularity of page protection and commitment.
				/// This is the page size used by the VirtualAlloc function.
				/// </summary>
				public uint pageSize;

				/// <summary>
				/// A pointer to the lowest memory address accessible to applications
				/// and dynamic-link libraries (DLLs).
				/// </summary>
				public IntPtr minimumApplicationAddress;

				/// <summary>
				/// A pointer to the highest memory address accessible to applications
				/// and DLLs.
				/// </summary>
				public IntPtr maximumApplicationAddress;

				/// <summary>
				/// A mask representing the set of processors configured into the system.
				/// Bit 0 is processor 0; bit 31 is processor 31.
				/// </summary>
				public IntPtr activeProcessorMask;

				/// <summary>
				/// The number of processors in the system.
				/// </summary>
				public uint numberOfProcessors;

				/// <summary>
				/// An obsolete member that is retained for compatibility. Use the
				/// wProcessorArchitecture, wProcessorLevel, and wProcessorRevision
				/// members to determine the type of processor.
				/// Name						Value
				/// PROCESSOR_INTEL_386			386
				/// PROCESSOR_INTEL_486			486
				/// PROCESSOR_INTEL_PENTIUM		586
				/// PROCESSOR_INTEL_IA64		2200
				/// PROCESSOR_AMD_X8664			8664
				/// </summary>
				public uint processorType;

				/// <summary>
				/// The granularity for the starting address at which virtual memory
				/// can be allocated. For more information, see VirtualAlloc.
				/// </summary>
				public uint allocationGranularity;

				/// <summary>
				/// The architecture-dependent processor level. It should be used only
				/// for display purposes. To determine the feature set of a processor,
				/// use the IsProcessorFeaturePresent function.
				/// 
				/// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_INTEL, wProcessorLevel
				/// is defined by the CPU vendor.
				/// If wProcessorArchitecture is PROCESSOR_ARCHITECTURE_IA64, wProcessorLevel
				/// is set to 1.
				/// </summary>
				public ushort processorLevel;

				/// <summary>
				/// The architecture-dependent processor revision. The following table
				/// shows how the revision value is assembled for each type of
				/// processor architecture.
				/// 
				/// Processor					Value
				/// Intel Pentium, Cyrix		The high byte is the model and the
				/// or NextGen 586				low byte is the stepping. For example,
				///								if the value is xxyy, the model number
				///								and stepping can be displayed as follows:
				///								Model xx, Stepping yy
				///	Intel 80386 or 80486		A value of the form xxyz.
				///								If xx is equal to 0xFF, y - 0xA is the model
				///								number, and z is the stepping identifier.
				/// 
				///								If xx is not equal to 0xFF, xx + 'A'
				///								is the stepping letter and yz is the minor stepping.
				/// </summary>
				public ushort processorRevision;
			}

			[DllImport("Kernel32.dll")]
			public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

			/// <summary>
			/// Execution state values to be used in conjuction with SetThreadExecutionState
			/// </summary>
			[Flags]
			public enum EXECUTION_STATE : uint
			{
				ES_AWAYMODE_REQUIRED = 0x00000040,
				ES_CONTINUOUS = 0x80000000,
				ES_DISPLAY_REQUIRED = 0x00000002,
				ES_SYSTEM_REQUIRED = 0x00000001,
				ES_USER_PRESENT = 0x00000004
			}

			[DllImport("Kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool AllocConsole();

			[DllImport("Kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool FreeConsole();
		}
	}

	public enum ThreadExecutionState
	{
		/// <summary>
		/// No specific state
		/// </summary>
		None = 0,

		/// <summary>
		/// Enables away mode. This value must be specified with ES_CONTINUOUS.
		/// 
		/// Away mode should be used only by media-recording and media-distribution
		/// applications that must perform critical background processing on
		/// desktop computers while the computer appears to be sleeping.
		/// See remarks.
		/// 
		/// Windows Server 2003 and Windows XP/2000: ES_AWAYMODE_REQUIRED is
		/// not supported.
		/// </summary>
		AwayModeRequired = (int)KernelAPI.NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED,

		/// <summary>
		/// Informs the system that the state being set should remain in effect
		/// until the next call that uses ES_CONTINUOUS and one of the other
		/// state flags is cleared.
		/// </summary>
		Continuous = unchecked((int)KernelAPI.NativeMethods.EXECUTION_STATE.ES_CONTINUOUS),

		/// <summary>
		/// Forces the display to be on by resetting the display idle timer.
		/// </summary>
		DisplayRequired = (int)KernelAPI.NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED,

		/// <summary>
		/// Forces the system to be in the working state by resetting the system
		/// idle timer.
		/// </summary>
		SystemRequired = (int)KernelAPI.NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED,

		/// <summary>
		/// This value is not supported. If ES_USER_PRESENT is combined with
		/// other esFlags values, the call will fail and none of the specified
		/// states will be set.
		/// 
		/// Windows Server 2003 and Windows XP/2000: Informs the system that a
		/// user is present and resets the display and system idle timers.
		/// ES_USER_PRESENT must be called with ES_CONTINUOUS.
		/// </summary>
		UserPresent = (int)KernelAPI.NativeMethods.EXECUTION_STATE.ES_USER_PRESENT
	}
}
