/* 
 * $Id$
 * Copyright 2008-2010 The Eraser Project
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
using System.Globalization;

using Eraser.Util;
using Eraser.Plugins;
using Eraser.Plugins.ExtensionPoints;

namespace Eraser.Manager
{
	/// <summary>
	/// Presents an opaque type for the management of the Manager settings.
	/// </summary>
	public class ManagerSettings
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="settings">The Persistent Store for this object.</param>
		public ManagerSettings(PersistentStore store)
		{
			Store = store;
		}

		/// <summary>
		/// Whether files which are locked when being erased should be forcibly
		/// unlocked for erasure.
		/// </summary>
		public bool ForceUnlockLockedFiles
		{
			get
			{
				return Store.GetValue("ForceUnlockLockedFiles", true);
			}
			set
			{
				Store.SetValue("ForceUnlockLockedFiles", value);
			}
		}

		/// <summary>
		/// Whether missed tasks should be run when the program next starts.
		/// </summary>
		public bool ExecuteMissedTasksImmediately
		{
			get
			{
				return Store.GetValue("ExecuteMissedTasksImmediately", true);
			}
			set
			{
				Store.SetValue("ExecuteMissedTasksImmediately", value);
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
				return Store.GetValue("PlausibleDeniability", false);
			}
			set
			{
				Store.SetValue("PlausibleDeniability", value);
			}
		}

		/// <summary>
		/// The files which are overwritten with when a file has been erased.
		/// </summary>
		public IList<string> PlausibleDeniabilityFiles
		{
			get
			{
				return Store.GetValue("PlausibleDeniabilityFiles", new List<string>());
			}
		}

		/// <summary>
		/// Holds user decisions on whether the plugin will be loaded at the next
		/// start up.
		/// </summary>
		public IDictionary<Guid, bool> PluginApprovals
		{
			get
			{
				return Store.GetValue("ApprovedPlugins", new Dictionary<Guid, bool>());
			}
		}

		/// <summary>
		/// The Persistent Store behind all these settings.
		/// </summary>
		private PersistentStore Store;
	}
}
