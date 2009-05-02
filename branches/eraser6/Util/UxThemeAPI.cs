/* 
 * $Id: UserAPI.cs 461 2008-11-06 10:16:32Z lowjoel $
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Eraser.Util
{
	public static class UxThemeAPI
	{
		/// <summary>
		/// Updates the control's theme to fit in with the latest Windows visuals.
		/// </summary>
		/// <remarks>This function will also set the volume on all child controls.</remarks>
		public static void UpdateControlTheme(Control control)
		{
			if (control is ListView)
				UpdateControlTheme((ListView)control);
			else if (control is ContextMenuStrip)
				UpdateControlTheme((ContextMenuStrip)control);
			foreach (Control child in control.Controls)
				UpdateControlTheme(child);
		}

		/// <summary>
		/// Updates the control's theme to fit in with the latest Windows visuals.
		/// </summary>
		/// <param name="lv">The List View control to set the theme on.</param>
		public static void UpdateControlTheme(ListView lv)
		{
			try
			{
				SetWindowTheme(lv.Handle, "EXPLORER", null);
				SendMessage(lv.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, (UIntPtr)LVS_EX_DOUBLEBUFFER,
					(IntPtr)LVS_EX_DOUBLEBUFFER);
			}
			catch (DllNotFoundException)
			{
			}
		}

		/// <summary>
		/// Updates the control's theme to fit in with the latest Windows visuals.
		/// </summary>
		/// <param name="lv">The List View control to set the theme on.</param>
		public static void UpdateControlTheme(ContextMenuStrip lv)
		{
			if (Environment.OSVersion.Version.Major >= 6)
				lv.Renderer = new UxThemeMenuRenderer();
		}

		#region ListView double buffering
		/// <summary>
		/// Paints via double-buffering, which reduces flicker. This extended
		/// style also enables alpha-blended marquee selection on systems where
		/// it is supported.
		/// </summary>
		const uint LVS_EX_DOUBLEBUFFER = 0x00010000u;

		const uint LVM_FIRST = 0x1000;

		/// <summary>
		/// Sets extended styles in list-view controls.
		/// </summary>
		const uint LVM_SETEXTENDEDLISTVIEWSTYLE = (LVM_FIRST + 54);

		/// <summary>
		/// Sends the specified message to a window or windows. The SendMessage
		/// function calls the window procedure for the specified window and does
		/// not return until the window procedure has processed the message.
		/// </summary>
		/// <param name="hWnd">Handle to the window whose window procedure will
		/// receive the message. If this parameter is HWND_BROADCAST, the message
		/// is sent to all top-level windows in the system, including disabled
		/// or invisible unowned windows, overlapped windows, and pop-up windows;
		/// but the message is not sent to child windows.</param>
		/// <param name="Msg">Specifies the message to be sent.</param>
		/// <param name="wParam">Specifies additional message-specific information.</param>
		/// <param name="lParam">Specifies additional message-specific information.</param>
		[DllImport("User32.dll", SetLastError = true)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam,
			IntPtr lParam);
		#endregion

		/// <summary>
		/// Causes a window to use a different set of visual style information
		/// than its class normally uses.
		/// </summary>
		/// <param name="hwnd">Handle to the window whose visual style information
		/// is to be changed.</param>
		/// <param name="pszSubAppName">Pointer to a string that contains the
		/// application name to use in place of the calling application's name.
		/// If this parameter is NULL, the calling application's name is used.</param>
		/// <param name="pszSubIdList">Pointer to a string that contains a
		/// semicolon-separated list of class identifier (CLSID) names to use
		/// in place of the actual list passed by the window's class. If this
		/// parameter is NULL, the ID list from the calling class is used.</param>
		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern void SetWindowTheme(IntPtr hwnd, string pszSubAppName,
			string pszSubIdList);
	}

	public class UxThemeMenuRenderer : ToolStripRenderer
	{
		~UxThemeMenuRenderer()
		{
			CloseThemeData(hTheme);
		}

		protected override void Initialize(ToolStrip toolStrip)
		{
			base.Initialize(toolStrip);

			control = toolStrip;
			hTheme = OpenThemeData(toolStrip.Handle, "MENU");
		}

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			IntPtr hDC = e.Graphics.GetHdc();
			Rectangle rect = e.AffectedBounds;

			if (IsThemeBackgroundPartiallyTransparent(hTheme, (int)MENUPARTS.MENU_POPUPBACKGROUND, 0))
				DrawThemeParentBackground(control.Handle, hDC, ref rect);
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPBACKGROUND, 0, ref rect, ref rect);
			
			if (IsThemeBackgroundPartiallyTransparent(hTheme, (int)MENUPARTS.MENU_POPUPBORDERS, 0))
				DrawThemeParentBackground(control.Handle, hDC, ref rect);
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPBORDERS, 0, ref rect, ref rect);

			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			IntPtr hDC = e.Graphics.GetHdc();
			Rectangle rect = e.AffectedBounds;
			rect.Width = GutterWidth;
			rect.Inflate(-1, -1);
			rect.Offset(1, 0);

			if (IsThemeBackgroundPartiallyTransparent(hTheme, (int)MENUPARTS.MENU_POPUPGUTTER, 0))
				DrawThemeParentBackground(control.Handle, hDC, ref rect);
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPGUTTER, 0, ref rect, ref rect);
			
			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			ToolStripItem item = e.Item as ToolStripItem;

			Rectangle rect = Rectangle.Truncate(e.Graphics.VisibleClipBounds);
			rect.Inflate(-1, 0);
			rect.Offset(1, 0);
			IntPtr hDC = e.Graphics.GetHdc();

			int itemState = (int)(e.Item.Selected ?
				(e.Item.Enabled ? POPUPITEMSTATES.MPI_HOT : POPUPITEMSTATES.MPI_DISABLEDHOT) :
				(e.Item.Enabled ? POPUPITEMSTATES.MPI_NORMAL : POPUPITEMSTATES.MPI_DISABLED));
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPITEM, itemState, ref rect, ref rect);

			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			IntPtr hDC = e.Graphics.GetHdc();
			Rectangle rect = new Rectangle(GutterWidth, 0, e.Item.Width, e.Item.Height);
			rect.Inflate(4, 0);

			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPSEPARATOR, 0, ref rect, ref rect);

			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			if (!(e.Item is ToolStripMenuItem))
			{
				base.OnRenderItemCheck(e);
				return;
			}

			Rectangle imgRect = e.ImageRectangle;
			imgRect.Inflate(4, 3);
			imgRect.Offset(1, 0);
			Rectangle bgRect = imgRect;

			IntPtr hDC = e.Graphics.GetHdc();
			ToolStripMenuItem item = (ToolStripMenuItem)e.Item;
			
			int bgState = (int)(e.Item.Enabled ? POPUPCHECKBACKGROUNDSTATES.MCB_NORMAL:
				POPUPCHECKBACKGROUNDSTATES.MCB_DISABLED);
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPCHECKBACKGROUND, bgState,
				ref bgRect, ref bgRect);

			int checkState = (int)(item.Checked ?
				(item.Enabled ? POPUPCHECKSTATES.MC_CHECKMARKNORMAL : POPUPCHECKSTATES.MC_CHECKMARKDISABLED) : 0);
			if (IsThemeBackgroundPartiallyTransparent(hTheme, (int)MENUPARTS.MENU_POPUPCHECK, checkState))
				DrawThemeParentBackground(control.Handle, hDC, ref imgRect);
			DrawThemeBackground(hTheme, hDC, (int)MENUPARTS.MENU_POPUPCHECK, checkState,
				ref imgRect, ref imgRect);

			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			int itemState = (int)(e.Item.Selected ?
				(e.Item.Enabled ? POPUPITEMSTATES.MPI_HOT : POPUPITEMSTATES.MPI_DISABLEDHOT) :
				(e.Item.Enabled ? POPUPITEMSTATES.MPI_NORMAL : POPUPITEMSTATES.MPI_DISABLED));

			Rectangle rect = new Rectangle(e.TextRectangle.Left, 0,
				e.Item.Width - e.TextRectangle.Left, e.Item.Height);
			IntPtr hFont = e.TextFont.ToHfont();
			IntPtr hDC = e.Graphics.GetHdc();
			SelectObject(hDC, hFont);

			DrawThemeText(hTheme, hDC, (int)MENUPARTS.MENU_POPUPITEM, itemState, e.Text,
				-1, e.TextFormat | TextFormatFlags.WordEllipsis | TextFormatFlags.SingleLine, 0, ref rect);

			e.Graphics.ReleaseHdc();
		}

		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			base.OnRenderArrow(e);
		}

		private int GutterWidth
		{
			get
			{
				return 2 * (SystemInformation.MenuCheckSize.Width + SystemInformation.BorderSize.Width);
			}
		}

		private ToolStrip control;
		private IntPtr hTheme;

		#region Imported UxTheme functions and constants
		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr OpenThemeData(IntPtr hwnd, string pszClassList);

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr CloseThemeData(IntPtr hwndTeme);

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr DrawThemeParentBackground(IntPtr hwnd,
			IntPtr hdc, ref Rectangle prc);

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern bool IsThemeBackgroundPartiallyTransparent(IntPtr hTheme,
			int iPartId, int iStateId);

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr DrawThemeBackground(
			IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref Rectangle pRect,
			ref Rectangle pClipRect);

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		private extern static int DrawThemeText(IntPtr hTheme, IntPtr hDC, int iPartId,
			int iStateId, [MarshalAs(UnmanagedType.LPWStr)] string pszText, int iCharCount,
			TextFormatFlags dwTextFlag, int dwTextFlags2, ref Rectangle pRect);

		[DllImport("Gdi32.dll")]
		private extern static IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		private enum MENUPARTS
		{
			MENU_MENUITEM_TMSCHEMA = 1,
			MENU_MENUDROPDOWN_TMSCHEMA = 2,
			MENU_MENUBARITEM_TMSCHEMA = 3,
			MENU_MENUBARDROPDOWN_TMSCHEMA = 4,
			MENU_CHEVRON_TMSCHEMA = 5,
			MENU_SEPARATOR_TMSCHEMA = 6,
			MENU_BARBACKGROUND = 7,
			MENU_BARITEM = 8,
			MENU_POPUPBACKGROUND = 9,
			MENU_POPUPBORDERS = 10,
			MENU_POPUPCHECK = 11,
			MENU_POPUPCHECKBACKGROUND = 12,
			MENU_POPUPGUTTER = 13,
			MENU_POPUPITEM = 14,
			MENU_POPUPSEPARATOR = 15,
			MENU_POPUPSUBMENU = 16,
			MENU_SYSTEMCLOSE = 17,
			MENU_SYSTEMMAXIMIZE = 18,
			MENU_SYSTEMMINIMIZE = 19,
			MENU_SYSTEMRESTORE = 20,
		}

		private enum POPUPCHECKSTATES
		{
			MC_CHECKMARKNORMAL = 1,
			MC_CHECKMARKDISABLED = 2,
			MC_BULLETNORMAL = 3,
			MC_BULLETDISABLED = 4,
		}

		private enum POPUPCHECKBACKGROUNDSTATES
		{
			MCB_DISABLED = 1,
			MCB_NORMAL = 2,
			MCB_BITMAP = 3,
		}

		private enum POPUPITEMSTATES
		{
			MPI_NORMAL = 1,
			MPI_HOT = 2,
			MPI_DISABLED = 3,
			MPI_DISABLEDHOT = 4,
		}
		#endregion
	}
}
