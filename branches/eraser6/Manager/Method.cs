using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eraser.Manager
{
	public partial class Globals
	{
		internal static ErasureMethodManager ErasureMethodManager =
			new ErasureMethodManager();
	}

	/// <summary>
	/// An interface class representing the method for erasure.
	/// </summary>
	public abstract class ErasureMethod
	{
		public override string ToString()
		{
			if (Passes == 0)
				return Name;
			return string.Format("{0} ({1} {2})", Name, Passes, "passes");
		}

		/// <summary>
		/// The name of this erase pass, used for display in the UI
		/// </summary>
		public abstract string Name
		{
			get;
		}

		/// <summary>
		/// The number of erase passes for this erasure method.
		/// </summary>
		public abstract int Passes
		{
			get;
		}

		/// <summary>
		/// The GUID for this erasure method.
		/// </summary>
		public abstract Guid GUID
		{
			get;
		}

		/// <summary>
		/// Disk operation write unit. Chosen such that this value mod 3, 4, 512,
		/// and 1024 is 0
		/// </summary>
		protected const int DiskOperationUnit = 1536 * 4096;

		/// <summary>
		/// Unused space erasure file size. Each of the files used in erasing
		/// unused space will be of this size.
		/// </summary>
		protected const int FreeSpaceFileUnit = DiskOperationUnit * 36;

		/// <summary>
		/// A simple callback for clients to retrieve progress information from
		/// the erase method.
		/// </summary>
		/// <param name="currentProgress">A value from 0 to 1 stating the
		/// percentage progress of the erasure.</param>
		/// <param name="currentPass">The current pass number. The total number
		/// of passes can be found from the Passes property.</param>
		public delegate void OnProgress(float currentProgress, int currentPass);

		/// <summary>
		/// The main bit of the class! This function is called whenever data has
		/// to be erased. Erase the stream passed in, using the given PRNG for
		/// randomness where necessary.
		/// 
		/// This function should be implemented thread-safe as using the same
		/// instance, this function may be called across different threads.
		/// </summary>
		/// <param name="strm">The stream which needs to be erased.</param>
		/// <param name="prng">The PRNG source for random data.</param>
		/// <param name="callback">The progress callback function.</param>
		public abstract void Erase(Stream strm, PRNG prng, OnProgress callback);

		/// <summary>
		/// Shuffles the passes in the input array, effectively randomizing the
		/// order or rewrites.
		/// </summary>
		/// <param name="passes">The input set of passes.</param>
		/// <returns>The shuffled set of passes.</returns>
		protected static Pass[] ShufflePasses(Pass[] passes)
		{
			//Make a copy.
			Pass[] result = new Pass[passes.Length];
			passes.CopyTo(result, 0);

			//Randomize.
			PRNG rand = PRNGManager.GetInstance(Globals.Settings.ActivePRNG);
			for (int i = 0; i < result.Length; ++i)
			{
				int val = rand.Next(result.Length - 1);
				Pass tmpPass = result[val];
				result[val] = result[i];
				result[i] = tmpPass;
			}

			return result;
		}

		/// <summary>
		/// Helper function. This function will write random data to the stream
		/// using the provided PRNG.
		/// </summary>
		/// <param name="strm">The buffer to populate with data to write to disk.</param>
		/// <param name="prng">The PRNG used.</param>
		protected static void WriteRandom(ref byte[] buffer, object value)
		{
			((PRNG)value).NextBytes(buffer);
		}

		/// <summary>
		/// Helper function. This function will write the repeating pattern
		/// to the stream.
		/// </summary>
		/// <param name="strm">The buffer to populate with data to write to disk.</param>
		/// <param name="value">The byte[] to write.</param>
		protected static void WriteConstant(ref byte[] buffer, object value)
		{
			byte[] pattern = (byte[])value;
			for (int i = 0; i < buffer.Length; ++i)
				buffer[i] = pattern[i % pattern.Length];
		}

		/// <summary>
		/// The prototype of a pass.
		/// </summary>
		/// <param name="strm">The buffer to populate with data to write to disk.</param>
		/// <param name="opaque">An opaque value, depending on the type of callback.</param>
		protected delegate void PassFunction(ref byte[] buffer, object opaque);

		/// <summary>
		/// A pass object. This object holds both the pass function, as well as the
		/// data used for the pass (random, byte, or triplet)
		/// </summary>
		protected struct Pass
		{
			public override string ToString()
			{
				return OpaqueValue == null ? "Random" : OpaqueValue.ToString();
			}

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="function">The delegate to the function.</param>
			/// <param name="opaqueValue">The opaque value passed to the function.</param>
			public Pass(PassFunction function, object opaqueValue)
			{
				Function = function;
				OpaqueValue = opaqueValue;
			}

			/// <summary>
			/// Executes the pass.
			/// </summary>
			/// <param name="buffer">The buffer to populate with the data to write.</param>
			/// <param name="prng">The PRNG used for random passes.</param>
			public void Execute(ref byte[] buffer, PRNG prng)
			{
				Function(ref buffer, OpaqueValue == null ? prng : OpaqueValue);
			}

			PassFunction Function;
			object OpaqueValue;
		}
	}

	/// <summary>
	/// Class managing all the erasure methods.
	/// </summary>
	public class ErasureMethodManager
	{
		#region Default Erasure method
		private class DefaultMethod : ErasureMethod
		{
			public DefaultMethod()
			{
			}

			public override string Name
			{
				get { return "(default)"; }
			}

			public override int Passes
			{
				get { return 0; }
			}

			public override Guid GUID
			{
				get { return Guid.Empty; }
			}

			public override void Erase(Stream strm, PRNG prng, OnProgress callback)
			{
				throw new NotImplementedException("The DefaultMethod class should never be " +
					"used and should instead be replaced before execution!");
			}
		}

		/// <summary>
		/// A dummy method placeholder used for representing the default erase
		/// method. Do not use this variable when trying to call the erase function,
		/// this is just a placeholder and will throw a NotImplementedException.
		/// </summary>
		public static readonly ErasureMethod Default = new DefaultMethod();
		#endregion

		#region Registrar fields
		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each method.</returns>
		public static Dictionary<Guid, ErasureMethod> GetAll()
		{
			lock (Globals.ErasureMethodManager.methods)
				return Globals.ErasureMethodManager.methods;
		}

		/// <summary>
		/// Retrieves the instance of the erasure method with the given GUID.
		/// </summary>
		/// <param name="guid">The GUID of the erasure method.</param>
		/// <returns>The erasure method instance.</returns>
		public static ErasureMethod GetInstance(Guid guid)
		{
			try
			{
				lock (Globals.ErasureMethodManager.methods)
					return Globals.ErasureMethodManager.methods[guid];
			}
			catch (KeyNotFoundException)
			{
				throw new FatalException("PRNG not found: " + guid.ToString());
			}
		}


		/// <summary>
		/// Allows plug-ins to register methods with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void Register(ErasureMethod method)
		{
			lock (Globals.ErasureMethodManager.methods)
				Globals.ErasureMethodManager.methods.Add(method.GUID, method);
		}

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private Dictionary<Guid, ErasureMethod> methods =
			new Dictionary<Guid, ErasureMethod>();
		#endregion
	}
}
