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
				get { return "Running on restart"; }
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

	/// <summary>
	/// Recurring runs schedule type.
	/// </summary>
	public class RecurringSchedule : Schedule
	{
		public override string UIText
		{
			get { return "Recurring schedule, not implemented."; }
		}

		/// <summary>
		/// The types of schedule
		/// </summary>
		public enum ScheduleUnit
		{
			/// <summary>
			/// Daily schedule type
			/// </summary>
			DAILY,

			/// <summary>
			/// Weekdays-only schedule type
			/// </summary>
			WEEKDAYS,

			/// <summary>
			/// Weekly schedule type
			/// </summary>
			WEEKLY,

			/// <summary>
			/// Monthly schedule type
			/// </summary>
			MONTHLY
		}

		/// <summary>
		/// The days of the week, with values usable in a bitfield.
		/// </summary>
		public enum DaysOfWeek
		{
			SUNDAY = 1,
			MONDAY = 1 << 1,
			TUESDAY = 1 << 2,
			WEDNESDAY = 1 << 3,
			THURSDAY = 1 << 4,
			FRIDAY = 1 << 5,
			SATURDAY = 1 << 6
		}

		/// <summary>
		/// The type of schedule.
		/// </summary>
		public ScheduleUnit Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// The frequency of the event. This value is valid only with Daily,
		/// Weekly and Monthly schedules.
		/// </summary>
		public uint Frequency
		{
			get
			{
				if (Type != ScheduleUnit.DAILY && Type != ScheduleUnit.WEEKLY &&
					Type != ScheduleUnit.MONTHLY)
					throw new ArgumentException("The ScheduleUnit of the schedule does " +
						"not require a frequency value, this field would contain garbage.");

				return frequency;
			}
			set
			{
				if (value == 0)
					throw new ArgumentException("The frequency of the recurrance should " +
						"be greater than one");

				frequency = value;
			}
		}

		/// <summary>
		/// The days of the week which this task should be run. This is valid only
		/// with Weekly schedules. This field is the DaysOfWeek enumerations
		/// ORed together.
		/// </summary>
		public DaysOfWeek WeeklySchedule
		{
			get
			{
				if (Type != ScheduleUnit.WEEKLY)
					throw new ArgumentException("The ScheduleUnit of the schedule does " +
						"not require the WeeklySchedule value, this field would contain garbage");

				return weeklySchedule;
			}
			set
			{
				if (value == 0)
					throw new ArgumentException("The WeeklySchedule should have at " +
						"least one day where the task should be run.");

				weeklySchedule = value;
			}
		}

		/// <summary>
		/// The nth day of the month on which this task will run. This is valid
		/// only with Monthly schedules
		/// </summary>
		public uint MonthlySchedule
		{
			get
			{
				if (Type != ScheduleUnit.MONTHLY)
					throw new ArgumentException("The ScheduleUnit of the schedule does " +
						"not require the MonthlySchedule value, this field would contain garbage");

				return monthlySchedule;
			}
			set { monthlySchedule = value; }
		}

		/// <summary>
		/// The last time this task was executed. This value is used for computing
		/// the next time the task should be run.
		/// </summary>
		public DateTime LastRun
		{
			get { return lastRun; }
			set { LastRun = value; }
		}

		/// <summary>
		/// Based on the last run time and the current schedule, the next run time
		/// will be computed.
		/// </summary>
		public DateTime NextRun
		{
			get
			{
				DateTime result = LastRun;
				switch (Type)
				{
					case ScheduleUnit.DAILY:
						while (result < DateTime.Now)
							result = result.AddDays(frequency);
						break;
					case ScheduleUnit.WEEKDAYS:
						while (result < DateTime.Now ||
							lastRun.DayOfWeek == DayOfWeek.Saturday ||
							lastRun.DayOfWeek == DayOfWeek.Sunday)
							result = result.AddDays(1.0);
						break;
					case ScheduleUnit.WEEKLY:
						//First find the next day of the week within this week.
						for (DayOfWeek day = lastRun.DayOfWeek; day <= DayOfWeek.Sunday; ++day)
							if (((int)weeklySchedule & (1 << (int)day)) != 0)
								//Bullseye! Run the task next day in the week.
								result = result.AddDays(day - lastRun.DayOfWeek);

						//Ok, we now need to find the earliest day of the week where
						//the task will run.
						throw new NotImplementedException("Weekly schedule calculation not implemented");
						break;
					case ScheduleUnit.MONTHLY:
						//Increment the month until we are past our current date.
						while (lastRun < DateTime.Now)
							result = result.AddMonths((int)frequency);
						result = result.AddDays(-((int)monthlySchedule - result.Day));
						break;
				}

				return result;
			}
		}

		private ScheduleUnit type;
		private uint frequency;
		private DaysOfWeek weeklySchedule;
		private uint monthlySchedule;

		private DateTime lastRun;
	}
}
