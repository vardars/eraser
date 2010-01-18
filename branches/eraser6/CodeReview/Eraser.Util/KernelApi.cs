/* 
 * $Id$
 * Copyright 2008-2009 The Eraser Project
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
using System.ComponentModel;

namespace Eraser.Util
{
	public static class KernelApi
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

		private static DateTime FileTimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME value)
		{
			long time = (long)((((ulong)value.dwHighDateTime) << sizeof(int) * 8) |
				(uint)value.dwLowDateTime);
			return DateTime.FromFileTime(time);
		}

		private static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFileTime(DateTime value)
		{
			long time = value.ToFileTime();

			System.Runtime.InteropServices.ComTypes.FILETIME result =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			result.dwLowDateTime = (int)(time & 0xFFFFFFFFL);
			result.dwHighDateTime = (int)(time >> 32);

			return result;
		}

		/// <summary>
		/// Converts a Win32 Error code to a HRESULT.
		/// </summary>
		/// <param name="errorCode">The error code to convert.</param>
		/// <returns>A HRESULT value representing the error code.</returns>
		internal static int GetHRForWin32Error(int errorCode)
		{
			const uint FACILITY_WIN32 = 7;
			return errorCode <= 0 ? errorCode :
				(int)((((uint)errorCode) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
		}

		/// <summary>
		/// Gets a Exception for the given Win32 error code.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <returns>An exception object representing the error code.</returns>
		internal static Exception GetExceptionForWin32Error(int errorCode)
		{
			int HR = GetHRForWin32Error(errorCode);
			return Marshal.GetExceptionForHR(HR);
		}

		public static void GetFileTime(SafeFileHandle file, out DateTime creationTime,
			out DateTime accessedTime, out DateTime modifiedTime)
		{
			System.Runtime.InteropServices.ComTypes.FILETIME accessedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME modifiedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME createdTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();

			if (!NativeMethods.GetFileTime(file, out createdTimeNative, out accessedTimeNative,
				out modifiedTimeNative))
			{
				throw GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}

			creationTime = FileTimeToDateTime(createdTimeNative);
			accessedTime = FileTimeToDateTime(accessedTimeNative);
			modifiedTime = FileTimeToDateTime(modifiedTimeNative);
		}

		public static void SetFileTime(SafeFileHandle file, DateTime creationTime,
			DateTime accessedTime, DateTime modifiedTime)
		{
			System.Runtime.InteropServices.ComTypes.FILETIME accessedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME modifiedTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();
			System.Runtime.InteropServices.ComTypes.FILETIME createdTimeNative =
				new System.Runtime.InteropServices.ComTypes.FILETIME();

			if (!NativeMethods.GetFileTime(file, out createdTimeNative,
				out accessedTimeNative, out modifiedTimeNative))
			{
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}

			if (creationTime != DateTime.MinValue)
				createdTimeNative = DateTimeToFileTime(creationTime);
			if (accessedTime != DateTime.MinValue)
				accessedTimeNative = DateTimeToFileTime(accessedTime);
			if (modifiedTime != DateTime.MinValue)
				modifiedTimeNative = DateTimeToFileTime(modifiedTime);

			if (!NativeMethods.SetFileTime(file, ref createdTimeNative,
				ref accessedTimeNative, ref modifiedTimeNative))
			{
				throw KernelApi.GetExceptionForWin32Error(Marshal.GetLastWin32Error());
			}
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

		public class DiskPerformanceInfo
		{
			unsafe internal DiskPerformanceInfo(NativeMethods.DiskPerformanceInfoInternal info)
			{
				BytesRead = info.BytesRead;
				BytesWritten = info.BytesWritten;
				ReadTime = info.ReadTime;
				WriteTime = info.WriteTime;
				IdleTime = info.IdleTime;
				ReadCount = info.ReadCount;
				WriteCount = info.WriteCount;
				QueueDepth = info.QueueDepth;
				SplitCount = info.SplitCount;
				QueryTime = info.QueryTime;
				StorageDeviceNumber = info.StorageDeviceNumber;
				StorageManagerName = new string((char*)info.StorageManagerName);
			}

			public long BytesRead { get; private set; }
			public long BytesWritten { get; private set; }
			public long ReadTime { get; private set; }
			public long WriteTime { get; private set; }
			public long IdleTime { get; private set; }
			public uint ReadCount { get; private set; }
			public uint WriteCount { get; private set; }
			public uint QueueDepth { get; private set; }
			public uint SplitCount { get; private set; }
			public long QueryTime { get; private set; }
			public uint StorageDeviceNumber { get; private set; }
			public string StorageManagerName { get; private set; }
		}

		/// <summary>
		/// Queries the performance information for the given disk.
		/// </summary>
		/// <param name="diskHandle">A read-only handle to a device (disk).</param>
		/// <returns>A DiskPerformanceInfo structure describing the performance
		/// information for the given disk.</returns>
		public static DiskPerformanceInfo QueryDiskPerformanceInfo(SafeFileHandle diskHandle)
		{
			if (diskHandle.IsInvalid)
				throw new ArgumentException("The disk handle must not be invalid.");

			//This only works if the user has turned on the disk performance
			//counters with 'diskperf -y'. These counters are off by default
			NativeMethods.DiskPerformanceInfoInternal result =
				new NativeMethods.DiskPerformanceInfoInternal();
			uint bytesReturned = 0;
			if (NativeMethods.DeviceIoControl(diskHandle, NativeMethods.IOCTL_DISK_PERFORMANCE,
				IntPtr.Zero, 0, out result, (uint)Marshal.SizeOf(result), out bytesReturned, IntPtr.Zero))
			{
				return new DiskPerformanceInfo(result);
			}

			return null;
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
		AwayModeRequired = (int)NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED,

		/// <summary>
		/// Informs the system that the state being set should remain in effect
		/// until the next call that uses ES_CONTINUOUS and one of the other
		/// state flags is cleared.
		/// </summary>
		Continuous = unchecked((int)NativeMethods.EXECUTION_STATE.ES_CONTINUOUS),

		/// <summary>
		/// Forces the display to be on by resetting the display idle timer.
		/// </summary>
		DisplayRequired = (int)NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED,

		/// <summary>
		/// Forces the system to be in the working state by resetting the system
		/// idle timer.
		/// </summary>
		SystemRequired = (int)NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED,

		/// <summary>
		/// This value is not supported. If ES_USER_PRESENT is combined with
		/// other esFlags values, the call will fail and none of the specified
		/// states will be set.
		/// 
		/// Windows Server 2003 and Windows XP/2000: Informs the system that a
		/// user is present and resets the display and system idle timers.
		/// ES_USER_PRESENT must be called with ES_CONTINUOUS.
		/// </summary>
		UserPresent = (int)NativeMethods.EXECUTION_STATE.ES_USER_PRESENT
	}
}