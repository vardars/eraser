using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// The library instance which initializes and cleans up data required for the
	/// library to function.
	/// </summary>
	public class ManagerLibrary : IDisposable
	{
		public ManagerLibrary()
		{
			Instance = this;
			PRNGManager = new PRNGManager();
			ErasureMethodManager = new ErasureMethodManager();
			Settings = new Settings();
			Host = new Plugin.DefaultHost();
		}

		public void Dispose()
		{
			PRNGManager.entropyThread.Abort();
		}

		/// <summary>
		/// The global library instance.
		/// </summary>
		public static ManagerLibrary Instance = null;

		/// <summary>
		/// The global instance of the PRNG Manager
		/// </summary>
		internal PRNGManager PRNGManager;

		/// <summary>
		/// The global instance of the Erasure method manager.
		/// </summary>
		internal ErasureMethodManager ErasureMethodManager;

		/// <summary>
		/// Global instance of the Settings object.
		/// </summary>
		public Settings Settings;

		/// <summary>
		/// The global instance of the Plugin host
		/// </summary>
		internal Plugin.DefaultHost Host;
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
