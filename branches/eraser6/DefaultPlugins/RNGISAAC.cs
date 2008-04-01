using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using Eraser.Util;

namespace Eraser.DefaultPlugins
{
	/// <summary>
	/// ISAAC CSPRNG.
	/// </summary>
	public class ISAAC : PRNG
	{
		public ISAAC()
		{
			isaac = new Rand.ISAAC(ConvertSeedArray(PRNGManager.GetEntropy()));
		}

		public override string Name
		{
			get { return S._("ISAAC CSPRNG"); }
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

		protected override void Reseed(byte[] seed)
		{
			lock (isaac)
				isaac = new Rand.ISAAC(ConvertSeedArray(seed));
		}

		/// <summary>
		/// Converts the source seed array constituting bytes to one made up of ints.
		/// </summary>
		/// <param name="seed">The seed to convert.</param>
		/// <returns>An int[] containins the seed array.</returns>
		private unsafe static int[] ConvertSeedArray(byte[] seed)
		{
			//Copy the seed to an int array
			int[] newSeed = new int[seed.Length / sizeof(int)];
			fixed (byte* fSeed = seed)
			fixed (int* fNewSeed = newSeed)
			{
				//Calculate the amount in bytes to copy
				byte* pSeed = fSeed;
				byte* pNewSeed = (byte*)fNewSeed;
				int amount = Math.Min(seed.Length, Rand.ISAAC.SIZE * sizeof(int));

				//Copy
				while (amount-- != 0)
					*pNewSeed++ = *pSeed++;
			}

			return newSeed;
		}

		private Rand.ISAAC isaac;
	}
}
