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
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Eraser.Util;
using Eraser.Manager;
using Eraser.Properties;

namespace Eraser
{
	public partial class MainForm : Form
	{
		public enum Pages
		{
			SCHEDULER = 0,
			SETTINGS
		}

		private ToolBar ToolBar = new ToolBar();
		private BasePanel CurrPage = null;
		private SchedulerPanel SchedulerPage = new SchedulerPanel();
		private SettingsPanel SettingsPage = new SettingsPanel();

		public MainForm()
		{
			InitializeComponent();
			SettingsPage.CreateControl();
			SchedulerPage.CreateControl();

			//Connect to the executor task processing and processed events.
			Program.eraserClient.TaskProcessing +=
				new Executor.TaskProcessingEvent(OnTaskProcessing);
			Program.eraserClient.TaskProcessed +=
				new Executor.TaskProcessedEvent(OnTaskProcessed);

			//Check the notification area context menu's minimise to tray item.
			hideWhenMinimiseToolStripMenuItem.Checked = EraserSettings.Get().HideWhenMinimised;

			//Create the toolbar control
			ToolBar.Name = "toolBar";
			ToolBar.Location = new Point(14, 27);
			ToolBar.Size = new Size(500, 26);
			ToolBar.TabIndex = 1;
			Controls.Add(ToolBar);

			ToolBarItem tbSchedule = new ToolBarItem();
			tbSchedule.Bitmap = Properties.Resources.ToolbarSchedule;
			tbSchedule.Text = S._("Erase Schedule");
			tbSchedule.Menu = toolbarScheduleMenu;
			tbSchedule.ToolbarItemClicked += delegate(object sender, EventArgs args)
			{
				ChangePage(Pages.SCHEDULER);
			};
			ToolBar.Items.Add(tbSchedule);

			ToolBarItem tbSettings = new ToolBarItem();
			tbSettings.Bitmap = Properties.Resources.ToolbarSettings;
			tbSettings.Text = S._("Settings");
			tbSettings.ToolbarItemClicked += delegate(object sender, EventArgs args)
			{
				ChangePage(Pages.SETTINGS);
			};
			ToolBar.Items.Add(tbSettings);

			ToolBarItem tbHelp = new ToolBarItem();
			tbHelp.Bitmap = Properties.Resources.ToolbarHelp;
			tbHelp.Text = S._("Help");
			tbHelp.Menu = toolbarHelpMenu;
			ToolBar.Items.Add(tbHelp);

			//Set the docking style for each of the pages
			SchedulerPage.Dock = DockStyle.Fill;

			//Show the default page.
			ChangePage(Pages.SCHEDULER);
		}

		/// <summary>
		/// Diplays the given title, message and icon as a system notification area balloon.
		/// </summary>
		/// <param name="title">The title of the balloon.</param>
		/// <param name="message">The message to display.</param>
		/// <param name="icon">The icon to show.</param>
		public void ShowNotificationBalloon(string title, string message, ToolTipIcon icon)
		{
			notificationIcon.BalloonTipTitle = title;
			notificationIcon.BalloonTipText = message;
			notificationIcon.BalloonTipIcon = icon;
			notificationIcon.ShowBalloonTip(0);
		}

		/// <summary>
		/// Changes the active page displayed in the form.
		/// </summary>
		/// <param name="page">The new page to change to. No action is done when the
		/// current page is the same as the new page requested</param>
		public void ChangePage(Pages page)
		{
			BasePanel oldPage = CurrPage;
			switch (page)
			{
				case Pages.SCHEDULER:
					CurrPage = SchedulerPage;
					break;
				case Pages.SETTINGS:
					CurrPage = SettingsPage;
					break;
			}

			if (oldPage != CurrPage)
			{
				contentPanel.SuspendLayout();
				contentPanel.Controls.Remove(oldPage);
				contentPanel.Controls.Add(CurrPage);

				if (CurrPage.Dock == DockStyle.None)
				{
					CurrPage.Anchor = AnchorStyles.Left | AnchorStyles.Right |
						AnchorStyles.Top;
					CurrPage.Left = 0;
					CurrPage.Top = 0;
					CurrPage.Width = contentPanel.Width;
				}
				contentPanel.ResumeLayout();
			}
		}

		private static GraphicsPath CreateRoundRect(float X, float Y, float width,
			float height, float radius)
		{
			GraphicsPath result = new GraphicsPath();

			//Top line.
			result.AddLine(X + radius, Y, X + width - 2 * radius, Y);

			//Top-right corner
			result.AddArc(X + width - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);

			//Right line.
			result.AddLine(X + width, Y + radius, X + width, Y + height - 2 * radius);

			//Bottom-right corner
			result.AddArc(X + width - 2 * radius, Y + height - 2 * radius, 2 * radius, 2 * radius, 0, 90);

			//Bottom line.
			result.AddLine(X + width - 2 * radius, Y + height, X + radius, Y + height);

			//Bottom-left corner
			result.AddArc(X, Y + height - 2 *radius, 2 * radius, 2 * radius, 90, 90);

			//Left line
			result.AddLine(X, Y + height - 2 * radius, X, Y + radius);

			//Top-left corner
			result.AddArc(X, Y, 2 * radius, 2 * radius, 180, 90);
			result.CloseFigure();

			return result;
		}

