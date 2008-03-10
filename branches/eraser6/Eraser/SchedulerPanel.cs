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
	public partial class SchedulerPanel : Eraser.BasePanel
	{
		public SchedulerPanel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Adds a task to the list of scheduled tasks
		/// </summary>
		/// <param name="task">The task object. The task object will be modified
		/// to have its progress event firing to one of the member functions of
		/// the panel.</param>
		public void AddTask(ref Task task)
		{
			//Insert the item into the list-view.
			ListViewItem item = scheduler.Items.Add(GenerateTaskName(task));
			if (task.Schedule is RecurringSchedule)
				item.SubItems.Add((task.Schedule as RecurringSchedule).NextRun.
					ToString(DateTimeFormatInfo.CurrentInfo.FullDateTimePattern));
			else
				item.SubItems.Add(task.Schedule.UIText);
			item.SubItems.Add(string.Empty);

			//Set the group of the task.
			if (task.Schedule == Schedule.RunNow)
				item.Group = scheduler.Groups["immediate"];
			else if (task.Schedule == Schedule.RunOnRestart)
				item.Group = scheduler.Groups["restart"];
			else
				item.Group = scheduler.Groups["recurring"];

			//Set the tag of the item so we know which task on the LV corresponds
			//to the physical task object.
			item.Tag = task.ID;

			//Set the handler for the progress event
			task.ProgressChanged += new Task.ProgressEventFunction(task_ProgressChanged);
		}

		/// <summary>
		/// Determines the task name to display, deciding on whether a task name is
		/// provided by the user.
		/// </summary>
		/// <param name="task">The task object for which a name is to be generated</param>
		/// <returns>A task name, may not be unique.</returns>
		private string GenerateTaskName(Task task)
		{
			//Simple case, the task name was given by the user.
			if (task.Name.Length != 0)
				return task.Name;

			string result = string.Empty;
			if (task.Entries.Count < 3)
				//Simpler case, small set of data.
				foreach (Task.ErasureTarget tgt in task.Entries)
					result += tgt.UIText + ", ";
			else
				//Ok, we've quite a few entries, get the first, the mid and the end.
				for (int i = 0; i < task.Entries.Count; i += task.Entries.Count / 3)
					result += task.Entries[i].UIText + ", ";
			return result.Substring(0, result.Length - 2);
		}

		/// <summary>
		/// Handles the progress event by the task.
		/// </summary>
		/// <param name="e">Event Argument.</param>
		void task_ProgressChanged(TaskProgressEventArgs e)
		{
			//Make sure we handle the event in the main thread as this requires
			//GUI calls.
			if (scheduler.InvokeRequired)
			{
				Task.ProgressEventFunction func =
					new Task.ProgressEventFunction(task_ProgressChanged);
				this.Invoke(func, new object[] { e });
				return;
			}

			//Find the list view item
			foreach (ListViewItem item in scheduler.Items)
				if ((uint)item.Tag == e.Task.ID)
				{
					//Update the text
					item.SubItems[2].Text = string.Format("{0}%", e.CurrentItemProgress);
				}
		}
	}
}

