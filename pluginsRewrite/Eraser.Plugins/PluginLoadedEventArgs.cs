using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eraser.Plugins
{
	/// <summary>
	/// Event argument for the plugin loaded event.
	/// </summary>
	public class PluginLoadedEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="instance">The plugin instance of the recently loaded plugin.</param>
		public PluginLoadedEventArgs(PluginInstance instance)
		{
			Instance = instance;
		}

		/// <summary>
		/// The <see cref="PluginInstance"/> object representing the newly loaded plugin.
		/// </summary>
		public PluginInstance Instance { get; private set; }
	}
}
