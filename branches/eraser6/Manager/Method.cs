using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eraser.Manager
{
	internal partial class Globals
	{
		public static ErasureMethodManager ErasureMethodManager =
			new ErasureMethodManager();
	}

	/// <summary>
	/// An interface class representing the method for erasure.
	/// </summary>
	public interface IErasureMethod
	{
		/// <summary>
		/// The name of this erase pass, used for display in the UI
		/// </summary>
		string Name
		{
			get;
		}

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
		void Erase(Stream strm, PRNG prng);
	}

	/// <summary>
	/// Class managing all the erasure methods.
	/// </summary>
	public class ErasureMethodManager
	{
		#region Default Erase method
		private class DefaultErase : IErasureMethod
		{
			public DefaultErase()
			{
				ErasureMethodManager.RegisterMethod(this);
			}

			string IErasureMethod.Name
			{
				get { return "(default)"; }
			}

			void IErasureMethod.Erase(Stream strm, PRNG prng)
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// A dummy method placeholder used for representing the default erase
		/// method. Using this variable when passing it to erase functions taking
		/// an IErasureMethod argument is acceptable.
		/// </summary>
		public static readonly IErasureMethod Default = new DefaultErase();
		#endregion

		#region Registrar fields
		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each method.</returns>
		public static List<IErasureMethod> GetMethods()
		{
			lock (Globals.ErasureMethodManager.methods)
				return Globals.ErasureMethodManager.methods.GetRange(0,
					Globals.ErasureMethodManager.methods.Count);
		}

		/// <summary>
		/// Allows plug-ins to register methods with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void RegisterMethod(IErasureMethod method)
		{
			lock (Globals.ErasureMethodManager.methods)
				Globals.ErasureMethodManager.methods.Add(method);
		}

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private List<IErasureMethod> methods = new List<IErasureMethod>();
		#endregion
	}
}
