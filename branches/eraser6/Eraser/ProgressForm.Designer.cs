namespace Eraser
{
	partial class ProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
			this.overallProgressLbl = new System.Windows.Forms.Label();
			this.overallProgress = new System.Windows.Forms.ProgressBar();
			this.jobTitle = new System.Windows.Forms.Label();
			this.status = new System.Windows.Forms.Label();
			this.statusLbl = new System.Windows.Forms.Label();
			this.itemLbl = new System.Windows.Forms.Label();
			this.item = new System.Windows.Forms.Label();
			this.passLbl = new System.Windows.Forms.Label();
			this.pass = new System.Windows.Forms.Label();
			this.title = new System.Windows.Forms.PictureBox();
			this.titleLbl = new System.Windows.Forms.Label();
			this.itemProgressLbl = new System.Windows.Forms.Label();
			this.itemProgress = new System.Windows.Forms.ProgressBar();
			this.stop = new System.Windows.Forms.Button();
			this.bevelLine1 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine2 = new Trustbridge.Windows.Controls.BevelLine();
			this.timeLeftLbl = new System.Windows.Forms.Label();
			this.timeLeft = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.title)).BeginInit();
			this.SuspendLayout();
			// 
			// overallProgressLbl
			// 
			this.overallProgressLbl.Location = new System.Drawing.Point(6, 174);
			this.overallProgressLbl.Name = "overallProgressLbl";
			this.overallProgressLbl.Size = new System.Drawing.Size(128, 15);
			this.overallProgressLbl.TabIndex = 1;
			this.overallProgressLbl.Text = "Total: 75%";
			this.overallProgressLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// overallProgress
			// 
			this.overallProgress.Location = new System.Drawing.Point(6, 192);
			this.overallProgress.Name = "overallProgress";
			this.overallProgress.Size = new System.Drawing.Size(129, 15);
			this.overallProgress.TabIndex = 2;
			this.overallProgress.Value = 75;
			// 
			// jobTitle
			// 
			this.jobTitle.AutoSize = true;
			this.jobTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.jobTitle.Location = new System.Drawing.Point(149, 8);
			this.jobTitle.Name = "jobTitle";
			this.jobTitle.Size = new System.Drawing.Size(180, 15);
			this.jobTitle.TabIndex = 3;
			this.jobTitle.Text = "C:\\Users\\Joel Low\\Documents\\";
			// 
			// status
			// 
			this.status.AutoSize = true;
			this.status.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.status.Location = new System.Drawing.Point(215, 28);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(84, 15);
			this.status.TabIndex = 5;
			this.status.Text = "Overwriting...";
			// 
			// statusLbl
			// 
			this.statusLbl.AutoSize = true;
			this.statusLbl.Location = new System.Drawing.Point(147, 28);
			this.statusLbl.Name = "statusLbl";
			this.statusLbl.Size = new System.Drawing.Size(42, 15);
			this.statusLbl.TabIndex = 4;
			this.statusLbl.Text = "Status:";
			// 
			// itemLbl
			// 
			this.itemLbl.AutoSize = true;
			this.itemLbl.Location = new System.Drawing.Point(148, 61);
			this.itemLbl.Name = "itemLbl";
			this.itemLbl.Size = new System.Drawing.Size(34, 15);
			this.itemLbl.TabIndex = 6;
			this.itemLbl.Text = "Item:";
			// 
			// item
			// 
			this.item.AutoSize = true;
			this.item.Location = new System.Drawing.Point(215, 61);
			this.item.Name = "item";
			this.item.Size = new System.Drawing.Size(71, 15);
			this.item.TabIndex = 7;
			this.item.Text = "C:\\...\\File.txt";
			// 
			// passLbl
			// 
			this.passLbl.AutoSize = true;
			this.passLbl.Location = new System.Drawing.Point(148, 80);
			this.passLbl.Name = "passLbl";
			this.passLbl.Size = new System.Drawing.Size(33, 15);
			this.passLbl.TabIndex = 8;
			this.passLbl.Text = "Pass:";
			// 
			// pass
			// 
			this.pass.AutoSize = true;
			this.pass.Location = new System.Drawing.Point(215, 80);
			this.pass.Name = "pass";
			this.pass.Size = new System.Drawing.Size(42, 15);
			this.pass.TabIndex = 9;
			this.pass.Text = "7 of 35";
			// 
			// title
			// 
			this.title.Image = ((System.Drawing.Image)(resources.GetObject("title.Image")));
			this.title.Location = new System.Drawing.Point(6, 33);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(128, 128);
			this.title.TabIndex = 11;
			this.title.TabStop = false;
			// 
			// titleLbl
			// 
			this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLbl.Location = new System.Drawing.Point(6, 6);
			this.titleLbl.Name = "titleLbl";
			this.titleLbl.Size = new System.Drawing.Size(128, 18);
			this.titleLbl.TabIndex = 0;
			this.titleLbl.Text = "Erasing...";
			this.titleLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// itemProgressLbl
			// 
			this.itemProgressLbl.AutoSize = true;
			this.itemProgressLbl.Location = new System.Drawing.Point(413, 138);
			this.itemProgressLbl.Name = "itemProgressLbl";
			this.itemProgressLbl.Size = new System.Drawing.Size(29, 15);
			this.itemProgressLbl.TabIndex = 13;
			this.itemProgressLbl.Text = "97%";
			// 
			// itemProgress
			// 
			this.itemProgress.Location = new System.Drawing.Point(150, 137);
			this.itemProgress.Name = "itemProgress";
			this.itemProgress.Size = new System.Drawing.Size(257, 17);
			this.itemProgress.TabIndex = 12;
			this.itemProgress.Value = 97;
			// 
			// stop
			// 
			this.stop.Location = new System.Drawing.Point(367, 184);
			this.stop.Name = "stop";
			this.stop.Size = new System.Drawing.Size(75, 23);
			this.stop.TabIndex = 14;
			this.stop.Text = "Stop";
			this.stop.UseVisualStyleBackColor = true;
			this.stop.Click += new System.EventHandler(this.stop_Click);
			// 
			// bevelLine1
			// 
			this.bevelLine1.Angle = 90;
			this.bevelLine1.Location = new System.Drawing.Point(152, 51);
			this.bevelLine1.Name = "bevelLine1";
			this.bevelLine1.Size = new System.Drawing.Size(285, 2);
			this.bevelLine1.TabIndex = 16;
			// 
			// bevelLine2
			// 
			this.bevelLine2.Angle = 0;
			this.bevelLine2.Location = new System.Drawing.Point(140, 13);
			this.bevelLine2.Name = "bevelLine2";
			this.bevelLine2.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.bevelLine2.Size = new System.Drawing.Size(2, 190);
			this.bevelLine2.TabIndex = 15;
			// 
			// timeLeftLbl
			// 
			this.timeLeftLbl.AutoSize = true;
			this.timeLeftLbl.Location = new System.Drawing.Point(149, 99);
			this.timeLeftLbl.Name = "timeLeftLbl";
			this.timeLeftLbl.Size = new System.Drawing.Size(57, 15);
			this.timeLeftLbl.TabIndex = 10;
			this.timeLeftLbl.Text = "Time left:";
			// 
			// timeLeft
			// 
			this.timeLeft.AutoSize = true;
			this.timeLeft.Location = new System.Drawing.Point(215, 99);
			this.timeLeft.Name = "timeLeft";
			this.timeLeft.Size = new System.Drawing.Size(76, 15);
			this.timeLeft.TabIndex = 11;
			this.timeLeft.Text = "Calculating...";
			// 
			// ProgressForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(449, 216);
			this.Controls.Add(this.timeLeft);
			this.Controls.Add(this.timeLeftLbl);
			this.Controls.Add(this.bevelLine2);
			this.Controls.Add(this.bevelLine1);
			this.Controls.Add(this.stop);
			this.Controls.Add(this.itemProgress);
			this.Controls.Add(this.itemProgressLbl);
			this.Controls.Add(this.titleLbl);
			this.Controls.Add(this.title);
			this.Controls.Add(this.pass);
			this.Controls.Add(this.passLbl);
			this.Controls.Add(this.item);
			this.Controls.Add(this.itemLbl);
			this.Controls.Add(this.statusLbl);
			this.Controls.Add(this.status);
			this.Controls.Add(this.jobTitle);
			this.Controls.Add(this.overallProgress);
			this.Controls.Add(this.overallProgressLbl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgressForm";
			this.ShowInTaskbar = false;
			this.Text = "Erasure Progress";
			((System.ComponentModel.ISupportInitialize)(this.title)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label overallProgressLbl;
		private System.Windows.Forms.ProgressBar overallProgress;
		private System.Windows.Forms.Label jobTitle;
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.Label statusLbl;
		private System.Windows.Forms.Label itemLbl;
		private System.Windows.Forms.Label item;
		private System.Windows.Forms.Label passLbl;
		private System.Windows.Forms.Label pass;
		private System.Windows.Forms.PictureBox title;
		private System.Windows.Forms.Label titleLbl;
		private System.Windows.Forms.Label itemProgressLbl;
		private System.Windows.Forms.ProgressBar itemProgress;
		private System.Windows.Forms.Button stop;
		private Trustbridge.Windows.Controls.BevelLine bevelLine1;
		private Trustbridge.Windows.Controls.BevelLine bevelLine2;
		private System.Windows.Forms.Label timeLeftLbl;
		private System.Windows.Forms.Label timeLeft;
	}
}

