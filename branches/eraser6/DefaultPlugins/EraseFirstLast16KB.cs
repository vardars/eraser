using System;
using System.Collections.Generic;
using System.Text;

using Eraser.Manager;
using Eraser.Util;
using System.IO;
using System.Threading;

namespace Eraser.DefaultPlugins
{
	class FirstLast16KB : ErasureMethod
	{
		public override string Name
		{
			get { return S._("First/last 16KB Erasure"); }
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
			//Amount of data required to be written.
			long amountToWrite = 0;
			if (paths == null)
			{
				if (targetSize <= dataSize)
					amountToWrite = targetSize;
				else
					amountToWrite = dataSize * 2;
			}
			else
				amountToWrite = paths.Count * dataSize * 2;

			//The final amount has to be multiplied by the number of passes.
			return amountToWrite * Method.Passes;
		}

		public override void Erase(Stream strm, long erasureLength, PRNG prng,
			OnProgress callback)
		{
			//Make sure that the erasureLength passed in here is the maximum value
			//for the size of long, since we don't want to write extra or write
			//less.
			if (erasureLength != long.MaxValue)
				throw new ArgumentException(S._("The amount of data erased should not be " +
					"limited, since this is a self-limiting erasure method."));

			try
			{
				//Acquire a lock on the cached method object and set it if unset.
				cachedMethodSemaphore.WaitOne();
				if (cachedMethod == null)
					cachedMethod = Method;

				//If the target stream is shorter than 16kb, just forward it to the default
				//function.
				if (strm.Length < dataSize)
				{
					cachedMethod.Erase(strm, erasureLength, prng, callback);
					return;
				}

				//Seek to the beginning and write 16kb.
				strm.Seek(0, SeekOrigin.Begin);
				cachedMethod.Erase(strm, dataSize, prng, callback);

				//Seek to the end - 16kb, and write.
				strm.Seek(-dataSize, SeekOrigin.End);
				cachedMethod.Erase(strm, long.MaxValue, prng, callback);
			}
			finally
			{
				//Check if no other threads are using the cached erasure method.
				if (cachedMethodSemaphore.Release() == int.MaxValue - 1)
					//If so, we no longer need to cache.
					cachedMethod = null;
			}
		}

		private ErasureMethod Method
		{
			get
			{
				//If there are threads locking the cached method variable just
				//return that
				if (cachedMethod != null)
					return cachedMethod;

				//Try to retrieve the set erasure method
				ErasureMethod defaultMethod = ErasureMethodManager.GetInstance(
					(Guid)DefaultPlugin.Settings["FL16Method"]);

				//If we have no default or we are the default then throw an exception
				if (defaultMethod == null || defaultMethod.GUID == GUID)
					throw new InvalidOperationException(S._("The First/last 16KB erasure method " +
						"requires another erasure method to erase the file.\n\nThis can " +
						"be set in the Plugin Settings dialog."));

				return defaultMethod;
			}
		}

		/// <summary>
		/// The amount of data to be erased from the header and the end of the file.
		/// </summary>
		private const long dataSize = 16 * 1024;

		/// <summary>
		/// The cached erasure method. This is used when erasing so that the properties
		/// of this class is not inconsistent in the event that the method of erasure
		/// changes while executing.
		/// </summary>
		ErasureMethod cachedMethod = null;

		/// <summary>
		/// Tracks the number of erasures locking the cached method variable.
		/// </summary>
		Semaphore cachedMethodSemaphore = new Semaphore(int.MaxValue, int.MaxValue);
	}
}
