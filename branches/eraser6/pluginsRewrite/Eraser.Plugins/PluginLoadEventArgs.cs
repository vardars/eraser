using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eraser.Plugins
{
	/// <summary>
	/// Provides the event arguments for the PluginLoad event, raised when the Plugins
	/// library needs to decide whether to load a given plugin.
	/// </summary>
	public class PluginLoadEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="info">The plugin information to be passed to the approving
		/// delegate.</param>
		internal PluginLoadEventArgs(PluginInstance info)
		{
			Plugin = info;
			Load = true;
		}

		/// <summary>
		/// Gets the plugin associated with this event.
		/// </summary>
		public PluginInstance Plugin { get; private set; }

		/// <summary>
		/// Gets or Sets whether the current plugin should be loaded.
		/// </summary>
		public bool Load { get; set; }
	}
}
