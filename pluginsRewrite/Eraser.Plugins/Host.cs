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
using System.Linq;

using System.IO;
using System.Reflection;

using Eraser.Util;

namespace Eraser.Plugins
{
	/// <summary>
	/// The plugins host interface which is used for communicating with the host
	/// program.
	/// </summary>
	/// <remarks>Remember to call Load to load the plugins into memory, otherwise
	/// they will never be loaded.</remarks>
	public abstract class Host : IDisposable
	{
		#region IDisposable members
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Cleans up resources used by the host. Also unloads all loaded plugins.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		/// <summary>
		/// Initialises the Plugins library. Call <see cref="Host.Load"/> when object
		/// initialisation is complete.
		/// </summary>
		/// <remarks>Call <see cref="Host.Instance.Dispose"/> when exiting.</remarks>
		public static void Initialise()
		{
			new DefaultHost();
		}

		/// <summary>
		/// Constructor. Sets the global Plugin Host instance.
		/// </summary>
		/// <see cref="Host.Instance"/>
		protected Host()
		{
			if (Instance != null)
				throw new InvalidOperationException("Only one global Plugin Host instance can " +
					"exist at any one point of time.");
			Instance = this;
		}

		/// <summary>
		/// Getter that retrieves the global plugin host instance.
		/// </summary>
		public static Host Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Retrieves the list of currently loaded plugins.
		/// </summary>
		/// <remarks>The returned list is read-only</remarks>
		public abstract IList<PluginInstance> Plugins
		{
			get;
		}

		/// <summary>
		/// Loads all plugins into memory.
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// The plugin load event, allowing clients to decide whether to load
		/// the given plugin.
		/// </summary>
		public EventHandler<PluginLoadEventArgs> PluginLoad { get; set; }

		/// <summary>
		/// The plugin loaded event.
		/// </summary>
		public EventHandler<PluginLoadedEventArgs> PluginLoaded { get; set; }

		/// <summary>
		/// Event callback executor for the OnPluginLoad Event
		/// </summary>
		internal void OnPluginLoaded(object sender, PluginLoadedEventArgs e)
		{
			if (PluginLoaded != null)
				PluginLoaded(sender, e);
		}

		/// <summary>
		/// Loads a plugin.
		/// </summary>
		/// <param name="filePath">The absolute or relative file path to the
		/// DLL.</param>
		/// <returns>True if the plugin is loaded, false otherwise.</returns>
		/// <remarks>If a plugin is loaded twice, this function should do nothing
		/// and return True.</remarks>
		public abstract bool LoadPlugin(string filePath);
	}

	/// <summary>
	/// The default plugins host implementation.
	/// </summary>
	internal class DefaultHost : Host
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultHost() : base()
		{
			//Specify additional places to load assemblies from
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveReflectionDependency;
		}

