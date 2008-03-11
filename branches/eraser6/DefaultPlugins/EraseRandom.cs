using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Pseudorandom : PassBasedErasureMethod
	{
		public override string Name
		{
			get { return "Pseudorandom Data"; }
		}

		public override Guid GUID
		{
			get { return new Guid("{BF8BA267-231A-4085-9BF9-204DE65A6641}"); }
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
					new Pass(WriteRandom, null)
				};
			}
		}
	}
}
