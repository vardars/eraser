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
	/// <summary>
	/// Settings class. Holds the defaults for the manager's operations.
	/// </summary>
	public abstract class Settings
	{
		/// <summary>
		/// Saves all the settings to any persistent storage.
		/// </summary>
		protected internal abstract void Save();

		/// <summary>
		/// Loads all settings from storage.
		/// </summary>
		protected internal abstract void Load();

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

		/// <summary>
		/// The default file erasure method. This is a GUID since methods are
		/// implemented through plugins and plugins may not be loaded and missing
		/// references may follow.
		/// </summary>
		public Guid DefaultFileErasureMethod
		{
			get
			{
				lock (this)
					return defaultFileErasureMethod;
			}
			set
			{
				lock (this)
					defaultFileErasureMethod = value;
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
				lock (this)
					return defaultUnusedSpaceErasureMethod;
			}
			set
			{
				lock (this)
					defaultUnusedSpaceErasureMethod = value;
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
				lock (this)
					return activePRNG;
			}
			set
			{
				lock (this)
					activePRNG = value;
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
				lock (this)
					return eraseLockedFilesOnRestart;
			}
			set
			{
				lock (this)
					eraseLockedFilesOnRestart = value;
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
				lock (this)
					return confirmEraseOnRestart;
			}
			set
			{
				lock (this)
					confirmEraseOnRestart = value;
			}
		}

		/// <summary>
		/// Whether missed tasks should be run when the program next starts.
		/// </summary>
		public bool ExecuteMissedTasksImmediately
		{
			get
			{
				lock (this)
					return executeMissedTasksImmediately;
			}
			set
			{
				lock (this)
					executeMissedTasksImmediately = value;
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
				lock (this)
					return plausibleDeniability;
			}
			set
			{
				lock (this)
					plausibleDeniability = value;
			}
		}

		/// <summary>
		/// The files which are overwritten with when a file has been erased.
		/// </summary>
		public List<string> PlausibleDeniabilityFiles
		{
			get
			{
				lock (this)
					return plausibleDeniabilityFiles;
			}
			set
			{
				lock (this)
					plausibleDeniabilityFiles = value;
			}
		}

		/// <summary>
		/// Retrieves the dictionary holding settings for the calling assembly.
		/// </summary>
		/// <returns>A dictionary holding settings for the plugin. This dictionary
		/// will be automatically saved when the program exits holding the settings
		/// permanenently. An empty dictionary will be returned if no settings
		/// currently exist.</returns>
		public Dictionary<string, object> GetSettings()
		{
			return GetSettings(new Guid(((GuidAttribute)Assembly.GetCallingAssembly().
				GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value));
		}

		/// <summary>
		/// Gets the settings from the data source.
		/// </summary>
		/// <param name="guid">The GUID of the calling plugin</param>
		/// <returns>The dictionary containing settings for the plugin</returns>
		protected abstract Dictionary<string, object> GetSettings(Guid guid);

		/// <summary>
		/// Sets the settings for the calling plugin.
		/// </summary>
		/// <param name="settings">The settings of the plugin</param>
		public void SetSettings(Dictionary<string, object> settings)
		{
			SetSettings(new Guid(((GuidAttribute)Assembly.GetCallingAssembly().
				GetCustomAttributes(typeof(GuidAttribute), false)[0]).Value), settings);
		}

		/// <summary>
		/// Saves the settings from the plugin into the data source.
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="settings"></param>
		protected abstract void SetSettings(Guid guid, Dictionary<string, object> settings);

		private string uiLanguage;
		private Guid defaultFileErasureMethod = Guid.Empty;
		private Guid defaultUnusedSpaceErasureMethod = Guid.Empty;
		private Guid activePRNG = Guid.Empty;
		private bool eraseLockedFilesOnRestart = true;
		private bool confirmEraseOnRestart = true;
		private bool executeMissedTasksImmediately = true;
		private bool plausibleDeniability = true;
		private List<string> plausibleDeniabilityFiles = new List<string>();
	}
}
