namespace Eraser
{
	partial class BlackBoxMainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlackBoxMainForm));
			this.MainLbl = new System.Windows.Forms.Label();
			this.ReportsLb = new System.Windows.Forms.CheckedListBox();
			this.SubmitBtn = new System.Windows.Forms.Button();
			this.PostponeBtn = new System.Windows.Forms.Button();
			this.BlackBoxPic = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.BlackBoxPic)).BeginInit();
			this.SuspendLayout();
			// 
			// MainLbl
			// 
			resources.ApplyResources(this.MainLbl, "MainLbl");
			this.MainLbl.Name = "MainLbl";
			// 
			// ReportsLb
			// 
			resources.ApplyResources(this.ReportsLb, "ReportsLb");
			this.ReportsLb.CheckOnClick = true;
			this.ReportsLb.FormattingEnabled = true;
			this.ReportsLb.Name = "ReportsLb";
			this.ReportsLb.ThreeDCheckBoxes = true;
			// 
			// SubmitBtn
			// 
			resources.ApplyResources(this.SubmitBtn, "SubmitBtn");
			this.SubmitBtn.Name = "SubmitBtn";
			this.SubmitBtn.UseVisualStyleBackColor = true;
			this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
			// 
			// PostponeBtn
			// 
			resources.ApplyResources(this.PostponeBtn, "PostponeBtn");
			this.PostponeBtn.Name = "PostponeBtn";
			this.PostponeBtn.UseVisualStyleBackColor = true;
			this.PostponeBtn.Click += new System.EventHandler(this.PostponeBtn_Click);
			// 
			// BlackBoxPic
			// 
			this.BlackBoxPic.Image = global::Eraser.Properties.Resources.BlackBox;
			resources.ApplyResources(this.BlackBoxPic, "BlackBoxPic");
			this.BlackBoxPic.Name = "BlackBoxPic";
			this.BlackBoxPic.TabStop = false;
			// 
			// BlackBoxMainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.BlackBoxPic);
			this.Controls.Add(this.PostponeBtn);
			this.Controls.Add(this.SubmitBtn);
			this.Controls.Add(this.ReportsLb);
			this.Controls.Add(this.MainLbl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BlackBoxMainForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.BlackBoxPic)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label MainLbl;
		private System.Windows.Forms.CheckedListBox ReportsLb;
		private System.Windows.Forms.Button SubmitBtn;
		private System.Windows.Forms.Button PostponeBtn;
		private System.Windows.Forms.PictureBox BlackBoxPic;
	}
}