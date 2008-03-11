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

		~ProgressForm()
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

			item.Text = e.CurrentItemName;
			pass.Text = string.Format("{0} out of {1}", e.CurrentPass, e.TotalPasses);
			timeLeft.Text = string.Format("{0} left", new TimeSpan(0, 0, e.TimeLeft).ToString());

			itemProgress.Value = e.CurrentItemProgress;
			itemProgressLbl.Text = string.Format("{0}%", e.CurrentItemProgress);
			overallProgress.Value = e.OverallProgress;
			overallProgressLbl.Text = string.Format("Total: {0}%", e.OverallProgress);
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

			//Inform the user on the status of the task.
			status.Text = "Completed";
			LogLevel highestLevel = LogLevel.INFORMATION;
			foreach (LogEntry log in e.Task.Log)
				if (log.Level > highestLevel)
					highestLevel = log.Level;

			switch (highestLevel)
			{
				case LogLevel.WARNING:
					status.Text += " with warnings";
					break;
				case LogLevel.ERROR:
					status.Text  += " with errors";
					break;
				case LogLevel.FATAL:
					status.Text = "Not completed";
					break;
			}
		}

		private void stop_Click(object sender, EventArgs e)
		{
			if (task.Executing)
				task.Executor.CancelTask(task);
			Close();
		}
	}
}
