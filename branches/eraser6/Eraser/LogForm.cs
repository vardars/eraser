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
			Dictionary<DateTime, List<LogEntry>> log = task.Log.Entries;
			Dictionary<DateTime, List<LogEntry>>.Enumerator iter = log.GetEnumerator();
			foreach (DateTime sessionTime in log.Keys)
			{
				this.log.Groups.Add(new ListViewGroup("Session: " + sessionTime.ToString(DATEPATTERN)));
				foreach (LogEntry entry in log[sessionTime])
					task_Logged(entry);
			}

			//Register our event handler to get live log messages
			task.Log.OnLogged += new Logger.LogEventFunction(task_Logged);
		}

		private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			task.Log.OnLogged -= new Logger.LogEventFunction(task_Logged);
		}

		private void task_Logged(LogEntry e)
		{
			if (InvokeRequired)
			{
				//Todo: I get crashes here... but alas, I can't fix it!
				try
				{
					Invoke(new Logger.LogEventFunction(task_Logged), new object[] { e });
				}
				catch (ObjectDisposedException)
				{
				}
				catch (InvalidOperationException)
				{
				}
				return;
			}

			ListViewItem item = log.Items.Add(e.Timestamp.ToString(DATEPATTERN));
			item.SubItems.Add(e.Level.ToString());
			item.SubItems.Add(e.Message);
			if (log.Groups.Count != 0)
				item.Group = log.Groups[log.Groups.Count - 1];

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
			this.task.Log.Clear();
			log.Items.Clear();
		}

		private void close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private Task task;
		private static string DATEPATTERN =
			DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " +
			DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
	}
}