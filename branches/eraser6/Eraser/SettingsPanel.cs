using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager.Plugin;

namespace Eraser
{
	public partial class SettingsPanel : Eraser.BasePanel
	{
		public SettingsPanel()
		{
			InitializeComponent();
			Dock = DockStyle.None;

			//Load the list of DLLs
			Host instance = Host.Instance;
			List<PluginInstance>.Enumerator i = instance.Plugins.GetEnumerator();
			while (i.MoveNext())
			{
				ListViewItem item = pluginsManager.Items.Add(i.Current.Plugin.Name);
				item.SubItems.Add(i.Current.Plugin.Author);
				item.SubItems.Add(string.Empty);//item.SubItems.Add(i.Current.Version);
				item.SubItems.Add(i.Current.Path);
			}

			//For new DLLs, register the callback.
			instance.PluginLoad += new Host.OnPluginLoadEventHandler(OnNewPluginLoaded);
		}

		private void OnNewPluginLoaded(PluginInstance instance)
		{
			throw new NotImplementedException("The plugin load event handler has not been implemented.");
		}
	}
}

