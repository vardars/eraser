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
		public abstract uint Passes
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
		/// A simple callback for clients to retrieve progress information from
		/// the erase method.
		/// </summary>
		/// <param name="currentProgress">A value from 0 to 100 stating the
		/// percentage progress of the erasure.</param>
		/// <param name="currentPass">The current pass number. The total number
		/// of passes can be found from the Passes property.</param>
		public delegate void OnProgress(uint currentProgress, uint currentPass);

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

			public override uint Passes
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
		/// method. Using this variable when passing it to erase functions taking
		/// an IErasureMethod argument is acceptable.
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
