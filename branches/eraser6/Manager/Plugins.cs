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
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Eraser.Manager.Plugin
{
	/// <summary>
	/// The plugins host interface which is used for communicating with the host
	/// program.
	/// </summary>
	/// <remarks>Remember to call Load to load the plugins into memory, otherwise
	/// they will never be loaded.</remarks>
	public abstract class Host : IDisposable
	{
		/// <summary>
		/// Getter that retrieves the global plugin host instance.
		/// </summary>
		public static Host Instance
		{
			get { return ManagerLibrary.Instance.Host; }
		}

		/// <summary>
		/// Retrieves the list of currently loaded plugins.
		/// </summary>
		public abstract List<PluginInstance> Plugins
		{
			get;
		}

		/// <summary>
		/// Loads all plugins into memory.
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// Cleans up resources used by the host. Also unloads all loaded plugins.
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// The plugin load event delegate.
		/// </summary>
		/// <param name="instance">The instance of the plugin loaded.</param>
		public delegate void PluginLoadedFunction(PluginInstance instance);

		/// <summary>
		/// The plugin loaded event.
		/// </summary>
		public event PluginLoadedFunction PluginLoaded;

		/// <summary>
		/// Event callback executor for the OnPluginLoad Event
		/// </summary>
		/// <param name="instance"></param>
		protected void OnPluginLoaded(PluginInstance instance)
		{
			if (PluginLoaded != null)
				PluginLoaded(instance);
		}

		/// <summary>
		/// Loads a plugin.
		/// </summary>
		/// <param name="filePath">The absolute or relative file path to the
		/// DLL.</param>
		public abstract void LoadPlugin(string filePath);
	}

	/// <summary>
	/// The default plugins host implementation.
	/// </summary>
	internal class DefaultHost : Host
	{
		/// <summary>
		/// Constructor. Loads all plugins in the Plugins folder.
		/// </summary>
		public DefaultHost()
		{
		}

		public override void Load()
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveReflectionDependency;
			string pluginsFolder = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), //Assembly location
				PLUGINSFOLDER //Plugins folder
			);

			foreach (string fileName in Directory.GetFiles(pluginsFolder))
			{
				FileInfo file = new FileInfo(fileName);
				if (file.Extension.Equals(".dll"))
					try
					{
						LoadPlugin(file.FullName);
					}
					catch (BadImageFormatException)
					{
					}
					catch (FileLoadException)
					{
					}
			}
		}

		public override void Dispose()
		{
			//Unload all the plugins. This will cause all the plugins to execute
			//the cleanup code.
			foreach (PluginInstance plugin in plugins)
				plugin.Plugin.Dispose();
		}

		/// <summary>
		/// The path to the folder containing the plugins.
		/// </summary>
		public const string PLUGINSFOLDER = "Plugins";

		public override List<PluginInstance> Plugins
		{
			get { return plugins; }
		}

		public override void LoadPlugin(string filePath)
		{
			//Create the PluginInstance structure
			Assembly reflectAssembly = Assembly.ReflectionOnlyLoadFrom(filePath);
			PluginInstance instance = new PluginInstance(reflectAssembly, null);
			Type typePlugin = null;

			//Iterate over every exported type, checking if it implements IPlugin
			foreach (Type type in instance.Assembly.GetExportedTypes())
			{
				//Check for an implementation of IPlugin
				Type typeInterface = type.GetInterface("Eraser.Manager.Plugin.IPlugin", true);
				if (typeInterface != null)
				{
					typePlugin = type;
					break;
				}
			}

			//If the typePlugin type is empty the assembly doesn't implement IPlugin; we
			//aren't interested.
			if (typePlugin == null)
				return;

			//OK this assembly is a plugin
			lock (plugins)
				plugins.Add(instance);

			//First check the plugin for the presence of a signature.
			if (reflectAssembly.GetName().GetPublicKey().Length == 0)
			{
				//If the user did not allow the plug-in to load, don't load it.
				if (ManagerLibrary.Instance.Settings.ApprovedPlugins.
					IndexOf(instance.AssemblyInfo.GUID) == -1)
				{
					return;
				}
			}

			//Load the plugin
			instance.Assembly = Assembly.LoadFrom(filePath);

			//Initialize the plugin
			IPlugin pluginInterface = (IPlugin)Activator.CreateInstance(
				instance.Assembly.GetType(typePlugin.ToString()));
			pluginInterface.Initialize(this);
			instance.Plugin = pluginInterface;

			//And broadcast the plugin load event
			OnPluginLoaded(instance);
		}

		Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			lock (plugins)
				foreach (PluginInstance instance in plugins)
					if (instance.Assembly.FullName == args.Name)
						return instance.Assembly;
			return null;
		}

		Assembly ResolveReflectionDependency(object sender, ResolveEventArgs args)
		{
			return Assembly.ReflectionOnlyLoad(args.Name);
		}

		private List<PluginInstance> plugins = new List<PluginInstance>();
	}

	/// <summary>
	/// Structure holding the instance values of the plugin like handle and path.
	/// </summary>
	public class PluginInstance
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="assembly">The assembly representing this plugin.</param>
		/// <param name="path">The path to the ass</param>
		/// <param name="plugin"></param>
		internal PluginInstance(Assembly assembly, IPlugin plugin)
		{
			Assembly = assembly;
			Plugin = plugin;
		}

		/// <summary>
		/// Gets the Assembly this plugin instance came from.
		/// </summary>
		public Assembly Assembly
		{
			get
			{
				return assembly;
			}
			internal set
			{
				assembly = value;

				assemblyInfo.Version = assembly.GetName().Version;
				IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(assembly);
				foreach (CustomAttributeData attr in attributes)
					if (attr.Constructor.DeclaringType == typeof(GuidAttribute))
						assemblyInfo.GUID = new Guid((string)attr.ConstructorArguments[0].Value);
					else if (attr.Constructor.DeclaringType == typeof(AssemblyCompanyAttribute))
						assemblyInfo.Author = (string)attr.ConstructorArguments[0].Value;
			}
		}

		/// <summary>
		/// Gets the attributes of the assembly, loading from reflection-only sources.
		/// </summary>
		public AssemblyInfo AssemblyInfo
		{
			get
			{
				return assemblyInfo;
			}
			internal set
			{
				assemblyInfo = value;
			}
		}

		/// <summary>
		/// Gets the IPlugin interface which the plugin exposed.
		/// </summary>
		public IPlugin Plugin
		{
			get
			{
				return plugin;
			}
			internal set
			{
				plugin = value;
			}
		}

		private Assembly assembly;
		private AssemblyInfo assemblyInfo;
		private IPlugin plugin;
	}

	/// <summary>
	/// Reflection-only information retrieved from the assembly.
	/// </summary>
	public struct AssemblyInfo
	{
		/// <summary>
		/// The GUID of the assembly.
		/// </summary>
		public Guid GUID;

		/// <summary>
		/// The publisher of the assembly.
		/// </summary>
		public string Author;

		/// <summary>
		/// The version of the assembly.
		/// </summary>
		public Version Version;
	}

	/// <summary>
	/// Basic plugin interface which allows for the main program to utilize the
	/// functions in the DLL
	/// </summary>
	public interface IPlugin : IDisposable
	{
		/// <summary>
		/// Initializer.
		/// </summary>
		/// <param name="host">The host object which can be used for two-way
		/// communication with the program.</param>
		void Initialize(Host host);

		/// <summary>
		/// The name of the plug-in, used for descriptive purposes in the UI
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// The author of the plug-in, used for display in the UI and for users
		/// to contact the author about bugs. Must be in the format:
		///		(.+) \<([a-zA-Z0-9_.]+)@([a-zA-Z0-9_.]+)\.([a-zA-Z0-9]+)\>
		/// </summary>
		/// <example>Joel Low <joel@joelsplace.sg></example>
		string Author
		{
			get;
		}

		/// <summary>
		/// Determines whether the plug-in is configurable.
		/// </summary>
		bool Configurable
		{
			get;
		}

		/// <summary>
		/// Fulfil a request to display the settings for this plug-in.
		/// </summary>
		/// <param name="parent">The parent control which the settings dialog should
		/// be parented with.</param>
		void DisplaySettings(Control parent);
	}
}
