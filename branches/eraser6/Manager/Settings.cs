using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
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
		public bool EraseLockedFilesOnRestart
		{
			get
			{
				lock (this)
					return eraseLockedFilesOnRestart;
			}
			set
			{
				lock (this)
					eraseLockedFilesOnRestart = value;
			}
		}

		/// <summary>
		/// Whether scheduling files for restart erase should get the blessing of
		/// the user first.
		/// </summary>
		public bool ConfirmEraseOnRestart
		{
			get
			{
				lock (this)
					return confirmEraseOnRestart;
			}
			set
			{
				lock (this)
					confirmEraseOnRestart = value;
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
		public bool eraseLockedFilesOnRestart = true;
		public bool confirmEraseOnRestart = true;
		public bool executeMissedTasksImmediately = true;
	}
}
