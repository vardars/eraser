namespace Eraser
{
	partial class SchedulerPanel
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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Tasks executed immediately", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Tasks executed on restart", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Recurring tasks", System.Windows.Forms.HorizontalAlignment.Left);
			this.scheduler = new System.Windows.Forms.ListView();
			this.schedulerColName = new System.Windows.Forms.ColumnHeader();
			this.schedulerColNextRun = new System.Windows.Forms.ColumnHeader();
			this.schedulerColStatus = new System.Windows.Forms.ColumnHeader();
			this.schedulerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.runNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.viewTaskLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.editTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.schedulerProgress = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.content.SuspendLayout();
			this.schedulerMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.Size = new System.Drawing.Size(175, 32);
			this.titleLbl.Text = "Erase Schedule";
			// 
			// titleIcon
			// 
			this.titleIcon.Image = global::Eraser.Properties.Resources.ToolbarSchedule;
			// 
			// content
			// 
			this.content.Controls.Add(this.schedulerProgress);
			this.content.Controls.Add(this.scheduler);
			// 
			// scheduler
			// 
			this.scheduler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scheduler.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.schedulerColName,
            this.schedulerColNextRun,
            this.schedulerColStatus});
			this.scheduler.ContextMenuStrip = this.schedulerMenu;
			this.scheduler.FullRowSelect = true;
			listViewGroup1.Header = "Tasks executed immediately";
			listViewGroup1.Name = "immediate";
			listViewGroup2.Header = "Tasks executed on restart";
			listViewGroup2.Name = "restart";
			listViewGroup3.Header = "Recurring tasks";
			listViewGroup3.Name = "recurring";
			this.scheduler.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.scheduler.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.scheduler.Location = new System.Drawing.Point(0, 0);
			this.scheduler.Name = "scheduler";
			this.scheduler.OwnerDraw = true;
			this.scheduler.Size = new System.Drawing.Size(712, 377);
			this.scheduler.TabIndex = 0;
			this.scheduler.UseCompatibleStateImageBehavior = false;
			this.scheduler.View = System.Windows.Forms.View.Details;
			this.scheduler.ItemActivate += new System.EventHandler(this.scheduler_ItemActivate);
			this.scheduler.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.scheduler_DrawSubItem);
			this.scheduler.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.scheduler_DrawColumnHeader);
			// 
			// schedulerColName
			// 
			this.schedulerColName.Text = "Task Name";
			this.schedulerColName.Width = 280;
			// 
			// schedulerColNextRun
			// 
			this.schedulerColNextRun.Text = "Next Run";
			this.schedulerColNextRun.Width = 200;
			// 
			// schedulerColStatus
			// 
			this.schedulerColStatus.Text = "Status";
			this.schedulerColStatus.Width = 200;
			// 
			// schedulerMenu
			// 
			this.schedulerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runNowToolStripMenuItem,
            this.cancelTaskToolStripMenuItem,
            this.toolStripSeparator2,
            this.viewTaskLogToolStripMenuItem,
            this.toolStripSeparator1,
            this.editTaskToolStripMenuItem,
            this.deleteTaskToolStripMenuItem});
			this.schedulerMenu.Name = "schedulerMenu";
			this.schedulerMenu.Size = new System.Drawing.Size(153, 148);
			this.schedulerMenu.Opening += new System.ComponentModel.CancelEventHandler(this.schedulerMenu_Opening);
			// 
			// runNowToolStripMenuItem
			// 
			this.runNowToolStripMenuItem.Name = "runNowToolStripMenuItem";
			this.runNowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.runNowToolStripMenuItem.Text = "Run Now";
			this.runNowToolStripMenuItem.Click += new System.EventHandler(this.runNowToolStripMenuItem_Click);
			// 
			// cancelTaskToolStripMenuItem
			// 
			this.cancelTaskToolStripMenuItem.Name = "cancelTaskToolStripMenuItem";
			this.cancelTaskToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.cancelTaskToolStripMenuItem.Text = "Cancel Task";
			this.cancelTaskToolStripMenuItem.Click += new System.EventHandler(this.cancelTaskToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// viewTaskLogToolStripMenuItem
			// 
			this.viewTaskLogToolStripMenuItem.Name = "viewTaskLogToolStripMenuItem";
			this.viewTaskLogToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.viewTaskLogToolStripMenuItem.Text = "View Task Log";
			this.viewTaskLogToolStripMenuItem.Click += new System.EventHandler(this.viewTaskLogToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// editTaskToolStripMenuItem
			// 
			this.editTaskToolStripMenuItem.Name = "editTaskToolStripMenuItem";
			this.editTaskToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.editTaskToolStripMenuItem.Text = "Edit Task";
			this.editTaskToolStripMenuItem.Click += new System.EventHandler(this.editTaskToolStripMenuItem_Click);
			// 
			// deleteTaskToolStripMenuItem
			// 
			this.deleteTaskToolStripMenuItem.Name = "deleteTaskToolStripMenuItem";
			this.deleteTaskToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.deleteTaskToolStripMenuItem.Text = "Delete Task";
			this.deleteTaskToolStripMenuItem.Click += new System.EventHandler(this.deleteTaskToolStripMenuItem_Click);
			// 
			// schedulerProgress
			// 
			this.schedulerProgress.Location = new System.Drawing.Point(481, 28);
			this.schedulerProgress.Name = "schedulerProgress";
			this.schedulerProgress.Size = new System.Drawing.Size(200, 23);
			this.schedulerProgress.TabIndex = 1;
			this.schedulerProgress.Visible = false;
			// 
			// SchedulerPanel
			// 
			this.DoubleBuffered = true;
			this.Name = "SchedulerPanel";
			this.Controls.SetChildIndex(this.titleLbl, 0);
			this.Controls.SetChildIndex(this.titleIcon, 0);
			this.Controls.SetChildIndex(this.content, 0);
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.content.ResumeLayout(false);
			this.schedulerMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ColumnHeader schedulerColName;
		private System.Windows.Forms.ColumnHeader schedulerColNextRun;
		private System.Windows.Forms.ColumnHeader schedulerColStatus;
		private System.Windows.Forms.ListView scheduler;
		private System.Windows.Forms.ProgressBar schedulerProgress;
		private System.Windows.Forms.ContextMenuStrip schedulerMenu;
		private System.Windows.Forms.ToolStripMenuItem runNowToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem viewTaskLogToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editTaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteTaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cancelTaskToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	}
}

