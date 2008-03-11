using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Globalization;
using Eraser.Manager;

namespace Eraser
{
	public partial class TaskPropertiesForm : Form
	{
		public TaskPropertiesForm()
		{
			InitializeComponent();
			scheduleTime.CustomFormat = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;

			//Set a default task type
			typeImmediate.Checked = true;
			scheduleDaily.Checked = true;
		}

		/// <summary>
		/// Sets or retrieves the task object to be edited or being edited.
		/// </summary>
		public Task Task
		{
			get { UpdateTaskFromUI(); return task; }
			set { task = value; UpdateUIFromTask(); }
		}

		/// <summary>
		/// Updates the local task object from the UI elements.
		/// </summary>
		private void UpdateTaskFromUI()
		{
			//Set the name of the task
			task.Name = name.Text;

			//And the schedule, if selected.
			if (typeImmediate.Checked)
			{
				task.Schedule = Schedule.RunNow;
			}
			else if (typeRestart.Checked)
			{
				task.Schedule = Schedule.RunOnRestart;
			}
			else if (typeRecurring.Checked)
			{
				RecurringSchedule schedule = new RecurringSchedule();
				task.Schedule = schedule;
				schedule.ExecutionTime = new DateTime(1, 1, 1, scheduleTime.Value.Hour,
					scheduleTime.Value.Minute, scheduleTime.Value.Second);

				if (scheduleDaily.Checked)
				{
					if (scheduleDailyByDay.Checked)
					{
						schedule.Type = RecurringSchedule.ScheduleUnit.DAILY;
						schedule.Frequency = (int)scheduleDailyByDayFreq.Value;
					}
					else
					{
						schedule.Type = RecurringSchedule.ScheduleUnit.WEEKDAYS;
					}
				}
				else if (scheduleWeekly.Checked)
				{
					schedule.Type = RecurringSchedule.ScheduleUnit.WEEKLY;
					schedule.Frequency = (int)scheduleWeeklyFreq.Value;
					RecurringSchedule.DaysOfWeek weeklySchedule = 0;
					if (scheduleWeeklyMonday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.MONDAY;
					if (scheduleWeeklyTuesday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.TUESDAY;
					if (scheduleWeeklyWednesday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.WEDNESDAY;
					if (scheduleWeeklyThursday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.THURSDAY;
					if (scheduleWeeklyFriday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.FRIDAY;
					if (scheduleWeeklySaturday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.SATURDAY;
					if (scheduleWeeklySunday.Checked)
						weeklySchedule |= RecurringSchedule.DaysOfWeek.SUNDAY;
					schedule.WeeklySchedule = weeklySchedule;
				}
				else if (scheduleMonthly.Checked)
				{
					schedule.Type = RecurringSchedule.ScheduleUnit.MONTHLY;
					schedule.Frequency = (int)scheduleMonthlyFreq.Value;
					schedule.MonthlySchedule = (int)scheduleMonthlyDayNumber.Value;
				}
				else
					throw new NotImplementedException("No such scheduling method");
			}
		}

		/// <summary>
		/// Updates the UI elements to reflect the data in the Task object.
		/// </summary>
		private void UpdateUIFromTask()
		{
			//Set the name of the task
			name.Text = task.Name;

			//The data
			foreach (Task.ErasureTarget target in task.Entries)
			{
				ListViewItem item = data.Items.Add(target.UIText);
				item.SubItems.Add(target.Method.Name);
			}

			//And the schedule, if selected.
			if (task.Schedule == Schedule.RunNow)
			{
				typeImmediate.Checked = true;
			}
			else if (task.Schedule == Schedule.RunOnRestart)
			{
				typeRestart.Checked = true;
			}
			else
			{
				typeRecurring.Checked = true;
				RecurringSchedule schedule = (RecurringSchedule)task.Schedule;
				scheduleTime.Value = scheduleTime.MinDate.Add(schedule.ExecutionTime.TimeOfDay);

				switch (schedule.Type)
				{
					case RecurringSchedule.ScheduleUnit.DAILY:
						scheduleDailyByDay.Checked = true;
						scheduleDailyByDayFreq.Value = schedule.Frequency;
						break;
					case RecurringSchedule.ScheduleUnit.WEEKDAYS:
						scheduleDailyByWeekday.Checked = true;
						break;
					case RecurringSchedule.ScheduleUnit.WEEKLY:
						scheduleWeeklyFreq.Value = schedule.Frequency;
						scheduleWeeklyMonday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.MONDAY) != 0;
						scheduleWeeklyTuesday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.TUESDAY) != 0;
						scheduleWeeklyWednesday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.WEDNESDAY) != 0;
						scheduleWeeklyThursday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.THURSDAY) != 0;
						scheduleWeeklyFriday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.FRIDAY) != 0;
						scheduleWeeklySaturday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.SATURDAY) != 0;
						scheduleWeeklySunday.Checked =
							(schedule.WeeklySchedule & RecurringSchedule.DaysOfWeek.SUNDAY) != 0;
						break;
					case RecurringSchedule.ScheduleUnit.MONTHLY:
						scheduleMonthlyFreq.Value = schedule.Frequency;
						scheduleMonthlyDayNumber.Value = schedule.MonthlySchedule;
						break;
					default:
						throw new NotImplementedException("Unknown schedule type.");
				}
			}
		}

