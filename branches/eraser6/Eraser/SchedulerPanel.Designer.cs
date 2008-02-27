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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.titleLbl = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.listView1 = new System.Windows.Forms.ListView();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.AutoSize = true;
			this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLbl.Location = new System.Drawing.Point(50, 17);
			this.titleLbl.Name = "titleLbl";
			this.titleLbl.Size = new System.Drawing.Size(121, 32);
			this.titleLbl.TabIndex = 0;
			this.titleLbl.Text = "Scheduler";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Eraser.Properties.Resources.ToolbarSchedule;
			this.pictureBox1.Location = new System.Drawing.Point(20, 21);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 24);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Location = new System.Drawing.Point(20, 52);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(622, 272);
			this.listView1.TabIndex = 2;
			this.listView1.UseCompatibleStateImageBehavior = false;
			// 
			// SchedulerPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.titleLbl);
			this.Name = "SchedulerPanel";
			this.Padding = new System.Windows.Forms.Padding(17);
			this.Size = new System.Drawing.Size(662, 344);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label titleLbl;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ListView listView1;
	}
}
