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
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
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
			// SchedulerPanel
			// 
			this.Name = "SchedulerPanel";
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
	}
}

