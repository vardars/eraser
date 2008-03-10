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

		public unsafe override void NextBytes(byte[] buffer)
		{
			lock (isaac)
			{
				for (uint bytesGenerated = 0;
					bytesGenerated < buffer.Length * sizeof(byte);
					bytesGenerated += Rand.ISAAC.SIZE * sizeof(int))
				{
					//Generate 256 ints.
					isaac.Isaac();

					//Copy the data.
					fixed (int* src = isaac.rsl)
					fixed (byte* dest = buffer)
					{
						byte* bSrc = (byte*)src;
						int bytesToCopy = (int)Math.Min((uint)Rand.ISAAC.SIZE, buffer.Length),
						    randArraySize = isaac.rsl.Length * sizeof(int);

						for (uint i = 0; i < randArraySize && i < bytesToCopy; ++i)
							dest[i] = bSrc[i];
					}
				}
			}
		}

		private Rand.ISAAC isaac = new Rand.ISAAC();
	}
}
