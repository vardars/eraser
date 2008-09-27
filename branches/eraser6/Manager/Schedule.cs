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
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Eraser.Manager
{
	/// <summary>
	/// Base class for all schedule types.
	/// </summary>
	[Serializable]
	public abstract class Schedule : ISerializable
	{
		#region Default values
		[Serializable]
		private class RunNowSchedule : Schedule
		{
			#region Object serialization
			public RunNowSchedule(SerializationInfo info, StreamingContext context)
			{
			}

			public override void GetObjectData(SerializationInfo info,
				StreamingContext context)
			{
			}
			#endregion

			public RunNowSchedule()
			{
			}

			public override string UIText
			{
				get { return string.Empty; }
			}
		}

		[Serializable]
		private class RunOnRestartSchedule : Schedule
		{
			#region Object serialization
			public RunOnRestartSchedule(SerializationInfo info, StreamingContext context)
			{
			}

			public override void GetObjectData(SerializationInfo info,
				StreamingContext context)
			{
			}
			#endregion

			public RunOnRestartSchedule()
			{
			}

			public override string UIText
			{
				get { return "Running on restart"; }
			}
		}
		#endregion

		/// <summary>
		/// Retrieves the text that should be displayed detailing the nature of
		/// the schedule for use in user interface elements.
		/// </summary>
		public abstract string UIText
		{
			get;
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the
		/// target object.
		/// </summary>
		/// <param name="info">The SerializationInfo to populate with data.</param>
		/// <param name="context">The destination for this serialization.</param>
		public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

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
	[Serializable]
	public class RecurringSchedule : Schedule
	{
		#region Overridden members
		public override string UIText
		{
			get
			{
				string result = string.Empty;
				switch (type)
				{
					case ScheduleUnit.DAILY:
						result = "Once every ";
						if (frequency != 1)
							result += string.Format("{0} days", frequency);
						else
							result += "day";
						break;
					case ScheduleUnit.WEEKDAYS:
						result = "Every weekday";
						break;
					case ScheduleUnit.WEEKLY:
						result = "Every ";
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

						result += frequency == 1 ?
							string.Format("once every {0} week.", frequency) :
							string.Format("once every {0} weeks.", frequency);
						break;
					case ScheduleUnit.MONTHLY:
						result = string.Format("On day {0} of every {1} month{2}",
							monthlySchedule, frequency, frequency != 1 ? "s" : "");
						break;
				}

				return result + string.Format(", at {0}", executionTime.TimeOfDay.ToString());
			}
		}
		#endregion

		#region Object serialization
		public RecurringSchedule(SerializationInfo info, StreamingContext context)
		{
			type = (ScheduleUnit)info.GetValue("Type", typeof(ScheduleUnit));
			frequency = (int)info.GetValue("Frequency", typeof(int));
			executionTime = (DateTime)info.GetValue("ExecutionTime", typeof(DateTime));
			weeklySchedule = (DaysOfWeek)info.GetValue("WeeklySchedule", typeof(DaysOfWeek));
			monthlySchedule = (int)info.GetValue("MonthlySchedule", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info,
			StreamingContext context)
		{
			info.AddValue("Type", type);
			info.AddValue("Frequency", frequency);
			info.AddValue("ExecutionTime", executionTime);
			info.AddValue("WeeklySchedule", weeklySchedule);
			info.AddValue("MonthlySchedule", monthlySchedule);
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public RecurringSchedule()
		{
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
		public int Frequency
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
			get { return executionTime; }
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
		public int MonthlySchedule
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
						nextRun = nextRun.AddDays(daysToAdd);
						nextRun = nextRun.AddHours(executionTime.Hour - nextRun.Hour);
						nextRun = nextRun.AddMinutes(executionTime.Minute - nextRun.Minute);

						//If we have passed today's run time, schedule it after the next
						//frequency
						if (nextRun < DateTime.Now)
							nextRun = nextRun.AddDays(frequency);
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
						throw new NotImplementedException();
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
					"with last run time {0} and schedule {1}, is at {2}",
					lastRun.ToString(), UIText, nextRun.ToString()));
				return nextRun;
			}
		}

		/// <summary>
		/// Reschedules the task.
		/// </summary>
		/// <param name="lastRun">The last time the task was run.</param>
		internal void Reschedule(DateTime lastRun)
		{
			this.lastRun = lastRun;
		}

		private ScheduleUnit type;
		private int frequency;
		private DateTime executionTime;
		private DaysOfWeek weeklySchedule;
		private int monthlySchedule;

		private DateTime lastRun;
	}
}
