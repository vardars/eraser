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
				//If the user did not define anything for this field, check all plugins
				//and use the method which was declared by us to be the highest
				//priority default
				Guid result = settings.GetValue<Guid>("DefaultFileErasureMethod");
				if (result == Guid.Empty)
					result = FindHighestPriorityDefault(typeof(ErasureMethod),
						typeof(DefaultFileErasureAttribute));
				if (result == Guid.Empty)
					result = new Guid("{1407FC4E-FEFF-4375-B4FB-D7EFBB7E9922}");

				return result;
			}
			set
			{
				settings.SetValue("DefaultFileErasureMethod", value);
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
				Guid result = settings.GetValue<Guid>("DefaultUnusedSpaceErasureMethod");
				if (result == Guid.Empty)
					result = FindHighestPriorityDefault(typeof(UnusedSpaceErasureMethod),
						typeof(DefaultUnusedSpaceErasureAttribute));
				if (result == Guid.Empty)
					result = new Guid("{BF8BA267-231A-4085-9BF9-204DE65A6641}");
				return result;
			}
			set
			{
				settings.SetValue("DefaultUnusedSpaceErasureMethod", value);
			}
		}

		/// <summary>
		/// The PRNG used. This is a GUID since PRNGs are implemented through
		/// plugins and plugins may not be loaded and missing references may follow.
		/// </summary>
		public Guid ActivePrng
		{
			get
			{
				Guid result = settings.GetValue<Guid>("ActivePRNG");
				if (result == Guid.Empty)
					result = FindHighestPriorityDefault(typeof(Prng), typeof(DefaultPrngAttribute));
				if (result == Guid.Empty)
					result = new Guid("{6BF35B8E-F37F-476e-B6B2-9994A92C3B0C}");
				return result;
			}
			set
			{
				settings.SetValue("ActivePRNG", value);
			}
		}

		/// <summary>
		/// Whether files which are locked when being erased should be forcibly
		/// unlocked for erasure.
		/// </summary>
		public bool ForceUnlockLockedFiles
		{
			get
			{
				return settings.GetValue("ForceUnlockLockedFiles", true);
			}
			set
			{
				settings.SetValue("ForceUnlockLockedFiles", value);
			}
		}

		/// <summary>
		/// Whether missed tasks should be run when the program next starts.
		/// </summary>
		public bool ExecuteMissedTasksImmediately
		{
			get
			{
				return settings.GetValue("ExecuteMissedTasksImmediately", true);
			}
			set
			{
				settings.SetValue("ExecuteMissedTasksImmediately", value);
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
				return settings.GetValue("PlausibleDeniability", false);
			}
			set
			{
				settings.SetValue("PlausibleDeniability", value);
			}
		}

		/// <summary>
		/// The files which are overwritten with when a file has been erased.
		/// </summary>
		public IList<string> PlausibleDeniabilityFiles
		{
			get
			{
				return new SettingsList<string>(settings, "PlausibleDeniabilityFiles");
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
				return new SettingsDictionary<Guid, bool>(settings, "ApprovedPlugins");
			}
		}

		/// <summary>
		/// The Settings object which is the data store of this object.
		/// </summary>
		private Settings settings;

		/// <summary>
		/// Encapsulates an abstract list that is used to store settings.
		/// </summary>
		/// <typeparam name="T">The type of the list element.</typeparam>
		private class SettingsList<T> : IList<T>
		{
			public SettingsList(Settings settings, string settingName)
			{
				Settings = settings;
				SettingName = settingName;
				List = new List<T>();

				T[] values = settings.GetValue<T[]>(settingName);
				if (values != null)
					List.AddRange(values);
			}

			~SettingsList()
			{
				Save();
			}

			#region IList<T> Members

			public int IndexOf(T item)
			{
				return List.IndexOf(item);
			}

			public void Insert(int index, T item)
			{
				List.Insert(index, item);
				Save();
			}

			public void RemoveAt(int index)
			{
				List.RemoveAt(index);
				Save();
			}

			public T this[int index]
			{
				get
				{
					return List[index];
				}
				set
				{
					List[index] = value;
					Save();
				}
			}

			#endregion

			#region ICollection<T> Members

			public void Add(T item)
			{
				List.Add(item);
				Save();
			}

			public void Clear()
			{
				List.Clear();
				Save();
			}

			public bool Contains(T item)
			{
				return List.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				List.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return List.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(T item)
			{
				bool result = List.Remove(item);
				Save();
				return result;
			}

			#endregion

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator()
			{
				return List.GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return List.GetEnumerator();
			}

			#endregion

			/// <summary>
			/// Saves changes made to the list to the settings manager.
			/// </summary>
			private void Save()
			{
				Settings.SetValue(SettingName, List);
			}

			/// <summary>
			/// The settings object storing the settings.
			/// </summary>
			private Settings Settings;

			/// <summary>
			/// The name of the setting we are encapsulating.
			/// </summary>
			private string SettingName;

			/// <summary>
			/// The list we are using as scratch.
			/// </summary>
			private List<T> List;
		}
	}
}
