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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Once-only tasks", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Recurring tasks", System.Windows.Forms.HorizontalAlignment.Left);
			this.scheduler = new System.Windows.Forms.ListView();
			this.schedulerColName = new System.Windows.Forms.ColumnHeader();
			this.schedulerColMethod = new System.Windows.Forms.ColumnHeader();
			this.schedulerColStatus = new System.Windows.Forms.ColumnHeader();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.content.SuspendLayout();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.Size = new System.Drawing.Size(121, 32);
			this.titleLbl.Text = "Scheduler";
			// 
			// titleIcon
			// 
			this.titleIcon.Image = global::Eraser.Properties.Resources.ToolbarSchedule;
			// 
			// content
			// 
			this.content.Controls.Add(this.scheduler);
			// 
			// scheduler
			// 
			this.scheduler.AllowDrop = true;
			this.scheduler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scheduler.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.schedulerColName,
            this.schedulerColMethod,
            this.schedulerColStatus});
			this.scheduler.FullRowSelect = true;
			listViewGroup1.Header = "Once-only tasks";
			listViewGroup1.Name = "schedulerGrpSingle";
			listViewGroup2.Header = "Recurring tasks";
			listViewGroup2.Name = "schedulerGrpRecurring";
			this.scheduler.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
			this.scheduler.Location = new System.Drawing.Point(0, 0);
			this.scheduler.Name = "scheduler";
			this.scheduler.Size = new System.Drawing.Size(622, 377);
			this.scheduler.TabIndex = 0;
			this.scheduler.UseCompatibleStateImageBehavior = false;
			this.scheduler.View = System.Windows.Forms.View.Details;
			// 
			// schedulerColName
			// 
			this.schedulerColName.Text = "Task Name";
			this.schedulerColName.Width = 300;
			// 
			// schedulerColMethod
			// 
			this.schedulerColMethod.Text = "Method";
			this.schedulerColMethod.Width = 100;
			// 
			// schedulerColStatus
			// 
			this.schedulerColStatus.Text = "Status";
			this.schedulerColStatus.Width = 200;
			// 
			// SchedulerPanel
			// 
			this.Name = "SchedulerPanel";
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.content.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView scheduler;
		private System.Windows.Forms.ColumnHeader schedulerColName;
		private System.Windows.Forms.ColumnHeader schedulerColMethod;
		private System.Windows.Forms.ColumnHeader schedulerColStatus;
	}
}

