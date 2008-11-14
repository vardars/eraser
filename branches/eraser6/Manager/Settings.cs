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
using System.Reflection;
using System.Runtime.InteropServices;

namespace Eraser.Manager
{
	public abstract class SettingsManager
	{
		/// <summary>
		/// Saves all the settings to persistent storage.
		/// </summary>
		public abstract void Save();

		/// <summary>
		/// Gets the dictionary holding settings for the calling assembly.
		/// </summary>
		public Settings ModuleSettings
		{
			get
			{
				return GetSettings(new Guid(((GuidAttribute)Assembly.GetCallingAssembly().
					GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value));
			}
		}

		/// <summary>
		/// Gets the settings from the data source.
		/// </summary>
		/// <param name="guid">The GUID of the calling plugin</param>
		/// <returns>The Settings object which will act as the data store.</returns>
		protected abstract Settings GetSettings(Guid guid);
	}

	/// <summary>
	/// Settings class. Represents settings to a given client.
	/// </summary>
	public abstract class Settings
	{
		/// <summary>
		/// Gets the setting
		/// </summary>
		/// <param name="setting">The name of the setting.</param>
		/// <returns>The object stored in the settings database, or null if undefined.</returns>
		public abstract object this[string setting]
		{
			get;
			set;
		}

		/// <summary>
		/// The language which all user interface elements should be presented in.
		/// This is a GUID since languages are supplied through plugins.
		/// </summary>
		public string UILanguage
		{
			get
			{
				lock (this)
					return uiLanguage;
			}
			set
			{
				lock (this)
					uiLanguage = value;
			}
		}

		private string uiLanguage;
	}

	/// <summary>
	/// Handles the settings related to the Eraser Manager.
	/// </summary>
	public class ManagerSettings
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings">The Settings object which is the data store for
		/// this object.</param>
		public ManagerSettings()
		{
			settings = ManagerLibrary.Instance.SettingsManager.ModuleSettings;
		}

		/// <summary>
		/// The default file erasure method. This is a GUID since methods are
		/// implemented through plugins and plugins may not be loaded and missing
		/// references may follow.
		/// </summary>
		public Guid DefaultFileErasureMethod
		{
			get
			{
				return settings["DefaultFileErasureMethod"] == null ? Guid.Empty :
					(Guid)settings["DefaultFileErasureMethod"];
			}
			set
			{
				settings["DefaultFileErasureMethod"] = value;
			}
		}

		/// <summary>
		/// The default unused space erasure method. This is a GUID since methods
		/// are implemented through plugins and plugins may not be loaded and
		/// missing references may follow.
		/// </summary>
		public Guid DefaultUnusedSpaceErasureMethod
		{
			get
			{
				return settings["DefaultUnusedSpaceErasureMethod"] == null ? Guid.Empty :
					(Guid)settings["DefaultUnusedSpaceErasureMethod"];
			}
			set
			{
				settings["DefaultUnusedSpaceErasureMethod"] = value;
			}
		}

		/// <summary>
		/// The PRNG used. This is a GUID since PRNGs are implemented through
		/// plugins and plugins may not be loaded and missing references may follow.
		/// </summary>
		public Guid ActivePRNG
		{
			get
			{
				return settings["ActivePRNG"] == null ? Guid.Empty :
					(Guid)settings["ActivePRNG"];
			}
			set
			{
				settings["ActivePRNG"] = value;
			}
		}

		/// <summary>
		/// Whether files which are locked when being erased can be scheduled for
		/// erasure on system restart.
		/// </summary>
		public bool EraseLockedFilesOnRestart
		{
			get
			{
				return settings["EraseLockedFilesOnRestart"] == null ? true :
					(bool)settings["EraseLockedFilesOnRestart"];
			}
			set
			{
				settings["EraseLockedFilesOnRestart"] = value;
			}
		}

		/// <summary>
		/// Whether scheduling files for restart erase should get the blessing of
		/// the user first.
		/// </summary>
		public bool ConfirmEraseOnRestart
		{
			get
			{
				return settings["ConfirmEraseOnRestart"] == null ?
					true : (bool)settings["ConfirmEraseOnRestart"];
			}
			set
			{
				settings["ConfirmEraseOnRestart"] = value;
			}
		}

		/// <summary>
		/// Whether missed tasks should be run when the program next starts.
		/// </summary>
		public bool ExecuteMissedTasksImmediately
		{
			get
			{
				return settings["ExecuteMissedTasksImmediately"] == null ?
					true : (bool)settings["ExecuteMissedTasksImmediately"];
			}
			set
			{
				settings["ExecuteMissedTasksImmediately"] = value;
			}
		}

		/// <summary>
		/// Whether erasures should be run with plausible deniability. This is
		/// achieved by the executor copying files over the file to be removed
		/// before removing it.
		/// </summary>
		/// <seealso cref="PlausibleDeniabilityFiles"/>
		public bool PlausibleDeniability
		{
			get
			{
				return settings["PlausibleDeniability"] == null ? false :
					(bool)settings["PlausibleDeniability"];
			}
			set
			{
				settings["PlausibleDeniability"] = value;
			}
		}

		/// <summary>
		/// The files which are overwritten with when a file has been erased.
		/// </summary>
		public List<string> PlausibleDeniabilityFiles
		{
			get
			{
				return settings["PlausibleDeniabilityFiles"] == null ?
					new List<string>() :
					(List<string>)settings["PlausibleDeniabilityFiles"];
			}
			set
			{
				settings["PlausibleDeniabilityFiles"] = value;
			}
		}

		/// <summary>
		/// The Settings object which is the data store of this object.
		/// </summary>
		private Settings settings;
	}
}
