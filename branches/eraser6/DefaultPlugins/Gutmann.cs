using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : IErasureMethod
	{
		string IErasureMethod.Name
		{
			get { return "Gutmann"; }
		}

		void IErasureMethod.Erase(System.IO.Stream strm, PRNG prng)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
