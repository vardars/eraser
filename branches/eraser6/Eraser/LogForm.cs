using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Eraser.Manager;
using System.Globalization;

namespace Eraser
{
	public partial class LogForm : Form
	{
		public LogForm(Task task)
		{
			InitializeComponent();
			this.task = task;

			//Update the title
			Text = string.Format("{0} - {1}", Text, task.UIText);

			//Add all the existing log messages
			List<LogEntry> log = task.Log;
			foreach (LogEntry entry in log)
				task_Logged(entry);

			//Register our event handler to get live log messages
			task.Logged += new Task.LogEvent(task_Logged);
		}

		~LogForm()
		{
			task.Logged -= new Task.LogEvent(task_Logged);
		}

		private void task_Logged(LogEntry e)
		{
			ListViewItem item = log.Items.Add(e.Timestamp.ToString(
					DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " +
					DateTimeFormatInfo.CurrentInfo.ShortTimePattern));
			item.SubItems.Add(e.Level.ToString());
			item.SubItems.Add(e.Message);

			switch (e.Level)
			{
				case LogLevel.FATAL:
				case LogLevel.ERROR:
					item.ForeColor = Color.Red;
					break;
				case LogLevel.WARNING:
					item.ForeColor = Color.OrangeRed;
					break;
			}
		}

		private void clear_Click(object sender, EventArgs e)
		{
			this.task.ClearLog();
			log.Items.Clear();
		}

		private void close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private Task task;
	}
}