		private void DrawBackground(Graphics dc)
		{
			//Draw the base background
			dc.FillRectangle(new SolidBrush(Color.FromArgb(unchecked((int)0xFF292929))),
				new Rectangle(new Point(0, 0), Size));

			//Then the side gradient
			dc.FillRectangle(new LinearGradientBrush(new Rectangle(0, 0, 338, Math.Max(1, ClientSize.Height)),
					Color.FromArgb(unchecked((int)0xFF363636)),
					Color.FromArgb(unchecked((int)0xFF292929)), 0.0),
				0, 0, 338, ClientSize.Height);

			//Draw the top background
			dc.FillRectangle(new SolidBrush(Color.FromArgb(unchecked((int)0xFF414141))),
				new Rectangle(0, 0, ClientSize.Width, 32));

			//The top gradient
			dc.DrawImage(Properties.Resources.BackgroundGradient, new Point(0, 0));

			//And the logo
			Bitmap logo = Properties.Resources.BackgroundLogo;
			dc.DrawImage(logo, new Rectangle(ClientSize.Width - logo.Width - 10,
				(contentPanel.Top - logo.Height) / 2, logo.Width, logo.Height));

			dc.SmoothingMode = SmoothingMode.AntiAlias;
			dc.FillPath(Brushes.White, CreateRoundRect(11, 74, contentPanel.Width + 8, ClientSize.Height - 85, 3));
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SetClip(new Rectangle(0, 0, Width, Height), CombineMode.Intersect);
			DrawBackground(e.Graphics);
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (WindowState != FormWindowState.Minimized)
			{
				Bitmap bmp = new Bitmap(Width, Height);
				Graphics dc = Graphics.FromImage(bmp);
				DrawBackground(dc);

				CreateGraphics().DrawImage(bmp, new Point(0, 0));
			}
			else if (EraserSettings.Get().HideWhenMinimised)
			{
				Visible = false;
			}
		}

		private void newTaskToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem item = sender as ToolStripDropDownItem;
			using (TaskPropertiesForm form = new TaskPropertiesForm())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					Task task = form.Task;
					Program.eraserClient.AddTask(ref task);
				}
			}
		}

		private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (UpdateForm form = new UpdateForm())
			{
				form.ShowDialog();
			}
		}

		private void aboutEraserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AboutForm form = new AboutForm(this))
			{
				form.ShowDialog();
			}
		}

		#region Task processing code (for notification area animation)
		void OnTaskProcessing(Eraser.Manager.Task task)
		{
			if (InvokeRequired)
			{
				Invoke(new Executor.TaskProcessingEvent(OnTaskProcessing), task);
				return;
			}

			string iconText = S._("Eraser") + " - " + S._("Processing:") + ' ' + task.UIText;
			if (iconText.Length >= 64)
				iconText = iconText.Remove(60) + "...";

			ProcessingAnimationFrame = 0;
			notificationIcon.Text = iconText;
			notificationIconTimer.Enabled = true;
		}

		void OnTaskProcessed(Eraser.Manager.Task task)
		{
			if (InvokeRequired)
			{
				Invoke(new Executor.TaskProcessedEvent(OnTaskProcessed), task);
				return;
			}

			//Reset the notification area icon.
			notificationIconTimer.Enabled = false;
			if (notificationIcon.Icon != null)
			{
				ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
				resources.ApplyResources(notificationIcon, "notificationIcon");
			}
		}

		private void notificationIconTimer_Tick(object sender, EventArgs e)
		{
			notificationIcon.Icon = ProcessingAnimationFrames[ProcessingAnimationFrame++];
			if (ProcessingAnimationFrame == ProcessingAnimationFrames.Length)
				ProcessingAnimationFrame = 0;
		}

		private int ProcessingAnimationFrame = 0;
		private Icon[] ProcessingAnimationFrames = new Icon[] {
			Resources.NotifyBusy1,
			Resources.NotifyBusy2,
			Resources.NotifyBusy3,
			Resources.NotifyBusy4,
			Resources.NotifyBusy5,
			Resources.NotifyBusy4,
			Resources.NotifyBusy3,
			Resources.NotifyBusy2,
			Resources.NotifyBusy1
		};
		#endregion

		#region Minimise to tray code
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (EraserSettings.Get().HideWhenMinimised && e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Visible = false;
			}
		}

		private void MainForm_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				WindowState = FormWindowState.Normal;
				Activate();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Visible = true;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void hideWhenMinimiseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EraserSettings.Get().HideWhenMinimised =
				hideWhenMinimiseToolStripMenuItem.Checked;
		}
		#endregion
	}
}
