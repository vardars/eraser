using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	/// <summary>
	/// ISAAC CSPRNG.
	/// </summary>
	public class ISAAC : PRNG
	{
		public override string Name
		{
			get { return "ISAAC CSPRNG"; }
		}

		public override Guid GUID
		{
			get { return new Guid("{CB7DE02E-8067-4270-B115-70AB49F23BB7}"); }
		}

		public override void NextBytes(byte[] buffer)
		{
			//Generate 256 ints.
			lock (isaac)
			{
				for (uint intsGenerated = 0; intsGenerated * sizeof(int) < buffer.Length * sizeof(byte);
					intsGenerated += Rand.ISAAC.SIZE)
				{
					isaac.Isaac();
					isaac.rsl.CopyTo(buffer, 0);
				}
			}
		}

		private Rand.ISAAC isaac = new Rand.ISAAC();
	}
}
