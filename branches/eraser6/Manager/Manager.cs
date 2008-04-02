/* 
 * $Id$
 * Copyright 2008 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By:
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Foobar is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

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
		public ManagerLibrary(Settings settings)
		{
			Instance = this;
			Settings = settings;

			PRNGManager = new PRNGManager();
			LanguageManager = new LanguageManager();
			ErasureMethodManager = new ErasureMethodManager();
			Host = new Plugin.DefaultHost();
		}

		public void Dispose()
		{
			PRNGManager.entropyThread.Abort();
			Host.Dispose();
			Settings.Save();
			Instance = null;
		}

		/// <summary>
		/// The global library instance.
		/// </summary>
		public static ManagerLibrary Instance = null;

		/// <summary>
		/// The global instance of the PRNG Manager.
		/// </summary>
		internal PRNGManager PRNGManager;

		/// <summary>
		/// Global instance of the Language Manager.
		/// </summary>
		internal LanguageManager LanguageManager;

		/// <summary>
		/// The global instance of the Erasure method manager.
		/// </summary>
		internal ErasureMethodManager ErasureMethodManager;

		/// <summary>
		/// Global instance of the Settings object.
		/// </summary>
		public Settings Settings;

		/// <summary>
		/// The global instance of the Plugin host.
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
