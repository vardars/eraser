using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eraser.Manager
{
	/// <summary>
	/// An interface class representing the method for erasure.
	/// </summary>
	public abstract class EraseMethod
	{
		#region Default Erase method
		private class DefaultErase : EraseMethod
		{
			public DefaultErase()
			{
				EraseMethod.methods.Add(this);
			}

			public override string Name
			{
				get { return "(default)"; }
			}

			public override void Erase(Stream strm, PRNG prng)
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// A dummy method placeholder used for representing the default erase
		/// method. Using this variable when passing it to erase functions taking
		/// an EraseMethod argument is acceptable.
		/// </summary>
		public static readonly EraseMethod Default = new DefaultErase();
		#endregion

		#region Registrar fields
		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A mutable list, with an instance of each method.</returns>
		public static List<EraseMethod> GetMethods()
		{
			lock (methods)
				return methods.GetRange(0, methods.Count);
		}

		/// <summary>
		/// Allows plug-ins to register methods with the main program. Thread-safe.
		/// </summary>
		/// <param name="method"></param>
		public static void RegisterMethod(EraseMethod method)
		{
			lock (methods)
				methods.Add(method);
		}

		/// <summary>
		/// The list of currently registered erasure methods.
		/// </summary>
		private static List<EraseMethod> methods = new List<EraseMethod>();
		#endregion

		/// <summary>
		/// Retrieves the name of the erase pass.
		/// </summary>
		/// <returns>A string containing the name of the erase pass.</returns>
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
		/// The main bit of the class! This function is called whenever data has
		/// to be erased. Erase the stream passed in, using the given PRNG for
		/// randomness where necessary.
		/// 
		/// This function should be implemented thread-safe as using the same
		/// instance, this function may be called across different threads.
		/// </summary>
		/// <param name="strm">The stream which needs to be erased.</param>
		/// <param name="prng">The PRNG source for random data.</param>
		public abstract void Erase(Stream strm, PRNG prng);
	}
}
