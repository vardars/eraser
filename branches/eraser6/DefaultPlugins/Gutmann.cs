using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : IEraseMethod
	{
		string IEraseMethod.Name
		{
			get { return "Gutmann"; }
		}

		void IEraseMethod.Erase(System.IO.Stream strm, PRNG prng)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
