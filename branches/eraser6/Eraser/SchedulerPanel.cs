using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Globalization;
using Eraser.Manager;
using System.Runtime.InteropServices;

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
		public void AddTask(Task task)
		{
			//Insert the item into the list-view.
			ListViewItem item = scheduler.Items.Add(task.UIText);
			if (task.Schedule is RecurringSchedule)
				item.SubItems.Add((task.Schedule as RecurringSchedule).NextRun.
					ToString(DateTimeFormatInfo.CurrentInfo.FullDateTimePattern));
			else
				item.SubItems.Add(task.Schedule.UIText);
			item.SubItems.Add(String.Empty);

			//Set the group of the task.
			if (task.Schedule == Schedule.RunNow)
				item.Group = scheduler.Groups["immediate"];
			else if (task.Schedule == Schedule.RunOnRestart)
				item.Group = scheduler.Groups["restart"];
			else
				item.Group = scheduler.Groups["recurring"];

			//Set the tag of the item so we know which task on the LV corresponds
			//to the physical task object.
			item.Tag = task;

			//Add our event handlers to the task
			task.TaskStarted += new Task.TaskEventFunction(task_TaskStarted);
			task.ProgressChanged += new Task.ProgressEventFunction(task_ProgressChanged);
			task.TaskFinished += new Task.TaskEventFunction(task_TaskFinished);

			//Then add the task to the executor.
			Program.eraserClient.AddTask(ref task);
		}

		/// <summary>
		/// Handles the task start event.
		/// </summary>
		/// <param name="e">The task event object.</param>
		void task_TaskStarted(TaskEventArgs e)
		{
			if (scheduler.InvokeRequired)
			{
				Task.TaskEventFunction func =
					new Task.TaskEventFunction(task_TaskStarted);
				Invoke(func, new object[] { e });
				return;
			}

			//Get the list view item
			ListViewItem item = GetTaskItem(e.Task);

			//Update the status.
			item.SubItems[1].Text = "Running...";

			//Show the progress bar
			schedulerProgress.Tag = item.Index;
			schedulerProgress.Visible = true;
			schedulerProgress.Value = 0;
			PositionProgressBar();
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
				Invoke(func, new object[] { e });
				return;
			}

			//Find the list view item
			ListViewItem item = GetTaskItem(e.Task);

			//Update the progress bar
			schedulerProgress.Value = e.OverallProgress;
		}

		/// <summary>
		/// Handles the task completion event.
		/// </summary>
		/// <param name="e">The task event object.</param>
		void task_TaskFinished(TaskEventArgs e)
		{
			if (scheduler.InvokeRequired)
			{
				Task.TaskEventFunction func =
					new Task.TaskEventFunction(task_TaskFinished);
				Invoke(func, new object[] { e });
				return;
			}

			//Get the list view item
			ListViewItem item = GetTaskItem(e.Task);

			//Update the status.
			item.SubItems[1].Text = "Completed";

			//Hide the progress bar
			if (schedulerProgress.Tag != null &&
				(int)schedulerProgress.Tag == item.Index)
			{
				schedulerProgress.Tag = null;
				schedulerProgress.Visible = false;
			}

			//Inform the user on the status of the task.
			LogLevel highestLevel = LogLevel.INFORMATION;
			foreach (LogEntry log in e.Task.Log)
				if (log.Level > highestLevel)
					highestLevel = log.Level;

			switch (highestLevel)
			{
				case LogLevel.WARNING:
					item.SubItems[1].Text += " with warnings.";
					break;
				case LogLevel.ERROR:
					item.SubItems[1].Text += " with errors.";
					break;
				case LogLevel.FATAL:
					item.SubItems[1].Text = "Not completed.";
					break;
				default:
					item.SubItems[1].Text += ".";
					break;
			}
		}

		/// <summary>
		/// Occurs when the user double-clicks a scheduler item. This will result
		/// in the log viewer being called, or the progress dialog to be displayed.
		/// </summary>
		/// <param name="sender">The list view which triggered the event.</param>
		/// <param name="e">Event argument.</param>
		private void scheduler_ItemActivate(object sender, EventArgs e)
		{
			if (scheduler.SelectedItems.Count == 0)
				return;

			ListViewItem item = scheduler.SelectedItems[0];
			if (((Task)item.Tag).Executing)
				using (ProgressForm form = new ProgressForm((Task)item.Tag))
					form.ShowDialog();
			else
				editTaskToolStripMenuItem_Click(sender, e);
		}

		/// <summary>
		/// Occurs when the user right-clicks the list view.
		/// </summary>
		/// <param name="sender">The list view which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void schedulerMenu_Opening(object sender, CancelEventArgs e)
		{
			//If nothing's selected, don't show the menu
			if (scheduler.SelectedItems.Count == 0)
			{
				e.Cancel = true;
				return;
			}

			bool aTaskNotQueued = false;
			bool aTaskExecuting = false;
			foreach (ListViewItem item in scheduler.SelectedItems)
			{
				Task task = (Task)item.Tag;
				aTaskNotQueued = aTaskNotQueued || (!task.Queued && !task.Executing);
				aTaskExecuting = aTaskExecuting || task.Executing;
			}

			runNowToolStripMenuItem.Enabled = aTaskNotQueued;
			cancelTaskToolStripMenuItem.Enabled = aTaskExecuting;

			editTaskToolStripMenuItem.Enabled = scheduler.SelectedItems.Count == 1;
			deleteTaskToolStripMenuItem.Enabled = !aTaskExecuting;
		}

		/// <summary>
		/// Occurs whent the user selects the Run Now context menu item.
		/// </summary>
		/// <param name="sender">The menu which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void runNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in scheduler.SelectedItems)
			{
				//Queue the task
				Task task = (Task)item.Tag;
				if (!task.Executing && !task.Queued)
				{
					Program.eraserClient.QueueTask(task);

					//Update the UI
					item.SubItems[1].Text = Schedule.RunNow.UIText;
				}
			}
		}

		/// <summary>
		/// Occurs whent the user selects the Cancel Task context menu item.
		/// </summary>
		/// <param name="sender">The menu which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void cancelTaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in scheduler.SelectedItems)
			{
				//Queue the task
				Task task = (Task)item.Tag;
				if (task.Executing || task.Queued)
				{
					Program.eraserClient.CancelTask(task);

					//Update the UI
					item.SubItems[1].Text = string.Empty;
				}
			}
		}

		/// <summary>
		/// Occurs when the user selects the View Task Log context menu item.
		/// </summary>
		/// <param name="sender">The menu item which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void viewTaskLogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException("Not implemented.");
		}

		/// <summary>
		/// Occurs when the user selects the Edit Task context menu item.
		/// </summary>
		/// <param name="sender">The menu item which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void editTaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			throw new NotImplementedException("Not implemented.");
		}

		/// <summary>
		/// Occurs when the user selects the Delete Task context menu item.
		/// </summary>
		/// <param name="sender">The menu item which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void deleteTaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in scheduler.SelectedItems)
			{
				Task task = (Task)item.Tag;
				if (!task.Executing)
				{
					Program.eraserClient.DeleteTask(task.ID);
					scheduler.Items.RemoveAt(item.Index);
				}
			}
		}

		#region Item management
		/// <summary>
		/// Retrieves the ListViewItem for the given task.
		/// </summary>
		/// <param name="task">The task object whose list view entry is being sought.</param>
		/// <returns>A ListViewItem for the given task object.</returns>
		private ListViewItem GetTaskItem(Task task)
		{
			foreach (ListViewItem item in scheduler.Items)
				if (item.Tag == task)
					return item;

			return null;
		}

		/// <summary>
		/// Maintains the position of the progress bar.
		/// </summary>
		private void PositionProgressBar()
		{
			if (schedulerProgress.Tag == null)
				return;

			Rectangle rect = GetSubItemRect((int)schedulerProgress.Tag, 2);
			schedulerProgress.Top = rect.Top;
			schedulerProgress.Left = rect.Left;
			schedulerProgress.Width = rect.Width;
			schedulerProgress.Height = rect.Height;
		}

		private void scheduler_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			e.DrawDefault = true;
			if (schedulerProgress.Tag != null)
				PositionProgressBar();
		}

		private void scheduler_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}
		#endregion

		#region GetSubItemRect
		[DllImport("User32.dll")]
		private static extern UIntPtr SendMessage(IntPtr HWND, uint Message,
			UIntPtr wParam, IntPtr lParam);

		private struct Rect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		};

		private unsafe Rectangle GetSubItemRect(int index, int subItemIndex)
		{
			Rect pRect = new Rect();
			pRect.top = subItemIndex;
			pRect.left = 0; //LVIR_BOUNDS
			SendMessage(scheduler.Handle, 0x1000 + 56, (UIntPtr)index, (IntPtr)(&pRect));

			return new Rectangle(pRect.left, pRect.top, pRect.right - pRect.left,
				pRect.bottom - pRect.top);
		}
		#endregion
	}
}

