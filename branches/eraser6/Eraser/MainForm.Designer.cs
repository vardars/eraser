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
			this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.contentPanel.AutoScroll = true;
			this.contentPanel.BackColor = System.Drawing.Color.White;
			this.contentPanel.Location = new System.Drawing.Point(14, 77);
			this.contentPanel.Name = "contentPanel";
			this.contentPanel.Size = new System.Drawing.Size(752, 449);
			this.contentPanel.TabIndex = 0;
			// 
			// toolbarScheduleMenu
			// 
			this.toolbarScheduleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTaskToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolbarScheduleMenuQueue,
            this.viewLogToolStripMenuItem});
			this.toolbarScheduleMenu.Name = "toolbarScheduleMenu";
			this.toolbarScheduleMenu.Size = new System.Drawing.Size(194, 76);
			// 
			// newTaskToolStripMenuItem
			// 
			this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
			this.newTaskToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.newTaskToolStripMenuItem.Text = "New Task";
			this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
			// 
			// toolbarScheduleMenuQueue
			// 
			this.toolbarScheduleMenuQueue.Name = "toolbarScheduleMenuQueue";
			this.toolbarScheduleMenuQueue.Size = new System.Drawing.Size(193, 22);
			this.toolbarScheduleMenuQueue.Text = "View Completed Tasks";
			// 
			// viewLogToolStripMenuItem
			// 
			this.viewLogToolStripMenuItem.Name = "viewLogToolStripMenuItem";
			this.viewLogToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.viewLogToolStripMenuItem.Text = "View Log";
			// 
			// toolbarHelpMenu
			// 
			this.toolbarHelpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.aboutEraserToolStripMenuItem});
			this.toolbarHelpMenu.Name = "toolbarHelpMenu";
			this.toolbarHelpMenu.Size = new System.Drawing.Size(172, 48);
			// 
			// checkForUpdatesToolStripMenuItem
			// 
			this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
			this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates";
			// 
			// aboutEraserToolStripMenuItem
			// 
			this.aboutEraserToolStripMenuItem.Name = "aboutEraserToolStripMenuItem";
			this.aboutEraserToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.aboutEraserToolStripMenuItem.Text = "About Eraser";
			this.aboutEraserToolStripMenuItem.Click += new System.EventHandler(this.aboutEraserToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(766, 538);
			this.Controls.Add(this.contentPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(760, 520);
			this.Name = "MainForm";
			this.Text = "Eraser";
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
