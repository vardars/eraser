using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : ErasureMethod
	{
		public override string Name
		{
			get { return "Gutmann"; }
		}

		public override uint Passes
		{
			get { return 35; }
		}

		public override void Erase(System.IO.Stream strm, PRNG prng, OnProgress callback)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}
}
