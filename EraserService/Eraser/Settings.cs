/* 
 * $Id$
 * Copyright 2008-2012 The Eraser Project
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
using System.Linq;

using System.Globalization;

using Eraser.Util;
using Eraser.Plugins;

namespace Eraser
{
	internal class EraserSettings
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private EraserSettings()
		{
			settings = Eraser.Service.Settings.Get();
		}

		/// <summary>
		/// Gets the singleton instance of the Eraser UI Settings.
		/// </summary>
		/// <returns>The global instance of the Eraser UI settings.</returns>
		public static EraserSettings Get()
		{
			if (instance == null)
				instance = new EraserSettings();
			return instance;
		}

		/// <summary>
		/// Gets or sets the LCID of the language which the UI should be displayed in.
		/// </summary>
		public string Language
		{
			get
			{
				return settings.GetValue("Language", GetCurrentCulture().Name);
			}
			set
			{
				settings.SetValue("Language", value);
			}
		}

		/// <summary>
		/// Gets or sets whether the Shell Extension should be loaded into Explorer.
		/// </summary>
		public bool IntegrateWithShell
		{
			get
			{
				return settings.GetValue("IntegrateWithShell", true);
			}
			set
			{
				settings.SetValue("IntegrateWithShell", value);
			}
		}

		/// <summary>
		/// Gets or sets a value on whether the main frame should be minimised to the
		/// system notification area.
		/// </summary>
		public bool HideWhenMinimised
		{
			get
			{
				return settings.GetValue("HideWhenMinimised", true);
			}
			set
			{
				settings.SetValue("HideWhenMinimised", value);
			}
		}

		/// <summary>
		/// Gets ot setts a value whether tasks which were completed successfully
		/// should be removed by the Eraser client.
		/// </summary>
		public bool ClearCompletedTasks
		{
			get
			{
				return settings.GetValue("ClearCompletedTasks", true);
			}
			set
			{
				settings.SetValue("ClearCompletedTasks", value);
			}
		}

		/// <summary>
		/// Gets the most specific UI culture with a localisation available, defaulting to English
		/// if none exist.
		/// </summary>
		/// <returns>The CultureInfo of the current UI culture, correct to the top level.</returns>
		private static CultureInfo GetCurrentCulture()
		{
			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
			CultureInfo culture = CultureInfo.CurrentUICulture;
			while (culture.Parent != CultureInfo.InvariantCulture &&
				!Localisation.LocalisationExists(culture, entryAssembly))
			{
				culture = culture.Parent;
			}

			//Default to English if any of our cultures don't exist.
			if (!Localisation.LocalisationExists(culture, entryAssembly))
				culture = new CultureInfo("en");

			return culture;
		}

		/// <summary>
		/// The data store behind the object.
		/// </summary>
		private PersistentStore settings;

		/// <summary>
		/// The global instance of the settings class.
		/// </summary>
		private static EraserSettings instance;
	}
}
