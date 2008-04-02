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

using GNU.Gettext;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;

namespace Eraser.Util
{
	/// <summary>
	/// GetText shortcut class. Instead of calling GetString on all strings, just
	/// call S._(string) or S._(string, object) for plurals
	/// </summary>
	public static class S
	{
		/// <summary>
		/// Translates the localizable string to the set localized string.
		/// </summary>
		/// <param name="str">The string to localize.</param>
		/// <returns>A localized string, or str if no localization exists.</returns>
		public static string _(string str)
		{
			return TranslateText(str, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Translates the localizable string to the set localized string.
		/// </summary>
		/// <param name="str">The string to localize.</param>
		/// <param name="assembly">The assembly from which localized resource satellite
		/// assemblies should be loaded from.</param>
		/// <returns>A localized string, or str if no localization exists.</returns>
		public static string TranslateText(string str, Assembly assembly)
		{
			//If the string is empty, forget it!
			if (str.Length == 0)
				return str;

			GettextResourceManager res = null;
			if (!managers.ContainsKey(assembly))
			{
				res = new GettextResourceManager(
					Path.GetFileNameWithoutExtension(assembly.Location), assembly);
				managers[assembly] = res;
			}
			else
				res = managers[assembly];

			return res.GetString(str, Language);
		}

		/// <summary>
		/// Translates a form control 
		/// </summary>
		/// <param name="c">The control to translate. Certain classes will be dealt with
		/// individually.</param>
		public static void TranslateControl(Control c)
		{
			TranslateControl(c, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Translates a menu
		/// </summary>
		/// <param name="c">The control to translate. Certain classes will be dealt with
		/// individually.</param>
		public static void TranslateControl(Menu c)
		{
			TranslateMenu(c, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Translates a menu
		/// </summary>
		/// <param name="c">The control to translate. Certain classes will be dealt with
		/// individually.</param>
		public static void TranslateControl(ToolStripDropDown c)
		{
			TranslateMenu(c, Assembly.GetCallingAssembly());
		}

		private static void TranslateControl(Control c, Assembly assembly)
		{
			c.Text = TranslateText(c.Text, assembly);

			//Translate the menu
			if (c.ContextMenu != null)
				TranslateMenu(c.ContextMenu, assembly);
			if (c.ContextMenuStrip != null)
				TranslateMenu(c.ContextMenuStrip, assembly);

			if (c is TranslatableControl)
				((TranslatableControl)c).Translate();
			else if (c is ListView)
			{
				ListView lv = (ListView)c;
				foreach (ListViewGroup group in lv.Groups)
					group.Header = TranslateText(group.Header, assembly);
				foreach (ColumnHeader header in lv.Columns)
					header.Text = TranslateText(header.Text, assembly);
			}

			foreach (Control child in c.Controls)
				TranslateControl(child, assembly);
		}

		private static void TranslateMenu(Menu m, Assembly assembly)
		{
			foreach (MenuItem item in m.MenuItems)
				if (item.IsParent)
					TranslateMenu((Menu)item, assembly);
		}

		private static void TranslateMenu(ToolStripDropDown m, Assembly assembly)
		{
			foreach (ToolStripItem item in m.Items)
				if (item is ToolStripMenuItem)
					((ToolStripMenuItem)item).Text = TranslateText(((ToolStripMenuItem)item).Text, assembly);
		}

		/// <summary>
		/// The current culture to use when looking up for localizations.
		/// </summary>
		public static CultureInfo Language = CultureInfo.CurrentUICulture;
		private static Dictionary<Assembly, GettextResourceManager> managers =
			new Dictionary<Assembly, GettextResourceManager>();

		/// <summary>
		/// Translatable control interface. Implement this interface to allow the
		/// control to be translated by gettext at runtime
		/// </summary>
		public interface TranslatableControl
		{
			/// <summary>
			/// Translates all strings in the control
			/// </summary>
			void Translate();
		}
	}
}
