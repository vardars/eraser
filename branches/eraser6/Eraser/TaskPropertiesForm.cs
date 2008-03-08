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
			typeOneTime.Checked = true;
			scheduleDaily.Checked = true;
		}

		/// <summary>
		/// Sets or retrieves the task object to be edited or being edited.
		/// </summary>
		public Task Task
		{
			get { return task; }
			set { task = value; UpdateUIFromTask(); }
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
			oneTimePanel.Visible = !typeRecurring.Checked;
			
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
			scheduleMonthlyByDay.Enabled = scheduleMonthlyByDayEveryLbl.Enabled =
				scheduleMonthlyByDayMonthLbl.Enabled = scheduleMonthlyRelative.Enabled =
				scheduleMonthlyRelativeEveryLbl.Enabled =
				scheduleMonthlyRelativeFreqLbl.Enabled = scheduleMonthly.Checked &&
				typeRecurring.Checked;

			scheduleDailySpan_CheckedChanged(sender, e);
			scheduleMonthlySpan_CheckedChanged(sender, e);
		}

		private void scheduleDailySpan_CheckedChanged(object sender, EventArgs e)
		{
			scheduleDailyByDayFreq.Enabled = scheduleDailyByDay.Checked &&
				scheduleDaily.Checked && typeRecurring.Checked;
		}

		private void scheduleMonthlySpan_CheckedChanged(object sender, EventArgs e)
		{
			scheduleMonthlyByDayNumber.Enabled = scheduleMonthlyByDayFreq.Enabled =
				scheduleMonthlyByDay.Checked && scheduleMonthly.Checked &&
				typeRecurring.Checked;
			scheduleMonthlyRelativeWeek.Enabled = scheduleMonthlyRelativeWeekDay.Enabled =
				scheduleMonthlyRelativeFreq.Enabled = scheduleMonthlyRelative.Checked &&
				scheduleMonthly.Checked && typeRecurring.Checked;
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

			//Set the name of the task
			task.Name = name.Text;

			//And the schedule, if selected.
			if (typeRecurring.Checked)
			{

			}

			//Close the dialog
			DialogResult = DialogResult.OK;
			Close();
		}

		private Task task = new Task();
	}
}