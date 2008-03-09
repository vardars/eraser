using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// An interface class for all pseudorandom number generators used for the
	/// random data erase passes.
	/// </summary>
	public abstract class PRNG
	{
		#region Registrar fields
		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each PRNG.</returns>
		public static List<PRNG> GetMethods()
		{
			lock (prngs)
				return prngs.GetRange(0, prngs.Count);
		}

		/// <summary>
		/// Allows plug-ins to register PRNGs with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void RegisterPRNG(PRNG method)
		{
			lock (prngs)
				prngs.Add(method);
		}

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private static List<PRNG> prngs = new List<PRNG>();
		#endregion
	}
}
