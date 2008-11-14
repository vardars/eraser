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
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
			string pluginsFolder = Path.Combine(
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), //Assembly location
				PLUGINSFOLDER //Plugins folder
			);

			foreach (string fileName in Directory.GetFiles(pluginsFolder))
			{
				FileInfo file = new FileInfo(fileName);
				if (file.Extension.Equals(".dll"))
					LoadPlugin(file.FullName);
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
			//Load the plugin
			Assembly assembly = Assembly.LoadFrom(filePath);

			//Iterate over every exported type, checking if it implements IPlugin
			foreach (Type type in assembly.GetTypes())
			{
				if (!type.IsPublic || type.IsAbstract)
					//Not interesting.
					continue;

				//Check for an implementation of IPlugin
				Type typeInterface = type.GetInterface("Eraser.Manager.Plugin.IPlugin", true);
				if (typeInterface != null)
				{
					//Create the PluginInstance structure
					PluginInstance instance = new PluginInstance(assembly, filePath, null);

					//Add the plugin to the list of loaded plugins
					lock (plugins)
						plugins.Add(instance);

					//Initialize the plugin
					IPlugin pluginInterface = (IPlugin)Activator.CreateInstance(
						assembly.GetType(type.ToString()));
					pluginInterface.Initialize(this);
					instance.Plugin = pluginInterface;

					//And broadcast the plugin load event
					OnPluginLoaded(instance);
				}
			}
		}

		Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			lock (plugins)
				foreach (PluginInstance instance in plugins)
					if (instance.Assembly.FullName == args.Name)
						return instance.Assembly;
			return null;
		}

		private List<PluginInstance> plugins = new List<PluginInstance>();
	}

	/// <summary>
	/// Structure holding the instance values of the plugin like handle and path.
	/// </summary>
	public class PluginInstance
	{
		internal PluginInstance(Assembly assembly, string path, IPlugin plugin)
		{
			Assembly = assembly;
			Path = path;
			Plugin = plugin;
		}

		public Assembly Assembly;
		public string Path;
		public IPlugin Plugin;
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