		/// <summary>
		/// Triggered when the user clicks on the Add Data button.
		/// </summary>
		/// <param name="sender">The button.</param>
		/// <param name="e">Event argument.</param>
		private void dataAdd_Click(object sender, EventArgs e)
		{
			using (TaskDataSelectionForm form = new TaskDataSelectionForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Task.ErasureTarget target = form.Target;
					ListViewItem item = data.Items.Add(target.UIText);
					
					item.SubItems.Add(target.Method.Name);
					task.Entries.Add(target);
					errorProvider.Clear();
				}
			}
		}

		/// <summary>
		/// Generated when the user double-clicks an item in the list-view.
		/// </summary>
		/// <param name="sender">The list-view which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void data_ItemActivate(object sender, EventArgs e)
		{
			using (TaskDataSelectionForm form = new TaskDataSelectionForm())
			{
				ListViewItem item = data.SelectedItems[0];
				form.Target = task.Entries[item.Index];

				if (form.ShowDialog() == DialogResult.OK)
				{
					Task.ErasureTarget target = form.Target;
					task.Entries[item.Index] = target;
					item.Text = target.UIText;
					item.SubItems[1].Text = target.Method.Name;
				}
			}
		}

		private void taskType_CheckedChanged(object sender, EventArgs e)
		{
			scheduleTimeLbl.Enabled = scheduleTime.Enabled = schedulePattern.Enabled =
				scheduleDaily.Enabled = scheduleWeekly.Enabled =
				scheduleMonthly.Enabled = typeRecurring.Checked;
			nonRecurringPanel.Visible = !typeRecurring.Checked;
			
			scheduleSpan_CheckedChanged(sender, e);
		}

		private void scheduleSpan_CheckedChanged(object sender, EventArgs e)
		{
			scheduleDailyByDay.Enabled = scheduleDailyByDayLbl.Enabled =
				scheduleDailyByWeekday.Enabled = scheduleDaily.Checked &&
				typeRecurring.Checked;
			scheduleWeeklyLbl.Enabled = scheduleWeeklyFreq.Enabled =
				scheduleWeeklyFreqLbl.Enabled = scheduleWeeklyMonday.Enabled =
				scheduleWeeklyTuesday.Enabled = scheduleWeeklyWednesday.Enabled =
				scheduleWeeklyThursday.Enabled = scheduleWeeklyFriday.Enabled =
				scheduleWeeklySaturday.Enabled = scheduleWeeklySunday.Enabled =
				scheduleWeekly.Checked && typeRecurring.Checked;
			scheduleMonthlyLbl.Enabled = scheduleMonthlyDayNumber.Enabled =
				scheduleMonthlyEveryLbl.Enabled = scheduleMonthlyFreq.Enabled =
				scheduleMonthlyMonthLbl.Enabled = scheduleMonthly.Checked &&
				typeRecurring.Checked;

			scheduleDailySpan_CheckedChanged(sender, e);
		}

		private void scheduleDailySpan_CheckedChanged(object sender, EventArgs e)
		{
			scheduleDailyByDayFreq.Enabled = scheduleDailyByDay.Checked &&
				scheduleDaily.Checked && typeRecurring.Checked;
		}

		private void ok_Click(object sender, EventArgs e)
		{
			if (data.Items.Count == 0)
			{
				errorProvider.SetIconPadding(data, -16);
				errorProvider.SetIconAlignment(data, ErrorIconAlignment.BottomRight);
				errorProvider.SetError(data, "The task has no data to erase.");
				container.SelectedIndex = 0;
				return;
			}
			else if (typeRecurring.Checked && scheduleWeekly.Checked)
			{
				if (!scheduleWeeklyMonday.Checked && !scheduleWeeklyTuesday.Checked &&
					!scheduleWeeklyWednesday.Checked && !scheduleWeeklyThursday.Checked &&
					!scheduleWeeklyFriday.Checked && !scheduleWeeklySaturday.Checked &&
					!scheduleWeeklySunday.Checked)
				{
					errorProvider.SetIconPadding(scheduleWeeklyDays, -16);
					errorProvider.SetError(scheduleWeeklyDays, "The task needs to run " +
						"on at least one day a week");
					container.SelectedIndex = 1;
					return;
				}
			}

			errorProvider.Clear();

			//Close the dialog
			DialogResult = DialogResult.OK;
			Close();
		}

		private Task task = new Task();
	}
}