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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using Eraser.Util;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Eraser
{
	public partial class SchedulerPanel : Eraser.BasePanel
	{
		public SchedulerPanel()
		{
			InitializeComponent();

			//Populate the scheduler list-view with the current task list
			List<Task> tasks = Program.eraserClient.GetTasks();
			foreach (Task task in tasks)
				DisplayTask(task);

			//Hook the event machinery to our class. Handle the task Added and Removed
			//events.
			Program.eraserClient.TaskAdded += TaskAdded;
			Program.eraserClient.TaskDeleted += TaskDeleted;
		}

		private void DisplayTask(Task task)
		{
			//Add the item to the list view
			ListViewItem item = scheduler.Items.Add(task.UIText);
			item.SubItems.Add(string.Empty);
			item.SubItems.Add(string.Empty);

			//Set the tag of the item so we know which task on the LV corresponds
			//to the physical task object.
			item.Tag = task;

			//Add our event handlers to the task
			task.TaskStarted += new Task.TaskEventFunction(task_TaskStarted);
			task.ProgressChanged += new Task.ProgressEventFunction(task_ProgressChanged);
			task.TaskFinished += new Task.TaskEventFunction(task_TaskFinished);

			//Show the fields on the list view
			UpdateTask(item);
		}

		private void UpdateTask(ListViewItem item)
		{
			//Get the task object
			Task task = (Task)item.Tag;

			//Set the task name
			item.Text = task.UIText;

			//Set the next run time of the task
			if (task.Queued)
				item.SubItems[1].Text = S._("Queued for execution");
			else if (task.Schedule is RecurringSchedule)
				item.SubItems[1].Text = ((task.Schedule as RecurringSchedule).NextRun.
					ToString(DateTimeFormatInfo.CurrentInfo.FullDateTimePattern));
			else if (task.Schedule == Schedule.RunNow)
				item.SubItems[1].Text = S._("Not queued");
			else
				item.SubItems[1].Text = task.Schedule.UIText;

			//Set the group of the task.
			CategorizeTask(task, item);
		}

		private void CategorizeTask(Task task)
		{
			CategorizeTask(task, GetTaskItem(task));
		}

		private void CategorizeTask(Task task, ListViewItem item)
		{
			if (task.Schedule == Schedule.RunNow)
				item.Group = scheduler.Groups["immediate"];
			else if (task.Schedule == Schedule.RunOnRestart)
				item.Group = scheduler.Groups["restart"];
			else
				item.Group = scheduler.Groups["recurring"];
		}

		private void TaskAdded(Task task)
		{
			if (InvokeRequired)
			{
				Invoke(new Executor.TaskAddedEvent(TaskAdded), new object[] { task });
				return;
			}

			//Display a balloon notification if the parent frame has been minimised.
			MainForm parent = (MainForm)FindForm();
			if (parent.WindowState == FormWindowState.Minimized || !parent.Visible)
			{
				parent.ShowNotificationBalloon(S._("New task added"), S._("{0} " +
					"has just been added to the list of tasks.", task.UIText),
					ToolTipIcon.Info);
			}

			DisplayTask(task);
		}

		/// <summary>
		/// Handles the task deleted event.
		/// </summary>
		/// <param name="task">The task being deleted.</param>
		private void TaskDeleted(Task task)
		{
			if (InvokeRequired)
			{
				Invoke(new Executor.TaskDeletedEvent(TaskDeleted), new object[] { task });
				return;
			}

			foreach (ListViewItem item in scheduler.Items)
				if (((Task)item.Tag) == task)
				{
					scheduler.Items.Remove(item);
					break;
				}
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
			item.SubItems[1].Text = S._("Running...");

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
			schedulerProgress.Value = (int)(e.OverallProgress * 1000.0);
		}

		/// <summary>
		/// Handles the task completion event.
		/// </summary>
		/// <param name="e">The task event object.</param>
		void task_TaskFinished(TaskEventArgs e)
		{
			if (InvokeRequired)
			{
				Task.TaskEventFunction func =
					new Task.TaskEventFunction(task_TaskFinished);
				Invoke(func, new object[] { e });
				return;
			}

			//Get the list view item
			ListViewItem item = GetTaskItem(e.Task);
			if (item == null)
				return;

			//Update the status.
			item.SubItems[1].Text = S._("Completed");

			//Hide the progress bar
			if (schedulerProgress.Tag != null &&
				(int)schedulerProgress.Tag == item.Index)
			{
				schedulerProgress.Tag = null;
				schedulerProgress.Visible = false;
			}

			//Inform the user on the status of the task.
			LogLevel highestLevel = LogLevel.INFORMATION;
			List<LogEntry> logs = e.Task.Log.LastSessionEntries;
			foreach (LogEntry log in logs)
				if (log.Level > highestLevel)
					highestLevel = log.Level;

			switch (highestLevel)
			{
				case LogLevel.WARNING:
					item.SubItems[1].Text = S._("Completed with warnings");
					break;
				case LogLevel.ERROR:
					item.SubItems[1].Text = S._("Completed with errors");
					break;
				case LogLevel.FATAL:
					item.SubItems[1].Text = S._("Not completed");
					break;
				default:
					item.SubItems[1].Text = S._("Completed");
					break;
			}

			//Recategorize the task. Do not assume the task has maintained the
			//category since run-on-restart tasks will be changed to immediately
			//run tasks.
			CategorizeTask(e.Task, item);
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

			editTaskToolStripMenuItem.Enabled = scheduler.SelectedItems.Count == 1 &&
				!((Task)scheduler.SelectedItems[0].Tag).Executing;
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
					item.SubItems[1].Text = S._("Queued for execution");
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
			if (scheduler.SelectedItems.Count != 1)
				return;

			ListViewItem item = scheduler.SelectedItems[0];
			using (LogForm form = new LogForm((Task)item.Tag))
				form.ShowDialog();
		}

		/// <summary>
		/// Occurs when the user selects the Edit Task context menu item.
		/// </summary>
		/// <param name="sender">The menu item which generated this event.</param>
		/// <param name="e">Event argument.</param>
		private void editTaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (scheduler.SelectedItems.Count != 1)
				return;

			//Make sure that the task is not being executed, or else. This can
			//be done in the Client library, but there will be no effect on the
			//currently running task.
			ListViewItem item = scheduler.SelectedItems[0];
			Task task = (Task)item.Tag;
			if (task.Executing)
				return;

			//Edit the task.
			using (TaskPropertiesForm form = new TaskPropertiesForm())
			{
				form.Task = task;
				if (form.ShowDialog() == DialogResult.OK)
				{
					task = form.Task;
					scheduler.SelectedItems[0].Tag = task;
					Program.eraserClient.ReplaceTask(task);

					//Update the list view
					UpdateTask(item);
				}
			}
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
					Program.eraserClient.DeleteTask(task.ID);
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
			UIntPtr wParam, out Rect lParam);

		private struct Rect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		};

		private Rectangle GetSubItemRect(int index, int subItemIndex)
		{
			Rect pRect = new Rect();
			pRect.top = subItemIndex;
			pRect.left = 0; //LVIR_BOUNDS
			SendMessage(scheduler.Handle, 0x1000 + 56, (UIntPtr)index, out pRect);

			return new Rectangle(pRect.left, pRect.top, pRect.right - pRect.left,
				pRect.bottom - pRect.top);
		}
		#endregion
	}
}

