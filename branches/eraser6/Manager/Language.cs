/* 
 * $Id: Plugin.cs 345 2008-04-02 12:58:48Z lowjoel $
 * Copyright 2008 The Eraser Project
 * Original Author: 
 * Modified By: Joel Low <lowjoel@users.sourceforge.net>
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
using System.Globalization;
using System.Reflection;
using System.IO;

namespace Eraser.Manager
{
	/// <summary>
	/// Basic language class holding the language-related subset of the CultureInfo class
	/// </summary>
	public class Language
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="info">The culture information structure to retrieve language
		/// information from.</param>
		public Language(CultureInfo info)
		{
			culture = info;
		}

		public override string ToString()
		{
			return culture.NativeName;
		}

		/// <summary>
		/// Gets the culture name in the format "<languagecode2>-<country/regioncode2>".
		/// 
		/// The culture name in the format "<languagecode2>-<country/regioncode2>", where
		/// <languagecode2> is a lowercase two-letter code derived from ISO 639-1 and
		/// <country/regioncode2> is an uppercase two-letter code derived from ISO 3166.
		/// </summary>
		public string Name
		{
			get { return culture.Name; }
		}

		/// <summary>
		/// Gets the culture name in the format "<languagefull> (<country/regionfull>)"
		/// in the language of the localized version of .NET Framework.
		/// 
		/// The culture name in the format "<languagefull> (<country/regionfull>)" in
		/// the language of the localized version of .NET Framework, where <languagefull>
		/// is the full name of the language and <country/regionfull> is the full name
		/// of the country/region.
		/// </summary>
		public string DisplayName
		{
			get { return culture.DisplayName; }
		}


		/// <summary>
		/// Gets the culture name in the format "<languagefull> (<country/regionfull>)"
		/// in English.
		/// 
		/// The culture name in the format "<languagefull> (<country/regionfull>)" in
		/// English, where <languagefull> is the full name of the language and <country/regionfull>
		/// is the full name of the country/region.
		/// </summary>
		public string EnglishName
		{
			get { return culture.EnglishName; }
		}

		CultureInfo culture;
	}

	/// <summary>
	/// A class managing all plugins dealing with languages.
	/// </summary>
	public class LanguageManager
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LanguageManager()
		{
		}

		/// <summary>
		/// Retrieves all present language plugins
		/// </summary>
		/// <returns>A list, with an instance of each Language class</returns>
		public static List<Language> GetAll()
		{
			List<Language> result = new List<Language>();
			foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				if (info.Name == string.Empty)
					continue;
				else if (new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +
					Path.DirectorySeparatorChar + info.Name).Exists)
					result.Add(new Language(info));
			}

			//Last resort
			if (result.Count == 0)
				result.Add(new Language(CultureInfo.GetCultureInfo("EN")));
			return result;
		}
	}
}
