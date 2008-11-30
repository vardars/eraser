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
		/// Retrieves the process identifier of the calling process.
		/// </summary>
		/// <returns>The return value is the process identifier of the calling
		/// process.</returns>
		[DllImport("Kernel32.dll")]
		public static extern uint GetCurrentProcessId();

		/// <summary>
		/// Retrieves a pseudo handle for the calling thread.
		/// </summary>
		/// <returns>The return value is a pseudo handle for the current thread.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentThread();

		/// <summary>
		/// Retrieves the thread identifier of the calling thread.
		/// </summary>
		/// <returns>The return value is the thread identifier of the calling
		/// thread.</returns>
		[DllImport("Kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		/// <summary>
		/// Retrieves a handle to the heap of the calling process. This handle
		/// can then be used in subsequent calls to the heap functions.
		/// </summary>
		/// <returns>If the function succeeds, the return value is a handle to
		/// the calling process's heap.
		/// 
		/// If the function fails, the return value is NULL. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetProcessHeap();

		/// <summary>
		/// Retrieves timing information for the specified process.
		/// </summary>
		/// <param name="hThread">A handle to the process whose timing information
		/// is sought. This handle must have the PROCESS_QUERY_INFORMATION access
		/// right. For more information, see Thread Security and Access Rights.</param>
		/// <param name="lpCreationTime">A reference to a long that receives the
		/// creation time of the thread.</param>
		/// <param name="lpExitTime">A reference to a long that receives the exit time
		/// of the thread. If the thread has not exited, the value of field is
		/// undefined.</param>
		/// <param name="lpKernelTime">A reference to a long that receives the
		/// amount of time that the thread has executed in kernel mode.</param>
		/// <param name="lpUserTime">A reference to a long that receives the amount
		/// of time that the thread has executed in user mode.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetProcessTimes(IntPtr hThread, out long lpCreationTime,
		   out long lpExitTime, out long lpKernelTime, out long lpUserTime);

		/// <summary>
		/// Retrieves the contents of the STARTUPINFO structure that was specified
		/// when the calling process was created.
		/// </summary>
		/// <param name="lpStartupInfo">A pointer to a STARTUPINFO structure that
		/// receives the startup information.</param>
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern void GetStartupInfo(out STARTUPINFO lpStartupInfo);

		/// <summary>
		/// Retrieves information about the current system.
		/// </summary>
		/// <param name="lpSystemInfo">A pointer to a SYSTEM_INFO structure that
		/// receives the information.</param>
		[DllImport("Kernel32.dll")]
		public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

		/// <summary>
		/// Retrieves timing information for the specified thread.
		/// </summary>
		/// <param name="hThread">A handle to the thread whose timing information
		/// is sought. This handle must have the THREAD_QUERY_INFORMATION access
		/// right. For more information, see Thread Security and Access Rights.</param>
		/// <param name="lpCreationTime">A reference to a long that receives the
		/// creation time of the thread.</param>
		/// <param name="lpExitTime">A reference to a long that receives the exit time
		/// of the thread. If the thread has not exited, the value of field is
		/// undefined.</param>
		/// <param name="lpKernelTime">A reference to a long that receives the
		/// amount of time that the thread has executed in kernel mode.</param>
		/// <param name="lpUserTime">A reference to a long that receives the amount
		/// of time that the thread has executed in user mode.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// 
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime,
		   out long lpExitTime, out long lpKernelTime, out long lpUserTime);

		/// <summary>
		/// Retrieves the number of milliseconds that have elapsed since the system
		/// was started, up to 49.7 days.
		/// </summary>
		/// <returns>The return value is the number of milliseconds that have elapsed
		/// since the system was started.</returns>
		/// <remarks>The resolution is limited to the resolution of the system timer.
		/// This value is also affected by adjustments made by the GetSystemTimeAdjustment
		/// function.
		/// 
		/// The elapsed time is stored as a DWORD value. Therefore, the time will
		/// wrap around to zero if the system is run continuously for 49.7 days.
		/// To avoid this problem, use GetTickCount64. Otherwise, check for an
		/// overflow condition when comparing times.
		/// 
		/// If you need a higher resolution timer, use a multimedia timer or a
		/// high-resolution timer.
		/// 
		/// To obtain the time elapsed since the computer was started, retrieve
		/// the System Up Time counter in the performance data in the registry key
		/// HKEY_PERFORMANCE_DATA. The value returned is an 8-byte value. For more
		/// information, see Performance Counters.</remarks>
		[DllImport("Kernel32.dll")]
		public static extern uint GetTickCount();

		/// <summary>
		/// Retrieves information about the system's current usage of both physical
		/// and virtual memory.
		/// </summary>
		/// <param name="lpBuffer">A pointer to a MEMORYSTATUSEX structure that
		/// receives information about current memory availability.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended
		/// error information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

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
		/// Contains information about the current state of both physical and
		/// virtual memory, including extended memory. The GlobalMemoryStatusEx
		/// function stores information in this structure.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct MEMORYSTATUSEX
		{
			/// <summary>
			/// The size of the structure, in bytes. You must set this member
			/// before calling GlobalMemoryStatusEx.
			/// </summary>
			public uint dwLength;

			/// <summary>
			/// A number between 0 and 100 that specifies the approximate percentage
			/// of physical memory that is in use (0 indicates no memory use and
			/// 100 indicates full memory use).
			/// </summary>
			public uint dwMemoryLoad;

			/// <summary>
			/// The amount of actual physical memory, in bytes.
			/// </summary>
			public ulong ullTotalPhys;

			/// <summary>
			/// The amount of physical memory currently available, in bytes. This
			/// is the amount of physical memory that can be immediately reused
			/// without having to write its contents to disk first. It is the sum
			/// of the size of the standby, free, and zero lists.
			/// </summary>
			public ulong ullAvailPhys;

			/// <summary>
			/// The current committed memory limit for the system or the current
			/// process, whichever is smaller, in bytes. To get the system-wide
			/// committed memory limit, call GetPerformanceInfo.
			/// </summary>
			public ulong ullTotalPageFile;

			/// <summary>
			/// The maximum amount of memory the current process can commit, in
			/// bytes. This value is equal to or smaller than the system-wide
			/// available commit value. To calculate the system-wide available
			/// commit value, call GetPerformanceInfo and subtract the value of
			/// CommitTotal from the value of CommitLimit.
			/// </summary>
			public ulong ullAvailPageFile;

			/// <summary>
			/// The size of the user-mode portion of the virtual address space of
			/// the calling process, in bytes. This value depends on the type of
			/// process, the type of processor, and the configuration of the
			/// operating system. For example, this value is approximately 2 GB
			/// for most 32-bit processes on an x86 processor and approximately
			/// 3 GB for 32-bit processes that are large address aware running on
			/// a system with 4-gigabyte tuning enabled.
			/// </summary>
			public ulong ullTotalVirtual;

			/// <summary>
			/// The amount of unreserved and uncommitted memory currently in the
			/// user-mode portion of the virtual address space of the calling
			/// process, in bytes.
			/// </summary>
			public ulong ullAvailVirtual;

			/// <summary>
			/// Reserved. This value is always 0.
			/// </summary>
			private ulong ullAvailExtendedVirtual;
		}

		/// <summary>
		/// Contains information about the current computer system. This includes
		/// the architecture and type of the processor, the number of processors
		/// in the system, the page size, and other such information.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SYSTEM_INFO
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

		/// <summary>
		/// Specifies the window station, desktop, standard handles, and appearance
		/// of the main window for a process at creation time.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct STARTUPINFO
		{
			/// <summary>
			/// The size of the structure, in bytes.
			/// </summary>
			private uint cb;

			/// <summary>
			/// Reserved; must be NULL.
			/// </summary>
			private readonly IntPtr lpReserved;

			/// <summary>
			/// The name of the desktop, or the name of both the desktop and window
			/// station for this process. A backslash in the string indicates that
			/// the string includes both the desktop and window station names.
			/// For more information, see Thread Connection to a Desktop.
			/// </summary>
			public IntPtr lpDesktop;

			/// <summary>
			/// For console processes, this is the title displayed in the title
			/// bar if a new console window is created. If NULL, the name of the
			/// executable file is used as the window title instead. This parameter
			/// must be NULL for GUI or console processes that do not create a
			/// new console window.
			/// </summary>
			public string lpTitle;

			/// <summary>
			/// If dwFlags specifies STARTF_USEPOSITION, this member is the x
			/// offset of the upper left corner of a window if a new window is
			/// created, in pixels. Otherwise, this member is ignored.
			/// 
			/// The offset is from the upper left corner of the screen. For GUI
			/// processes, the specified position is used the first time the
			/// new process calls CreateWindow to create an overlapped window
			/// if the x parameter of CreateWindow is CW_USEDEFAULT.
			/// </summary>
			public int dwX;

			/// <summary>
			/// If dwFlags specifies STARTF_USEPOSITION, this member is the y
			/// offset of the upper left corner of a window if a new window is
			/// created, in pixels. Otherwise, this member is ignored.
			/// 
			/// The offset is from the upper left corner of the screen. For GUI
			/// processes, the specified position is used the first time the new
			/// process calls CreateWindow to create an overlapped window if the
			/// y parameter of CreateWindow is CW_USEDEFAULT.
			/// </summary>
			public int dwY;

			/// <summary>
			/// If dwFlags specifies STARTF_USESIZE, this member is the width of
			/// the window if a new window is created, in pixels. Otherwise,
			/// this member is ignored.
			/// 
			/// For GUI processes, this is used only the first time the new process
			/// calls CreateWindow to create an overlapped window if the nWidth 
			/// parameter of CreateWindow is CW_USEDEFAULT.
			/// </summary>
			public int dwXSize;

			/// <summary>
			/// If dwFlags specifies STARTF_USESIZE, this member is the height of
			/// the window if a new window is created, in pixels. Otherwise, this
			/// member is ignored.
			/// 
			/// For GUI processes, this is used only the first time the new process
			/// calls CreateWindow to create an overlapped window if the nHeight
			/// parameter of CreateWindow is CW_USEDEFAULT.
			/// </summary>
			public int dwYSize;

			/// <summary>
			/// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window
			/// is created in a console process, this member specifies the screen
			/// buffer width, in character columns. Otherwise, this member is ignored.
			/// </summary>
			public int dwXCountChars;

			/// <summary>
			/// If dwFlags specifies STARTF_USECOUNTCHARS, if a new console window
			/// is created in a console process, this member specifies the screen
			/// buffer height, in character rows. Otherwise, this member is ignored.
			/// </summary>
			public int dwYCountChars;

			/// <summary>
			/// If dwFlags specifies STARTF_USEFILLATTRIBUTE, this member is the
			/// initial text and background colors if a new console window is
			/// created in a console application. Otherwise, this member is ignored.
			/// 
			/// This value can be any combination of the following values:
			/// FOREGROUND_BLUE, FOREGROUND_GREEN, FOREGROUND_RED, FOREGROUND_INTENSITY,
			/// BACKGROUND_BLUE, BACKGROUND_GREEN, BACKGROUND_RED, and BACKGROUND_INTENSITY.
			/// 
			/// For example, the following combination of values produces red text
			/// on a white background:
			///		FOREGROUND_RED| BACKGROUND_RED| BACKGROUND_GREEN| BACKGROUND_BLUE
			/// </summary>
			public int dwFillAttribute;

			/// <summary>
			/// A bit field that determines whether certain STARTUPINFO members
			/// are used when the process creates a window.
			/// </summary>
			public int dwFlags;

			/// <summary>
			/// If dwFlags specifies STARTF_USESHOWWINDOW, this member can be any
			/// of the SW_ constants defined in Winuser.h. Otherwise, this member is ignored.
			/// 
			/// For GUI processes, wShowWindow specifies the default value the
			/// first time ShowWindow is called. The nCmdShow parameter of ShowWindow
			/// is ignored. In subsequent calls to ShowWindow, the wShowWindow member
			/// is used if the nCmdShow parameter of ShowWindow is set to SW_SHOWDEFAULT.
			/// </summary>
			public short wShowWindow;

			/// <summary>
			/// Reserved for use by the C Run-time; must be zero.
			/// </summary>
			private const short cbReserved2 = 0;

			/// <summary>
			/// Reserved for use by the C Run-time; must be NULL.
			/// </summary>
			private readonly IntPtr lpReserved2;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the
			/// standard input handle for the process. Otherwise, this member is
			/// ignored and the default for standard input is the keyboard buffer.
			/// </summary>
			public IntPtr hStdInput;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the
			/// standard output handle for the process. Otherwise, this member is
			/// ignored and the default for standard output is the console window's
			/// buffer.
			/// </summary>
			public IntPtr hStdOutput;

			/// <summary>
			/// If dwFlags specifies STARTF_USESTDHANDLES, this member is the
			/// standard error handle for the process. Otherwise, this member is
			/// ignored and the default for standard error is the console window's
			/// buffer.
			/// </summary>
			public IntPtr hStdError;
		}

		/// <summary>
		/// Enables an application to inform the system that it is in use, thereby
		/// preventing the system from entering sleep or turning off the display
		/// while the application is running.
		/// </summary>
		/// <param name="esFlags">The thread's execution requirements. This parameter
		/// can be one or more of the EXECUTION_STATE values.</param>
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
		[DllImport("Kernel32.dll")]
		public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

		/// <summary>
		/// Execution state values to be used in conjuction with SetThreadExecutionState
		/// </summary>
		public enum EXECUTION_STATE : uint
		{
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
			ES_AWAYMODE_REQUIRED = 0x00000040,
	
			/// <summary>
			/// Informs the system that the state being set should remain in effect
			/// until the next call that uses ES_CONTINUOUS and one of the other
			/// state flags is cleared.
			/// </summary>
			ES_CONTINUOUS = 0x80000000,

			/// <summary>
			/// Forces the display to be on by resetting the display idle timer.
			/// </summary>
			ES_DISPLAY_REQUIRED = 0x00000002,

			/// <summary>
			/// Forces the system to be in the working state by resetting the system
			/// idle timer.
			/// </summary>
			ES_SYSTEM_REQUIRED = 0x00000001,
	
			/// <summary>
			/// This value is not supported. If ES_USER_PRESENT is combined with
			/// other esFlags values, the call will fail and none of the specified
			/// states will be set.
			/// 
			/// Windows Server 2003 and Windows XP/2000: Informs the system that a
			/// user is present and resets the display and system idle timers.
			/// ES_USER_PRESENT must be called with ES_CONTINUOUS.
			/// </summary>
			ES_USER_PRESENT = 0x00000004
		}

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
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool AllocConsole();

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
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool FreeConsole();
	}
}
