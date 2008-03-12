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
			host.RegisterErasureMethod(new Schneier());
			host.RegisterErasureMethod(new DoD_EcE());
			host.RegisterErasureMethod(new DoD_E());
			host.RegisterErasureMethod(new Pseudorandom());
			host.RegisterErasureMethod(new FirstLast16KB());
			host.RegisterPRNG(new ISAAC());
			host.RegisterPRNG(new RNGCrypto());
		}

		public string Name
		{
			get { return "Default Erasure Methods and PRNGs"; }
		}

		public string Author
		{
			get { return "The Eraser Project <eraser-development@lists.sourceforge.net>"; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		#endregion
	}
}
