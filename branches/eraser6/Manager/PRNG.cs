using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	internal partial class Globals
	{
		public static PRNGManager PRNGManager = new PRNGManager();
	}

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
		public static List<PRNG> GetGenerators()
		{
			lock (Globals.PRNGManager.prngs)
				return Globals.PRNGManager.prngs.GetRange(0,
					Globals.PRNGManager.prngs.Count);
		}

		/// <summary>
		/// Allows plug-ins to register PRNGs with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void Register(PRNG method)
		{
			lock (Globals.PRNGManager.prngs)
				Globals.PRNGManager.prngs.Add(method);
		}

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private List<PRNG> prngs = new List<PRNG>();
	}
}
