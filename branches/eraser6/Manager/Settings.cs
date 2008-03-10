using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	public partial class Globals
	{
		public static Settings Settings = new Settings();
	}

	/// <summary>
	/// Settings class. Holds the defaults for the manager's operations.
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// The default file erasure method. This is a GUID since methods are
		/// implemented through plugins and plugins may not be loaded and missing
		/// references may follow.
		/// </summary>
		public Guid DefaultFileErasureMethod
		{
			get
			{
				lock (this)
					return defaultFileErasureMethod;
			}
			set
			{
				lock (this)
					defaultFileErasureMethod = value;
			}
		}

		/// <summary>
		/// The default unused space erasure method. This is a GUID since methods
		/// are implemented through plugins and plugins may not be loaded and
		/// missing references may follow.
		/// </summary>
		public Guid DefaultUnusedSpaceErasureMethod
		{
			get
			{
				lock (this)
					return defaultUnusedSpaceErasureMethod;
			}
			set
			{
				lock (this)
					defaultUnusedSpaceErasureMethod = value;
			}
		}

		/// <summary>
		/// The PRNG used. This is a GUID since PRNGs are implemented through
		/// plugins and plugins may not be loaded and missing references may follow.
		/// </summary>
		public Guid ActivePRNG
		{
			get
			{
				lock (this)
					return activePRNG;
			}
			set
			{
				lock (this)
					activePRNG = value;
			}
		}

		/// <summary>
		/// Whether files which are locked when being erased can be scheduled for
		/// erasure on system restart.
		/// </summary>
		public bool AllowFilesToBeErasedOnRestart
		{
			get
			{
				lock (this)
					return allowFilesToBeErasedOnRestart;
			}
			set
			{
				lock (this)
					allowFilesToBeErasedOnRestart = value;
			}
		}

		/// <summary>
		/// Whether scheduling files for restart erase should get the blessing of
		/// the user first.
		/// </summary>
		public bool ConfirmWithUserBeforeReschedulingErase
		{
			get
			{
				lock (this)
					return confirmWithUserBeforeReschedulingErase;
			}
			set
			{
				lock (this)
					confirmWithUserBeforeReschedulingErase = value;
			}
		}

		/// <summary>
		/// Whether missed tasks should be run when the program next starts.
		/// </summary>
		public bool ExecuteMissedTasksImmediately
		{
			get
			{
				lock (this)
					return executeMissedTasksImmediately;
			}
			set
			{
				lock (this)
					executeMissedTasksImmediately = value;
			}
		}

		public Guid defaultFileErasureMethod = Guid.Empty;
		public Guid defaultUnusedSpaceErasureMethod = Guid.Empty;
		public Guid activePRNG = Guid.Empty;
		public bool allowFilesToBeErasedOnRestart = true;
		public bool confirmWithUserBeforeReschedulingErase = true;
		public bool executeMissedTasksImmediately = true;
	}
}
