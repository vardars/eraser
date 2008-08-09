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

using System.IO;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;
using System.Threading;

namespace Eraser.Util
{
	/// <summary>
	/// Internationalisation class. Instead of calling GetString on all strings, just
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

			//First get the dictionary mapping assemblies and ResourceManagers (i.e. pick out
			//the dictionary with ResourceManagers representing the current culture.)
			if (!managers.ContainsKey(Thread.CurrentThread.CurrentUICulture))
				managers[Thread.CurrentThread.CurrentUICulture] =
					new Dictionary<Assembly, ResourceManager>();
			Dictionary<Assembly, ResourceManager> assemblies = managers[
				Thread.CurrentThread.CurrentUICulture];

			//Then look for the ResourceManager dealing with the calling assembly's
			//resources
			ResourceManager res = null;
			if (!assemblies.ContainsKey(assembly))
			{
				//Load the resource DLL. The resource DLL is located in the <LanguageName-RegionName>
				//subfolder of the folder containing the main assembly
				string languageID = string.Empty;
				Assembly languageAssembly = LoadLanguage(Path.GetDirectoryName(
					Assembly.GetEntryAssembly().Location), Thread.CurrentThread.CurrentUICulture,
					assembly, out languageID);

				//If we found the language assembly to load, then we load it directly, otherwise
				//fall back to the invariant culture.
				string resourceName = Path.GetFileNameWithoutExtension(assembly.Location) +
					".Strings" + (languageID.Length != 0 ? ("." + languageID) : "");
				res = new ResourceManager(resourceName,
					languageAssembly != null ? languageAssembly : assembly);
				assemblies[assembly] = res;
			}
			else
				res = assemblies[assembly];

			string result = res.GetString(Escape(str), Thread.CurrentThread.CurrentUICulture);
			return result == null ? str : Unescape(result);
		}

		/// <summary>
		/// Replaces non-printable codes used in the string to translate into translatable placeholders.
		/// </summary>
		/// <param name="str">The string to escape</param>
		/// <returns>An escaped string</returns>
		private static string Escape(string str)
		{
			str.Replace("\n", "\\n");
			str.Replace("\r", "\\r");
			return str;
		}

		/// <summary>
		/// Replaces all escape codes used in the translated string into real character codes.
		/// </summary>
		/// <param name="str">The string to unescape</param>
		/// <returns>An unescaped string</returns>
		private static string Unescape(string str)
		{
			str.Replace("\\n", "\n");
			str.Replace("\\r", "\r");
			return str;
		}

		/// <summary>
		/// Looks in the folder denoted by <paramref name="path"/> for the resource providing
		/// resources for <paramref name="culture"/>. The name of the resource DLL will be the
		/// culture name &gt;languagecode2-country/regioncode2&lt;.
		/// </summary>
		/// <param name="directory">The directory to look for assemblies in. Subfolders are not
		/// included.</param>
		/// <param name="culture">The culture to load.</param>
		/// <param name="assembly">The assembly to look for localised resources for.</param>
		/// <returns>An assembly containing the required resources, or null.</returns>
		private static Assembly LoadLanguage(string directory, CultureInfo culture, Assembly assembly,
			out string languageID)
		{
			languageID = string.Empty;
			string path = string.Empty;
			while (culture != CultureInfo.InvariantCulture)
			{
				path = Path.Combine(directory, culture.Name);
				if (System.IO.Directory.Exists(path))
				{
					string assemblyPath = Path.Combine(path,
						Path.GetFileNameWithoutExtension(assembly.Location) + ".resources.dll");
					if (System.IO.File.Exists(assemblyPath))
					{
						languageID = culture.Name;
						return Assembly.LoadFile(assemblyPath);
					}
				}
				culture = culture.Parent;
			}

			return null;
		}

		private static Dictionary<CultureInfo, Dictionary<Assembly, ResourceManager>> managers =
			new Dictionary<CultureInfo, Dictionary<Assembly, ResourceManager>>();
	}
}
