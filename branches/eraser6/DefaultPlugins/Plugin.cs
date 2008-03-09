using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using Eraser.Manager.Plugin;

namespace Eraser.DefaultPlugins
{
	public class DefaultPlugin : IPlugin
	{
		#region IPlugin Members

		public void Initialize(Host host)
		{
			host.RegisterErasureMethod(new Gutmann());
		}

		public string Name
		{
			get { return "Default Erase Methods and PRNGs"; }
		}

		public string Author
		{
			get { return "The Eraser Project <eraser-development@lists.sourceforge.net>"; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
