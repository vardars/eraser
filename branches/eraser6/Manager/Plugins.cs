using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Eraser.Manager.Plugin
{
	/// <summary>
	/// Globals class which holds global instances of the necessary plugin objects.
	/// </summary>
	internal partial class Globals
	{
		public static DefaultHost Host = new DefaultHost();
	}

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
			get { return Globals.Host; }
		}

		/// <summary>
		/// Retrieves the list of currently loaded plugins.
		/// </summary>
		public abstract List<IPlugin> Plugins
		{
			get;
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
		/// <param name="method">The erase method to register.</param>
		public abstract void RegisterEraseMethod(IEraseMethod method);
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

		public override List<IPlugin> Plugins
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
					IPlugin pluginInterface = (IPlugin)Activator.CreateInstance(
						assembly.GetType(type.ToString()));
					pluginInterface.Initialize(this);
					lock (plugins)
						plugins.Add(pluginInterface);
				}
			}
		}

		public override void RegisterEraseMethod(IEraseMethod method)
		{
			ErasureMethodManager.RegisterMethod(method);
		}

		private List<IPlugin> plugins = new List<IPlugin>();
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
	}
}
