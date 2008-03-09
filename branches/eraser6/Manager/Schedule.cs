using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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
			get
			{
				string result = string.Empty;
				switch (type)
				{
					case ScheduleUnit.DAILY:
						result += "Once every ";
						if (frequency != 1)
							result += string.Format("{0} days", frequency);
						else
							result += "day";
						break;
					case ScheduleUnit.WEEKDAYS:
						result += "Every weekday";
						break;
					case ScheduleUnit.WEEKLY:
						result += "Every ";
						if ((weeklySchedule & DaysOfWeek.MONDAY) != 0)
							result += "Monday, ";
						if ((weeklySchedule & DaysOfWeek.TUESDAY) != 0)
							result += "Tuesday, ";
						if ((weeklySchedule & DaysOfWeek.WEDNESDAY) != 0)
							result += "Wednesday, ";
						if ((weeklySchedule & DaysOfWeek.THURSDAY) != 0)
							result += "Thursday, ";
						if ((weeklySchedule & DaysOfWeek.FRIDAY) != 0)
							result += "Friday, ";
						if ((weeklySchedule & DaysOfWeek.SATURDAY) != 0)
							result += "Saturday, ";
						if ((weeklySchedule & DaysOfWeek.SUNDAY) != 0)
							result += "Sunday, ";

						result += string.Format("once every {0} week{1}.", frequency,
							frequency == 1 ? "s" : "");
						break;
					case ScheduleUnit.MONTHLY:
						result += "Monthly, ";
						break;
				}

				return result + string.Format(", at {0}", executionTime.TimeOfDay.ToString());
			}
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
		/// The time of day where the task will be executed.
		/// </summary>
		public DateTime ExecutionTime
		{
			get { return ExecutionTime; }
			set { executionTime = value; }
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
				//Get a good starting point, either now, or the last time the task
				//was run.
				DateTime nextRun = LastRun;
				if (nextRun == DateTime.MinValue)
					nextRun = DateTime.Now;

				switch (Type)
				{
					case ScheduleUnit.DAILY:
						//First assume that it is today that we are running the schedule
						long daysToAdd = (DateTime.Now - nextRun).Days;
						if (daysToAdd % frequency != 0)
							daysToAdd += frequency - (daysToAdd % frequency);
						nextRun = nextRun.AddDays(daysToAdd);
						nextRun = nextRun.AddHours(executionTime.Hour);
						nextRun = nextRun.AddMinutes(executionTime.Minute);

						//If we have passed today's run time, schedule it after the next
						//frequency
						if (nextRun < DateTime.Now)
							nextRun = nextRun.AddDays(1);
						break;
					case ScheduleUnit.WEEKDAYS:
						while (nextRun < DateTime.Now ||
							lastRun.DayOfWeek == DayOfWeek.Saturday ||
							lastRun.DayOfWeek == DayOfWeek.Sunday)
							nextRun = nextRun.AddDays(1);
						break;
					case ScheduleUnit.WEEKLY:
						//First find the next day of the week within this week.
						for (DayOfWeek day = lastRun.DayOfWeek; day <= DayOfWeek.Sunday; ++day)
							if (((int)weeklySchedule & (1 << (int)day)) != 0)
								//Bullseye! Run the task next day in the week.
								nextRun = nextRun.AddDays(day - lastRun.DayOfWeek);

						//Ok, we now need to find the earliest day of the week where
						//the task will run.
						throw new NotImplementedException("Weekly schedule calculation not implemented");
						break;
					case ScheduleUnit.MONTHLY:
						//Increment the month until we are past our current date.
						nextRun = nextRun.AddMinutes(executionTime.Minute - nextRun.Minute);
						nextRun = nextRun.AddHours(executionTime.Hour - nextRun.Hour);
						nextRun = nextRun.AddDays(-((int)monthlySchedule - nextRun.Day));
						while (nextRun < DateTime.Now)
							nextRun = nextRun.AddMonths((int)frequency);
						break;
				}

				Debug.WriteLine(string.Format("Next scheduled run time, for object " +
					"with last run time {0} and schedule {1}", lastRun.ToString(),
					UIText));
				return nextRun;
			}
		}

		private ScheduleUnit type;
		private uint frequency;
		private DateTime executionTime;
		private DaysOfWeek weeklySchedule;
		private uint monthlySchedule;

		private DateTime lastRun;
	}
}
