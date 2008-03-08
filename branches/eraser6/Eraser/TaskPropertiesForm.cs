using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;

namespace Eraser
{
	public partial class TaskPropertiesForm : Form
	{
		public TaskPropertiesForm()
		{
			InitializeComponent();

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

				if (scheduleDaily.Checked)
				{
					if (scheduleDailyByDay.Checked)
					{
						schedule.Type = RecurringSchedule.ScheduleUnit.DAILY;
						schedule.Frequency = (uint)scheduleDailyByDayFreq.Value;
					}
					else
					{
						schedule.Type = RecurringSchedule.ScheduleUnit.WEEKDAYS;
					}
				}
				else if (scheduleWeekly.Checked)
				{
					schedule.Type = RecurringSchedule.ScheduleUnit.WEEKLY;
					schedule.Frequency = (uint)scheduleWeeklyFreq.Value;
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
					schedule.Frequency = (uint)scheduleMonthlyFreq.Value;
					schedule.MonthlySchedule = (uint)scheduleMonthlyDayNumber.Value;
				}
				else
					throw new ArgumentOutOfRangeException("No such scheduling method");
			}
		}

		private void UpdateUIFromTask()
		{
			throw new NotImplementedException("UpdateUIFromTask not implemented.");
		}

		private void dataAdd_Click(object sender, EventArgs e)
		{
			using (TaskDataSelectionForm form = new TaskDataSelectionForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Task.EraseTarget entry = form.GetTaskEntry();
					ListViewItem item = data.Items.Add(entry.UIText);
					
					item.SubItems.Add(entry.Method.Name);
					task.Entries.Add(entry);
					errorProvider.Clear();
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
				errorProvider.SetError(data, "The task has no data to erase.");
				return;
			}

			errorProvider.Clear();

			//Close the dialog
			DialogResult = DialogResult.OK;
			Close();
		}

		private Task task = new Task();
	}
}