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
			this.totalProgressLbl = new System.Windows.Forms.Label();
			this.totalProgressPb = new System.Windows.Forms.ProgressBar();
			this.jobTitleLbl = new System.Windows.Forms.Label();
			this.status = new System.Windows.Forms.Label();
			this.statusLbl = new System.Windows.Forms.Label();
			this.itemLbl = new System.Windows.Forms.Label();
			this.item = new System.Windows.Forms.Label();
			this.passLbl = new System.Windows.Forms.Label();
			this.pass = new System.Windows.Forms.Label();
			this.methodLbl = new System.Windows.Forms.Label();
			this.method = new System.Windows.Forms.Label();
			this.titlePic = new System.Windows.Forms.PictureBox();
			this.titleLbl = new System.Windows.Forms.Label();
			this.itemPbLbl = new System.Windows.Forms.Label();
			this.itemPb = new System.Windows.Forms.ProgressBar();
			this.stopBtn = new System.Windows.Forms.Button();
			this.bevelLine1 = new Trustbridge.Windows.Controls.BevelLine();
			this.bevelLine2 = new Trustbridge.Windows.Controls.BevelLine();
			((System.ComponentModel.ISupportInitialize)(this.titlePic)).BeginInit();
			this.SuspendLayout();
			// 
			// totalProgressLbl
			// 
			this.totalProgressLbl.Location = new System.Drawing.Point(6, 174);
			this.totalProgressLbl.Name = "totalProgressLbl";
			this.totalProgressLbl.Size = new System.Drawing.Size(128, 15);
			this.totalProgressLbl.TabIndex = 0;
			this.totalProgressLbl.Text = "Total: 75%";
			this.totalProgressLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// totalProgressPb
			// 
			this.totalProgressPb.Location = new System.Drawing.Point(6, 192);
			this.totalProgressPb.Name = "totalProgressPb";
			this.totalProgressPb.Size = new System.Drawing.Size(129, 15);
			this.totalProgressPb.TabIndex = 1;
			this.totalProgressPb.Value = 75;
			// 
			// jobTitleLbl
			// 
			this.jobTitleLbl.AutoSize = true;
			this.jobTitleLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.jobTitleLbl.Location = new System.Drawing.Point(149, 8);
			this.jobTitleLbl.Name = "jobTitleLbl";
			this.jobTitleLbl.Size = new System.Drawing.Size(180, 15);
			this.jobTitleLbl.TabIndex = 2;
			this.jobTitleLbl.Text = "C:\\Users\\Joel Low\\Documents\\";
			// 
			// status
			// 
			this.status.AutoSize = true;
			this.status.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.status.Location = new System.Drawing.Point(215, 28);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(84, 15);
			this.status.TabIndex = 3;
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
			this.itemLbl.TabIndex = 5;
			this.itemLbl.Text = "Item:";
			// 
			// item
			// 
			this.item.AutoSize = true;
			this.item.Location = new System.Drawing.Point(215, 61);
			this.item.Name = "item";
			this.item.Size = new System.Drawing.Size(71, 15);
			this.item.TabIndex = 6;
			this.item.Text = "C:\\...\\File.txt";
			// 
			// passLbl
			// 
			this.passLbl.AutoSize = true;
			this.passLbl.Location = new System.Drawing.Point(148, 80);
			this.passLbl.Name = "passLbl";
			this.passLbl.Size = new System.Drawing.Size(33, 15);
			this.passLbl.TabIndex = 7;
			this.passLbl.Text = "Pass:";
			// 
			// pass
			// 
			this.pass.AutoSize = true;
			this.pass.Location = new System.Drawing.Point(215, 80);
			this.pass.Name = "pass";
			this.pass.Size = new System.Drawing.Size(131, 15);
			this.pass.TabIndex = 8;
			this.pass.Text = "7 of 35 (15 seconds left)";
			// 
			// methodLbl
			// 
			this.methodLbl.AutoSize = true;
			this.methodLbl.Location = new System.Drawing.Point(149, 99);
			this.methodLbl.Name = "methodLbl";
			this.methodLbl.Size = new System.Drawing.Size(52, 15);
			this.methodLbl.TabIndex = 9;
			this.methodLbl.Text = "Method:";
			// 
			// method
			// 
			this.method.AutoSize = true;
			this.method.Location = new System.Drawing.Point(215, 99);
			this.method.Name = "method";
			this.method.Size = new System.Drawing.Size(57, 15);
			this.method.TabIndex = 10;
			this.method.Text = "Gutmann";
			// 
			// titlePic
			// 
			this.titlePic.Image = ((System.Drawing.Image)(resources.GetObject("titlePic.Image")));
			this.titlePic.Location = new System.Drawing.Point(6, 33);
			this.titlePic.Name = "titlePic";
			this.titlePic.Size = new System.Drawing.Size(128, 128);
			this.titlePic.TabIndex = 11;
			this.titlePic.TabStop = false;
			// 
			// titleLbl
			// 
			this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLbl.Location = new System.Drawing.Point(6, 6);
			this.titleLbl.Name = "titleLbl";
			this.titleLbl.Size = new System.Drawing.Size(128, 18);
			this.titleLbl.TabIndex = 12;
			this.titleLbl.Text = "Erasing...";
			this.titleLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// itemPbLbl
			// 
			this.itemPbLbl.AutoSize = true;
			this.itemPbLbl.Location = new System.Drawing.Point(413, 138);
			this.itemPbLbl.Name = "itemPbLbl";
			this.itemPbLbl.Size = new System.Drawing.Size(29, 15);
			this.itemPbLbl.TabIndex = 13;
			this.itemPbLbl.Text = "97%";
			// 
			// itemPb
			// 
			this.itemPb.Location = new System.Drawing.Point(150, 137);
			this.itemPb.Name = "itemPb";
			this.itemPb.Size = new System.Drawing.Size(257, 17);
			this.itemPb.TabIndex = 14;
			this.itemPb.Value = 97;
			// 
			// stopBtn
			// 
			this.stopBtn.Location = new System.Drawing.Point(367, 184);
			this.stopBtn.Name = "stopBtn";
			this.stopBtn.Size = new System.Drawing.Size(75, 23);
			this.stopBtn.TabIndex = 17;
			this.stopBtn.Text = "Stop";
			this.stopBtn.UseVisualStyleBackColor = true;
			// 
			// bevelLine1
			// 
			this.bevelLine1.Angle = 90;
			this.bevelLine1.Location = new System.Drawing.Point(152, 51);
			this.bevelLine1.Name = "bevelLine1";
			this.bevelLine1.Size = new System.Drawing.Size(285, 2);
			this.bevelLine1.TabIndex = 18;
			// 
			// bevelLine2
			// 
			this.bevelLine2.Angle = 0;
			this.bevelLine2.Location = new System.Drawing.Point(140, 13);
			this.bevelLine2.Name = "bevelLine2";
			this.bevelLine2.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.bevelLine2.Size = new System.Drawing.Size(2, 190);
			this.bevelLine2.TabIndex = 20;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(449, 216);
			this.Controls.Add(this.bevelLine2);
			this.Controls.Add(this.bevelLine1);
			this.Controls.Add(this.stopBtn);
			this.Controls.Add(this.itemPb);
			this.Controls.Add(this.itemPbLbl);
			this.Controls.Add(this.titleLbl);
			this.Controls.Add(this.titlePic);
			this.Controls.Add(this.method);
			this.Controls.Add(this.methodLbl);
			this.Controls.Add(this.pass);
			this.Controls.Add(this.passLbl);
			this.Controls.Add(this.item);
			this.Controls.Add(this.itemLbl);
			this.Controls.Add(this.statusLbl);
			this.Controls.Add(this.status);
			this.Controls.Add(this.jobTitleLbl);
			this.Controls.Add(this.totalProgressPb);
			this.Controls.Add(this.totalProgressLbl);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainForm";
			this.Text = "Eraser - Erasing";
			((System.ComponentModel.ISupportInitialize)(this.titlePic)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label totalProgressLbl;
		private System.Windows.Forms.ProgressBar totalProgressPb;
		private System.Windows.Forms.Label jobTitleLbl;
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.Label statusLbl;
		private System.Windows.Forms.Label itemLbl;
		private System.Windows.Forms.Label item;
		private System.Windows.Forms.Label passLbl;
		private System.Windows.Forms.Label pass;
		private System.Windows.Forms.Label methodLbl;
		private System.Windows.Forms.Label method;
		private System.Windows.Forms.PictureBox titlePic;
		private System.Windows.Forms.Label titleLbl;
		private System.Windows.Forms.Label itemPbLbl;
		private System.Windows.Forms.ProgressBar itemPb;
		private System.Windows.Forms.Button stopBtn;
		private Trustbridge.Windows.Controls.BevelLine bevelLine1;
		private Trustbridge.Windows.Controls.BevelLine bevelLine2;
	}
}

