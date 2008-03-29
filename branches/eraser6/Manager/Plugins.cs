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
	public abstract class Host
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
		/// The plugin load event delegate.
		/// </summary>
		/// <param name="instance">The instance of the plugin loaded.</param>
		public delegate void OnPluginLoadEventHandler(PluginInstance instance);

		/// <summary>
		/// The plugin loaded event.
		/// </summary>
		public event OnPluginLoadEventHandler PluginLoad;

		/// <summary>
		/// Event callback executor for the OnPluginLoad Event
		/// </summary>
		/// <param name="instance"></param>
		protected void OnPluginLoad(PluginInstance instance)
		{
			if (PluginLoad != null)
				PluginLoad(instance);
		}

		/// <summary>
		/// Loads a plugin.
		/// </summary>
		/// <param name="filePath">The absolute or relative file path to the
		/// DLL.</param>
		public abstract void LoadPlugin(string filePath);

		/// <summary>
		/// Registers an erasure method with the manager.
		/// </summary>
		/// <param name="method">The erasure method to register.</param>
		public abstract void RegisterErasureMethod(ErasureMethod method);

		/// <summary>
		/// Registers a PRNG with the manager.
		/// </summary>
		/// <param name="prng">The PRNG algorithm to register.</param>
		public abstract void RegisterPRNG(PRNG prng);
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
			foreach (string fileName in Directory.GetFiles(PLUGINSFOLDER))
			{
				FileInfo file = new FileInfo(fileName);
				if (file.Extension.Equals(".dll"))
					LoadPlugin(file.FullName);
			}
		}

		/// <summary>
		/// The path to the folder containing the plugins.
		/// </summary>
		public const string PLUGINSFOLDER = "Plugins/";

		public override List<PluginInstance> Plugins
		{
			get { return plugins; }
		}

		public override void LoadPlugin(string filePath)
		{
			//Load the plugin
			Assembly assembly = Assembly.LoadFile(filePath);

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
					//Initialize the plugin
					IPlugin pluginInterface = (IPlugin)Activator.CreateInstance(
						assembly.GetType(type.ToString()));
					pluginInterface.Initialize(this);

					//Create the PluginInstance structure
					PluginInstance instance = new PluginInstance(assembly, filePath, pluginInterface);

					//Add the plugin to the list of loaded plugins
					lock (plugins)
						plugins.Add(instance);
					OnPluginLoad(instance);
				}
			}
		}

		public override void RegisterErasureMethod(ErasureMethod method)
		{
			ErasureMethodManager.Register(method);
		}

		public override void RegisterPRNG(PRNG prng)
		{
			PRNGManager.Register(prng);
		}

		private List<PluginInstance> plugins = new List<PluginInstance>();
	}

	/// <summary>
	/// Structure holding the instance values of the plugin like handle and path.
	/// </summary>
	public struct PluginInstance
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
