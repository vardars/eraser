using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Eraser.Util;
using Microsoft.Win32.SafeHandles;

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
				KernelAPI.STARTUPINFO startupInfo = new KernelAPI.STARTUPINFO();
				KernelAPI.GetStartupInfo(out startupInfo);
				AddEntropy(startupInfo);

				//System information
				KernelAPI.SYSTEM_INFO systemInfo = new KernelAPI.SYSTEM_INFO();
				KernelAPI.GetSystemInfo(out systemInfo);
				AddEntropy(systemInfo);

				MixPool();
			}

			//This entropy thread will utilize a polling loop.
			while (thread.ThreadState != ThreadState.AbortRequested)
			{
				FastAddEntropy();
				SlowAddEntropy();
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

		/// <summary>
		/// Adds data which is random to the pool
		/// </summary>
		/// <typeparam name="T">Any value type</typeparam>
		/// <param name="entropy">A value which will be XORed with pool contents.</param>
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
		/// Adds entropy to the pool. The sources of the entropy data is queried
		/// quickly.
		/// </summary>
		private void FastAddEntropy()
		{
			//Add the free disk space to the pool
			AddEntropy(Drive.GetFreeSpace(Environment.SystemDirectory));

			//Miscellaneous window handles
			AddEntropy(UserAPI.GetCapture());
			AddEntropy(UserAPI.GetClipboardOwner());
			AddEntropy(UserAPI.GetClipboardViewer());
			AddEntropy(UserAPI.GetDesktopWindow());
			AddEntropy(UserAPI.GetForegroundWindow());
			AddEntropy(UserAPI.GetMessagePos());
			AddEntropy(UserAPI.GetMessageTime());
			AddEntropy(UserAPI.GetOpenClipboardWindow());
			AddEntropy(UserAPI.GetProcessWindowStation());
			AddEntropy(KernelAPI.GetCurrentProcessId());
			AddEntropy(KernelAPI.GetCurrentThreadId());
			AddEntropy(KernelAPI.GetProcessHeap());

			//The caret and cursor positions
			UserAPI.POINT point;
			UserAPI.GetCaretPos(out point);
			AddEntropy(point);
			UserAPI.GetCursorPos(out point);
			AddEntropy(point);

			//Amount of free memory
			KernelAPI.MEMORYSTATUSEX memoryStatus = new KernelAPI.MEMORYSTATUSEX();
			memoryStatus.dwLength = (uint)Marshal.SizeOf(memoryStatus);
			if (KernelAPI.GlobalMemoryStatusEx(ref memoryStatus))
			{
				AddEntropy(memoryStatus.ullAvailPhys);
				AddEntropy(memoryStatus.ullAvailVirtual);
			}

			//Thread execution times
			long creationTime, exitTime, kernelTime, userTime;
			if (KernelAPI.GetThreadTimes(KernelAPI.GetCurrentThread(), out creationTime,
				out exitTime, out kernelTime, out userTime))
			{
				AddEntropy(creationTime);
				AddEntropy(kernelTime);
				AddEntropy(userTime);
			}

			//Process execution times
			if (KernelAPI.GetProcessTimes(KernelAPI.GetCurrentProcess(), out creationTime,
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
			if (KernelAPI.QueryPerformanceCounter(out perfCount))
				AddEntropy(perfCount);

			//Ticks since start up
			uint tickCount = KernelAPI.GetTickCount();
			if (tickCount != 0)
				AddEntropy(tickCount);

			//CryptGenRandom
			byte[] cryptGenRandom = new byte[160];
			if (CryptAPI.CryptGenRandom(cryptGenRandom))
				AddEntropy(cryptGenRandom);
		}

		/// <summary>
		/// Adds entropy to the pool. The sources of the entropy data is queried
		/// relatively slowly compared to the FastAddEntropy function.
		/// </summary>
		private void SlowAddEntropy()
		{
			//NetAPI statistics
			unsafe
			{
				IntPtr netAPIStats = IntPtr.Zero;
				if (NetAPI.NetStatisticsGet(null, NetAPI.SERVICE_WORKSTATION,
					0, 0, out netAPIStats) == 0)
				{
					//Get the size of the buffer
					uint size = 0;
					NetAPI.NetApiBufferSize(netAPIStats, out size);
					byte[] entropy = new byte[size];

					//Copy the buffer
					fixed (byte* fEntropy = entropy)
					{
						byte* pSrc = (byte*)netAPIStats.ToPointer();
						byte* pEntropy = fEntropy;
						while (size-- != 0)
							*pEntropy++ = *pSrc++;
					}

					//And add it to the pool
					AddEntropy(entropy);

					//Free the statistics buffer
					NetAPI.NetApiBufferFree(netAPIStats);
				}
			}

#if false
			//Get disk I/O statistics for all the hard drives
			for (int drive = 0; ; ++drive)
			{
				//Try to open the drive.
				using (SafeFileHandle hDevice = File.CreateFile(
					string.Format("\\\\.\\PhysicalDrive%d", drive), 0,
					File.FILE_SHARE_READ | File.FILE_SHARE_WRITE, IntPtr.Zero,
					File.OPEN_EXISTING, 0, IntPtr.Zero))
				{
					if (hDevice.IsInvalid)
						break;

					//This only works if the user has turned on the disk performance
					//counters with 'diskperf -y'. These counters are off by default
					if (File.DeviceIoControl(hDevice, IOCTL_DISK_PERFORMANCE, NULL, 0,
						&diskPerformance, sizeof(DISK_PERFORMANCE), &uSize, NULL))
					{
						addEntropy(&diskPerformance, uSize);
					}
				}
			}
#endif

			/*
			 Query performance data. Because the Win32 version of this API (through
			 registry) may be buggy, use the NT Native API instead.
			 
			 Scan the first 64 possible information types (we don't bother
			 with increasing the buffer size as we do with the Win32
			 version of the performance data read, we may miss a few classes
			 but it's no big deal).  In addition the returned size value for
			 some classes is wrong (eg 23 and 24 return a size of 0) so we
			 miss a few more things, but again it's no big deal.  This scan
			 typically yields around 20 pieces of data, there's nothing in
			 the range 65...128 so chances are there won't be anything above
			 there either.
			*/
			uint dataWritten = 0;
			byte[] infoBuffer = new byte[65536];
			uint totalEntropy = 0;
			for (uint infoType = 0; infoType < 64; ++infoType)
			{
				uint result = NTAPI.NtQuerySystemInformation(infoType, infoBuffer,
					(uint)infoBuffer.Length, out dataWritten);

				if (result == 0 /*ERROR_SUCCESS*/ && dataWritten > 0)
				{
					byte[] entropy = new byte[dataWritten];
					for (int i = 0; i < dataWritten; ++i)
						entropy[i] = infoBuffer[i];
					AddEntropy(entropy);
					totalEntropy += dataWritten;
				}
			}

			//Finally, our good friend CryptGenRandom()
			byte[] cryptGenRandom = new byte[1536];
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
