using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// Settings class. Holds the defaults for the manager's operations.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// The language which all user interface elements should be presented in.
		/// This is a GUID since languages are supplied through plugins.
		/// </summary>
		public Guid UILanguage
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
		/// Retrieves the dictionary holding settings for the given plugin.
		/// </summary>
		/// <param name="plugin">The GUID of the plugin querying for settings</param>
		/// <returns>A dictionary holding settings for the plugin. This dictionary
		/// will be automatically saved when the program exits holding the settings
		/// permanenently. An empty dictionary will be returned if no settings
		/// currently exist.</returns>
		public Dictionary<string, object> GetSettings(Guid plugin)
		{
			lock (pluginSettings)
			{
				if (pluginSettings.ContainsKey(plugin))
					return pluginSettings[plugin];

				Dictionary<string, object> result = new Dictionary<string, object>();
				pluginSettings.Add(plugin, result);
				return result;
			}
		}

		private Guid uiLanguage = Guid.Empty;
		private Guid defaultFileErasureMethod = Guid.Empty;
		private Guid defaultUnusedSpaceErasureMethod = Guid.Empty;
		private Guid activePRNG = Guid.Empty;
		private bool eraseLockedFilesOnRestart = true;
		private bool confirmEraseOnRestart = true;
		private bool executeMissedTasksImmediately = true;
		private bool plausibleDeniability = true;

		protected Dictionary<Guid, Dictionary<string, object>> pluginSettings =
			new Dictionary<Guid, Dictionary<string, object>>();
	}
}
