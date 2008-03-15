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
				for (int bytesGenerated = 0;
					bytesGenerated < buffer.Length * sizeof(byte);
					bytesGenerated += Rand.ISAAC.SIZE * sizeof(int))
				{
					//Generate 256 ints.
					if (isaac.count == 0)
						isaac.Isaac();

					//Copy the data.
					fixed (int* src = isaac.rsl)
					fixed (byte* dest = &buffer[bytesGenerated])
					{
						//Advance to the end of the array of unused randomness.
						byte* pSrc = (byte*)(src + isaac.count);
						byte* pDest = dest;

						//Copy four bytes at a time, moving the source pointer backwards,
						//and the destination pointer forwards.
						int bytesToCopy = Math.Min(isaac.count * sizeof(int),
							buffer.Length - bytesGenerated);
						while (bytesToCopy >= 4)
						{
							pSrc -= sizeof(int);
							*(int*)pDest = *(int*)pSrc;
							pDest += sizeof(int);

							bytesToCopy -= sizeof(int);
							--isaac.count;
						}

						//Copy the remaining bytes over.
						if (bytesToCopy != 0)
						{
							while (bytesToCopy-- != 0)
								*pDest++ = *pSrc--;
							--isaac.count;
						}
					}
				}
			}
		}

		private Rand.ISAAC isaac = new Rand.ISAAC();
	}
}
