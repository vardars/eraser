using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Eraser.Util;

namespace Eraser.Manager
{
	/// <summary>
	/// An interface class for all pseudorandom number generators used for the
	/// random data erase passes.
	/// </summary>
	public abstract class PRNG : Random
	{
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// The name of this erase pass, used for display in the UI
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// The GUID for this PRNG.
		/// </summary>
		public abstract Guid GUID
		{
			get;
		}

		/// <summary>
		/// Reseeds the PRNG. This can be called by inherited classes, but its most
		/// important function is to provide new seeds regularly. The PRNGManager
		/// will call this function once in a whle to maintain the quality of
		/// generated numbers.
		/// </summary>
		/// <param name="seed">An arbitrary length of information that will be
		/// used to reseed the PRNG</param>
		//protected internal abstract void Reseed(byte[] seed);
	}

	/// <summary>
	/// Class managing all the PRNG algorithms.
	/// </summary>
	public class PRNGManager
	{
		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each PRNG.</returns>
		public static Dictionary<Guid, PRNG> GetAll()
		{
			lock (Globals.PRNGManager.prngs)
				return Globals.PRNGManager.prngs;
		}

		/// <summary>
		/// Retrieves the instance of the PRNG with the given GUID.
		/// </summary>
		/// <param name="guid">The GUID of the PRNG.</param>
		/// <returns>The PRNG instance.</returns>
		public static PRNG GetInstance(Guid guid)
		{
			try
			{
				lock (Globals.PRNGManager.prngs)
					return Globals.PRNGManager.prngs[guid];
			}
			catch (KeyNotFoundException)
			{
				throw new FatalException("PRNG not found: " + guid.ToString());
			}
		}

		/// <summary>
		/// Allows plug-ins to register PRNGs with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void Register(PRNG prng)
		{
			lock (Globals.PRNGManager.prngs)
				Globals.PRNGManager.prngs.Add(prng.GUID, prng);
		}

		private EntropyThread entropyThread = new EntropyThread();

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private Dictionary<Guid, PRNG> prngs = new Dictionary<Guid, PRNG>();
	}

	/// <summary>
	/// A thread class that will get data from the system to let PRNGs use as seeds.
	/// </summary>
	internal class EntropyThread
	{
		public EntropyThread()
		{
			pool = new byte[512];
			thread = new Thread(delegate()
				{
					this.Main();
				}
			);
			thread.Start();
		}

		/// <summary>
		/// The PRNG entropy thread. This thread will run in the background, getting
		/// random data to be used for entropy. This will maintain the integrity
		/// of generated data from the PRNGs.
		/// </summary>
		private void Main()
		{
			//First add the simple startup random information.
			{
				//Process startup information
				Kernel.STARTUPINFO startupInfo = new Kernel.STARTUPINFO();
				Kernel.GetStartupInfo(out startupInfo);
				AddEntropy(startupInfo);

				//System information
				Kernel.SYSTEM_INFO systemInfo = new Kernel.SYSTEM_INFO();
				Kernel.GetSystemInfo(out systemInfo);
				AddEntropy(systemInfo);

				MixPool();
			}

			//This entropy thread will utilize a polling loop.
			while (thread.ThreadState != ThreadState.AbortRequested)
			{
				FastAddEntropy();

				Thread.Sleep(3000);
			}
		}

		/// <summary>
		/// Mixes the contents of the pool.
		/// </summary>
		private void MixPool()
		{
			lock (poolLock)
			using (SHA512 hash = SHA512.Create())
			{
				//Mix the last 128 bytes first.
				const int mixBlockSize = 128;
				int hashSize = hash.HashSize / 8;
				hash.ComputeHash(pool, pool.Length - mixBlockSize, mixBlockSize).CopyTo(pool, 0);

				//Then mix the following bytes until wraparound is required
				int i = 0;
				for (; i < pool.Length - hashSize; i += hashSize)
					hash.ComputeHash(pool, i, mixBlockSize).CopyTo(pool, i);

				//Mix the remaining blocks which require copying from the front
				byte[] combinedBuffer = new byte[mixBlockSize];
				for (; i < pool.Length; i += hashSize)
				{
					for (int j = i; j < pool.Length; ++j)
						combinedBuffer[j - i] = pool[j];
					for (int j = 0, k = mixBlockSize - (pool.Length - i); j < k; ++j)
						combinedBuffer[j + pool.Length - i] = pool[j];
					hash.ComputeHash(combinedBuffer, 0, mixBlockSize).CopyTo(pool, i);
				}
			}
		}

		/// <summary>
		/// Adds data which is random to the pool
		/// </summary>
		/// <param name="entropy">An array of data which will be XORed with pool
		/// contents.</param>
		public unsafe void AddEntropy(byte[] entropy)
		{
			lock (poolLock)
			fixed (byte* pEntropy = entropy)
			{
				//Add entropy to the pool by XORing every value with the given entropy.
				byte* bytes = (byte*)pEntropy;
				for (int i = 0; i < entropy.Length; ++i)
				{
					if (poolPosition == pool.Length)
						poolPosition = 0;
					pool[poolPosition++] ^= bytes[i];
				}
			}
		}

