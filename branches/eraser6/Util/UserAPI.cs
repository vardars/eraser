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
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY
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

namespace Eraser.Util
{
	public static class UserAPI
	{
		/// <summary>
		/// The GetCapture function retrieves a handle to the window (if any) that
		/// has captured the mouse. Only one window at a time can capture the mouse;
		/// this window receives mouse input whether or not the cursor is within
		/// its borders.
		/// </summary>
		/// <returns></returns>
		[DllImport("User32.dll")]
		public static extern IntPtr GetCapture();

		/// <summary>
		/// The GetCaretPos function copies the caret's position to the specified
		/// POINT structure.
		/// </summary>
		/// <param name="lpPoint">[out] Pointer to the POINT structure that is to
		/// receive the client coordinates of the caret.</param>
		/// <returns>If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCaretPos(out POINT lpPoint);

		/// <summary>
		/// Retrieves the cursor's position, in screen coordinates.
		/// </summary>
		/// <param name="lpPoint">[out] Pointer to a POINT structure that receives
		/// the screen coordinates of the cursor.</param>
		/// <returns>Returns nonzero if successful or zero otherwise. To get
		/// extended error information, call GetLastError.</returns>
		[DllImport("User32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(out POINT lpPoint);

		/// <summary>
		/// The GetClipboardOwner function retrieves the window handle of the
		/// current owner of the clipboard.
		/// </summary>
		/// <returns>If the function succeeds, the return value is the handle to
		/// the window that owns the clipboard.
		/// 
		/// If the clipboard is not owned, the return value is NULL. To get
		/// extended error information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetClipboardOwner();

		/// <summary>
		/// The GetClipboardViewer function retrieves the handle to the first
		/// window in the clipboard viewer chain.
		/// </summary>
		/// <returns>If the function succeeds, the return value is the handle to
		/// the first window in the clipboard viewer chain.
		/// 
		/// If there is no clipboard viewer, the return value is NULL. To get
		/// extended error information, call Marshal.GetLastWin32Error. </returns>
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetClipboardViewer();

		/// <summary>
		/// The GetDesktopWindow function returns a handle to the desktop window.
		/// The desktop window covers the entire screen. The desktop window is
		/// the area on top of which other windows are painted.
		/// </summary>
		/// <returns>The return value is a handle to the desktop window.</returns>
		[DllImport("User32.dll")]
		public static extern IntPtr GetDesktopWindow();

		/// <summary>
		/// The GetForegroundWindow function returns a handle to the foreground
		/// window (the window with which the user is currently working). The
		/// system assigns a slightly higher priority to the thread that creates
		/// the foreground window than it does to other threads.
		/// </summary>
		/// <returns>The return value is a handle to the foreground window. The
		/// foreground window can be NULL in certain circumstances, such as when
		/// a window is losing activation.</returns>
		[DllImport("User32.dll")]
		public static extern IntPtr GetForegroundWindow();

		/// <summary>
		/// The GetMessagePos function retrieves the cursor position for the
		/// last message retrieved by the GetMessage function.
		/// </summary>
		/// <returns>The return value specifies the x- and y-coordinates of the
		/// cursor position. The x-coordinate is the low order short and the
		/// y-coordinate is the high-order short.</returns>
		[DllImport("User32.dll")]
		public static extern uint GetMessagePos();

		/// <summary>
		/// The GetMessageTime function retrieves the message time for the last
		/// message retrieved by the GetMessage function. The time is a long
		/// integer that specifies the elapsed time, in milliseconds, from the
		/// time the system was started to the time the message was created
		/// (that is, placed in the thread's message queue).
		/// </summary>
		/// <returns>The return value specifies the message time.</returns>
		[DllImport("User32.dll")]
		public static extern int GetMessageTime();

		/// <summary>
		/// The GetOpenClipboardWindow function retrieves the handle to the
		/// window that currently has the clipboard open.
		/// </summary>
		/// <returns>If the function succeeds, the return value is the handle to
		/// the window that has the clipboard open. If no window has the
		/// clipboard open, the return value is NULL. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetOpenClipboardWindow();

		/// <summary>
		/// Retrieves a handle to the current window station for the calling process.
		/// </summary>
		/// <returns>If the function succeeds, the return value is a handle to
		/// the window station.
		/// 
		/// If the function fails, the return value is NULL. To get extended error
		/// information, call Marshal.GetLastWin32Error.</returns>
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr GetProcessWindowStation();

		/// <summary>
		/// The POINT structure defines the x- and y- coordinates of a point.
		/// </summary>
		public struct POINT
		{
			/// <summary>
			/// Specifies the x-coordinate of the point.
			/// </summary>
			public int x;

			/// <summary>
			/// Specifies the y-coordinate of the point.
			/// </summary>
			public int y;
		}
	}
}
