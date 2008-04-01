using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Manager.Plugin;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	public class DefaultPlugin : IPlugin
	{
		public void Initialize(Host host)
		{
			host.RegisterErasureMethod(new Gutmann());
			host.RegisterErasureMethod(new Schneier());
			host.RegisterErasureMethod(new DoD_EcE());
			host.RegisterErasureMethod(new DoD_E());
			host.RegisterErasureMethod(new Pseudorandom());
			host.RegisterErasureMethod(new FirstLast16KB());
			host.RegisterPRNG(new ISAAC());
			host.RegisterPRNG(new RNGCrypto());

			Settings = Manager.ManagerLibrary.Instance.Settings.GetSettings();
		}

		public void Dispose()
		{
			Manager.ManagerLibrary.Instance.Settings.SetSettings(Settings);
		}

		public string Name
		{
			get { return S._("Default Erasure Methods and PRNGs"); }
		}

		public string Author
		{
			get { return S._("The Eraser Project <eraser-development@lists.sourceforge.net>"); }
		}

		public bool Configurable
		{
			get { return true; }
		}

		public void DisplaySettings(Control parent)
		{
			SettingsForm form = new SettingsForm();
			form.ShowDialog();
		}

		/// <summary>
		/// The dictionary holding settings for this plugin.
		/// </summary>
		public static Dictionary<string, object> Settings = null;
	}
}
