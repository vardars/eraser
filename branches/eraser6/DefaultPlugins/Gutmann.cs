using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using Eraser.Manager;

namespace Eraser.DefaultPlugins
{
	class Gutmann : ErasureMethod
	{
		public override string Name
		{
			get { return "Gutmann"; }
		}

		public override int Passes
		{
			get { return passes.Length; }
		}

		public override Guid GUID
		{
			get { return new Guid("{1407FC4E-FEFF-4375-B4FB-D7EFBB7E9922}"); }
		}

		public override void Erase(Stream strm, PRNG prng, OnProgress callback)
		{
			//Randomize the order of the passes
			Pass[] randomizedPasses = ShufflePasses(passes);

			//Remember the starting position of the stream.
			long strmStart = strm.Position;
			long strmLength = strm.Length - strmStart;

			//Allocate memory for a buffer holding data for the pass.
			byte[] buffer = new byte[DiskOperationUnit];

			//Run every pass!
			for (int pass = 0; pass < Passes; ++pass)
			{
				//Do a progress callback first.
				callback(pass / (float)Passes, pass);

				//Start from the beginning again
				strm.Seek(strmStart, SeekOrigin.Begin);

				//Write the buffer to disk.
				long toWrite = strmLength;
				int dataStopped = buffer.Length;
				while (toWrite > 0)
				{
					//Calculate how much of the buffer to write to disk.
					int amount = (int)Math.Min(toWrite, buffer.Length - dataStopped);

					//If we have no data left, get more!
					if (amount == 0)
					{
						randomizedPasses[pass].Execute(ref buffer, prng);
						dataStopped = 0;
						continue;
					}

					//Write the data.
					strm.Write(buffer, dataStopped, amount);
					strm.Flush();
					toWrite -= amount;

					//Do a progress callback.
					callback(((float)pass + ((strmLength - toWrite) / (float)strmLength)) / (float)Passes, pass);
				}
			}
		}

		/// <summary>
		/// The 35 passes of the Gutmann method.
		/// </summary>
		private static readonly ErasureMethod.Pass[] passes =
		{
			new Pass(WriteRandom, null),                                   // 1
			new Pass(WriteRandom, null),
			new Pass(WriteRandom, null),
			new Pass(WriteRandom, null),
			new Pass(WriteConstant, new byte[] {0x55}),                    // 5
			new Pass(WriteConstant, new byte[] {0xAA}),
			new Pass(WriteConstant, new byte[] {0x92, 0x49, 0x24}),
			new Pass(WriteConstant, new byte[] {0x49, 0x24, 0x92}),
			new Pass(WriteConstant, new byte[] {0x24, 0x92, 0x49}),
			new Pass(WriteConstant, new byte[] {0x00}),                    // 10
			new Pass(WriteConstant, new byte[] {0x11}),
			new Pass(WriteConstant, new byte[] {0x22}),
			new Pass(WriteConstant, new byte[] {0x33}),
			new Pass(WriteConstant, new byte[] {0x44}),
			new Pass(WriteConstant, new byte[] {0x55}),                    // 15
			new Pass(WriteConstant, new byte[] {0x66}),
			new Pass(WriteConstant, new byte[] {0x77}),
			new Pass(WriteConstant, new byte[] {0x88}),
			new Pass(WriteConstant, new byte[] {0x99}),
			new Pass(WriteConstant, new byte[] {0xAA}),                    // 20
			new Pass(WriteConstant, new byte[] {0xBB}),
			new Pass(WriteConstant, new byte[] {0xCC}),
			new Pass(WriteConstant, new byte[] {0xDD}),
			new Pass(WriteConstant, new byte[] {0xEE}),
			new Pass(WriteConstant, new byte[] {0xFF}),                    // 25
			new Pass(WriteConstant, new byte[] {0x92, 0x49, 0x24}),
			new Pass(WriteConstant, new byte[] {0x49, 0x24, 0x92}),
			new Pass(WriteConstant, new byte[] {0x24, 0x92, 0x49}),
			new Pass(WriteConstant, new byte[] {0x6D, 0xB6, 0xDB}),
			new Pass(WriteConstant, new byte[] {0xB6, 0xDB, 0x6D}),        // 30
			new Pass(WriteConstant, new byte[] {0xDB, 0x6D, 0xB6}),
			new Pass(WriteRandom, null),
			new Pass(WriteRandom, null),
			new Pass(WriteRandom, null),
			new Pass(WriteRandom, null)                                    // 35
		};
	}
}
