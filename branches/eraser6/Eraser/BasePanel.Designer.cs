namespace Eraser
{
	partial class BasePanel
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
			this.content = new System.Windows.Forms.Panel();
			this.titleIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// titleLbl
			// 
			this.titleLbl.AutoSize = true;
			this.titleLbl.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLbl.Location = new System.Drawing.Point(50, 17);
			this.titleLbl.Name = "titleLbl";
			this.titleLbl.Size = new System.Drawing.Size(119, 32);
			this.titleLbl.TabIndex = 0;
			this.titleLbl.Text = "PanelTitle";
			// 
			// content
			// 
			this.content.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.content.Location = new System.Drawing.Point(20, 52);
			this.content.Name = "content";
			this.content.Size = new System.Drawing.Size(712, 377);
			this.content.TabIndex = 3;
			// 
			// titleIcon
			// 
			this.titleIcon.Location = new System.Drawing.Point(20, 21);
			this.titleIcon.Name = "titleIcon";
			this.titleIcon.Size = new System.Drawing.Size(24, 24);
			this.titleIcon.TabIndex = 1;
			this.titleIcon.TabStop = false;
			// 
			// BasePanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.content);
			this.Controls.Add(this.titleIcon);
			this.Controls.Add(this.titleLbl);
			this.Name = "BasePanel";
			this.Padding = new System.Windows.Forms.Padding(17);
			this.Size = new System.Drawing.Size(752, 449);
			((System.ComponentModel.ISupportInitialize)(this.titleIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Label titleLbl;
		protected System.Windows.Forms.PictureBox titleIcon;
		protected System.Windows.Forms.Panel content;
	}
}
