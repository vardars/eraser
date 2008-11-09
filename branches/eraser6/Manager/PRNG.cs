/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: Kasra Nasiri <cjax@users.sourceforge.net> @10/7/2008
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Microsoft.Win32.SafeHandles;
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
		protected internal abstract void Reseed(byte[] seed);

		#region Random members
		public override int Next(int maxValue)
		{
			if (maxValue == 0)
				return 0;
			return Next() % maxValue;
		}

		public override int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
				throw new ArgumentOutOfRangeException("minValue", minValue, "minValue is greater than maxValue");
			else if (minValue == maxValue)
				return minValue;
			return (Next() % (maxValue - minValue)) + minValue;
		}

		public unsafe override int Next()
		{
			//Declare a return variable
			int result;
			int* fResult = &result;

			//Get the random-valued bytes to fill the int.
			byte[] rand = new byte[sizeof(int)];
			NextBytes(rand);

			//Copy the random buffer into the int.
			fixed (byte* fRand = rand)
			{
				byte* pResult = (byte*)fResult;
				byte* pRand = fRand;
				for (int i = 0; i != sizeof(int); ++i)
					*pResult++ = *pRand++;
			}

			return Math.Abs(result);
		}

		protected unsafe override double Sample()
		{
			//Declare a return variable
			double result;
			double* fResult = &result;

			//Get the random-valued bytes to fill the int.
			byte[] rand = new byte[sizeof(double)];
			NextBytes(rand);

			//Copy the random buffer into the int.
			fixed (byte* fRand = rand)
			{
				byte* pResult = (byte*)fResult;
				byte* pRand = fRand;
				for (int i = 0; i != sizeof(double); ++i)
					*pResult++ = *pRand++;
			}

			return result;
		}

		public abstract override void NextBytes(byte[] buffer);
		#endregion
	}

	/// <summary>
	/// Class managing all the PRNG algorithms.
	/// </summary>
	public class PRNGManager
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PRNGManager()
		{
			entropyThread.AddEntropySource(new KernelEntropySource());
		}

		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each PRNG.</returns>
		public static Dictionary<Guid, PRNG> GetAll()
		{
			lock (ManagerLibrary.Instance.PRNGManager.prngs)
				return ManagerLibrary.Instance.PRNGManager.prngs;
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
				lock (ManagerLibrary.Instance.PRNGManager.prngs)
					return ManagerLibrary.Instance.PRNGManager.prngs[guid];
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
			lock (ManagerLibrary.Instance.PRNGManager.prngs)
				ManagerLibrary.Instance.PRNGManager.prngs.Add(prng.GUID, prng);
		}

		/// <summary>
		/// Allows the EntropyThread to get entropy to the PRNG functions as seeds.
		/// </summary>
		/// <param name="entropy">An array of bytes, being entropy for the PRNG.</param>
		internal void AddEntropy(byte[] entropy)
		{
			lock (ManagerLibrary.Instance.PRNGManager.prngs)
				foreach (PRNG prng in prngs.Values)
					prng.Reseed(entropy);
		}

		/// <summary>
		/// Gets entropy from the EntropyThread.
		/// </summary>
		/// <returns>A buffer of arbitrary length containing random information.</returns>
		public static byte[] GetEntropy()
		{
			return ManagerLibrary.Instance.PRNGManager.entropyThread.GetPool();
		}
		
		/// <summary>
		/// The entropy thread gathering entropy for the RNGs.
		/// </summary>
		internal EntropyPoller entropyThread = new EntropyPoller();

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private Dictionary<Guid, PRNG> prngs = new Dictionary<Guid, PRNG>();
	}

	/// <summary>
	/// A class which uses EntropyPoll class to fetch system data as a source of
	/// randomness at "regular" but "random" intervals
	/// </summary>
	class EntropyPoller
	{
		/// <summary>
		/// The algorithm used for mixing
		/// </summary>
		private enum PRFAlgorithms
		{
			MD5,
			SHA1,
			RIPEMD160,
			SHA256,
			SHA384,
			SHA512,
		};

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntropyPoller()
		{
			//Create the pool.
			pool = new byte[sizeof(uint) * 128];

			//Then start the thread which maintains the pool.
			Thread = new Thread(delegate()
				{
					this.Main();
				}
			);
			Thread.Start();
		}

		/// <summary>
		/// The PRNG entropy thread. This thread will run in the background, getting
		/// random data to be used for entropy. This will maintain the integrity
		/// of generated data from the PRNGs.
		/// </summary>
		private void Main()
		{
			//This entropy thread will utilize a polling loop.
			DateTime lastAddedEntropy = DateTime.Now;
			TimeSpan managerEntropySpan = new TimeSpan(0, 10, 0);
			Stopwatch st = new Stopwatch();

			while (Thread.ThreadState != System.Threading.ThreadState.AbortRequested)
			{
				st.Start();
				lock (EntropySources)
					foreach (EntropySource src in EntropySources)
					{
						byte[] entropy = src.GetEntropy();
						AddEntropy(entropy);
					}
				
				st.Stop(); 
				Thread.Sleep(2000 + (int)(st.ElapsedTicks % 2049L));
				st.Reset();

				//Send entropy to the PRNGs for new seeds.
				if (DateTime.Now - lastAddedEntropy > managerEntropySpan)
					ManagerLibrary.Instance.PRNGManager.AddEntropy(GetPool());
			}
		}
		
		/// <summary>
		/// Stops the execution of the thread.
		/// </summary>
		public void Abort()
		{
			Thread.Abort();
		}

		/// <summary>
		/// Adds a new Entropy Source to the Poller.
		/// </summary>
		/// <param name="source">The EntropySource object to add.</param>
		public void AddEntropySource(EntropySource source)
		{
			lock (EntropySources)
				EntropySources.Add(source);

			AddEntropy(source.GetPrimer());
			MixPool();

			//Apply whitening effect
			PRFAlgorithm = PRFAlgorithms.RIPEMD160;
			MixPool();
			PRFAlgorithm = PRFAlgorithms.SHA512;
		}

		/// <summary>
		/// Retrieves the current contents of the entropy pool.
		/// </summary>
		/// <returns>A byte array containing all the randomness currently found.</returns>
		public byte[] GetPool()
		{
			//Mix and invert the pool
			MixPool();
			InvertPool();

			//Return a safe copy
			lock (pool)
			{
				byte[] result = new byte[pool.Length];
				pool.CopyTo(result, 0);

				return result;
			}
		}

		/// <summary>
		/// Inverts the contents of the pool
		/// </summary>
		private void InvertPool()
		{
			lock (poolLock)
				unsafe
				{
					fixed (byte* fPool = pool)
					{
						uint* pPool = (uint*)fPool;
						uint poolLength = (uint)(pool.Length / sizeof(uint));
						while (poolLength-- != 0)
							*pPool = (uint)(*pPool++ ^ unchecked((uint)-1));
					}
				}
		}

		/// <summary>
		/// Mixes the contents of the pool.
		/// </summary>
		private void MixPool()
		{
			lock (poolLock)
			{
				//Mix the last 128 bytes first.
				const int mixBlockSize = 128;
				int hashSize = PRF.HashSize / 8;
				PRF.ComputeHash(pool, pool.Length - mixBlockSize, mixBlockSize).CopyTo(pool, 0);

				//Then mix the following bytes until wraparound is required
				int i = 0;
				for (; i < pool.Length - hashSize; i += hashSize)
					Buffer.BlockCopy(PRF.ComputeHash(pool, i,
						i + mixBlockSize >= pool.Length ? pool.Length - i : mixBlockSize),
						0, pool, i, i + hashSize >= pool.Length ? pool.Length - i : hashSize);

				//Mix the remaining blocks which require copying from the front
				byte[] combinedBuffer = new byte[mixBlockSize];
				for (; i < pool.Length; i += hashSize)
				{
					Buffer.BlockCopy(pool, i, combinedBuffer, 0, pool.Length - i);

					Buffer.BlockCopy(pool, 0, combinedBuffer, pool.Length - i,
								mixBlockSize - (pool.Length - i));

					Buffer.BlockCopy(PRF.ComputeHash(combinedBuffer, 0, mixBlockSize), 0,
						pool, i, pool.Length - i > hashSize ? hashSize : pool.Length - i);
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
				fixed (byte* pPool = pool)
				{
					int size = entropy.Length;
					byte* mpEntropy = pEntropy;
					while (size > 0)
					{
						//Bring the pool position back to the front if we are at our end
						if (poolPosition >= pool.Length)
							poolPosition = 0;

						int amountToMix = Math.Min(size, pool.Length - poolPosition);
						MemoryXor(pPool + poolPosition, mpEntropy, amountToMix);
						mpEntropy = mpEntropy + amountToMix;
						size -= amountToMix;
					}
				}
		}

		/// <summary>
		/// XOR's memory a DWORD at a time.
		/// </summary>
		/// <param name="destination">The destination buffer to be XOR'ed</param>
		/// <param name="source">The source buffer to XOR with</param>
		/// <param name="size">The size of the source buffer</param>
		private static unsafe void MemoryXor(byte* destination, byte* source, int size)
		{
			int wsize = size / sizeof(uint);
			size -= wsize * sizeof(uint);
			uint* d = (uint*)destination;
			uint* s = (uint*)source;

			while (wsize-- > 0)
				*d++ ^= *s++;

			if (size > 0)
			{
				byte* db = (byte*)d,
				      ds = (byte*)s;
				while (size-- > 0)
					*db++ ^= *ds++;
			}
		}

		/// <summary>
		/// PRF algorithm handle
		/// </summary>
		private HashAlgorithm PRF
		{
			get
			{
				Type type = null;
				switch (PRFAlgorithm)
				{
					case PRFAlgorithms.MD5:
						type = typeof(MD5CryptoServiceProvider);
						break;
					case PRFAlgorithms.SHA1:
						type = typeof(SHA1Managed);
						break;
					case PRFAlgorithms.RIPEMD160:
						type = typeof(RIPEMD160Managed);
						break;
					case PRFAlgorithms.SHA256:
						type = typeof(SHA256Managed);
						break;
					case PRFAlgorithms.SHA384:
						type = typeof(SHA384Managed);
						break;
					default:
						type = typeof(SHA512Managed);
						break;
				}

				if (type.IsInstanceOfType(prfCache))
					return prfCache;
				ConstructorInfo hashConstructor = type.GetConstructor(Type.EmptyTypes);
				return prfCache = (HashAlgorithm)hashConstructor.Invoke(null);
			}
		}

		/// <summary>
		/// The last created PRF algorithm handle.
		/// </summary>
		private HashAlgorithm prfCache;

		/// <summary>
		/// PRF algorithm identifier
		/// </summary>
		private PRFAlgorithms PRFAlgorithm = PRFAlgorithms.SHA512;

		/// <summary>
		/// The pool of data which we currently maintain.
		/// </summary>
		private byte[] pool;

		/// <summary>
		/// The next position where entropy will be added to the pool.
		/// </summary>
		private int poolPosition = 0;

		/// <summary>
		/// The lock guarding the pool array and the current entropy addition index.
		/// </summary>
		private object poolLock = new object();

		/// <summary>
		/// The thread object.
		/// </summary>
		Thread Thread;

		/// <summary>
		/// The list of entropy sources registered with the Poller.
		/// </summary>
		private List<EntropySource> EntropySources = new List<EntropySource>();
	}
	
	/// <summary>
	/// Provides an abstract interface to allow multiple sources of entropy into
	/// the EntropyPoller class.
	/// </summary>
	public abstract class EntropySource
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntropySource()
		{
		}

		/// <summary>
		/// Gets a primer to add to the pool when this source is first initialised, to
		/// further add entropy to the pool.
		/// </summary>
		/// <returns>A byte array containing the entropy.</returns>
		public abstract byte[] GetPrimer();

		/// <summary>
		/// Gets entropy from the entropy source. This will be called repetitively.
		/// </summary>
		/// <returns>A byte array containing the entropy.</returns>
		public abstract byte[] GetEntropy();

		/// <summary>
		/// Converts value types into a byte array. This is a helper function to allow
		/// inherited classes to convert value types into byte arrays which can be
		/// returned to the EntropyPoller class.
		/// </summary>
		/// <typeparam name="T">Any value type</typeparam>
		/// <param name="entropy">A value which will be XORed with pool contents.</param>
		protected unsafe static byte[] StructToBuffer<T>(T entropy) where T: struct
		{
			int sizeofObject = Marshal.SizeOf(entropy);
			IntPtr memory = Marshal.AllocHGlobal(sizeofObject);
			try
			{
				Marshal.StructureToPtr(entropy, memory, false);
				byte[] dest = new byte[sizeofObject];

				//Copy the memory
				Marshal.Copy(memory, dest, 0, sizeofObject);
				return dest;
			}
			finally
			{
				Marshal.FreeHGlobal(memory);
			}
		}
	}

	/// <summary>
	/// Provides means of generating random entropy from the system or user space
	/// randomness.
	/// </summary>
	internal class KernelEntropySource : EntropySource
	{
		public override byte[] GetPrimer()
		{
			List<byte> result = new List<byte>();

			//Process startup information
			KernelAPI.STARTUPINFO startupInfo = new KernelAPI.STARTUPINFO();
			KernelAPI.GetStartupInfo(out startupInfo);
			result.AddRange(StructToBuffer(startupInfo));

			//System information
			KernelAPI.SYSTEM_INFO systemInfo = new KernelAPI.SYSTEM_INFO();
			KernelAPI.GetSystemInfo(out systemInfo);
			result.AddRange(StructToBuffer(systemInfo));

			result.AddRange(GetFastEntropy());
			result.AddRange(GetSlowEntropy());
			return result.ToArray();
		}

		public override byte[] GetEntropy()
		{
			List<byte> result = new List<byte>();
			result.AddRange(GetFastEntropy());
			result.AddRange(GetSlowEntropy());
			
			return result.ToArray();
		}

		/// <summary>
		/// Retrieves entropy from quick sources.
		/// </summary>
		private byte[] GetFastEntropy()
		{
			List<byte> result = new List<byte>();

			//Add the free disk space to the pool
			result.AddRange(StructToBuffer(new DriveInfo(new DirectoryInfo(Environment.SystemDirectory).
				Root.FullName).TotalFreeSpace));

			//Miscellaneous window handles
			result.AddRange(StructToBuffer(UserAPI.GetCapture()));
			result.AddRange(StructToBuffer(UserAPI.GetClipboardOwner()));
			result.AddRange(StructToBuffer(UserAPI.GetClipboardViewer()));
			result.AddRange(StructToBuffer(UserAPI.GetDesktopWindow()));
			result.AddRange(StructToBuffer(UserAPI.GetForegroundWindow()));
			result.AddRange(StructToBuffer(UserAPI.GetMessagePos()));
			result.AddRange(StructToBuffer(UserAPI.GetMessageTime()));
			result.AddRange(StructToBuffer(UserAPI.GetOpenClipboardWindow()));
			result.AddRange(StructToBuffer(UserAPI.GetProcessWindowStation()));
			result.AddRange(StructToBuffer(KernelAPI.GetCurrentProcessId()));
			result.AddRange(StructToBuffer(KernelAPI.GetCurrentThreadId()));
			result.AddRange(StructToBuffer(KernelAPI.GetProcessHeap()));

			//The caret and cursor positions
			UserAPI.POINT point;
			UserAPI.GetCaretPos(out point);
			result.AddRange(StructToBuffer(point));
			UserAPI.GetCursorPos(out point);
			result.AddRange(StructToBuffer(point));

			//Amount of free memory
			KernelAPI.MEMORYSTATUSEX memoryStatus = new KernelAPI.MEMORYSTATUSEX();
			memoryStatus.dwLength = (uint)Marshal.SizeOf(memoryStatus);
			if (KernelAPI.GlobalMemoryStatusEx(ref memoryStatus))
			{
				result.AddRange(StructToBuffer(memoryStatus.ullAvailPhys));
				result.AddRange(StructToBuffer(memoryStatus.ullAvailVirtual));
				result.AddRange(StructToBuffer(memoryStatus));
			}

			//Thread execution times
			long creationTime, exitTime, kernelTime, userTime;
			if (KernelAPI.GetThreadTimes(KernelAPI.GetCurrentThread(), out creationTime,
				out exitTime, out kernelTime, out userTime))
			{
				result.AddRange(StructToBuffer(creationTime));
				result.AddRange(StructToBuffer(kernelTime));
				result.AddRange(StructToBuffer(userTime));
			}

			//Process execution times
			if (KernelAPI.GetProcessTimes(KernelAPI.GetCurrentProcess(), out creationTime,
				out exitTime, out kernelTime, out userTime))
			{
				result.AddRange(StructToBuffer(creationTime));
				result.AddRange(StructToBuffer(kernelTime));
				result.AddRange(StructToBuffer(userTime));
			}

			//Current system time
			result.AddRange(StructToBuffer(DateTime.Now.Ticks));

			//The high resolution performance counter
			long perfCount = 0;
			if (KernelAPI.QueryPerformanceCounter(out perfCount))
				result.AddRange(StructToBuffer(perfCount));

			//Ticks since start up
			uint tickCount = KernelAPI.GetTickCount();
			if (tickCount != 0)
				result.AddRange(StructToBuffer(tickCount));

			//CryptGenRandom
			byte[] cryptGenRandom = new byte[160];
			if (CryptAPI.CryptGenRandom(cryptGenRandom))
				result.AddRange(cryptGenRandom);

			return result.ToArray();
		}

		/// <summary>
		/// Retrieves entropy from sources which are relatively slower than those from
		/// the FastAddEntropy function.
		/// </summary>
		private byte[] GetSlowEntropy()
		{
			List<byte> result = new List<byte>();

			//NetAPI statistics
			unsafe
			{
				IntPtr netAPIStats = IntPtr.Zero;
				if (NetAPI.NetStatisticsGet(null, NetAPI.SERVICE_WORKSTATION,
					0, 0, out netAPIStats) == 0)
				{
					try
					{
						//Get the size of the buffer
						uint size = 0;
						NetAPI.NetApiBufferSize(netAPIStats, out size);
						byte[] entropy = new byte[size];

						//Copy the buffer
						Marshal.Copy(entropy, 0, netAPIStats, entropy.Length);

						//And add it to the pool
						result.AddRange(entropy);
					}
					finally
					{
						//Free the statistics buffer
						NetAPI.NetApiBufferFree(netAPIStats);
					}
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
				uint sysInfo = NTAPI.NtQuerySystemInformation(infoType, infoBuffer,
					(uint)infoBuffer.Length, out dataWritten);

				if (sysInfo == 0 /*ERROR_SUCCESS*/ && dataWritten > 0)
				{
					byte[] entropy = new byte[dataWritten];
					Buffer.BlockCopy(infoBuffer, 0, entropy, 0, (int)dataWritten);
					result.AddRange(entropy);
					totalEntropy += dataWritten;
				}
			}

			result.AddRange(StructToBuffer(totalEntropy));

			//Finally, our good friend CryptGenRandom()
			byte[] cryptGenRandom = new byte[1536];
			if (CryptAPI.CryptGenRandom(cryptGenRandom))
				result.AddRange(cryptGenRandom);

			return result.ToArray();
		}
	}
}