		public override void Load()
		{
			//Load all core plugins first
			foreach (KeyValuePair<string, string> plugin in CorePlugins)
			{
				LoadCorePlugin(Path.Combine(PluginsFolder, plugin.Key), plugin.Value);
			}

			//Then load the rest
			foreach (string fileName in Directory.GetFiles(PluginsFolder))
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

		protected override void Dispose(bool disposing)
		{
			if (plugins == null)
				return;

			if (disposing)
			{
				//Unload all the plugins. This will cause all the plugins to execute
				//the cleanup code.
				foreach (PluginInstance plugin in plugins)
					if (plugin.Plugin != null)
						plugin.Plugin.Dispose();
			}

			plugins = null;
		}

		public override IList<PluginInstance> Plugins
		{
			get { return plugins.AsReadOnly(); }
		}

		/// <summary>
		/// Verifies whether the provided assembly is a plugin.
		/// </summary>
		/// <param name="assembly">The assembly to verify.</param>
		/// <returns>True if the assembly provided is a plugin, false otherwise.</returns>
		private bool IsPlugin(Assembly assembly)
		{
			//Iterate over every exported type, checking if it implements IPlugin
			Type typePlugin = assembly.GetExportedTypes().FirstOrDefault(
					type => type.GetInterface("Eraser.Plugins.IPlugin", true) != null);

			//If the typePlugin type is empty the assembly doesn't implement IPlugin it's not
			//a plugin.
			return typePlugin != null;
		}

		/// <summary>
		/// Loads the assembly at the specified path, and verifying its assembly name,
		/// ensuring that the assembly contains a core plugin.
		/// </summary>
		/// <param name="filePath">The path to the assembly.</param>
		/// <param name="assemblyName">The name of the assembly.</param>
		private void LoadCorePlugin(string filePath, string assemblyName)
		{
			Assembly assembly = Assembly.ReflectionOnlyLoadFrom(filePath);
			if (assembly.GetName().FullName.Substring(0, assemblyName.Length + 1) !=
				assemblyName + ",")
			{
				throw new FileLoadException(S._("The Core plugin assembly is not one which" +
					"Eraser expects.\n\nCheck that the Eraser installation is not corrupt, or " +
					"reinstall the program."));
			}

			//Create the PluginInstance structure
			PluginInstance instance = new PluginInstance(assembly, null);

			//Ignore non-plugins
			if (!IsPlugin(instance.Assembly))
				throw new FileLoadException(S._("The provided Core plugin assembly is not a " +
					"plugin.\n\nCheck that the Eraser installation is not corrupt, or reinstall " +
					"the program."));

			//OK this assembly is a plugin
			lock (plugins)
				plugins.Add(instance);

			//Check for the presence of a valid signature: Core plugins must have the same
			//public key as the current assembly
			if (!assembly.GetName().GetPublicKey().SequenceEqual(
					Assembly.GetExecutingAssembly().GetName().GetPublicKey()))
			{
				throw new FileLoadException(S._("The provided Core plugin does not have an " +
					"identical public key as the Eraser assembly.\n\nCheck that the Eraser " +
					"installation is not corrupt, or reinstall the program."));
			}

			//Okay, everything's fine, initialise the plugin
			instance.Assembly = Assembly.Load(instance.Assembly.GetName());
			instance.LoadingPolicy = LoadingPolicy.Core;
			InitialisePlugin(instance);
		}

		public override bool LoadPlugin(string filePath)
		{
			//Create the PluginInstance structure
			Assembly reflectAssembly = Assembly.ReflectionOnlyLoadFrom(filePath);
			PluginInstance instance = new PluginInstance(reflectAssembly, null);

			//Check that the plugin hasn't yet been loaded.
			if (Plugins.Count(
					plugin => plugin.Assembly.GetName().FullName ==
					reflectAssembly.GetName().FullName) > 0)
			{
				return true;
			}

			//Ignore non-plugins
			if (!IsPlugin(instance.Assembly))
				return false;

			//OK this assembly is a plugin
			lock (plugins)
				plugins.Add(instance);

			PluginLoadEventArgs e = new PluginLoadEventArgs(instance);
			PluginLoad(this, e);
			if (PluginLoad == null || e.Load)
			{
				InitialisePlugin(instance);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Initialises the given plugin from the plugin's description.
		/// </summary>
		/// <param name="instance">The <see cref="PluginInstance"/> structure to fill.</param>
		/// <exception cref="System.IO.FileLoadException" />
		private void InitialisePlugin(PluginInstance instance)
		{
			try
			{
				//Iterate over every exported type, checking for the IPlugin implementation
				Type typePlugin = instance.Assembly.GetExportedTypes().First(
					type => type.GetInterface("Eraser.Manager.Plugin.IPlugin", true) != null);
				if (typePlugin == null)
					return;

				//Initialize the plugin
				instance.Plugin = (IPlugin)Activator.CreateInstance(
					instance.Assembly.GetType(typePlugin.ToString()));
				instance.Plugin.Initialize(this);

				//And broadcast the plugin load event
				OnPluginLoaded(this, new PluginLoadedEventArgs(instance));
			}
			catch (System.Security.SecurityException e)
			{
				throw new FileLoadException(S._("Could not load the plugin."),
					instance.Assembly.Location, e);
			}
		}

		private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			//Check the plugins folder
			foreach (string fileName in Directory.GetFiles(PluginsFolder))
			{
				FileInfo file = new FileInfo(fileName);
				if (file.Extension.Equals(".dll"))
					try
					{
						Assembly assembly = Assembly.ReflectionOnlyLoadFrom(file.FullName);
						if (assembly.GetName().FullName == args.Name)
							return Assembly.LoadFile(file.FullName);
					}
					catch (BadImageFormatException)
					{
					}
					catch (FileLoadException)
					{
					}
			}

			return null;
		}

		private Assembly ResolveReflectionDependency(object sender, ResolveEventArgs args)
		{
			return Assembly.ReflectionOnlyLoad(args.Name);
		}

		/// <summary>
		/// The path to the folder containing the plugins.
		/// </summary>
		public readonly string PluginsFolder = Path.Combine(
			Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), //Assembly location
			"Plugins" //Plugins folder
		);

		/// <summary>
		/// The list of plugins which are core, the key is the file name, the value
		/// is the assembly name.
		/// </summary>
		private readonly KeyValuePair<string, string>[] CorePlugins =
			new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"Eraser.DefaultPlugins.dll",
					"Eraser.DefaultPlugins"
				)
			};

		/// <summary>
		/// Stores the list of plugins found within the Plugins folder.
		/// </summary>
		private List<PluginInstance> plugins = new List<PluginInstance>();
	}
}
