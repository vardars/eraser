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

namespace Eraser
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.contentPanel = new System.Windows.Forms.Panel();
			this.toolbarScheduleMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbarHelpMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutEraserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notificationIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.notificationMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.openEraserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.hideWhenMinimiseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notificationIconTimer = new System.Windows.Forms.Timer(this.components);
			this.toolbarScheduleMenu.SuspendLayout();
			this.toolbarHelpMenu.SuspendLayout();
			this.notificationMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// contentPanel
			// 
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.BackColor = System.Drawing.Color.White;
			this.contentPanel.Name = "contentPanel";
			// 
			// toolbarScheduleMenu
			// 
			this.toolbarScheduleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTaskToolStripMenuItem});
			this.toolbarScheduleMenu.Name = "toolbarScheduleMenu";
			resources.ApplyResources(this.toolbarScheduleMenu, "toolbarScheduleMenu");
			// 
			// newTaskToolStripMenuItem
			// 
			this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
			resources.ApplyResources(this.newTaskToolStripMenuItem, "newTaskToolStripMenuItem");
			this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
			// 
			// toolbarHelpMenu
			// 
			this.toolbarHelpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.aboutEraserToolStripMenuItem});
			this.toolbarHelpMenu.Name = "toolbarHelpMenu";
			resources.ApplyResources(this.toolbarHelpMenu, "toolbarHelpMenu");
			// 
			// checkForUpdatesToolStripMenuItem
			// 
			this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
			resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
			this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
			// 
			// aboutEraserToolStripMenuItem
			// 
			this.aboutEraserToolStripMenuItem.Name = "aboutEraserToolStripMenuItem";
			resources.ApplyResources(this.aboutEraserToolStripMenuItem, "aboutEraserToolStripMenuItem");
			this.aboutEraserToolStripMenuItem.Click += new System.EventHandler(this.aboutEraserToolStripMenuItem_Click);
			// 
			// notificationIcon
			// 
			this.notificationIcon.ContextMenuStrip = this.notificationMenu;
			resources.ApplyResources(this.notificationIcon, "notificationIcon");
			this.notificationIcon.DoubleClick += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// notificationMenu
			// 
			this.notificationMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openEraserToolStripMenuItem,
            this.toolStripMenuItem1,
            this.hideWhenMinimiseToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.notificationMenu.Name = "notificationMenu";
			resources.ApplyResources(this.notificationMenu, "notificationMenu");
			// 
			// openEraserToolStripMenuItem
			// 
			this.openEraserToolStripMenuItem.Name = "openEraserToolStripMenuItem";
			resources.ApplyResources(this.openEraserToolStripMenuItem, "openEraserToolStripMenuItem");
			this.openEraserToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			// 
			// hideWhenMinimiseToolStripMenuItem
			// 
			this.hideWhenMinimiseToolStripMenuItem.CheckOnClick = true;
			this.hideWhenMinimiseToolStripMenuItem.Name = "hideWhenMinimiseToolStripMenuItem";
			resources.ApplyResources(this.hideWhenMinimiseToolStripMenuItem, "hideWhenMinimiseToolStripMenuItem");
			this.hideWhenMinimiseToolStripMenuItem.Click += new System.EventHandler(this.hideWhenMinimiseToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// notificationIconTimer
			// 
			this.notificationIconTimer.Tick += new System.EventHandler(this.notificationIconTimer_Tick);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.contentPanel);
			this.DoubleBuffered = true;
			this.Name = "MainForm";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.VisibleChanged += new System.EventHandler(this.MainForm_VisibleChanged);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.toolbarScheduleMenu.ResumeLayout(false);
			this.toolbarHelpMenu.ResumeLayout(false);
			this.notificationMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.ContextMenuStrip toolbarScheduleMenu;
		private System.Windows.Forms.ContextMenuStrip toolbarHelpMenu;
		private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutEraserToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newTaskToolStripMenuItem;
		private System.Windows.Forms.NotifyIcon notificationIcon;
		private System.Windows.Forms.Timer notificationIconTimer;
		private System.Windows.Forms.ContextMenuStrip notificationMenu;
		private System.Windows.Forms.ToolStripMenuItem openEraserToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hideWhenMinimiseToolStripMenuItem;

	}
}
