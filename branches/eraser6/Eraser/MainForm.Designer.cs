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
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolbarScheduleMenuQueue = new System.Windows.Forms.ToolStripMenuItem();
			this.viewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbarHelpMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutEraserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolbarScheduleMenu.SuspendLayout();
			this.toolbarHelpMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// contentPanel
			// 
			this.contentPanel.AccessibleDescription = null;
			this.contentPanel.AccessibleName = null;
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.BackColor = System.Drawing.Color.White;
			this.contentPanel.BackgroundImage = null;
			this.contentPanel.Font = null;
			this.contentPanel.Name = "contentPanel";
			// 
			// toolbarScheduleMenu
			// 
			this.toolbarScheduleMenu.AccessibleDescription = null;
			this.toolbarScheduleMenu.AccessibleName = null;
			resources.ApplyResources(this.toolbarScheduleMenu, "toolbarScheduleMenu");
			this.toolbarScheduleMenu.BackgroundImage = null;
			this.toolbarScheduleMenu.Font = null;
			this.toolbarScheduleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTaskToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolbarScheduleMenuQueue,
            this.viewLogToolStripMenuItem});
			this.toolbarScheduleMenu.Name = "toolbarScheduleMenu";
			// 
			// newTaskToolStripMenuItem
			// 
			this.newTaskToolStripMenuItem.AccessibleDescription = null;
			this.newTaskToolStripMenuItem.AccessibleName = null;
			resources.ApplyResources(this.newTaskToolStripMenuItem, "newTaskToolStripMenuItem");
			this.newTaskToolStripMenuItem.BackgroundImage = null;
			this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
			this.newTaskToolStripMenuItem.ShortcutKeyDisplayString = null;
			this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.AccessibleDescription = null;
			this.toolStripSeparator1.AccessibleName = null;
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			// 
			// toolbarScheduleMenuQueue
			// 
			this.toolbarScheduleMenuQueue.AccessibleDescription = null;
			this.toolbarScheduleMenuQueue.AccessibleName = null;
			resources.ApplyResources(this.toolbarScheduleMenuQueue, "toolbarScheduleMenuQueue");
			this.toolbarScheduleMenuQueue.BackgroundImage = null;
			this.toolbarScheduleMenuQueue.Name = "toolbarScheduleMenuQueue";
			this.toolbarScheduleMenuQueue.ShortcutKeyDisplayString = null;
			// 
			// viewLogToolStripMenuItem
			// 
			this.viewLogToolStripMenuItem.AccessibleDescription = null;
			this.viewLogToolStripMenuItem.AccessibleName = null;
			resources.ApplyResources(this.viewLogToolStripMenuItem, "viewLogToolStripMenuItem");
			this.viewLogToolStripMenuItem.BackgroundImage = null;
			this.viewLogToolStripMenuItem.Name = "viewLogToolStripMenuItem";
			this.viewLogToolStripMenuItem.ShortcutKeyDisplayString = null;
			// 
			// toolbarHelpMenu
			// 
			this.toolbarHelpMenu.AccessibleDescription = null;
			this.toolbarHelpMenu.AccessibleName = null;
			resources.ApplyResources(this.toolbarHelpMenu, "toolbarHelpMenu");
			this.toolbarHelpMenu.BackgroundImage = null;
			this.toolbarHelpMenu.Font = null;
			this.toolbarHelpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.aboutEraserToolStripMenuItem});
			this.toolbarHelpMenu.Name = "toolbarHelpMenu";
			// 
			// checkForUpdatesToolStripMenuItem
			// 
			this.checkForUpdatesToolStripMenuItem.AccessibleDescription = null;
			this.checkForUpdatesToolStripMenuItem.AccessibleName = null;
			resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
			this.checkForUpdatesToolStripMenuItem.BackgroundImage = null;
			this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
			this.checkForUpdatesToolStripMenuItem.ShortcutKeyDisplayString = null;
			// 
			// aboutEraserToolStripMenuItem
			// 
			this.aboutEraserToolStripMenuItem.AccessibleDescription = null;
			this.aboutEraserToolStripMenuItem.AccessibleName = null;
			resources.ApplyResources(this.aboutEraserToolStripMenuItem, "aboutEraserToolStripMenuItem");
			this.aboutEraserToolStripMenuItem.BackgroundImage = null;
			this.aboutEraserToolStripMenuItem.Name = "aboutEraserToolStripMenuItem";
			this.aboutEraserToolStripMenuItem.ShortcutKeyDisplayString = null;
			this.aboutEraserToolStripMenuItem.Click += new System.EventHandler(this.aboutEraserToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BackgroundImage = null;
			this.Controls.Add(this.contentPanel);
			this.Name = "MainForm";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.toolbarScheduleMenu.ResumeLayout(false);
			this.toolbarHelpMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel contentPanel;
		private System.Windows.Forms.ContextMenuStrip toolbarScheduleMenu;
		private System.Windows.Forms.ToolStripMenuItem toolbarScheduleMenuQueue;
		private System.Windows.Forms.ContextMenuStrip toolbarHelpMenu;
		private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutEraserToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newTaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

	}
}
