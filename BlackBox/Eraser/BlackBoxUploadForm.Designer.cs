namespace Eraser
{
	partial class BlackBoxUploadForm
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
			this.ButtonsBevel = new Trustbridge.Windows.Controls.BevelLine();
			this.ButtonsPnl = new System.Windows.Forms.Panel();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.TitleLbl = new System.Windows.Forms.Label();
			this.ProgressPb = new System.Windows.Forms.ProgressBar();
			this.UploadWorker = new System.ComponentModel.BackgroundWorker();
			this.ProgressLbl = new System.Windows.Forms.Label();
			this.ButtonsPnl.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonsBevel
			// 
			this.ButtonsBevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonsBevel.Angle = 90;
			this.ButtonsBevel.Location = new System.Drawing.Point(0, 0);
			this.ButtonsBevel.Name = "ButtonsBevel";
			this.ButtonsBevel.Size = new System.Drawing.Size(345, 2);
			this.ButtonsBevel.TabIndex = 0;
			// 
			// ButtonsPnl
			// 
			this.ButtonsPnl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonsPnl.BackColor = System.Drawing.SystemColors.Control;
			this.ButtonsPnl.Controls.Add(this.ButtonsBevel);
			this.ButtonsPnl.Controls.Add(this.CancelBtn);
			this.ButtonsPnl.Location = new System.Drawing.Point(0, 96);
			this.ButtonsPnl.Name = "ButtonsPnl";
			this.ButtonsPnl.Size = new System.Drawing.Size(345, 38);
			this.ButtonsPnl.TabIndex = 1;
			// 
			// CancelBtn
			// 
			this.CancelBtn.Location = new System.Drawing.Point(260, 8);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 2;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// TitleLbl
			// 
			this.TitleLbl.AutoSize = true;
			this.TitleLbl.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.TitleLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
			this.TitleLbl.Location = new System.Drawing.Point(9, 9);
			this.TitleLbl.Name = "TitleLbl";
			this.TitleLbl.Size = new System.Drawing.Size(177, 21);
			this.TitleLbl.TabIndex = 2;
			this.TitleLbl.Text = "Uploading Crash Report";
			// 
			// ProgressPb
			// 
			this.ProgressPb.Location = new System.Drawing.Point(13, 70);
			this.ProgressPb.Name = "ProgressPb";
			this.ProgressPb.Size = new System.Drawing.Size(321, 17);
			this.ProgressPb.TabIndex = 3;
			// 
			// UploadWorker
			// 
			this.UploadWorker.WorkerReportsProgress = true;
			this.UploadWorker.WorkerSupportsCancellation = true;
			this.UploadWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UploadWorker_DoWork);
			this.UploadWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UploadWorker_RunWorkerCompleted);
			this.UploadWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UploadWorker_ProgressChanged);
			// 
			// ProgressLbl
			// 
			this.ProgressLbl.AutoSize = true;
			this.ProgressLbl.Location = new System.Drawing.Point(11, 52);
			this.ProgressLbl.Name = "ProgressLbl";
			this.ProgressLbl.Size = new System.Drawing.Size(126, 15);
			this.ProgressLbl.TabIndex = 4;
			this.ProgressLbl.Text = "Compressing reports...";
			// 
			// BlackBoxUploadForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(344, 132);
			this.Controls.Add(this.ProgressLbl);
			this.Controls.Add(this.ProgressPb);
			this.Controls.Add(this.TitleLbl);
			this.Controls.Add(this.ButtonsPnl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BlackBoxUploadForm";
			this.ShowInTaskbar = false;
			this.Text = "Eraser Crash Assistant";
			this.TopMost = true;
			this.ButtonsPnl.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Trustbridge.Windows.Controls.BevelLine ButtonsBevel;
		private System.Windows.Forms.Panel ButtonsPnl;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Label TitleLbl;
		private System.Windows.Forms.ProgressBar ProgressPb;
		private System.ComponentModel.BackgroundWorker UploadWorker;
		private System.Windows.Forms.Label ProgressLbl;
	}
}