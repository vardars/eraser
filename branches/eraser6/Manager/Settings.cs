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
using Eraser.Util;

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

	#region Default attributes
	/// <summary>
	/// Indicates that the marked class should be used as a default when no
	/// settings have been set by the user.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public abstract class DefaultAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="priority">The priority of the current element in terms of
		/// it being the default.</param>
		public DefaultAttribute(int priority)
		{
			Priority = priority;
		}

		/// <summary>
		/// The priority of the default.
		/// </summary>
		public int Priority
		{
			get
			{
				return priority;
			}
			private set
			{
				priority = value;
			}
		}

		private int priority;
	}

	/// <summary>
	/// Indicates that the marked class should be used as the default file erasure
	/// method.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class DefaultFileErasureAttribute : DefaultAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="priority">The priority of the current element in terms of
		/// it being the default.</param>
		public DefaultFileErasureAttribute(int priority)
			: base(priority)
		{
		}
	}

	/// <summary>
	/// Indicates that the marked class should be used as the default unused space
	/// erasure method.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class DefaultUnusedSpaceErasureAttribute : DefaultAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="priority">The priority of the current element in terms of
		/// it being the default.</param>
		public DefaultUnusedSpaceErasureAttribute(int priority)
			: base(priority)
		{
		}
	}

	/// <summary>
	/// Indicates that the marked class should be used as the default PRNG.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class DefaultPRNGAttribute : DefaultAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="priority">The priority of the current element in terms of
		/// it being the default.</param>
		public DefaultPRNGAttribute(int priority)
			: base(priority)
		{
		}
	}
	#endregion

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
				//If the user did not define anything for this field, check all plugins
				//and use the method which was declared by us to be the highest
				//priority default
				if (settings["DefaultFileErasureMethod"] == null)
				{
					Guid result = FindHighestPriorityDefault(typeof(ErasureMethod),
						typeof(DefaultFileErasureAttribute));
					return result == Guid.Empty ? new Guid("{1407FC4E-FEFF-4375-B4FB-D7EFBB7E9922}") :
						result;
				}
				else
					return (Guid)settings["DefaultFileErasureMethod"];
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
				if (settings["DefaultUnusedSpaceErasureMethod"] == null)
				{
					Guid result = FindHighestPriorityDefault(typeof(UnusedSpaceErasureMethod),
						typeof(DefaultUnusedSpaceErasureAttribute));
					return result == Guid.Empty ? new Guid("{BF8BA267-231A-4085-9BF9-204DE65A6641}") :
						result;
				}
				else
					return (Guid)settings["DefaultUnusedSpaceErasureMethod"];
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
				if (settings["ActivePRNG"] == null)
				{
					Guid result = FindHighestPriorityDefault(typeof(PRNG),
						typeof(DefaultPRNGAttribute));
					return result == Guid.Empty ? new Guid("{6BF35B8E-F37F-476e-B6B2-9994A92C3B0C}") :
						result;
				}
				else
					return (Guid)settings["ActivePRNG"];
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

		#region Default Attributes retrieval
		private static Guid FindHighestPriorityDefault(Type superClass,
			Type attributeType)
		{
			Plugin.Host pluginHost = ManagerLibrary.Instance.Host;
			List<Plugin.PluginInstance> plugins = pluginHost.Plugins;
			SortedList<int, Guid> priorities = new SortedList<int, Guid>();

			foreach (Plugin.PluginInstance plugin in plugins)
			{
				Type[] types = FindTypeAttributeInAssembly(plugin.Assembly,
					superClass, attributeType);

				//Prioritize the types.
				if (types != null)
					foreach (Type type in types)
					{
						object[] guids =
							type.GetCustomAttributes(typeof(GuidAttribute), false);
						DefaultAttribute defaultAttr = (DefaultAttribute)
							type.GetCustomAttributes(attributeType, false)[0];

						if (guids.Length == 1)
							priorities.Add(defaultAttr.Priority,
								new Guid(((GuidAttribute)guids[0]).Value));
					}
			}

			//Return the highest priority one.
			if (priorities.Count > 0)
				return priorities[priorities.Keys[priorities.Count - 1]];
			return Guid.Empty;
		}

		private static Type[] FindTypeAttributeInAssembly(Assembly assembly, Type superClass,
			Type attributeType)
		{
			//Check whether the plugin is signed by us.
			byte[] pluginKey = assembly.GetName().GetPublicKey();
			byte[] ourKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();

			//Definitely not signed by us.
			if (pluginKey.Length != ourKey.Length ||
				!MsCorEEAPI.VerifyStrongName(assembly.Location))
				return null;

			//Not by us, either
			bool officialPlugin = true;
			for (int i = 0, j = ourKey.Length; i != j; ++i)
				if (pluginKey[i] != ourKey[i])
					officialPlugin = false;
			if (!officialPlugin)
				return null;

			//Yes, if we got here the plugin is signed by us. Find the
			//type which inherits from ErasureMethod.
			Type[] types = assembly.GetExportedTypes();
			List<Type> result = new List<Type>();
			foreach (Type type in types)
			{
				if (!type.IsPublic || type.IsAbstract)
					//Not interesting.
					continue;

				//Try to see if this class inherits from the specified super class.
				if (!type.IsSubclassOf(superClass))
					continue;

				//See if this class has the DefaultFileErasureAttribute
				object[] attributes = type.GetCustomAttributes(attributeType, false);
				if (attributes.Length > 0)
					result.Add(type);
			}

			return result.ToArray();
		}
		#endregion

		/// <summary>
		/// Holds user decisions on whether the plugin will be loaded at the next
		/// start up.
		/// </summary>
		public Dictionary<Guid, bool> PluginApprovals
		{
			get
			{
				if (settings["ApprovedPlugins"] == null)
					return new Dictionary<Guid, bool>();
				return (Dictionary<Guid, bool>)settings["ApprovedPlugins"];
			}
			set
			{
				settings["ApprovedPlugins"] = value;
			}
		}

		/// <summary>
		/// The Settings object which is the data store of this object.
		/// </summary>
		private Settings settings;
	}
}
