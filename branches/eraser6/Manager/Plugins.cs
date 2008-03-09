using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager.Plugin
{
	/// <summary>
	/// The plugins host interface which is used for communicating with the host
	/// program.
	/// </summary>
	public abstract class Host
	{
		/// <summary>
		/// Retrieves the list of currently loaded plugins.
		/// </summary>
		public abstract List<IPlugin> Plugins
		{
			get;
		}

		/// <summary>
		/// Registers an erasure method with the manager.
		/// </summary>
		/// <param name="method">The erase method to register.</param>
		public abstract void RegisterEraseMethod(EraseMethod method);
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

	/// <summary>
	/// An erasure method DLL.
	/// </summary>
	public interface IEraseMethod : IPlugin
	{
	}

	/// <summary>
	/// A PRNG provider.
	/// </summary>
	public interface IPRNG : IPlugin
	{
	}

	/// <summary>
	/// A history cleaner plugin.
	/// </summary>
	public interface IHistoryCleaner : IPlugin
	{
	}
}
