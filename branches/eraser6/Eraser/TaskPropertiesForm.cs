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

		private void dataAdd_Click(object sender, EventArgs e)
		{
			using (TaskDataSelectionForm form = new TaskDataSelectionForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Task.EraseTarget entry = form.GetTaskEntry();
					ListViewItem item = null;
					if (entry is Task.FilesystemObject)
						item = data.Items.Add((entry as Task.FilesystemObject).Path);
					else if (entry is Task.FreeSpace)
						item = data.Items.Add("Unused space on " + (entry as Task.FreeSpace).Drive);
					else
						throw new NotImplementedException("Unimplemented data erasure type.");
					item.SubItems.Add(entry.Method.Name);
					task.Entries.Add(entry);
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

		private Task task = new Task();
	}
}