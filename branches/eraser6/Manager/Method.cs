using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// An interface class representing the method for erasure.
	/// </summary>
	public abstract class EraseMethod
	{
		private class DefaultErase : EraseMethod
		{
			public DefaultErase()
			{
				EraseMethod.methods.Add(this);
			}

			public override string Name
			{
				get { return "(default)";  }
			}
		}

		/// <summary>
		/// Retrieves all currently registered erasure methods.
		/// </summary>
		/// <returns>A list, with an instance of each method.</returns>
		public static List<EraseMethod> GetMethods()
		{
			return methods.GetRange(0, methods.Count);
		}

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
		/// The list of currently registered erasure methods.
		/// </summary>
		private static List<EraseMethod> methods = new List<EraseMethod>();

		/// <summary>
		/// A dummy method placeholder used for representing the default erase
		/// method. Using this variable when passing it to erase functions taking
		/// an EraseMethod argument is acceptable.
		/// </summary>
		public static readonly EraseMethod Default = new DefaultErase();

	}
}