		public unsafe void AddEntropy<T>(T entropy) where T : struct
		{
			int sizeofObject = Marshal.SizeOf(entropy);
			IntPtr memory = Marshal.AllocHGlobal(sizeofObject);
			try
			{
				Marshal.StructureToPtr(entropy, memory, false);
				byte* pMemory = (byte*)memory.ToPointer();
				byte[] dest = new byte[sizeofObject];

				//Copy the memory
				for (int i = 0; i != dest.Length; ++i)
					dest[i] = *pMemory++;

				//Add entropy
				AddEntropy(dest);
			}
			finally
			{
				Marshal.FreeHGlobal(memory);
			}
		}

		/// <summary>
		/// Adds random data to the pool.
		/// </summary>
		/// <param name="entropy">Data to add to the pool.</param>
		public unsafe void AddEntropy(long entropy)
		{
			byte* pEntropy = (byte*)&entropy;
			byte[] array = new byte[sizeof(long)];
			for (int i = 0; i < array.Length; ++i)
				array[i] = *pEntropy++;
			AddEntropy(array);
		}

		/// <summary>
		/// Adds random data to the pool.
		/// </summary>
		/// <param name="entropy">Data to add to the pool.</param>
		public unsafe void AddEntropy(IntPtr entropy)
		{
			AddEntropy(entropy.ToInt64());
		}

		/// <summary>
		/// Adds entropy to the pool. The sources of the entropya data is queried
		/// quickly.
		/// </summary>
		private void FastAddEntropy()
		{
			//Add the free disk space to the pool
			AddEntropy(Drive.GetFreeSpace(Environment.SystemDirectory));

			//Miscellaneous window handles
			AddEntropy(UI.GetCapture());
			AddEntropy(UI.GetClipboardOwner());
			AddEntropy(UI.GetClipboardViewer());
			AddEntropy(UI.GetDesktopWindow());
			AddEntropy(UI.GetForegroundWindow());
			AddEntropy(UI.GetMessagePos());
			AddEntropy(UI.GetMessageTime());
			AddEntropy(UI.GetOpenClipboardWindow());
			AddEntropy(UI.GetProcessWindowStation());
			AddEntropy(Kernel.GetCurrentProcessId());
			AddEntropy(Kernel.GetCurrentThreadId());
			AddEntropy(Kernel.GetProcessHeap());

			//The caret and cursor positions
			UI.POINT point;
			UI.GetCaretPos(out point);
			AddEntropy(point);
			UI.GetCursorPos(out point);
			AddEntropy(point);

			//Amount of free memory
			Kernel.MEMORYSTATUSEX memoryStatus = new Kernel.MEMORYSTATUSEX();
			memoryStatus.dwLength = (uint)Marshal.SizeOf(memoryStatus);
			if (Kernel.GlobalMemoryStatusEx(ref memoryStatus))
			{
				AddEntropy(memoryStatus.ullAvailPhys);
				AddEntropy(memoryStatus.ullAvailVirtual);
			}

			//Thread execution times
			long creationTime, exitTime, kernelTime, userTime;
			if (Kernel.GetThreadTimes(Kernel.GetCurrentThread(), out creationTime,
				out exitTime, out kernelTime, out userTime))
			{
				AddEntropy(creationTime);
				AddEntropy(kernelTime);
				AddEntropy(userTime);
			}

			//Process execution times
			if (Kernel.GetProcessTimes(Kernel.GetCurrentProcess(), out creationTime,
				out exitTime, out kernelTime, out userTime))
			{
				AddEntropy(creationTime);
				AddEntropy(kernelTime);
				AddEntropy(userTime);
			}

			//Current system time
			AddEntropy(DateTime.Now.Ticks);

			//The high resolution performance counter
			long perfCount = 0;
			if (Kernel.QueryPerformanceCounter(out perfCount))
				AddEntropy(perfCount);

			//Ticks since start up
			uint tickCount = Kernel.GetTickCount();
			if (tickCount != 0)
				AddEntropy(tickCount);

			//CryptGenRandom
			byte[] cryptGenRandom = new byte[160];
			if (CryptAPI.CryptGenRandom(cryptGenRandom))
				AddEntropy(cryptGenRandom);
		}

		/// <summary>
		/// The thread object.
		/// </summary>
		Thread thread;

		/// <summary>
		/// The pool of data which we currently maintain.
		/// </summary>
		byte[] pool;

		/// <summary>
		/// The next position where entropy will be added to the pool.
		/// </summary>
		int poolPosition = 0;

		/// <summary>
		/// The lock guarding the pool array and the current entropy addition index.
		/// </summary>
		object poolLock = new object();
	}
}
