using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// The globals class. This static class holds global variables used in
	/// the other Manager classes.
	/// </summary>
	public static class Globals
	{
		/// <summary>
		/// The global instance of the PRNG Manager
		/// </summary>
		internal static PRNGManager PRNGManager = new PRNGManager();

		/// <summary>
		/// The global instance of the Erasure method manager.
		/// </summary>
		internal static ErasureMethodManager ErasureMethodManager = new ErasureMethodManager();

		/// <summary>
		/// Global instance of the Settings object.
		/// </summary>
		public static Settings Settings = new Settings();

		/// <summary>
		/// The global instance of the Plugin host
		/// </summary>
		internal static Plugin.DefaultHost Host =
			new Plugin.DefaultHost();
	}

	/// <summary>
	/// Fatal exception class.
	/// </summary>
	internal class FatalException : Exception
	{
		public FatalException(string message)
			: base(message)
		{
		}

		public FatalException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
