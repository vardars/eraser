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

namespace Eraser
{
	public partial class ProgressForm : Form
	{
		private Task task;

		public ProgressForm(Task task)
		{
			InitializeComponent();
			this.task = task;

			//Register the event handlers
			jobTitle.Text = task.UIText;
			task.ProgressChanged += new Task.ProgressEventFunction(task_ProgressChanged);
			task.TaskFinished += new Task.TaskEventFunction(task_TaskFinished);
		}

		private void ProgressForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			task.ProgressChanged -= new Task.ProgressEventFunction(task_ProgressChanged);
			task.TaskFinished -= new Task.TaskEventFunction(task_TaskFinished);
		}

		void task_ProgressChanged(TaskProgressEventArgs e)
		{
			if (InvokeRequired)
			{
				Task.ProgressEventFunction func =
					new Task.ProgressEventFunction(task_ProgressChanged);
				Invoke(func, new object[] {e});
				return;
			}

			item.Text = File.GetCompactPath(e.CurrentItemName, item.Width * 2, item.Font);
			pass.Text = e.CurrentTargetTotalPasses != 0 ?
				S._("{0} out of {1}", e.CurrentItemPass, e.CurrentTargetTotalPasses) :
				string.Format("{0}", e.CurrentItemPass);

			if (e.TimeLeft >= TimeSpan.Zero)
				timeLeft.Text = S._("About {0:hh:mm:ss} left", e.TimeLeft);
			else
				timeLeft.Text = S._("Unknown");

			itemProgress.Value = (int)(e.CurrentItemProgress * 1000);
			itemProgressLbl.Text = e.CurrentItemProgress.ToString("#0%");
			overallProgress.Value = (int)(e.OverallProgress * 1000);
			overallProgressLbl.Text = S._("Total: {0,2:#0.00%}", e.OverallProgress);
		}

		void task_TaskFinished(TaskEventArgs e)
		{
			if (InvokeRequired)
			{
				Task.TaskEventFunction func =
					new Task.TaskEventFunction(task_TaskFinished);
				Invoke(func, new object[] { e });
				return;
			}

			//Update the UI. Set everything to 100%
			timeLeft.Text = item.Text = string.Empty;
			overallProgressLbl.Text = S._("Total: {0,2:#0.00%}", 1.0);
			overallProgress.Value = overallProgress.Maximum;
			itemProgressLbl.Text = "100%";
			itemProgress.Value = itemProgress.Maximum;

			//Inform the user on the status of the task.
			LogLevel highestLevel = LogLevel.INFORMATION;
			List<LogEntry> entries = e.Task.Log.LastSessionEntries;
			foreach (LogEntry log in entries)
				if (log.Level > highestLevel)
					highestLevel = log.Level;

			switch (highestLevel)
			{
				case LogLevel.WARNING:
					status.Text = S._("Completed with warnings");
					break;
				case LogLevel.ERROR:
					status.Text = S._("Completed with errors");
					break;
				case LogLevel.FATAL:
					status.Text = S._("Not completed");
					break;
				default:
					status.Text = S._("Completed");
					break;
			}

			//Change the Stop button to be a Close button.
			stop.Text = S._("Close");
		}

		private void stop_Click(object sender, EventArgs e)
		{
			if (task.Executing)
				task.Executor.CancelTask(task);
			Close();
		}
	}
}
