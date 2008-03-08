using System;
using System.Collections.Generic;
using System.Text;

namespace Eraser.Manager
{
	/// <summary>
	/// Base class for all schedule types.
	/// </summary>
	public abstract class Schedule
	{
		private class RunNowSchedule : Schedule
		{
			public override string UIText
			{
				get { return "Pending execution..."; }
			}
		}

		private class RunOnRestartSchedule : Schedule
		{
			public override string UIText
			{
				get { return "Running when restarted"; }
			}
		}

		/// <summary>
		/// Retrieves the text that should be displayed detailing the nature of
		/// the schedule for use in user interface elements.
		/// </summary>
		public abstract string UIText
		{
			get;
		}

		/// <summary>
		/// The global value for tasks which should be run immediately.
		/// </summary>
		public static readonly Schedule RunNow = new RunNowSchedule();

		/// <summary>
		/// The global value for tasks which should be run when the computer is
		/// restarted
		/// </summary>
		public static readonly Schedule RunOnRestart = new RunOnRestartSchedule();
	}

	public class RecurringSchedule : Schedule
	{
		public override string UIText
		{
			get { return "Recurring schedule, not implemented."; }
		}
	}
}
