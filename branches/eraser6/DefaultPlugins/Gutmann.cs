using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : EraseMethod
	{
		public override string Name
		{
			get { return "Gutmann"; }
		}

		public override void Erase(System.IO.Stream strm, PRNG prng)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
