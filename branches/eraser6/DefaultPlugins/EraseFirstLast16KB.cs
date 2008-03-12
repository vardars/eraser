using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using System.IO;

namespace Eraser.DefaultPlugins
{
	class FirstLast16KB : ErasureMethod
	{
		public override string Name
		{
			get { return "First/last 16KB Erasure"; }
		}

		public override int Passes
		{
			get { return 0; } //Variable number, depending on defaults.
		}

		public override Guid GUID
		{
			get { return new Guid("{0C2E07BF-0207-49a3-ADE8-46F9E1499C01}"); }
		}

		public override long CalculateEraseDataSize(List<string> paths, long targetSize)
		{
			//Simple. Number of files multiplied by 32kb.
			return paths.Count * dataSize * 2;
		}

		public override void Erase(Stream strm, long erasureLength, PRNG prng,
			OnProgress callback)
		{
			//Make sure that the erasureLength passed in here is the maximum value
			//for the size of long, since we don't want to write extra or write
			//less.
			if (erasureLength != long.MaxValue)
				throw new ArgumentException("The amount of data erased should not be " +
					"limited, since this is a self-limiting erasure method.");

			//Try to retrieve the default erasure method.
			ErasureMethod defaultMethod = ErasureMethodManager.GetInstance(
				Globals.Settings.DefaultFileErasureMethod);

			//If we are the default, use the default pseudorandom pass.
			if (defaultMethod.GUID == GUID)
				defaultMethod = ErasureMethodManager.GetInstance(new Guid(
					"{BF8BA267-231A-4085-9BF9-204DE65A6641}"));

			//If the target stream is shorter than 16kb, just forward it to the default
			//function.
			if (strm.Length < dataSize)
			{
				defaultMethod.Erase(strm, erasureLength, prng, callback);
				return;
			}

			//Declare our local anonymous function to forward the progress event
			//to the callback function.
			float baseCompleted = 0;
			OnProgress chainCallbackHandler = delegate(float currentProgress, int currentPass)
			{
				callback(baseCompleted + currentProgress / 2,
					(int)((baseCompleted * defaultMethod.Passes) + currentPass / 2));
			};

			//Seek to the beginning and write 16kb.
			strm.Seek(0, SeekOrigin.Begin);
			defaultMethod.Erase(strm, dataSize, prng, chainCallbackHandler);
			baseCompleted = 0.5F;

			//Seek to the end - 16kb, and write.
			strm.Seek(-dataSize, SeekOrigin.End);
			defaultMethod.Erase(strm, long.MaxValue, prng, chainCallbackHandler);
		}

		/// <summary>
		/// The amount of data to be erased from the header and the end of the file.
		/// </summary>
		private const long dataSize = 16 * 1024;
	}
}
