using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Schneier : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return "Schneier 7 pass"; }
		}

		public override Guid GUID
		{
			get { return new Guid("{B1BFAB4A-31D3-43a5-914C-E9892C78AFD8}"); }
		}

		protected override bool RandomizePasses
		{
			get { return false; }
		}

		protected override Pass[] PassesSet
		{
			get
			{
				return new Pass[]
				{
					new Pass(WriteConstant, new byte[] { 1 }),
					new Pass(WriteConstant, new byte[] { 0 }),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null),
					new Pass(WriteRandom, null)
				};
			}
		}
	}
}
